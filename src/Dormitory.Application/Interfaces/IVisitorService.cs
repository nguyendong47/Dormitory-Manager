using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý khách đến thăm sinh viên KTX và kiểm soát ra vào
/// </summary>
public interface IVisitorService
{
    /// <summary>
    /// Đăng ký thông tin và ghi nhận lượt khách check-in vào KTX
    /// </summary>
    /// <param name="dto">Dữ liệu thông tin khách đăng ký</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Thông tin bản ghi khách đã được check-in</returns>
    Task<VisitorLogDto> CheckInAsync(CreateVisitorLogDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ghi nhận khách check-out rời khỏi ký túc xá
    /// </summary>
    /// <param name="visitorLogId">Mã định danh bản ghi khách ra vào</param>
    /// <param name="note">Ghi chú bổ sung lúc rời đi (tùy chọn)</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Thông tin bản ghi khách sau khi check-out</returns>
    Task<VisitorLogDto> CheckOutAsync(int visitorLogId, string? note = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách lượt khách ra vào KTX theo bộ lọc tìm kiếm
    /// </summary>
    /// <param name="filter">Tiêu chí bộ lọc</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Danh sách thông tin lượt khách thỏa mãn bộ lọc</returns>
    Task<IReadOnlyList<VisitorLogDto>> GetVisitorsAsync(VisitorFilterDto filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy thông tin chi tiết một lượt khách theo mã định danh
    /// </summary>
    /// <param name="id">Mã định danh bản ghi</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Chi tiết lượt khách hoặc null nếu không tìm thấy</returns>
    Task<VisitorLogDto?> GetVisitorByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Quét và tự động đánh dấu các lượt khách đang ở KTX nhưng quá giờ quy định (sau 22:00) thành Overstayed
    /// </summary>
    /// <param name="currentTime">Thời điểm đối soát (mặc định lấy UtcNow nếu null)</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>Số lượng lượt khách đã được đánh dấu quá giờ</returns>
    Task<int> MarkOverstayedVisitorsAsync(DateTime? currentTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy số liệu tổng hợp thống kê lượt khách đến thăm trong ngày
    /// </summary>
    /// <param name="date">Ngày cần thống kê (mặc định hôm nay nếu null)</param>
    /// <param name="cancellationToken">Token hủy thao tác bất đồng bộ</param>
    /// <returns>DTO tổng hợp số liệu khách</returns>
    Task<VisitorSummaryDto> GetVisitorSummaryAsync(DateTime? date = null, CancellationToken cancellationToken = default);
}
