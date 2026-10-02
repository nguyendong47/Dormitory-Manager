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
}
