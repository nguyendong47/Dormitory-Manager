using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Application.Services;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử các quy tắc tính toán hóa đơn tiền điện, nước, phòng
/// </summary>
public class BillCalculationTests
{
    [Fact]
    public void Bill_CalculatedProperties_ShouldCalculateCorrectly()
    {
        // Sắp đặt hóa đơn phòng
        var bill = new Bill
        {
            RoomFee = 600000m,
            OldElectricIndex = 100m,
            NewElectricIndex = 160m, // Tiêu thụ 60 kWh
            ElectricRate = 3500m,     // 60 * 3500 = 210,000 VND
            OldWaterIndex = 20m,
            NewWaterIndex = 30m,     // Tiêu thụ 10 m3
            WaterRate = 15000m,      // 10 * 15000 = 150,000 VND
            OtherServiceFee = 50000m // Phụ phí 50,000 VND
        };

        // Thực hiện & Kiểm tra
        bill.ElectricUsage.Should().Be(60m);
        bill.ElectricFee.Should().Be(210000m);
        bill.WaterUsage.Should().Be(10m);
        bill.WaterFee.Should().Be(150000m);

        // Tổng tiền = 600,000 + 210,000 + 150,000 + 50,000 = 1,010,000 VND
        bill.TotalAmount.Should().Be(1010000m);
    }

    [Fact]
    public void Bill_WithZeroUsage_ShouldChargeOnlyRoomAndServiceFee()
    {
        // Sắp đặt hóa đơn không sử dụng điện nước
        var bill = new Bill
        {
            RoomFee = 800000m,
            OldElectricIndex = 50m,
            NewElectricIndex = 50m,
            ElectricRate = 3500m,
            OldWaterIndex = 10m,
            NewWaterIndex = 10m,
            WaterRate = 15000m,
            OtherServiceFee = 30000m
        };

        // Kiểm tra
        bill.ElectricUsage.Should().Be(0m);
        bill.ElectricFee.Should().Be(0m);
        bill.WaterUsage.Should().Be(0m);
        bill.WaterFee.Should().Be(0m);
        bill.TotalAmount.Should().Be(830000m);
    }

    [Fact]
    public async Task CreateBillAsync_WithNewElectricLessThanOld_ShouldThrowArgumentException()
    {
        // Sắp đặt mock DbContext
        var mockContext = new Mock<IDormitoryDbContext>();
        var service = new BillService(mockContext.Object);

        var request = new CreateBillRequest
        {
            RoomId = 1,
            Month = 10,
            Year = 2024,
            OldElectricIndex = 200m,
            NewElectricIndex = 150m, // Lỗi: Chỉ số mới nhỏ hơn chỉ số cũ
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        // Thực hiện & Kiểm tra
        var act = async () => await service.CreateBillAsync(request);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Chỉ số điện mới không được nhỏ hơn chỉ số cũ*");
    }

    [Fact]
    public async Task CreateBillAsync_WithNewWaterLessThanOld_ShouldThrowArgumentException()
    {
        // Sắp đặt mock DbContext
        var mockContext = new Mock<IDormitoryDbContext>();
        var service = new BillService(mockContext.Object);

        var request = new CreateBillRequest
        {
            RoomId = 1,
            Month = 10,
            Year = 2024,
            OldElectricIndex = 100m,
            NewElectricIndex = 120m,
            OldWaterIndex = 50m,
            NewWaterIndex = 40m, // Lỗi: Chỉ số nước mới nhỏ hơn chỉ số cũ
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        // Thực hiện & Kiểm tra
        var act = async () => await service.CreateBillAsync(request);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Chỉ số nước mới không được nhỏ hơn chỉ số cũ*");
    }
}
