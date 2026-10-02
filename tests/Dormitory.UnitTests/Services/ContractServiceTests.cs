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
/// Kiểm thử các ràng buộc nghiệp vụ trong phân hệ Hợp đồng thuê phòng
/// </summary>
public class ContractServiceTests
{
    [Fact]
    public async Task CreateContractAsync_WithEndDateBeforeStartDate_ShouldThrowArgumentException()
    {
        // Sắp đặt mock
        var mockContext = new Mock<IDormitoryDbContext>();
        var service = new ContractService(mockContext.Object);

        var request = new CreateContractRequest
        {
            StudentId = 1,
            RoomId = 1,
            StartDate = new DateTime(2025, 6, 1),
            EndDate = new DateTime(2025, 1, 1), // Lỗi: Ngày kết thúc trước ngày bắt đầu
            MonthlyRate = 500000m
        };

        // Thực hiện & Kiểm tra
        var act = async () => await service.CreateContractAsync(request);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Ngày kết thúc hợp đồng phải sau ngày bắt đầu*");
    }

    [Fact]
    public void Room_HasVacancy_WhenFull_ShouldBeFalse()
    {
        // Sắp đặt phòng đã đầy
        var room = new Room
        {
            Capacity = 4,
            CurrentOccupancy = 4,
            Status = RoomStatus.Occupied
        };

        // Kiểm tra
        room.HasVacancy.Should().BeFalse();
    }

    [Fact]
    public void Room_HasVacancy_WhenAvailableAndUnderCapacity_ShouldBeTrue()
    {
        // Sắp đặt phòng còn chỗ
        var room = new Room
        {
            Capacity = 4,
            CurrentOccupancy = 2,
            Status = RoomStatus.Available
        };

        // Kiểm tra
        room.HasVacancy.Should().BeTrue();
    }

    [Fact]
    public void Room_HasVacancy_WhenInMaintenance_ShouldBeFalse()
    {
        // Sắp đặt phòng đang bảo trì
        var room = new Room
        {
            Capacity = 4,
            CurrentOccupancy = 0,
            Status = RoomStatus.Maintenance
        };

        // Kiểm tra
        room.HasVacancy.Should().BeFalse();
    }
}
