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
/// Bộ kiểm thử đơn vị cho dịch vụ điểm danh và theo dõi chuyên cần sinh viên KTX (AttendanceService)
/// </summary>
public class AttendanceServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly AttendanceService _service;
    private readonly Room _room1;
    private readonly Room _room2;
    private readonly Student _student1;
    private readonly Student _student2;
    private readonly Student _student3;

    public AttendanceServiceTests()
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
            RoomNumber = "P301",
            Building = "Tòa C",
            Floor = 3,
            Capacity = 4,
            CurrentOccupancy = 2,
            PricePerMonth = 500000,
            Status = RoomStatus.Available,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male
        };

        _room2 = new Room
        {
            RoomNumber = "P302",
            Building = "Tòa C",
            Floor = 3,
            Capacity = 4,
            CurrentOccupancy = 1,
            PricePerMonth = 500000,
            Status = RoomStatus.Available,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male
        };

        _context.Rooms.AddRange(_room1, _room2);
        _context.SaveChanges();

        _student1 = new Student
        {
            StudentCode = "SV101",
            FullName = "Nguyễn Văn Bảo",
            Email = "bao.nguyen@email.com",
            PhoneNumber = "0911000101",
            IdentityCard = "001200003001",
            DateOfBirth = new DateTime(2003, 3, 1),
            Gender = Gender.Male,
            CurrentRoomId = _room1.Id,
            ClassName = "XD01",
            Faculty = "Kỹ thuật Xây dựng"
        };

        _student2 = new Student
        {
            StudentCode = "SV102",
            FullName = "Trần Tuấn Cường",
            Email = "cuong.tran@email.com",
            PhoneNumber = "0911000102",
            IdentityCard = "001200003002",
            DateOfBirth = new DateTime(2003, 4, 2),
            Gender = Gender.Male,
            CurrentRoomId = _room1.Id,
            ClassName = "KT01",
            Faculty = "Kiến trúc"
        };

        _student3 = new Student
        {
            StudentCode = "SV103",
            FullName = "Lê Hải Đăng",
            Email = "dang.le@email.com",
            PhoneNumber = "0911000103",
            IdentityCard = "001200003003",
            DateOfBirth = new DateTime(2003, 7, 7),
            Gender = Gender.Male,
            CurrentRoomId = _room2.Id,
            ClassName = "DTVT01",
            Faculty = "Điện tử viễn thông"
        };

        _context.Students.AddRange(_student1, _student2, _student3);
        _context.SaveChanges();

        _service = new AttendanceService(_context);
    }

    [Fact]
    public async Task RecordSingleAttendanceAsync_WithNewRecord_ShouldCreateSuccessfully()
    {
        // Arrange
        var date = new DateTime(2026, 10, 10, 21, 30, 0, DateTimeKind.Utc);

        // Act
        var result = await _service.RecordSingleAttendanceAsync(
            studentId: _student1.Id,
            roomId: _room1.Id,
            date: date,
            status: AttendanceStatus.Present,
            reason: "Có mặt đúng giờ",
            recordedBy: "Quản lý KTX");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.AttendanceDate.Should().Be(new DateTime(2026, 10, 10));
        result.StudentId.Should().Be(_student1.Id);
        result.StudentName.Should().Be("Nguyễn Văn Bảo");
        result.StudentCode.Should().Be("SV101");
        result.RoomId.Should().Be(_room1.Id);
        result.RoomNumber.Should().Be("P301");
        result.Status.Should().Be(AttendanceStatus.Present);
        result.Reason.Should().Be("Có mặt đúng giờ");
        result.RecordedBy.Should().Be("Quản lý KTX");

        var inDb = await _context.AttendanceRecords.FindAsync(result.Id);
        inDb.Should().NotBeNull();
        inDb!.Status.Should().Be(AttendanceStatus.Present);
    }

    [Fact]
    public async Task RecordSingleAttendanceAsync_WhenRecordExistsForSameDate_ShouldUpdateRecord()
    {
        // Arrange
        var date = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);

        // Tạo bản ghi ban đầu: Vắng không phép
        var initial = await _service.RecordSingleAttendanceAsync(
            _student1.Id, _room1.Id, date, AttendanceStatus.AbsentWithoutPermission, "Không thấy trong phòng", "Cán bộ 1");

        // Act: Sinh viên về muộn, cập nhật thành Về muộn
        var updated = await _service.RecordSingleAttendanceAsync(
            _student1.Id, _room1.Id, date, AttendanceStatus.Late, "Về muộn lúc 22:30 có lý do", "Cán bộ 2");

        // Assert
        updated.Id.Should().Be(initial.Id);
        updated.Status.Should().Be(AttendanceStatus.Late);
        updated.Reason.Should().Be("Về muộn lúc 22:30 có lý do");
        updated.RecordedBy.Should().Be("Cán bộ 2");

        var count = await _context.AttendanceRecords.CountAsync(a => a.StudentId == _student1.Id && a.AttendanceDate == date.Date);
        count.Should().Be(1);
    }

    [Fact]
    public async Task RecordSingleAttendanceAsync_WithInvalidStudentOrRoom_ShouldThrowException()
    {
        var date = DateTime.UtcNow.Date;

        // 1. StudentId <= 0
        var act1 = async () => await _service.RecordSingleAttendanceAsync(0, _room1.Id, date, AttendanceStatus.Present);
        await act1.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Mã sinh viên không hợp lệ.*");

        // 2. RoomId <= 0
        var act2 = async () => await _service.RecordSingleAttendanceAsync(_student1.Id, 0, date, AttendanceStatus.Present);
        await act2.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Mã phòng không hợp lệ.*");

        // 3. Student không tồn tại
        var act3 = async () => await _service.RecordSingleAttendanceAsync(99999, _room1.Id, date, AttendanceStatus.Present);
        await act3.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Không tìm thấy sinh viên có ID: 99999.*");

        // 4. Room không tồn tại
        var act4 = async () => await _service.RecordSingleAttendanceAsync(_student1.Id, 99999, date, AttendanceStatus.Present);
        await act4.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Không tìm thấy phòng có ID: 99999.*");
    }

    [Fact]
    public async Task RecordBatchRoomAttendanceAsync_ShouldSaveBatchRecordsSuccessfully()
    {
        // Arrange
        var date = new DateTime(2026, 10, 10);
        var batch = new BatchRoomAttendanceDto
        {
            RoomId = _room1.Id,
            AttendanceDate = date,
            RecordedBy = "Trưởng tầng 3",
            StudentAttendances = new List<AttendanceRecordDto>
            {
                new()
                {
                    StudentId = _student1.Id,
                    Status = AttendanceStatus.Present,
                    Reason = "Có mặt"
                },
                new()
                {
                    StudentId = _student2.Id,
                    Status = AttendanceStatus.AbsentWithPermission,
                    Reason = "Về quê cuối tuần"
                }
            }
        };

        // Act
        var results = await _service.RecordBatchRoomAttendanceAsync(batch);

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(r => r.StudentId == _student1.Id && r.Status == AttendanceStatus.Present);
        results.Should().Contain(r => r.StudentId == _student2.Id && r.Status == AttendanceStatus.AbsentWithPermission);

        var count = await _context.AttendanceRecords.CountAsync(a => a.RoomId == _room1.Id && a.AttendanceDate == date);
        count.Should().Be(2);
    }

    [Fact]
    public async Task RecordBatchRoomAttendanceAsync_WhenUpdatingExistingRecords_ShouldUpsert()
    {
        // Arrange
        var date = new DateTime(2026, 10, 10);

        // Ghi nhận lần 1
        await _service.RecordBatchRoomAttendanceAsync(new BatchRoomAttendanceDto
        {
            RoomId = _room1.Id,
            AttendanceDate = date,
            RecordedBy = "Cán bộ A",
            StudentAttendances = new List<AttendanceRecordDto>
            {
                new() { StudentId = _student1.Id, Status = AttendanceStatus.AbsentWithoutPermission }
            }
        });

        // Ghi nhận lần 2 (cập nhật lại)
        var updatedBatch = await _service.RecordBatchRoomAttendanceAsync(new BatchRoomAttendanceDto
        {
            RoomId = _room1.Id,
            AttendanceDate = date,
            RecordedBy = "Cán bộ B",
            StudentAttendances = new List<AttendanceRecordDto>
            {
                new() { StudentId = _student1.Id, Status = AttendanceStatus.Present, Reason = "Đã bổ sung minh chứng" }
            }
        });

        // Assert
        updatedBatch.Should().HaveCount(1);
        updatedBatch[0].Status.Should().Be(AttendanceStatus.Present);
        updatedBatch[0].Reason.Should().Be("Đã bổ sung minh chứng");
        updatedBatch[0].RecordedBy.Should().Be("Cán bộ B");

        var total = await _context.AttendanceRecords.CountAsync(a => a.RoomId == _room1.Id && a.AttendanceDate == date);
        total.Should().Be(1);
    }

    [Fact]
    public async Task RecordBatchRoomAttendanceAsync_WithInvalidRoom_ShouldThrowException()
    {
        // 1. Batch null
        var act1 = async () => await _service.RecordBatchRoomAttendanceAsync(null!);
        await act1.Should().ThrowAsync<ArgumentNullException>();

        // 2. RoomId <= 0
        var act2 = async () => await _service.RecordBatchRoomAttendanceAsync(new BatchRoomAttendanceDto { RoomId = 0 });
        await act2.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Mã phòng không hợp lệ.*");

        // 3. Room không tồn tại
        var act3 = async () => await _service.RecordBatchRoomAttendanceAsync(new BatchRoomAttendanceDto { RoomId = 99999 });
        await act3.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Không tìm thấy phòng có ID: 99999.*");
    }

    [Fact]
    public async Task GetRoomAttendanceAsync_ShouldReturnRoomAttendanceSortedByStudentName()
    {
        // Arrange
        var date = new DateTime(2026, 10, 10);
        await _service.RecordSingleAttendanceAsync(_student2.Id, _room1.Id, date, AttendanceStatus.Present);
        await _service.RecordSingleAttendanceAsync(_student1.Id, _room1.Id, date, AttendanceStatus.Late);

        // Act
        var result = await _service.GetRoomAttendanceAsync(_room1.Id, date);

        // Assert
        result.Should().HaveCount(2);
        // Sắp xếp theo Student.FullName ("Nguyễn Văn Bảo" trước "Trần Tuấn Cường")
        result[0].StudentName.Should().Be("Nguyễn Văn Bảo");
        result[1].StudentName.Should().Be("Trần Tuấn Cường");
    }

    [Fact]
    public async Task GetAttendanceRecordsAsync_WithFilters_ShouldReturnFilteredData()
    {
        // Arrange
        var date1 = new DateTime(2026, 10, 10);
        var date2 = new DateTime(2026, 10, 11);

        await _service.RecordSingleAttendanceAsync(_student1.Id, _room1.Id, date1, AttendanceStatus.Present);
        await _service.RecordSingleAttendanceAsync(_student2.Id, _room1.Id, date1, AttendanceStatus.AbsentWithoutPermission);
        await _service.RecordSingleAttendanceAsync(_student3.Id, _room2.Id, date1, AttendanceStatus.Late);
        await _service.RecordSingleAttendanceAsync(_student1.Id, _room1.Id, date2, AttendanceStatus.Present);

        // 1. Lọc theo Date
        var filterDate = new AttendanceFilterDto { Date = date1 };
        var dateResult = await _service.GetAttendanceRecordsAsync(filterDate);
        dateResult.Should().HaveCount(3);

        // 2. Lọc theo RoomId
        var filterRoom = new AttendanceFilterDto { RoomId = _room2.Id };
        var roomResult = await _service.GetAttendanceRecordsAsync(filterRoom);
        roomResult.Should().HaveCount(1);
        roomResult[0].StudentName.Should().Be("Lê Hải Đăng");

        // 3. Lọc theo Status
        var filterStatus = new AttendanceFilterDto { Status = AttendanceStatus.AbsentWithoutPermission };
        var statusResult = await _service.GetAttendanceRecordsAsync(filterStatus);
        statusResult.Should().HaveCount(1);
        statusResult[0].StudentName.Should().Be("Trần Tuấn Cường");

        // 4. Lọc theo SearchKeyword (MSSV)
        var filterKeyword = new AttendanceFilterDto { SearchKeyword = "SV103" };
        var keywordResult = await _service.GetAttendanceRecordsAsync(filterKeyword);
        keywordResult.Should().HaveCount(1);
        keywordResult[0].StudentCode.Should().Be("SV103");
    }

    [Fact]
    public async Task GetAttendanceStatsAsync_ShouldCalculateRatesAndCountsAccurately()
    {
        // Arrange
        var date = new DateTime(2026, 10, 10);
        await _service.RecordSingleAttendanceAsync(_student1.Id, _room1.Id, date, AttendanceStatus.Present);
        await _service.RecordSingleAttendanceAsync(_student2.Id, _room1.Id, date, AttendanceStatus.AbsentWithoutPermission);
        await _service.RecordSingleAttendanceAsync(_student3.Id, _room2.Id, date, AttendanceStatus.AbsentWithPermission);

        // Giả sử có thêm 1 sinh viên Late ở ngày khác
        var dateNext = new DateTime(2026, 10, 11);
        await _service.RecordSingleAttendanceAsync(_student1.Id, _room1.Id, dateNext, AttendanceStatus.Late);

        // Act 1: Thống kê ngày 2026-10-10 (3 bản ghi: 1 Present, 1 AbsentWithoutPermission, 1 AbsentWithPermission)
        var stats1 = await _service.GetAttendanceStatsAsync(fromDate: date, toDate: date);

        // Assert 1
        stats1.Should().NotBeNull();
        stats1.TotalStudents.Should().Be(3);
        stats1.PresentCount.Should().Be(1);
        stats1.AbsentWithoutPermissionCount.Should().Be(1);
        stats1.AbsentWithPermissionCount.Should().Be(1);
        stats1.LateCount.Should().Be(0);
        // Tỷ lệ có mặt = 1/3 * 100 = 33.33%
        stats1.AttendanceRate.Should().Be(33.33);

        // Act 2: Thống kê cho phòng 1 trên cả 2 ngày (2 bản ghi: 1 Present, 1 Late, 1 AbsentWithoutPermission = 3 lượt của phòng 1)
        var statsRoom1 = await _service.GetAttendanceStatsAsync(roomId: _room1.Id);
        statsRoom1.TotalStudents.Should().Be(3);
        statsRoom1.PresentCount.Should().Be(1);
        statsRoom1.LateCount.Should().Be(1);
        statsRoom1.AbsentWithoutPermissionCount.Should().Be(1);
        statsRoom1.AttendanceRate.Should().Be(33.33);
    }

    [Fact]
    public async Task GetAttendanceStatsAsync_WhenNoRecords_ShouldReturnZeroRateWithoutDivisionByZero()
    {
        // Act
        var emptyStats = await _service.GetAttendanceStatsAsync(fromDate: new DateTime(2020, 1, 1), toDate: new DateTime(2020, 1, 2));

        // Assert
        emptyStats.Should().NotBeNull();
        emptyStats.TotalStudents.Should().Be(0);
        emptyStats.PresentCount.Should().Be(0);
        emptyStats.AttendanceRate.Should().Be(0.0);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
