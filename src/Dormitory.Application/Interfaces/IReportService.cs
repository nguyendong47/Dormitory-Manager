using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ tổng hợp và xuất bản báo cáo phân tích ký túc xá
/// </summary>
public interface IReportService
{
    // ==========================================
    // 1. NHÓM TRUY VẤN VÀ TỔNG HỢP DỮ LIỆU
    // ==========================================

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo vi phạm nội quy &amp; kỷ luật sinh viên
    /// </summary>
    Task<ViolationReportSummaryDto> GetViolationReportDataAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo tài chính &amp; thu phí ký túc xá
    /// </summary>
    Task<FinancialReportSummaryDto> GetFinancialReportDataAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo tỷ lệ lấp đầy và tình trạng phòng ở
    /// </summary>
    Task<OccupancyReportSummaryDto> GetOccupancyReportDataAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo kiểm kê tài sản &amp; thiết bị
    /// </summary>
    Task<EquipmentReportSummaryDto> GetEquipmentReportDataAsync(GenerateReportRequestDto request);

    // ==========================================
    // 2. NHÓM XUẤT BẢN FILE NHỊ PHÂN (BINARY GENERATION)
    // ==========================================

    /// <summary>
    /// Xuất báo cáo định dạng Microsoft Excel (.xlsx) qua ClosedXML
    /// </summary>
    Task<byte[]> GenerateExcelReportAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Xuất báo cáo định dạng Adobe PDF (.pdf) qua QuestPDF
    /// </summary>
    Task<byte[]> GeneratePdfReportAsync(GenerateReportRequestDto request);

    // ==========================================
    // 3. NHÓM QUẢN LÝ LỊCH SỬ VÀ LƯU TRỮ (HISTORY & STORAGE)
    // ==========================================

    /// <summary>
    /// Thực hiện trọn gói: Sinh báo cáo, lưu tệp vào thư mục đích và ghi bản ghi ReportHistory
    /// </summary>
    Task<ReportHistoryDto> GenerateAndSaveReportAsync(GenerateReportRequestDto request, string? targetDirectory = null);

    /// <summary>
    /// Lấy danh sách lịch sử các lượt xuất báo cáo có lọc theo tiêu chí
    /// </summary>
    Task<List<ReportHistoryDto>> GetReportHistoriesAsync(
        ReportType? type = null,
        ReportFormat? format = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    /// <summary>
    /// Lấy chi tiết một bản ghi lịch sử báo cáo theo ID
    /// </summary>
    Task<ReportHistoryDto?> GetReportHistoryByIdAsync(int id);

    /// <summary>
    /// Đọc nội dung file nhị phân của báo cáo đã lưu theo ID lịch sử
    /// </summary>
    Task<byte[]> GetReportFileAsync(int id);

    /// <summary>
    /// Xóa một bản ghi lịch sử báo cáo và tùy chọn xóa tệp đính kèm trên đĩa
    /// </summary>
    Task<bool> DeleteReportHistoryAsync(int id, bool deletePhysicalFile = true);
}
