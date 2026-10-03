using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ xuất dữ liệu báo cáo ra file Excel
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Xuất danh sách phòng ở ra file Excel
    /// </summary>
    /// <param name="rooms">Danh sách thông tin phòng</param>
    /// <returns>Mảng byte của file Excel (.xlsx)</returns>
    Task<byte[]> ExportRoomsToExcelAsync(List<RoomDto> rooms);

    /// <summary>
    /// Xuất danh sách sinh viên nội trú ra file Excel
    /// </summary>
    /// <param name="students">Danh sách thông tin sinh viên</param>
    /// <returns>Mảng byte của file Excel (.xlsx)</returns>
    Task<byte[]> ExportStudentsToExcelAsync(List<StudentDto> students);

    /// <summary>
    /// Xuất danh sách hóa đơn điện nước ra file Excel
    /// </summary>
    /// <param name="bills">Danh sách thông tin hóa đơn</param>
    /// <returns>Mảng byte của file Excel (.xlsx)</returns>
    Task<byte[]> ExportBillsToExcelAsync(List<BillDto> bills);

    /// <summary>
    /// Xuất báo cáo tổng hợp Dashboard (chỉ số KPI, tỷ lệ lấp đầy theo tòa nhà và xu hướng doanh thu) ra file Excel
    /// </summary>
    /// <param name="stats">Số liệu thống kê tổng quan</param>
    /// <param name="buildings">Danh sách tỷ lệ lấp đầy theo từng tòa nhà</param>
    /// <param name="trends">Danh sách xu hướng doanh thu theo các tháng gần nhất</param>
    /// <returns>Mảng byte của file Excel (.xlsx)</returns>
    Task<byte[]> ExportDashboardSummaryToExcelAsync(
        DashboardStatsDto stats,
        List<BuildingOccupancyDto> buildings,
        List<MonthlyRevenueTrendDto> trends);
}
