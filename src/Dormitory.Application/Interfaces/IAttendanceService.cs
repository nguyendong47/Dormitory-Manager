using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ điểm danh và theo dõi chuyên cần sinh viên nội trú ban đêm
/// </summary>
public interface IAttendanceService
{
    /// <summary>
    /// Ghi nhận hoặc cập nhật kết quả điểm danh cho một sinh viên cụ thể trong ngày
    /// </summary>
    /// <param name="studentId">Mã định danh sinh viên</param>
    /// <param name="roomId">Mã định danh phòng cư trú</param>
    /// <param name="date">Ngày điểm danh</param>
    /// <param name="status">Trạng thái điểm danh (Có mặt, Vắng không phép, Vắng có phép, Về muộn)</param>
    /// <param name="reason">Lý do vắng hoặc ghi chú cụ thể (tùy chọn)</param>
    /// <param name="recordedBy">Tên người thực hiện điểm danh (tùy chọn)</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>DTO thông tin bản ghi điểm danh</returns>
    Task<AttendanceRecordDto> RecordSingleAttendanceAsync(
        int studentId,
        int roomId,
        DateTime date,
        AttendanceStatus status,
        string? reason = null,
        string? recordedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ghi nhận hoặc cập nhật kết quả điểm danh theo lô cho cả phòng sinh viên
    /// </summary>
    /// <param name="batchDto">Dữ liệu kết quả điểm danh theo phòng</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Danh sách các bản ghi điểm danh đã được ghi nhận</returns>
    Task<IReadOnlyList<AttendanceRecordDto>> RecordBatchRoomAttendanceAsync(
        BatchRoomAttendanceDto batchDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách kết quả điểm danh của một phòng trong ngày chỉ định
    /// </summary>
    /// <param name="roomId">Mã định danh phòng</param>
    /// <param name="date">Ngày điểm danh</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Danh sách bản ghi điểm danh của phòng</returns>
    Task<IReadOnlyList<AttendanceRecordDto>> GetRoomAttendanceAsync(
        int roomId,
        DateTime date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách bản ghi điểm danh theo bộ lọc tìm kiếm
    /// </summary>
    /// <param name="filter">Tiêu chí lọc điểm danh</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Danh sách bản ghi điểm danh thỏa mãn</returns>
    Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(
        AttendanceFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Thống kê kết quả và tính tỷ lệ chuyên cần theo khoảng ngày hoặc phòng
    /// </summary>
    /// <param name="fromDate">Từ ngày (tùy chọn)</param>
    /// <param name="toDate">Đến ngày (tùy chọn)</param>
    /// <param name="roomId">Mã phòng lọc thống kê (tùy chọn)</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>DTO số liệu thống kê và tỷ lệ chuyên cần</returns>
    Task<AttendanceStatsDto> GetAttendanceStatsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? roomId = null,
        CancellationToken cancellationToken = default);
}
