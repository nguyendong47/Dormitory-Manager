using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
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
/// Kiểm thử đơn vị cho dịch vụ Sao lưu & Phục hồi CSDL (DatabaseService) theo quy trình TDD
/// </summary>
public class DatabaseServiceTests : IDisposable
{
    private readonly string _tempDbPath;
    private readonly DormitoryDbContext _context;
    private readonly IDatabaseService _service;

    public DatabaseServiceTests()
    {
        // Khởi tạo file CSDL SQLite vật lý tạm thời để kiểm thử sao lưu/phục hồi thực tế
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"dormitory_test_{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite($"Data Source={_tempDbPath}")
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        // Khởi tạo DatabaseService với context và file DB tạm
        _service = new DatabaseService(_context, customDbPath: _tempDbPath);
    }

    public void Dispose()
    {
        _context.Dispose();
        SqliteConnection.ClearAllPools();

        try
        {
            if (File.Exists(_tempDbPath))
                File.Delete(_tempDbPath);

            var backupPath = $"{_tempDbPath}.bak";
            if (File.Exists(backupPath))
                File.Delete(backupPath);

            var walPath = $"{_tempDbPath}-wal";
            if (File.Exists(walPath))
                File.Delete(walPath);

            var shmPath = $"{_tempDbPath}-shm";
            if (File.Exists(shmPath))
                File.Delete(shmPath);
        }
        catch
        {
            // Bỏ qua lỗi dọn dẹp file tạm khi test hoàn tất
        }
    }

    [Fact]
    public async Task BackupDatabaseAsync_ShouldReturnNonEmptyByteArray()
    {
        // Sắp đặt dữ liệu mẫu
        _context.Rooms.Add(new Room
        {
            RoomNumber = "A101",
            Building = "Tòa A",
            Capacity = 4,
            PricePerMonth = 1500000m,
            Status = RoomStatus.Available,
            Type = RoomType.Standard
        });
        await _context.SaveChangesAsync();

        // Thực hiện
        byte[] backupBytes = await _service.BackupDatabaseAsync();

        // Kiểm tra
        backupBytes.Should().NotBeNull();
        backupBytes.Length.Should().BeGreaterThan(0);

        // Header của SQLite bắt đầu bằng "SQLite format 3\0" (16 bytes)
        var header = System.Text.Encoding.ASCII.GetString(backupBytes.Take(16).ToArray());
        header.Should().StartWith("SQLite format 3");
    }

    [Fact]
    public async Task RestoreDatabaseAsync_WithCorruptData_ShouldReturnFalseAndNotCorruptCurrentDb()
    {
        // Sắp đặt dữ liệu gốc
        _context.Rooms.Add(new Room
        {
            RoomNumber = "A201",
            Building = "Tòa A",
            Capacity = 4,
            PricePerMonth = 1200000m,
            Status = RoomStatus.Available,
            Type = RoomType.Standard
        });
        await _context.SaveChangesAsync();

        // Dữ liệu sao lưu hỏng/không hợp lệ
        byte[] corruptBytes = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };

        // Thực hiện phục hồi
        bool result = await _service.RestoreDatabaseAsync(corruptBytes);

        // Kiểm tra phục hồi thất bại
        result.Should().BeFalse();

        // Kiểm tra CSDL hiện tại vẫn nguyên vẹn và đọc được dữ liệu gốc
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == "A201");
        room.Should().NotBeNull();
    }

    [Fact]
    public async Task RestoreDatabaseAsync_WithValidBackup_ShouldRestoreSuccessfully()
    {
        // Sắp đặt: Thêm 2 phòng ban đầu và tạo bản backup
        _context.Rooms.AddRange(
            new Room
            {
                RoomNumber = "B101",
                Building = "Tòa B",
                Capacity = 4,
                PricePerMonth = 1500000m,
                Status = RoomStatus.Available,
                Type = RoomType.Standard
            },
            new Room
            {
                RoomNumber = "B102",
                Building = "Tòa B",
                Capacity = 2,
                PricePerMonth = 2000000m,
                Status = RoomStatus.Available,
                Type = RoomType.Vip
            }
        );
        await _context.SaveChangesAsync();

        // Tạo bản backup chứa 2 phòng
        byte[] backupBytes = await _service.BackupDatabaseAsync();
        backupBytes.Should().NotBeNullOrEmpty();

        // Thay đổi CSDL: Xóa 1 phòng và thêm phòng mới
        var roomToDelete = await _context.Rooms.FirstAsync(r => r.RoomNumber == "B102");
        _context.Rooms.Remove(roomToDelete);
        _context.Rooms.Add(new Room
        {
            RoomNumber = "B103",
            Building = "Tòa B",
            Capacity = 4,
            PricePerMonth = 1500000m,
            Status = RoomStatus.Available,
            Type = RoomType.Standard
        });
        await _context.SaveChangesAsync();

        // Xác nhận trạng thái sau khi thay đổi (chứa B101 và B103, không có B102)
        var modifiedRooms = await _context.Rooms.AsNoTracking().ToListAsync();
        modifiedRooms.Should().HaveCount(2);
        modifiedRooms.Should().Contain(r => r.RoomNumber == "B103");
        modifiedRooms.Should().NotContain(r => r.RoomNumber == "B102");

        // Thực hiện phục hồi từ bản backup
        bool restoreResult = await _service.RestoreDatabaseAsync(backupBytes);
        restoreResult.Should().BeTrue();

        // Kiểm tra dữ liệu được khôi phục nguyên vẹn về trạng thái backup (chứa B101, B102, không có B103)
        // Cần tạo DbContext mới hoặc mở connection mới để đọc lại file DB sau khi thay thế
        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite($"Data Source={_tempDbPath}")
            .Options;

        using var verifyContext = new DormitoryDbContext(options);
        var restoredRooms = await verifyContext.Rooms.AsNoTracking().ToListAsync();

        restoredRooms.Should().HaveCount(2);
        restoredRooms.Should().Contain(r => r.RoomNumber == "B101");
        restoredRooms.Should().Contain(r => r.RoomNumber == "B102");
        restoredRooms.Should().NotContain(r => r.RoomNumber == "B103");
    }

    [Fact]
    public async Task VerifyDatabaseIntegrityAsync_WithValidAndCorruptDatabase_ShouldReturnExpectedResults()
    {
        // 1. File CSDL hợp lệ
        bool validResult = await _service.VerifyDatabaseIntegrityAsync(_tempDbPath);
        validResult.Should().BeTrue();

        // 2. File không tồn tại
        bool nonExistentResult = await _service.VerifyDatabaseIntegrityAsync("non_existent_file.db");
        nonExistentResult.Should().BeFalse();

        // 3. File tồn tại nhưng không phải SQLite hợp lệ
        var corruptFile = Path.Combine(Path.GetTempPath(), $"corrupt_{Guid.NewGuid():N}.db");
        try
        {
            await File.WriteAllBytesAsync(corruptFile, new byte[] { 1, 2, 3, 4, 5 });
            bool corruptResult = await _service.VerifyDatabaseIntegrityAsync(corruptFile);
            corruptResult.Should().BeFalse();
        }
        finally
        {
            if (File.Exists(corruptFile))
                File.Delete(corruptFile);
        }
    }

    [Fact]
    public async Task GetDatabaseInfoAsync_ShouldReturnCorrectRecordCountAndFileSize()
    {
        // Sắp đặt: Thêm dữ liệu vào nhiều bảng
        _context.Rooms.Add(new Room
        {
            RoomNumber = "C101",
            Building = "Tòa C",
            Capacity = 4,
            PricePerMonth = 1500000m,
            Status = RoomStatus.Available,
            Type = RoomType.Standard
        });

        _context.Students.Add(new Student
        {
            StudentCode = "SV001",
            FullName = "Nguyễn Văn A",
            IdentityCard = "123456789",
            Email = "a@test.com",
            PhoneNumber = "0901234567"
        });

        _context.Users.Add(new User
        {
            Username = "admin_test",
            PasswordHash = "hash",
            FullName = "Quản trị viên",
            Role = UserRole.Admin
        });

        await _context.SaveChangesAsync();

        // Thực hiện
        var info = await _service.GetDatabaseInfoAsync();

        // Kiểm tra
        info.Should().NotBeNull();
        info.DatabasePath.Should().Be(_tempDbPath);
        info.FileSizeBytes.Should().BeGreaterThan(0);
        info.FormattedFileSize.Should().NotBeNullOrWhiteSpace();
        info.TotalRecords.Should().Be(3); // 1 room + 1 student + 1 user + 0 contract + 0 bill + 0 employee
        info.LastModified.Should().BeAfter(DateTime.MinValue);
    }

    [Fact]
    public async Task RestoreDatabaseAsync_WithNullOrEmptyBytes_ShouldReturnFalse()
    {
        // Kiểm tra với mảng byte null
        bool nullResult = await _service.RestoreDatabaseAsync(null!);
        nullResult.Should().BeFalse();

        // Kiểm tra với mảng byte rỗng
        bool emptyResult = await _service.RestoreDatabaseAsync(Array.Empty<byte>());
        emptyResult.Should().BeFalse();
    }

    [Theory]
    [InlineData(500, "500 B")]
    [InlineData(1024, "1 KB")]
    [InlineData(1536, "1.5 KB")]
    [InlineData(1048576, "1 MB")]
    [InlineData(2516582, "2.4 MB")]
    [InlineData(1073741824, "1 GB")]
    public void FormatFileSize_ShouldFormatVariousSizesCorrectly(long bytes, string expected)
    {
        string formatted = DatabaseService.FormatFileSize(bytes);
        formatted.Should().Be(expected);
    }

    [Fact]
    public async Task BackupDatabaseAsync_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Service với đường dẫn file không tồn tại
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.db");
        var serviceWithMissingDb = new DatabaseService(_context, customDbPath: nonExistentPath);

        Func<Task> act = async () => await serviceWithMissingDb.BackupDatabaseAsync();
        await act.Should().ThrowAsync<FileNotFoundException>();
    }
}

