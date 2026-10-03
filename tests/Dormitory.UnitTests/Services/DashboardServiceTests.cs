using Dormitory.Application.DTOs;
using Dormitory.Application.Services;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử các dịch vụ thống kê phân tích nâng cao cho Dashboard (TDD)
/// </summary>
public class DashboardServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        // Khởi tạo kết nối SQLite In-Memory độc lập
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _service = new DashboardService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetBuildingOccupancyAsync_ShouldGroupRoomsByBuildingCorrectly()
    {
        // Sắp đặt dữ liệu: 2 phòng Tòa A, 1 phòng Tòa B
        _context.Rooms.AddRange(
            new Room
            {
                RoomNumber = "A101",
                Building = "Tòa A",
                Capacity = 4,
                CurrentOccupancy = 4,
                Status = RoomStatus.Occupied
            },
            new Room
            {
                RoomNumber = "A102",
                Building = "Tòa A",
                Capacity = 4,
                CurrentOccupancy = 2,
                Status = RoomStatus.Available
            },
            new Room
            {
                RoomNumber = "B201",
                Building = "Tòa B",
                Capacity = 2,
                CurrentOccupancy = 0,
                Status = RoomStatus.Available
            }
        );
        await _context.SaveChangesAsync();

        // Thực hiện
        var result = await _service.GetBuildingOccupancyAsync();

        // Kiểm tra
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var toaA = result.First(b => b.BuildingName == "Tòa A");
        toaA.TotalRooms.Should().Be(2);
        toaA.OccupiedRooms.Should().Be(1);
        toaA.AvailableRooms.Should().Be(1);
        toaA.TotalBeds.Should().Be(8);
        toaA.OccupiedBeds.Should().Be(6);
        toaA.OccupancyRate.Should().Be(75.0m); // 6 / 8 * 100 = 75%

        var toaB = result.First(b => b.BuildingName == "Tòa B");
        toaB.TotalRooms.Should().Be(1);
        toaB.OccupiedRooms.Should().Be(0);
        toaB.AvailableRooms.Should().Be(1);
        toaB.TotalBeds.Should().Be(2);
        toaB.OccupiedBeds.Should().Be(0);
        toaB.OccupancyRate.Should().Be(0m);
    }

    [Fact]
    public async Task GetBuildingOccupancyAsync_WithEmptyBuilding_ShouldAssignToChuaPhanToa()
    {
        // Sắp đặt phòng có tên tòa rỗng hoặc khoảng trắng
        _context.Rooms.Add(new Room
        {
            RoomNumber = "X01",
            Building = "   ",
            Capacity = 2,
            CurrentOccupancy = 1,
            Status = RoomStatus.Available
        });
        await _context.SaveChangesAsync();

        // Thực hiện
        var result = await _service.GetBuildingOccupancyAsync();

        // Kiểm tra
        result.Should().ContainSingle();
        result[0].BuildingName.Should().Be("Chưa phân tòa");
        result[0].TotalRooms.Should().Be(1);
        result[0].OccupiedBeds.Should().Be(1);
        result[0].TotalBeds.Should().Be(2);
        result[0].OccupancyRate.Should().Be(50m);
    }

    [Fact]
    public async Task GetBuildingOccupancyAsync_WhenNoRooms_ShouldReturnEmptyList()
    {
        // Thực hiện khi DB trống
        var result = await _service.GetBuildingOccupancyAsync();

        // Kiểm tra
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRevenueTrendsAsync_ShouldCalculateLastNMonthsRevenueCorrectly()
    {
        var now = DateTime.UtcNow;
        var currentMonth = now.Month;
        var currentYear = now.Year;

        // Tính tháng trước
        var prevDate = now.AddMonths(-1);
        var prevMonth = prevDate.Month;
        var prevYear = prevDate.Year;

        // Tạo phòng hợp lệ cho hóa đơn (ràng buộc khóa ngoại)
        var room = new Room
        {
            RoomNumber = "A101",
            Building = "Tòa A",
            Capacity = 4,
            PricePerMonth = 1000000m
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Tạo hóa đơn mẫu
        // Tháng này: 1 Paid, 1 Unpaid (phải bỏ qua Unpaid)
        // Tháng trước: 1 Paid
        _context.Bills.AddRange(
            new Bill
            {
                BillCode = "HD-THIS-PAID",
                RoomId = room.Id,
                Month = currentMonth,
                Year = currentYear,
                RoomFee = 1000000m,
                OldElectricIndex = 0,
                NewElectricIndex = 100, // 100 * 3500 = 350,000
                ElectricRate = 3500m,
                OldWaterIndex = 0,
                NewWaterIndex = 10,     // 10 * 15000 = 150,000
                WaterRate = 15000m,
                OtherServiceFee = 50000m, // Tổng utility = 350k + 150k + 50k = 550,000
                Status = BillStatus.Paid
            },
            new Bill
            {
                BillCode = "HD-THIS-UNPAID",
                RoomId = room.Id,
                Month = currentMonth,
                Year = currentYear,
                RoomFee = 2000000m,
                OldElectricIndex = 0,
                NewElectricIndex = 50,
                ElectricRate = 3500m,
                OldWaterIndex = 0,
                NewWaterIndex = 5,
                WaterRate = 15000m,
                OtherServiceFee = 20000m,
                Status = BillStatus.Unpaid // Không được tính vào doanh thu
            },
            new Bill
            {
                BillCode = "HD-PREV-PAID",
                RoomId = room.Id,
                Month = prevMonth,
                Year = prevYear,
                RoomFee = 800000m,
                OldElectricIndex = 0,
                NewElectricIndex = 50, // 50 * 3500 = 175,000
                ElectricRate = 3500m,
                OldWaterIndex = 0,
                NewWaterIndex = 5,     // 5 * 15000 = 75,000
                WaterRate = 15000m,
                OtherServiceFee = 30000m, // Tổng utility = 175k + 75k + 30k = 280,000
                Status = BillStatus.Paid
            }
        );
        await _context.SaveChangesAsync();

        // Thực hiện với 6 tháng mặc định
        var trends = await _service.GetRevenueTrendsAsync(6);

        // Kiểm tra
        trends.Should().NotBeNull();
        trends.Should().HaveCount(6);

        // Tháng cuối cùng phải là tháng hiện tại
        var currentTrend = trends.Last();
        currentTrend.Month.Should().Be(currentMonth);
        currentTrend.Year.Should().Be(currentYear);
        currentTrend.Label.Should().Be($"T{currentMonth:D2}/{currentYear}");
        currentTrend.RoomFeeRevenue.Should().Be(1000000m);
        currentTrend.UtilityFeeRevenue.Should().Be(550000m);
        currentTrend.TotalRevenue.Should().Be(1550000m);

        // Tháng áp chót phải là tháng trước
        var prevTrend = trends[^2];
        prevTrend.Month.Should().Be(prevMonth);
        prevTrend.Year.Should().Be(prevYear);
        prevTrend.Label.Should().Be($"T{prevMonth:D2}/{prevYear}");
        prevTrend.RoomFeeRevenue.Should().Be(800000m);
        prevTrend.UtilityFeeRevenue.Should().Be(280000m);
        prevTrend.TotalRevenue.Should().Be(1080000m);

        // Các tháng còn lại trong 6 tháng không có hóa đơn phải có doanh thu bằng 0
        for (int i = 0; i < trends.Count - 2; i++)
        {
            trends[i].RoomFeeRevenue.Should().Be(0m);
            trends[i].UtilityFeeRevenue.Should().Be(0m);
            trends[i].TotalRevenue.Should().Be(0m);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task GetRevenueTrendsAsync_WithInvalidMonths_ShouldDefaultToSixMonths(int invalidMonths)
    {
        // Thực hiện
        var trends = await _service.GetRevenueTrendsAsync(invalidMonths);

        // Kiểm tra: Tự động fallback về 6 tháng
        trends.Should().NotBeNull();
        trends.Should().HaveCount(6);
    }

    [Fact]
    public async Task GetStatsAsync_ShouldCalculateDashboardMetricsCorrectly()
    {
        // Sắp đặt phòng
        var room1 = new Room { RoomNumber = "101", Building = "A", Capacity = 4, CurrentOccupancy = 4, Status = RoomStatus.Occupied };
        var room2 = new Room { RoomNumber = "102", Building = "A", Capacity = 4, CurrentOccupancy = 0, Status = RoomStatus.Available };
        _context.Rooms.AddRange(room1, room2);

        // Sắp đặt sinh viên
        var student = new Student { FullName = "Nguyen Van A", StudentCode = "SV001", PhoneNumber = "0123456789" };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Sắp đặt hợp đồng đang hoạt động
        _context.Contracts.Add(new Contract
        {
            ContractNumber = "HD01",
            StudentId = student.Id,
            RoomId = room1.Id,
            StartDate = DateTime.UtcNow.AddMonths(-1),
            EndDate = DateTime.UtcNow.AddMonths(5),
            MonthlyRate = 1000000m,
            Status = ContractStatus.Active
        });

        // Sắp đặt hóa đơn
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        _context.Bills.AddRange(
            new Bill
            {
                BillCode = "B1",
                RoomId = room1.Id,
                Month = currentMonth,
                Year = currentYear,
                RoomFee = 1000000m,
                OldElectricIndex = 0,
                NewElectricIndex = 10,
                ElectricRate = 3500m,
                OldWaterIndex = 0,
                NewWaterIndex = 1,
                WaterRate = 15000m,
                Status = BillStatus.Paid // 1,000,000 + 35,000 + 15,000 = 1,050,000
            },
            new Bill
            {
                BillCode = "B2",
                RoomId = room2.Id,
                Month = currentMonth,
                Year = currentYear,
                RoomFee = 1000000m,
                Status = BillStatus.Unpaid
            }
        );

        await _context.SaveChangesAsync();

        // Thực hiện
        var stats = await _service.GetStatsAsync();

        // Kiểm tra
        stats.Should().NotBeNull();
        stats.TotalRooms.Should().Be(2);
        stats.OccupiedRooms.Should().Be(1);
        stats.AvailableRooms.Should().Be(1);
        stats.TotalStudents.Should().Be(1);
        stats.ActiveContractsCount.Should().Be(1);
        stats.UnpaidBillsCount.Should().Be(1);
        stats.MonthlyRevenue.Should().Be(1050000m);
    }
}
