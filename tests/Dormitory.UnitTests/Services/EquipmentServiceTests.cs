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
/// Kiểm thử các chức năng xử lý nghiệp vụ quản lý trang thiết bị phòng KTX (EquipmentService)
/// </summary>
public class EquipmentServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly EquipmentService _service;
    private readonly Room _room1;
    private readonly Room _room2;

    public EquipmentServiceTests()
    {
        // Khởi tạo kết nối SQLite In-Memory độc lập cho từng bài test
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        // Nạp phòng mẫu để liên kết thiết bị
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

        _service = new EquipmentService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task CreateEquipmentAsync_ValidInput_ShouldCreateAndReturnDto()
    {
        // Sắp đặt
        var dto = new CreateEquipmentDto
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-A101-01",
            Name = "Điều hòa Daikin 12000BTU",
            Status = EquipmentStatus.Good,
            Quantity = 1,
            Price = 9500000,
            Notes = "Mới lắp đặt năm 2024"
        };

        // Thực hiện
        var result = await _service.CreateEquipmentAsync(dto);

        // Kiểm tra kết quả trả về
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.RoomId.Should().Be(_room1.Id);
        result.RoomNumber.Should().Be("A101");
        result.BuildingName.Should().Be("Tòa A");
        result.EquipmentCode.Should().Be("TB-A101-01");
        result.Name.Should().Be("Điều hòa Daikin 12000BTU");
        result.Status.Should().Be(EquipmentStatus.Good);
        result.StatusText.Should().Be("Hoạt động tốt");
        result.Quantity.Should().Be(1);
        result.Price.Should().Be(9500000);
        result.Notes.Should().Be("Mới lắp đặt năm 2024");

        // Kiểm tra lưu trong cơ sở dữ liệu
        var dbEntity = await _context.Equipments.FindAsync(result.Id);
        dbEntity.Should().NotBeNull();
        dbEntity!.EquipmentCode.Should().Be("TB-A101-01");
        dbEntity.Name.Should().Be("Điều hòa Daikin 12000BTU");
    }

    [Theory]
    [InlineData("", "Tủ lạnh")]
    [InlineData("   ", "Tủ lạnh")]
    [InlineData("TB01", "")]
    [InlineData("TB01", "   ")]
    public async Task CreateEquipmentAsync_EmptyCodeOrName_ShouldThrowArgumentException(string code, string name)
    {
        // Sắp đặt
        var dto = new CreateEquipmentDto
        {
            RoomId = _room1.Id,
            EquipmentCode = code,
            Name = name,
            Status = EquipmentStatus.Good,
            Quantity = 1,
            Price = 1000000
        };

        // Thực hiện & Kiểm tra
        var act = async () => await _service.CreateEquipmentAsync(dto);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 100000)]
    [InlineData(-1, 100000)]
    [InlineData(1, -50000)]
    public async Task CreateEquipmentAsync_InvalidQuantityOrPrice_ShouldThrowArgumentException(int quantity, decimal price)
    {
        // Sắp đặt
        var dto = new CreateEquipmentDto
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB99",
            Name = "Bàn học",
            Status = EquipmentStatus.Good,
            Quantity = quantity,
            Price = price
        };

        // Thực hiện & Kiểm tra
        var act = async () => await _service.CreateEquipmentAsync(dto);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateEquipmentAsync_NonExistentRoom_ShouldThrowKeyNotFoundException()
    {
        // Sắp đặt
        var dto = new CreateEquipmentDto
        {
            RoomId = 9999, // Không tồn tại
            EquipmentCode = "TB01",
            Name = "Quạt điện",
            Status = EquipmentStatus.Good,
            Quantity = 1,
            Price = 300000
        };

        // Thực hiện & Kiểm tra
        var act = async () => await _service.CreateEquipmentAsync(dto);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetAllEquipmentsAsync_WithSearchTermAndRoomFilter_ShouldReturnFilteredResults()
    {
        // Sắp đặt danh sách thiết bị mẫu
        var item1 = new Equipment
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-01",
            Name = "Điều hòa Daikin",
            Status = EquipmentStatus.Good,
            Quantity = 1,
            Price = 9000000
        };
        var item2 = new Equipment
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-02",
            Name = "Quạt trần Vinawind",
            Status = EquipmentStatus.NeedsRepair,
            Quantity = 2,
            Price = 800000
        };
        var item3 = new Equipment
        {
            RoomId = _room2.Id,
            EquipmentCode = "TB-03",
            Name = "Bình nóng lạnh Ariston",
            Status = EquipmentStatus.Broken,
            Quantity = 1,
            Price = 2500000
        };

        _context.Equipments.AddRange(item1, item2, item3);
        await _context.SaveChangesAsync();

        // 1. Lọc theo phòng 1
        var byRoom = await _service.GetAllEquipmentsAsync(roomId: _room1.Id);
        byRoom.Should().HaveCount(2);
        byRoom.Select(e => e.EquipmentCode).Should().Contain(new[] { "TB-01", "TB-02" });

        // 2. Lọc theo từ khóa "Daikin"
        var bySearch = await _service.GetAllEquipmentsAsync(searchTerm: "Daikin");
        bySearch.Should().HaveCount(1);
        bySearch[0].Name.Should().Be("Điều hòa Daikin");

        // 3. Lọc theo trạng thái Broken
        var byStatus = await _service.GetAllEquipmentsAsync(status: EquipmentStatus.Broken);
        byStatus.Should().HaveCount(1);
        byStatus[0].EquipmentCode.Should().Be("TB-03");

        // 4. Lọc kết hợp phòng và trạng thái
        var combined = await _service.GetAllEquipmentsAsync(roomId: _room1.Id, status: EquipmentStatus.NeedsRepair);
        combined.Should().HaveCount(1);
        combined[0].EquipmentCode.Should().Be("TB-02");
    }

    [Fact]
    public async Task GetEquipmentByIdAsync_WithExistingId_ShouldReturnDto()
    {
        // Sắp đặt
        var item = new Equipment
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-10",
            Name = "Tủ locker 4 ngăn",
            Status = EquipmentStatus.Good,
            Quantity = 1,
            Price = 1800000
        };
        _context.Equipments.Add(item);
        await _context.SaveChangesAsync();

        // Thực hiện
        var result = await _service.GetEquipmentByIdAsync(item.Id);

        // Kiểm tra
        result.Should().NotBeNull();
        result!.EquipmentCode.Should().Be("TB-10");
        result.RoomNumber.Should().Be("A101");
        result.Name.Should().Be("Tủ locker 4 ngăn");
    }

    [Fact]
    public async Task GetEquipmentByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Thực hiện
        var result = await _service.GetEquipmentByIdAsync(9999);

        // Kiểm tra
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEquipmentsByRoomIdAsync_ShouldReturnRoomEquipments()
    {
        // Sắp đặt
        _context.Equipments.AddRange(
            new Equipment { RoomId = _room1.Id, EquipmentCode = "EQ-1", Name = "Bàn", Status = EquipmentStatus.Good, Quantity = 2, Price = 500000 },
            new Equipment { RoomId = _room2.Id, EquipmentCode = "EQ-2", Name = "Ghế", Status = EquipmentStatus.Good, Quantity = 4, Price = 200000 }
        );
        await _context.SaveChangesAsync();

        // Thực hiện
        var list = await _service.GetEquipmentsByRoomIdAsync(_room1.Id);

        // Kiểm tra
        list.Should().HaveCount(1);
        list[0].EquipmentCode.Should().Be("EQ-1");
    }

    [Fact]
    public async Task UpdateEquipmentAsync_ValidInput_ShouldUpdateAndReturnDto()
    {
        // Sắp đặt
        var item = new Equipment
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-20",
            Name = "Quạt cây",
            Status = EquipmentStatus.Good,
            Quantity = 1,
            Price = 400000
        };
        _context.Equipments.Add(item);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateEquipmentDto
        {
            RoomId = _room2.Id, // Chuyển sang phòng 2
            EquipmentCode = "TB-20-MOD",
            Name = "Quạt lửng Senko",
            Status = EquipmentStatus.NeedsRepair,
            Quantity = 2,
            Price = 450000,
            Notes = "Cần tra dầu định kỳ"
        };

        // Thực hiện
        var result = await _service.UpdateEquipmentAsync(item.Id, updateDto);

        // Kiểm tra kết quả trả về
        result.Should().NotBeNull();
        result.RoomId.Should().Be(_room2.Id);
        result.RoomNumber.Should().Be("B201");
        result.EquipmentCode.Should().Be("TB-20-MOD");
        result.Name.Should().Be("Quạt lửng Senko");
        result.Status.Should().Be(EquipmentStatus.NeedsRepair);
        result.StatusText.Should().Be("Cần bảo trì");
        result.Quantity.Should().Be(2);
        result.Notes.Should().Be("Cần tra dầu định kỳ");

        // Kiểm tra lưu DB
        var dbItem = await _context.Equipments.FindAsync(item.Id);
        dbItem!.Name.Should().Be("Quạt lửng Senko");
        dbItem.RoomId.Should().Be(_room2.Id);
    }

    [Fact]
    public async Task UpdateEquipmentAsync_NonExistentId_ShouldThrowKeyNotFoundException()
    {
        var updateDto = new UpdateEquipmentDto
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-99",
            Name = "Thiết bị ảo",
            Status = EquipmentStatus.Good,
            Quantity = 1,
            Price = 100000
        };

        var act = async () => await _service.UpdateEquipmentAsync(9999, updateDto);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldChangeStatusAndRecordMaintenanceDate()
    {
        // Sắp đặt thiết bị đang hỏng
        var item = new Equipment
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-30",
            Name = "Máy nước nóng",
            Status = EquipmentStatus.Broken,
            Quantity = 1,
            Price = 3000000,
            LastMaintainedAt = null
        };
        _context.Equipments.Add(item);
        await _context.SaveChangesAsync();

        // Thực hiện: Cập nhật thành trạng thái Good (đã sửa xong) kèm ghi chú bảo trì
        var success = await _service.UpdateStatusAsync(item.Id, EquipmentStatus.Good, "Đã bảo trì thay rơ-le nhiệt");

        // Kiểm tra
        success.Should().BeTrue();
        var dbItem = await _context.Equipments.FindAsync(item.Id);
        dbItem!.Status.Should().Be(EquipmentStatus.Good);
        dbItem.LastMaintainedAt.Should().NotBeNull();
        dbItem.LastMaintainedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        dbItem.Notes.Should().Be("Đã bảo trì thay rơ-le nhiệt");
    }

    [Fact]
    public async Task DeleteEquipmentAsync_ShouldRemoveEntityFromDatabase()
    {
        // Sắp đặt
        var item = new Equipment
        {
            RoomId = _room1.Id,
            EquipmentCode = "TB-40",
            Name = "Ghế xoay văn phòng",
            Status = EquipmentStatus.Broken,
            Quantity = 1,
            Price = 600000
        };
        _context.Equipments.Add(item);
        await _context.SaveChangesAsync();

        // Thực hiện
        var success = await _service.DeleteEquipmentAsync(item.Id);

        // Kiểm tra
        success.Should().BeTrue();
        var dbItem = await _context.Equipments.FindAsync(item.Id);
        dbItem.Should().BeNull();
    }

    [Fact]
    public async Task DeleteEquipmentAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Thực hiện
        var success = await _service.DeleteEquipmentAsync(9999);

        // Kiểm tra
        success.Should().BeFalse();
    }
}
