using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Services;

/// <summary>
/// Dịch vụ thống kê số liệu tổng quan trên Dashboard
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IDormitoryDbContext _context;

    public DashboardService(IDormitoryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy toàn bộ số liệu thống kê tổng hợp cho Dashboard
    /// </summary>
    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var totalRooms = await _context.Rooms.CountAsync();
        var occupiedRooms = await _context.Rooms.CountAsync(r => r.Status == RoomStatus.Occupied);
        var availableRooms = await _context.Rooms.CountAsync(r => r.Status == RoomStatus.Available);

        var totalStudents = await _context.Students.CountAsync();
        var activeContracts = await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Active);
        var unpaidBills = await _context.Bills.CountAsync(b => b.Status == BillStatus.Unpaid || b.Status == BillStatus.Overdue);

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        // Tính doanh thu tháng hiện tại từ các hóa đơn đã thanh toán
        var paidBillsThisMonth = await _context.Bills
            .Where(b => b.Month == currentMonth && b.Year == currentYear && b.Status == BillStatus.Paid)
            .ToListAsync();

        var monthlyRevenue = paidBillsThisMonth.Sum(b => b.TotalAmount);

        return new DashboardStatsDto
        {
            TotalRooms = totalRooms,
            OccupiedRooms = occupiedRooms,
            AvailableRooms = availableRooms,
            TotalStudents = totalStudents,
            ActiveContractsCount = activeContracts,
            UnpaidBillsCount = unpaidBills,
            MonthlyRevenue = monthlyRevenue
        };
    }

    /// <summary>
    /// Thống kê tỷ lệ lấp đầy phòng và giường theo từng tòa nhà
    /// </summary>
    public async Task<List<BuildingOccupancyDto>> GetBuildingOccupancyAsync()
    {
        var rooms = await _context.Rooms.ToListAsync();

        return rooms
            .GroupBy(r => string.IsNullOrWhiteSpace(r.Building) ? "Chưa phân tòa" : r.Building.Trim())
            .Select(g => new BuildingOccupancyDto
            {
                BuildingName = g.Key,
                TotalRooms = g.Count(),
                OccupiedRooms = g.Count(r => r.Status == RoomStatus.Occupied),
                AvailableRooms = g.Count(r => r.Status == RoomStatus.Available),
                TotalBeds = g.Sum(r => r.Capacity),
                OccupiedBeds = g.Sum(r => r.CurrentOccupancy)
            })
            .OrderBy(b => b.BuildingName)
            .ToList();
    }

    /// <summary>
    /// Thống kê xu hướng doanh thu theo N tháng gần nhất (tiền phòng và dịch vụ điện nước)
    /// </summary>
    public async Task<List<MonthlyRevenueTrendDto>> GetRevenueTrendsAsync(int months = 6)
    {
        if (months <= 0)
        {
            months = 6;
        }

        var now = DateTime.UtcNow;
        var targetMonths = new List<(int Year, int Month)>();
        for (int i = months - 1; i >= 0; i--)
        {
            var date = now.AddMonths(-i);
            targetMonths.Add((date.Year, date.Month));
        }

        var minYear = targetMonths.Min(t => t.Year);
        var maxYear = targetMonths.Max(t => t.Year);

        var paidBills = await _context.Bills
            .Where(b => b.Status == BillStatus.Paid && b.Year >= minYear && b.Year <= maxYear)
            .ToListAsync();

        var trends = new List<MonthlyRevenueTrendDto>();
        foreach (var (year, month) in targetMonths)
        {
            var billsInMonth = paidBills.Where(b => b.Year == year && b.Month == month).ToList();
            var roomFee = billsInMonth.Sum(b => b.RoomFee);
            var utilityFee = billsInMonth.Sum(b => b.ElectricFee + b.WaterFee + b.OtherServiceFee);

            trends.Add(new MonthlyRevenueTrendDto
            {
                Month = month,
                Year = year,
                Label = $"T{month:D2}/{year}",
                RoomFeeRevenue = roomFee,
                UtilityFeeRevenue = utilityFee
            });
        }

        return trends;
    }
}
