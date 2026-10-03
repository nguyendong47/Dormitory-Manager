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
}
