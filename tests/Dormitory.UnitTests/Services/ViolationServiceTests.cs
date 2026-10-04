using Dormitory.Application.DTOs;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using Dormitory.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử các chức năng xử lý nghiệp vụ quản lý vi phạm kỷ luật KTX (ViolationService)
/// </summary>
public class ViolationServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly ViolationService _service;
    private readonly Room _room1;
    private readonly Room _room2;
    private readonly Student _student1;
    private readonly Student _student2;

    public ViolationServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _room1 = new Room
        {
            RoomNumber = "A101",
            Building = "Tòa A",
            Floor = 1,
            Capacity = 4,
            CurrentOccupancy = 2,
            PricePerMonth = 600000,
            Status = RoomStatus.Available,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male
        };

        _room2 = new Room
        {
            RoomNumber = "B201",
            Building = "Tòa B",
            Floor = 2,
            Capacity = 2,
            CurrentOccupancy = 1,
            PricePerMonth = 1200000,
            Status = RoomStatus.Available,
            Type = RoomType.Premium,
            AllowedGender = Gender.Female
        };

        _context.Rooms.AddRange(_room1, _room2);
        _context.SaveChanges();

        _student1 = new Student
        {
            StudentCode = "SV001",
            FullName = "Nguyễn Văn An",
            DateOfBirth = new DateTime(2003, 1, 15),
            Gender = Gender.Male,
            IdentityCard = "001203000001",
            PhoneNumber = "0912345678",
            ClassName = "CNTT1",
            Faculty = "Công nghệ thông tin",
            CurrentRoomId = _room1.Id
        };

        _student2 = new Student
        {
            StudentCode = "SV002",
            FullName = "Trần Thị Bình",
            DateOfBirth = new DateTime(2003, 5, 20),
            Gender = Gender.Female,
            IdentityCard = "001203000002",
            PhoneNumber = "0987654321",
            ClassName = "QTKD1",
            Faculty = "Quản trị kinh doanh",
            CurrentRoomId = _room2.Id
        };

        _context.Students.AddRange(_student1, _student2);
        _context.SaveChanges();

        _service = new ViolationService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task CreateViolationAsync_WithValidData_ShouldCreateAndReturnDto()
    {
        // Sắp đặt
        var dto = new CreateViolationDto
        {
            ViolationCode = "VP-001",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Nấu ăn bằng bếp điện trong phòng",
            Description = "Sử dụng bếp từ mini nấu lẩu gây nhảy aptomat",
            Severity = ViolationSeverity.Severe,
            FineAmount = 200000,
            DemeritPoints = 10,
            ViolationDate = DateTime.UtcNow,
            RecordedBy = "admin"
        };

        // Thực hiện
        var result = await _service.CreateViolationAsync(dto);

        // Kiểm tra kết quả
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.ViolationCode.Should().Be("VP-001");
        result.StudentId.Should().Be(_student1.Id);
        result.StudentName.Should().Be("Nguyễn Văn An");
        result.StudentCode.Should().Be("SV001");
        result.RoomId.Should().Be(_room1.Id);
        result.RoomNumber.Should().Be("A101");
        result.BuildingName.Should().Be("Tòa A");
        result.Title.Should().Be("Nấu ăn bằng bếp điện trong phòng");
        result.Severity.Should().Be(ViolationSeverity.Severe);
        result.SeverityText.Should().Be("Cảnh cáo");
        result.Status.Should().Be(ViolationStatus.Pending);
        result.StatusText.Should().Be("Chờ xử lý");
        result.FineAmount.Should().Be(200000);
        result.DemeritPoints.Should().Be(10);
        result.RecordedBy.Should().Be("admin");

        var entityInDb = await _context.Violations.FindAsync(result.Id);
        entityInDb.Should().NotBeNull();
        entityInDb!.Title.Should().Be(dto.Title);
    }

    [Fact]
    public async Task CreateViolationAsync_WithEmptyCode_ShouldAutoGenerateCode()
    {
        // Sắp đặt
        var dto = new CreateViolationDto
        {
            ViolationCode = null, // Không truyền mã
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Gây ồn ào sau 23h",
            Severity = ViolationSeverity.Moderate,
            FineAmount = 0,
            DemeritPoints = 5,
            RecordedBy = "admin"
        };

        // Thực hiện
        var result = await _service.CreateViolationAsync(dto);

        // Kiểm tra kết quả
        result.Should().NotBeNull();
        result.ViolationCode.Should().StartWith("VP-");
    }

    [Fact]
    public async Task CreateViolationAsync_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Sắp đặt
        var dto = new CreateViolationDto
        {
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "   ",
            Severity = ViolationSeverity.Minor
        };

        // Thực hiện & Kiểm tra ngoại lệ
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateViolationAsync(dto));
    }

    [Fact]
    public async Task CreateViolationAsync_WithNegativeFineAmount_ShouldThrowArgumentException()
    {
        // Sắp đặt
        var dto = new CreateViolationDto
        {
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Vi phạm",
            FineAmount = -50000
        };

        // Thực hiện & Kiểm tra ngoại lệ
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateViolationAsync(dto));
    }

    [Fact]
    public async Task CreateViolationAsync_WithNegativeDemeritPoints_ShouldThrowArgumentException()
    {
        // Sắp đặt
        var dto = new CreateViolationDto
        {
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Vi phạm",
            DemeritPoints = -5
        };

        // Thực hiện & Kiểm tra ngoại lệ
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateViolationAsync(dto));
    }

    [Fact]
    public async Task CreateViolationAsync_WithNonExistentStudent_ShouldThrowKeyNotFoundException()
    {
        // Sắp đặt
        var dto = new CreateViolationDto
        {
            StudentId = 9999,
            RoomId = _room1.Id,
            Title = "Vi phạm"
        };

        // Thực hiện & Kiểm tra ngoại lệ
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateViolationAsync(dto));
    }

    [Fact]
    public async Task CreateViolationAsync_WithNonExistentRoom_ShouldThrowKeyNotFoundException()
    {
        // Sắp đặt
        var dto = new CreateViolationDto
        {
            StudentId = _student1.Id,
            RoomId = 9999,
            Title = "Vi phạm"
        };

        // Thực hiện & Kiểm tra ngoại lệ
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateViolationAsync(dto));
    }

    [Fact]
    public async Task GetAllViolationsAsync_WithFilters_ShouldReturnMatchingViolations()
    {
        // Sắp đặt dữ liệu mẫu
        var v1 = new Violation
        {
            ViolationCode = "VP-101",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Nấu ăn bằng bếp điện",
            Severity = ViolationSeverity.Severe,
            Status = ViolationStatus.Pending,
            FineAmount = 200000,
            DemeritPoints = 10,
            ViolationDate = DateTime.UtcNow.AddDays(-2)
        };

        var v2 = new Violation
        {
            ViolationCode = "VP-102",
            StudentId = _student2.Id,
            RoomId = _room2.Id,
            Title = "Mở nhạc ồn ào sau 23h",
            Severity = ViolationSeverity.Moderate,
            Status = ViolationStatus.Resolved,
            FineAmount = 0,
            DemeritPoints = 5,
            ViolationDate = DateTime.UtcNow.AddDays(-1)
        };

        var v3 = new Violation
        {
            ViolationCode = "VP-103",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Phơi đồ sai quy định",
            Severity = ViolationSeverity.Minor,
            Status = ViolationStatus.Dismissed,
            FineAmount = 0,
            DemeritPoints = 0,
            ViolationDate = DateTime.UtcNow
        };

        _context.Violations.AddRange(v1, v2, v3);
        await _context.SaveChangesAsync();

        // 1. Lọc theo từ khóa
        var searchResult = await _service.GetAllViolationsAsync(searchTerm: "bếp");
        searchResult.Should().HaveCount(1);
        searchResult[0].ViolationCode.Should().Be("VP-101");

        // 2. Lọc theo mức độ nghiêm trọng
        var severityResult = await _service.GetAllViolationsAsync(severity: ViolationSeverity.Moderate);
        severityResult.Should().HaveCount(1);
        severityResult[0].ViolationCode.Should().Be("VP-102");

        // 3. Lọc theo trạng thái
        var statusResult = await _service.GetAllViolationsAsync(status: ViolationStatus.Resolved);
        statusResult.Should().HaveCount(1);
        statusResult[0].ViolationCode.Should().Be("VP-102");

        // 4. Lọc theo sinh viên
        var studentResult = await _service.GetAllViolationsAsync(studentId: _student1.Id);
        studentResult.Should().HaveCount(2);

        // 5. Lọc theo phòng
        var roomResult = await _service.GetAllViolationsAsync(roomId: _room2.Id);
        roomResult.Should().HaveCount(1);
        roomResult[0].ViolationCode.Should().Be("VP-102");
    }

    [Fact]
    public async Task GetViolationByIdAsync_WhenExists_ShouldReturnDto()
    {
        // Sắp đặt
        var violation = new Violation
        {
            ViolationCode = "VP-005",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Dẫn người ngoài vào phòng qua đêm",
            Severity = ViolationSeverity.Critical,
            Status = ViolationStatus.Pending,
            FineAmount = 500000,
            DemeritPoints = 20
        };

        _context.Violations.Add(violation);
        await _context.SaveChangesAsync();

        // Thực hiện
        var result = await _service.GetViolationByIdAsync(violation.Id);

        // Kiểm tra
        result.Should().NotBeNull();
        result!.Id.Should().Be(violation.Id);
        result.Title.Should().Be("Dẫn người ngoài vào phòng qua đêm");
        result.SeverityText.Should().Be("Buộc rời KTX");
        result.StudentName.Should().Be("Nguyễn Văn An");
        result.RoomNumber.Should().Be("A101");
    }

    [Fact]
    public async Task GetViolationByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        // Thực hiện
        var result = await _service.GetViolationByIdAsync(99999);

        // Kiểm tra
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetViolationsByStudentIdAsync_ShouldReturnStudentViolations()
    {
        // Sắp đặt
        var v1 = new Violation
        {
            ViolationCode = "VP-A",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Vi phạm A"
        };
        var v2 = new Violation
        {
            ViolationCode = "VP-B",
            StudentId = _student2.Id,
            RoomId = _room2.Id,
            Title = "Vi phạm B"
        };

        _context.Violations.AddRange(v1, v2);
        await _context.SaveChangesAsync();

        // Thực hiện
        var result = await _service.GetViolationsByStudentIdAsync(_student1.Id);

        // Kiểm tra
        result.Should().HaveCount(1);
        result[0].ViolationCode.Should().Be("VP-A");
    }

    [Fact]
    public async Task UpdateViolationAsync_WithValidData_ShouldUpdateAndReturnDto()
    {
        // Sắp đặt
        var violation = new Violation
        {
            ViolationCode = "VP-UPD",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Tiêu đề cũ",
            Severity = ViolationSeverity.Minor,
            Status = ViolationStatus.Pending,
            FineAmount = 50000,
            DemeritPoints = 2
        };
        _context.Violations.Add(violation);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateViolationDto
        {
            StudentId = _student2.Id,
            RoomId = _room2.Id,
            Title = "Tiêu đề mới đã chỉnh sửa",
            Description = "Chi tiết bổ sung",
            Severity = ViolationSeverity.Moderate,
            Status = ViolationStatus.Resolved,
            FineAmount = 100000,
            DemeritPoints = 5,
            ViolationDate = DateTime.UtcNow,
            ResolutionNotes = "Đã giải quyết ổn thỏa"
        };

        // Thực hiện
        var result = await _service.UpdateViolationAsync(violation.Id, updateDto);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Title.Should().Be("Tiêu đề mới đã chỉnh sửa");
        result.StudentId.Should().Be(_student2.Id);
        result.RoomId.Should().Be(_room2.Id);
        result.Severity.Should().Be(ViolationSeverity.Moderate);
        result.SeverityText.Should().Be("Khiển trách");
        result.Status.Should().Be(ViolationStatus.Resolved);
        result.StatusText.Should().Be("Đã xử lý");
        result.FineAmount.Should().Be(100000);
        result.DemeritPoints.Should().Be(5);
        result.ResolutionNotes.Should().Be("Đã giải quyết ổn thỏa");
    }

    [Fact]
    public async Task UpdateViolationAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        var updateDto = new UpdateViolationDto
        {
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Không tồn tại"
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateViolationAsync(99999, updateDto));
    }

    [Fact]
    public async Task ResolveViolationAsync_ShouldUpdateStatusAndResolutionNotes()
    {
        // Sắp đặt
        var violation = new Violation
        {
            ViolationCode = "VP-RES",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Biên bản cần xử lý",
            Status = ViolationStatus.Pending
        };
        _context.Violations.Add(violation);
        await _context.SaveChangesAsync();

        var resolveDto = new ResolveViolationDto
        {
            Status = ViolationStatus.Resolved,
            ResolutionNotes = "Sinh viên đã nộp tiền phạt 200.000đ và ký cam kết không tái phạm."
        };

        // Thực hiện
        var success = await _service.ResolveViolationAsync(violation.Id, resolveDto);

        // Kiểm tra
        success.Should().BeTrue();

        var updated = await _context.Violations.FindAsync(violation.Id);
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(ViolationStatus.Resolved);
        updated.ResolutionNotes.Should().Be(resolveDto.ResolutionNotes);
    }

    [Fact]
    public async Task ResolveViolationAsync_WhenNotFound_ShouldReturnFalse()
    {
        var resolveDto = new ResolveViolationDto
        {
            Status = ViolationStatus.Dismissed,
            ResolutionNotes = "Miễn trừ"
        };

        var result = await _service.ResolveViolationAsync(99999, resolveDto);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteViolationAsync_ShouldRemoveEntity()
    {
        // Sắp đặt
        var violation = new Violation
        {
            ViolationCode = "VP-DEL",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Title = "Biên bản cần xóa"
        };
        _context.Violations.Add(violation);
        await _context.SaveChangesAsync();

        // Thực hiện
        var success = await _service.DeleteViolationAsync(violation.Id);

        // Kiểm tra
        success.Should().BeTrue();

        var entity = await _context.Violations.FindAsync(violation.Id);
        entity.Should().BeNull();
    }

    [Fact]
    public async Task DeleteViolationAsync_WhenNotFound_ShouldReturnFalse()
    {
        var success = await _service.DeleteViolationAsync(99999);
        success.Should().BeFalse();
    }
}
