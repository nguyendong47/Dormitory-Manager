using Dormitory.Application.DTOs;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.UnitTests.Entities;

/// <summary>
/// Kiểm thử đơn vị cho các thực thể VisitorLog, AttendanceRecord, cấu hình EF Core SQLite và DTOs liên quan
/// </summary>
public class VisitorAndAttendanceEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;

    public VisitorAndAttendanceEntityTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public void VisitorLog_DefaultValues_ShouldBeInitializedProperly()
    {
        // Act
        var log = new VisitorLog();

        // Assert
        log.Id.Should().Be(0);
        log.VisitorName.Should().BeEmpty();
        log.IdentificationNumber.Should().BeEmpty();
        log.PhoneNumber.Should().BeEmpty();
        log.StudentId.Should().Be(0);
        log.RoomId.Should().Be(0);
        log.CheckInTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        log.CheckOutTime.Should().BeNull();
        log.Status.Should().Be(VisitorStatus.InDormitory);
        log.Note.Should().BeEmpty();
        log.Student.Should().BeNull();
        log.Room.Should().BeNull();
    }

    [Fact]
    public void VisitorLog_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var checkIn = DateTime.UtcNow.AddHours(-3);
        var checkOut = DateTime.UtcNow.AddHours(-1);

        // Act
        var log = new VisitorLog
        {
            Id = 10,
            VisitorName = "Nguyễn Văn Khách",
            IdentificationNumber = "001200001234",
            PhoneNumber = "0901234567",
            StudentId = 5,
            RoomId = 3,
            CheckInTime = checkIn,
            CheckOutTime = checkOut,
            Status = VisitorStatus.CheckedOut,
            Note = "Đến gửi đồ cho em trai"
        };

        // Assert
        log.Id.Should().Be(10);
        log.VisitorName.Should().Be("Nguyễn Văn Khách");
        log.IdentificationNumber.Should().Be("001200001234");
        log.PhoneNumber.Should().Be("0901234567");
        log.StudentId.Should().Be(5);
        log.RoomId.Should().Be(3);
        log.CheckInTime.Should().Be(checkIn);
        log.CheckOutTime.Should().Be(checkOut);
        log.Status.Should().Be(VisitorStatus.CheckedOut);
        log.Note.Should().Be("Đến gửi đồ cho em trai");
    }

    [Fact]
    public void AttendanceRecord_DefaultValues_ShouldBeInitializedProperly()
    {
        // Act
        var record = new AttendanceRecord();

        // Assert
        record.Id.Should().Be(0);
        record.AttendanceDate.Should().Be(default);
        record.StudentId.Should().Be(0);
        record.RoomId.Should().Be(0);
        record.Status.Should().Be(AttendanceStatus.Present);
        record.Reason.Should().BeNull();
        record.RecordedBy.Should().BeNull();
        record.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        record.Student.Should().BeNull();
        record.Room.Should().BeNull();
    }

    [Fact]
    public void AttendanceRecord_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var attendanceDate = new DateTime(2026, 10, 10);
        var createdAt = DateTime.UtcNow;

        // Act
        var record = new AttendanceRecord
        {
            Id = 20,
            AttendanceDate = attendanceDate,
            StudentId = 15,
            RoomId = 8,
            Status = AttendanceStatus.AbsentWithPermission,
            Reason = "Về quê cuối tuần có đơn xin phép",
            RecordedBy = "Quản lý Trần B",
            CreatedAt = createdAt
        };

        // Assert
        record.Id.Should().Be(20);
        record.AttendanceDate.Should().Be(attendanceDate);
        record.StudentId.Should().Be(15);
        record.RoomId.Should().Be(8);
        record.Status.Should().Be(AttendanceStatus.AbsentWithPermission);
        record.Reason.Should().Be("Về quê cuối tuần có đơn xin phép");
        record.RecordedBy.Should().Be("Quản lý Trần B");
        record.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData(VisitorStatus.InDormitory, "Đang trong KTX", "#107C41", "#DFF6DD")]
    [InlineData(VisitorStatus.CheckedOut, "Đã rời KTX", "#5D5A58", "#F3F2F1")]
    [InlineData(VisitorStatus.Overstayed, "Quá giờ quy định", "#A80000", "#FDE7E9")]
    public void VisitorLogDto_BadgeHelpers_ShouldReturnExpectedValues(
        VisitorStatus status,
        string expectedText,
        string expectedColor,
        string expectedBg)
    {
        // Arrange
        var dto = new VisitorLogDto { Status = status };

        // Assert
        dto.StatusText.Should().Be(expectedText);
        dto.StatusBadgeColor.Should().Be(expectedColor);
        dto.StatusBadgeBackground.Should().Be(expectedBg);
    }

    [Theory]
    [InlineData(AttendanceStatus.Present, "Có mặt", "#107C41", "#DFF6DD")]
    [InlineData(AttendanceStatus.AbsentWithoutPermission, "Vắng không phép", "#A80000", "#FDE7E9")]
    [InlineData(AttendanceStatus.AbsentWithPermission, "Vắng có phép", "#0078D4", "#DEECF9")]
    [InlineData(AttendanceStatus.Late, "Về muộn", "#C67D00", "#FFF4CE")]
    public void AttendanceRecordDto_BadgeHelpers_ShouldReturnExpectedValues(
        AttendanceStatus status,
        string expectedText,
        string expectedColor,
        string expectedBg)
    {
        // Arrange
        var dto = new AttendanceRecordDto { Status = status };

        // Assert
        dto.StatusText.Should().Be(expectedText);
        dto.StatusBadgeColor.Should().Be(expectedColor);
        dto.StatusBadgeBackground.Should().Be(expectedBg);
    }

    [Fact]
    public void VisitorDtos_MappingAndFilters_ShouldWorkProperly()
    {
        // Arrange & Act
        var now = DateTime.UtcNow;
        var logDto = new VisitorLogDto
        {
            Id = 1,
            VisitorName = "Lê Thị Lan",
            IdentificationNumber = "034200004567",
            PhoneNumber = "0987654321",
            StudentId = 2,
            StudentName = "Nguyễn Văn Sinh Viên",
            RoomId = 10,
            RoomNumber = "P201",
            CheckInTime = now.AddHours(-2),
            CheckOutTime = now,
            Status = VisitorStatus.CheckedOut,
            Note = "Gặp bạn học nhóm"
        };

        var createDto = new CreateVisitorLogDto
        {
            VisitorName = "Lê Thị Lan",
            IdentificationNumber = "034200004567",
            PhoneNumber = "0987654321",
            StudentId = 2,
            RoomId = 10,
            Note = "Gặp bạn học nhóm"
        };

        var filterDto = new VisitorFilterDto
        {
            Status = VisitorStatus.InDormitory,
            FromDate = now.Date,
            ToDate = now.Date.AddDays(1),
            RoomId = 10,
            SearchKeyword = "034200004567",
            OnlyOverstayed = false
        };

        // Assert
        logDto.VisitorName.Should().Be("Lê Thị Lan");
        logDto.StudentName.Should().Be("Nguyễn Văn Sinh Viên");
        logDto.RoomNumber.Should().Be("P201");
        logDto.StatusText.Should().Be("Đã rời KTX");

        createDto.VisitorName.Should().Be("Lê Thị Lan");
        createDto.StudentId.Should().Be(2);

        filterDto.Status.Should().Be(VisitorStatus.InDormitory);
        filterDto.SearchKeyword.Should().Be("034200004567");
        filterDto.OnlyOverstayed.Should().BeFalse();
    }

    [Fact]
    public void AttendanceDtos_StatsAndBatch_ShouldWorkProperly()
    {
        // Arrange & Act
        var attendanceDate = new DateTime(2026, 10, 10);
        var batchDto = new BatchRoomAttendanceDto
        {
            RoomId = 101,
            AttendanceDate = attendanceDate,
            RecordedBy = "Bảo vệ Hoàng",
            StudentAttendances = new List<AttendanceRecordDto>
            {
                new()
                {
                    Id = 1,
                    StudentId = 10,
                    StudentName = "SV A",
                    StudentCode = "SV001",
                    RoomId = 101,
                    RoomNumber = "P101",
                    Status = AttendanceStatus.Present
                },
                new()
                {
                    Id = 2,
                    StudentId = 11,
                    StudentName = "SV B",
                    StudentCode = "SV002",
                    RoomId = 101,
                    RoomNumber = "P101",
                    Status = AttendanceStatus.AbsentWithoutPermission,
                    Reason = "Không liên lạc được"
                }
            }
        };

        var statsDto = new AttendanceStatsDto
        {
            TotalStudents = 10,
            PresentCount = 8,
            AbsentWithoutPermissionCount = 1,
            AbsentWithPermissionCount = 1,
            LateCount = 0,
            AttendanceRate = 80.0
        };

        var filterDto = new AttendanceFilterDto
        {
            Date = attendanceDate,
            RoomId = 101,
            Status = AttendanceStatus.AbsentWithoutPermission,
            SearchKeyword = "SV002"
        };

        // Assert
        batchDto.RoomId.Should().Be(101);
        batchDto.StudentAttendances.Should().HaveCount(2);
        statsDto.TotalStudents.Should().Be(10);
        statsDto.AttendanceRate.Should().Be(80.0);
        filterDto.Date.Should().Be(attendanceDate);
        filterDto.SearchKeyword.Should().Be("SV002");
    }

    [Fact]
    public async Task VisitorLog_PersistenceAndNavigation_ShouldPersistAndCascadeProperly()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "P301",
            Building = "Tòa A",
            Floor = 3,
            Capacity = 4,
            PricePerMonth = 1500000m,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var student = new Student
        {
            StudentCode = "SV2026001",
            FullName = "Nguyễn Văn Sinh Viên",
            DateOfBirth = new DateTime(2004, 5, 10),
            IdentityCard = "012345678901",
            PhoneNumber = "0912345678",
            CurrentRoomId = room.Id
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var visitorLog = new VisitorLog
        {
            VisitorName = "Trần Thị Khách",
            IdentificationNumber = "098765432109",
            PhoneNumber = "0988776655",
            StudentId = student.Id,
            RoomId = room.Id,
            CheckInTime = DateTime.UtcNow.AddHours(-1),
            Status = VisitorStatus.InDormitory,
            Note = "Gửi tài liệu học tập"
        };
        _context.VisitorLogs.Add(visitorLog);
        await _context.SaveChangesAsync();

        // Act
        var savedLog = await _context.VisitorLogs
            .Include(v => v.Student)
            .Include(v => v.Room)
            .FirstOrDefaultAsync(v => v.Id == visitorLog.Id);

        var retrievedStudent = await _context.Students
            .Include(s => s.VisitorLogs)
            .FirstOrDefaultAsync(s => s.Id == student.Id);

        var retrievedRoom = await _context.Rooms
            .Include(r => r.VisitorLogs)
            .FirstOrDefaultAsync(r => r.Id == room.Id);

        // Assert
        savedLog.Should().NotBeNull();
        savedLog!.VisitorName.Should().Be("Trần Thị Khách");
        savedLog.Student.Should().NotBeNull();
        savedLog.Student!.FullName.Should().Be("Nguyễn Văn Sinh Viên");
        savedLog.Room.Should().NotBeNull();
        savedLog.Room!.RoomNumber.Should().Be("P301");

        retrievedStudent.Should().NotBeNull();
        retrievedStudent!.VisitorLogs.Should().ContainSingle(v => v.Id == visitorLog.Id);

        retrievedRoom.Should().NotBeNull();
        retrievedRoom!.VisitorLogs.Should().ContainSingle(v => v.Id == visitorLog.Id);
    }

    [Fact]
    public async Task AttendanceRecord_PersistenceAndNavigation_ShouldPersistProperly()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "P302",
            Building = "Tòa A",
            Floor = 3,
            Capacity = 4,
            PricePerMonth = 1500000m,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var student = new Student
        {
            StudentCode = "SV2026002",
            FullName = "Trần Văn An",
            DateOfBirth = new DateTime(2004, 8, 20),
            IdentityCard = "012345678902",
            PhoneNumber = "0912345679",
            CurrentRoomId = room.Id
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var attendanceDate = new DateTime(2026, 10, 10);
        var record = new AttendanceRecord
        {
            AttendanceDate = attendanceDate,
            StudentId = student.Id,
            RoomId = room.Id,
            Status = AttendanceStatus.Present,
            RecordedBy = "Quản lý ca đêm"
        };
        _context.AttendanceRecords.Add(record);
        await _context.SaveChangesAsync();

        // Act
        var savedRecord = await _context.AttendanceRecords
            .Include(a => a.Student)
            .Include(a => a.Room)
            .FirstOrDefaultAsync(a => a.Id == record.Id);

        var retrievedStudent = await _context.Students
            .Include(s => s.AttendanceRecords)
            .FirstOrDefaultAsync(s => s.Id == student.Id);

        var retrievedRoom = await _context.Rooms
            .Include(r => r.AttendanceRecords)
            .FirstOrDefaultAsync(r => r.Id == room.Id);

        // Assert
        savedRecord.Should().NotBeNull();
        savedRecord!.Student.Should().NotBeNull();
        savedRecord.Student!.FullName.Should().Be("Trần Văn An");
        savedRecord.Room.Should().NotBeNull();
        savedRecord.Room!.RoomNumber.Should().Be("P302");
        savedRecord.Status.Should().Be(AttendanceStatus.Present);

        retrievedStudent.Should().NotBeNull();
        retrievedStudent!.AttendanceRecords.Should().ContainSingle(a => a.Id == record.Id);

        retrievedRoom.Should().NotBeNull();
        retrievedRoom!.AttendanceRecords.Should().ContainSingle(a => a.Id == record.Id);
    }

    [Fact]
    public async Task AttendanceRecord_DuplicateStudentAndDate_ShouldThrowDbUpdateException()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "P303",
            Building = "Tòa B",
            Floor = 3,
            Capacity = 2,
            PricePerMonth = 1200000m,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var student = new Student
        {
            StudentCode = "SV2026003",
            FullName = "Lê Văn Bình",
            DateOfBirth = new DateTime(2003, 2, 14),
            IdentityCard = "012345678903",
            PhoneNumber = "0912345680",
            CurrentRoomId = room.Id
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var attendanceDate = new DateTime(2026, 10, 10);
        var record1 = new AttendanceRecord
        {
            AttendanceDate = attendanceDate,
            StudentId = student.Id,
            RoomId = room.Id,
            Status = AttendanceStatus.Present
        };
        _context.AttendanceRecords.Add(record1);
        await _context.SaveChangesAsync();

        var record2 = new AttendanceRecord
        {
            AttendanceDate = attendanceDate,
            StudentId = student.Id,
            RoomId = room.Id,
            Status = AttendanceStatus.Late,
            Reason = "Trùng ngày điểm danh"
        };
        _context.AttendanceRecords.Add(record2);

        // Act & Assert
        var act = async () => await _context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }
}
