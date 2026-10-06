namespace Dormitory.Core.Enums;

/// <summary>
/// Định dạng tệp xuất bản báo cáo (PDF hoặc Excel)
/// </summary>
public enum ReportFormat
{
    /// <summary>
    /// Định dạng tệp tài liệu Adobe PDF (.pdf) phục vụ in ấn hành chính
    /// </summary>
    Pdf = 1,

    /// <summary>
    /// Định dạng bảng tính Microsoft Excel (.xlsx) phục vụ thống kê chi tiết
    /// </summary>
    Excel = 2
}
