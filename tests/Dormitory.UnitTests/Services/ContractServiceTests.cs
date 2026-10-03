using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Application.Services;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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

    [Fact]
    public async Task RenewContractAsync_WhenContractIsTerminated_ShouldThrowInvalidOperationException()
    {
        // Sắp đặt kết nối SQLite In-Memory
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Dormitory.Infrastructure.Data.DormitoryDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new Dormitory.Infrastructure.Data.DormitoryDbContext(options);
        context.Database.EnsureCreated();

        var student = new Student
        {
            StudentCode = "SV001",
            FullName = "Nguyễn Văn Test",
            DateOfBirth = new DateTime(2002, 1, 1),
            IdentityCard = "123456789012"
        };
        var room = new Room
        {
            RoomNumber = "101",
            Building = "Tòa A",
            Capacity = 4,
            PricePerMonth = 500000m
        };
        context.Students.Add(student);
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ContractNumber = "HD-TEST-001",
            StudentId = student.Id,
            RoomId = room.Id,
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            MonthlyRate = 500000m,
            Status = ContractStatus.Terminated
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var service = new ContractService(context);
        var newEndDate = contract.EndDate.AddMonths(6);

        // Thực hiện & Kiểm tra
        var act = async () => await service.RenewContractAsync(contract.Id, newEndDate);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Không thể gia hạn hợp đồng đã bị chấm dứt*");
    }
}
