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
/// Bộ kiểm thử đơn vị cho dịch vụ quản lý khách đến thăm KTX (VisitorService)
/// </summary>
public class VisitorServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly VisitorService _service;
    private readonly Room _room1;
    private readonly Room _room2;
    private readonly Student _student1;
    private readonly Student _student2;

    public VisitorServiceTests()
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
            RoomNumber = "P101",
            Building = "Tòa A",
            Floor = 1,
            Capacity = 4,
            CurrentOccupancy = 2,
            PricePerMonth = 500000,
            Status = RoomStatus.Available,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male
        };

        _room2 = new Room
        {
            RoomNumber = "P202",
            Building = "Tòa B",
            Floor = 2,
            Capacity = 4,
            CurrentOccupancy = 1,
            PricePerMonth = 600000,
            Status = RoomStatus.Available,
            Type = RoomType.Standard,
            AllowedGender = Gender.Female
        };

        _context.Rooms.AddRange(_room1, _room2);
        _context.SaveChanges();

        _student1 = new Student
        {
            StudentCode = "SV001",
            FullName = "Nguyễn Văn An",
            Email = "an.nguyen@email.com",
            PhoneNumber = "0901234567",
            IdentityCard = "001200001111",
            DateOfBirth = new DateTime(2003, 1, 1),
            Gender = Gender.Male,
            CurrentRoomId = _room1.Id,
            ClassName = "CNTT1",
            Faculty = "Công nghệ thông tin"
        };

        _student2 = new Student
        {
            StudentCode = "SV002",
            FullName = "Trần Thị Mai",
            Email = "mai.tran@email.com",
            PhoneNumber = "0907654321",
            IdentityCard = "001200002222",
            DateOfBirth = new DateTime(2003, 5, 10),
            Gender = Gender.Female,
            CurrentRoomId = _room2.Id,
            ClassName = "QTKD1",
            Faculty = "Quản trị kinh doanh"
        };

        _context.Students.AddRange(_student1, _student2);
        _context.SaveChanges();

        _service = new VisitorService(_context);
    }

    [Fact]
    public async Task CheckInAsync_WithValidData_ShouldCreateVisitorLogWithInDormitoryStatus()
    {
        // Arrange
        var request = new CreateVisitorLogDto
        {
            VisitorName = "Lê Văn Khách",
            IdentificationNumber = "034200009999",
            PhoneNumber = "0988777666",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            Note = "Gửi CMND tại phòng trực bảo vệ"
        };

        var beforeTime = DateTime.UtcNow.AddSeconds(-2);

        // Act
        var result = await _service.CheckInAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.VisitorName.Should().Be("Lê Văn Khách");
        result.IdentificationNumber.Should().Be("034200009999");
        result.PhoneNumber.Should().Be("0988777666");
        result.StudentId.Should().Be(_student1.Id);
        result.StudentName.Should().Be("Nguyễn Văn An");
        result.RoomId.Should().Be(_room1.Id);
        result.RoomNumber.Should().Be("P101");
        result.Status.Should().Be(VisitorStatus.InDormitory);
        result.CheckOutTime.Should().BeNull();
        result.Note.Should().Be("Gửi CMND tại phòng trực bảo vệ");
        result.CheckInTime.Should().BeOnOrAfter(beforeTime);

        var inDb = await _context.VisitorLogs.FindAsync(result.Id);
        inDb.Should().NotBeNull();
        inDb!.Status.Should().Be(VisitorStatus.InDormitory);
    }

    [Theory]
    [InlineData("", "034200009999", "0988777666", "Họ và tên khách đến thăm không được để trống.")]
    [InlineData("   ", "034200009999", "0988777666", "Họ và tên khách đến thăm không được để trống.")]
    [InlineData("Lê Văn Khách", "", "0988777666", "Số CCCD/CMND của khách không được để trống.")]
    [InlineData("Lê Văn Khách", "034200009999", "", "Số điện thoại liên hệ của khách không được để trống.")]
    public async Task CheckInAsync_WithInvalidFields_ShouldThrowArgumentException(
        string name, string idNumber, string phone, string expectedError)
    {
        // Arrange
        var request = new CreateVisitorLogDto
        {
            VisitorName = name,
            IdentificationNumber = idNumber,
            PhoneNumber = phone,
            StudentId = _student1.Id,
            RoomId = _room1.Id
        };

        // Act & Assert
        var act = async () => await _service.CheckInAsync(request);
        var ex = await act.Should().ThrowAsync<ArgumentException>();
        ex.WithMessage($"*{expectedError}*");
    }

    [Fact]
    public async Task CheckInAsync_WithNullDto_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = async () => await _service.CheckInAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CheckInAsync_WithNonExistentStudent_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var request = new CreateVisitorLogDto
        {
            VisitorName = "Lê Văn Khách",
            IdentificationNumber = "034200009999",
            PhoneNumber = "0988777666",
            StudentId = 99999,
            RoomId = _room1.Id
        };

        // Act & Assert
        var act = async () => await _service.CheckInAsync(request);
        var ex = await act.Should().ThrowAsync<KeyNotFoundException>();
        ex.WithMessage("*Không tìm thấy sinh viên có ID: 99999.*");
    }

    [Fact]
    public async Task CheckInAsync_WithNonExistentRoom_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var request = new CreateVisitorLogDto
        {
            VisitorName = "Lê Văn Khách",
            IdentificationNumber = "034200009999",
            PhoneNumber = "0988777666",
            StudentId = _student1.Id,
            RoomId = 88888
        };

        // Act & Assert
        var act = async () => await _service.CheckInAsync(request);
        var ex = await act.Should().ThrowAsync<KeyNotFoundException>();
        ex.WithMessage("*Không tìm thấy phòng có ID: 88888.*");
    }

    [Fact]
    public async Task CheckOutAsync_WithValidVisitor_ShouldUpdateCheckOutTimeAndStatus()
    {
        // Arrange
        var log = new VisitorLog
        {
            VisitorName = "Hoàng Phụ Huynh",
            IdentificationNumber = "012345678901",
            PhoneNumber = "0911222333",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = DateTime.UtcNow.AddHours(-1),
            Status = VisitorStatus.InDormitory,
            Note = "Thăm con trai"
        };
        _context.VisitorLogs.Add(log);
        await _context.SaveChangesAsync();

        var beforeCheckOut = DateTime.UtcNow.AddSeconds(-2);

        // Act
        var result = await _service.CheckOutAsync(log.Id, "Đã trả lại thẻ khách");

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(VisitorStatus.CheckedOut);
        result.CheckOutTime.Should().NotBeNull();
        result.CheckOutTime.Should().BeOnOrAfter(beforeCheckOut);
        result.Note.Should().Be("Thăm con trai | Đã trả lại thẻ khách");

        var updatedInDb = await _context.VisitorLogs.FindAsync(log.Id);
        updatedInDb!.Status.Should().Be(VisitorStatus.CheckedOut);
        updatedInDb.CheckOutTime.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckOutAsync_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        // Act & Assert
        var act = async () => await _service.CheckOutAsync(99999);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Không tìm thấy bản ghi khách với ID: 99999.*");
    }

    [Fact]
    public async Task GetVisitorsAsync_WithFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var log1 = new VisitorLog
        {
            VisitorName = "Nguyễn Văn Khách 1",
            IdentificationNumber = "111111111",
            PhoneNumber = "0900000001",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = today.AddHours(8),
            Status = VisitorStatus.InDormitory,
            Note = "Ghi chú 1"
        };
        var log2 = new VisitorLog
        {
            VisitorName = "Trần Thị Khách 2",
            IdentificationNumber = "222222222",
            PhoneNumber = "0900000002",
            StudentId = _student2.Id,
            RoomId = _room2.Id,
            CheckInTime = today.AddHours(9),
            Status = VisitorStatus.CheckedOut,
            CheckOutTime = today.AddHours(10),
            Note = "Ghi chú 2"
        };
        var log3 = new VisitorLog
        {
            VisitorName = "Phạm Khách Quá Giờ",
            IdentificationNumber = "333333333",
            PhoneNumber = "0900000003",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = today.AddHours(7),
            Status = VisitorStatus.Overstayed,
            Note = "Quá giờ"
        };
        _context.VisitorLogs.AddRange(log1, log2, log3);
        await _context.SaveChangesAsync();

        // 1. Lọc theo Status
        var filterByStatus = new VisitorFilterDto { Status = VisitorStatus.CheckedOut };
        var statusResult = await _service.GetVisitorsAsync(filterByStatus);
        statusResult.Should().HaveCount(1);
        statusResult[0].VisitorName.Should().Be("Trần Thị Khách 2");

        // 2. Lọc OnlyOverstayed
        var filterOverstayed = new VisitorFilterDto { OnlyOverstayed = true };
        var overstayedResult = await _service.GetVisitorsAsync(filterOverstayed);
        overstayedResult.Should().HaveCount(1);
        overstayedResult[0].VisitorName.Should().Be("Phạm Khách Quá Giờ");

        // 3. Lọc theo RoomId
        var filterRoom = new VisitorFilterDto { RoomId = _room2.Id };
        var roomResult = await _service.GetVisitorsAsync(filterRoom);
        roomResult.Should().HaveCount(1);
        roomResult[0].RoomNumber.Should().Be("P202");

        // 4. Lọc theo từ khóa tìm kiếm (CCCD)
        var filterKeyword = new VisitorFilterDto { SearchKeyword = "111111111" };
        var keywordResult = await _service.GetVisitorsAsync(filterKeyword);
        keywordResult.Should().HaveCount(1);
        keywordResult[0].VisitorName.Should().Be("Nguyễn Văn Khách 1");
    }

    [Fact]
    public async Task GetVisitorByIdAsync_ShouldReturnCorrectDto_OrNullIfNotFound()
    {
        // Arrange
        var log = new VisitorLog
        {
            VisitorName = "Vũ Khách Thăm",
            IdentificationNumber = "099999999",
            PhoneNumber = "0933333333",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = DateTime.UtcNow,
            Status = VisitorStatus.InDormitory
        };
        _context.VisitorLogs.Add(log);
        await _context.SaveChangesAsync();

        // Act
        var found = await _service.GetVisitorByIdAsync(log.Id);
        var notFound = await _service.GetVisitorByIdAsync(99999);

        // Assert
        found.Should().NotBeNull();
        found!.VisitorName.Should().Be("Vũ Khách Thăm");
        found.StudentName.Should().Be("Nguyễn Văn An");
        found.RoomNumber.Should().Be("P101");

        notFound.Should().BeNull();
    }

    [Fact]
    public async Task MarkOverstayedVisitorsAsync_ShouldMarkOnlyVisitorsPastCurfew()
    {
        // Arrange: Ngày 2026-10-10
        var testDate = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);

        // Khách 1: Check-in lúc 14:00, vẫn đang trong KTX
        var visitor1 = new VisitorLog
        {
            VisitorName = "Khách Vào Sớm",
            IdentificationNumber = "111",
            PhoneNumber = "091",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = testDate.AddHours(14),
            Status = VisitorStatus.InDormitory
        };

        // Khách 2: Check-in lúc 15:00, đã check-out lúc 17:00
        var visitor2 = new VisitorLog
        {
            VisitorName = "Khách Đã Về",
            IdentificationNumber = "222",
            PhoneNumber = "092",
            StudentId = _student2.Id,
            RoomId = _room2.Id,
            CheckInTime = testDate.AddHours(15),
            CheckOutTime = testDate.AddHours(17),
            Status = VisitorStatus.CheckedOut
        };

        // Khách 3: Khách ngày hôm trước (2026-10-09) chưa check out
        var visitor3 = new VisitorLog
        {
            VisitorName = "Khách Quên Về Hôm Qua",
            IdentificationNumber = "333",
            PhoneNumber = "093",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = testDate.AddDays(-1).AddHours(10),
            Status = VisitorStatus.InDormitory
        };

        _context.VisitorLogs.AddRange(visitor1, visitor2, visitor3);
        await _context.SaveChangesAsync();

        // 1. Kiểm tra lúc 21:00 cùng ngày (chưa đến 22:00 giới nghiêm)
        var timeBeforeCurfew = testDate.AddHours(21);
        var marked1 = await _service.MarkOverstayedVisitorsAsync(timeBeforeCurfew);
        // Chỉ khách hôm qua bị quá giờ
        marked1.Should().Be(1);

        var dbVisitor1 = await _context.VisitorLogs.FindAsync(visitor1.Id);
        dbVisitor1!.Status.Should().Be(VisitorStatus.InDormitory);

        var dbVisitor3 = await _context.VisitorLogs.FindAsync(visitor3.Id);
        dbVisitor3!.Status.Should().Be(VisitorStatus.Overstayed);

        // 2. Kiểm tra lúc 22:30 cùng ngày (đã quá 22:00)
        var timeAfterCurfew = testDate.AddHours(22).AddMinutes(30);
        var marked2 = await _service.MarkOverstayedVisitorsAsync(timeAfterCurfew);
        marked2.Should().Be(1);

        var dbVisitor1After = await _context.VisitorLogs.FindAsync(visitor1.Id);
        dbVisitor1After!.Status.Should().Be(VisitorStatus.Overstayed);

        var dbVisitor2 = await _context.VisitorLogs.FindAsync(visitor2.Id);
        dbVisitor2!.Status.Should().Be(VisitorStatus.CheckedOut);
    }

    [Fact]
    public async Task GetVisitorSummaryAsync_ShouldCalculateCorrectMetrics()
    {
        // Arrange
        var testDate = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc);

        var log1 = new VisitorLog
        {
            VisitorName = "Khách A",
            IdentificationNumber = "111",
            PhoneNumber = "091",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = testDate.AddHours(8),
            Status = VisitorStatus.InDormitory
        };
        var log2 = new VisitorLog
        {
            VisitorName = "Khách B",
            IdentificationNumber = "222",
            PhoneNumber = "092",
            StudentId = _student2.Id,
            RoomId = _room2.Id,
            CheckInTime = testDate.AddHours(9),
            CheckOutTime = testDate.AddHours(11),
            Status = VisitorStatus.CheckedOut
        };
        var log3 = new VisitorLog
        {
            VisitorName = "Khách C",
            IdentificationNumber = "333",
            PhoneNumber = "093",
            StudentId = _student1.Id,
            RoomId = _room1.Id,
            CheckInTime = testDate.AddHours(10),
            Status = VisitorStatus.Overstayed
        };
        var logOtherDay = new VisitorLog
        {
            VisitorName = "Khách Khác Ngày",
            IdentificationNumber = "444",
            PhoneNumber = "094",
            StudentId = _student2.Id,
            RoomId = _room2.Id,
            CheckInTime = testDate.AddDays(-1).AddHours(10),
            Status = VisitorStatus.InDormitory
        };

        _context.VisitorLogs.AddRange(log1, log2, log3, logOtherDay);
        await _context.SaveChangesAsync();

        // Act
        var summary = await _service.GetVisitorSummaryAsync(testDate);

        // Assert
        summary.Should().NotBeNull();
        summary.TotalVisitorsToday.Should().Be(3);
        summary.ActiveVisitorsCount.Should().Be(1);
        summary.OverstayedCount.Should().Be(1);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
