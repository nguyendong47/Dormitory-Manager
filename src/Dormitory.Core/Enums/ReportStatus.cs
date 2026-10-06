namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái tiến trình tạo và lưu trữ báo cáo
/// </summary>
public enum ReportStatus
{
    /// <summary>
    /// Đang trong quá trình tổng hợp dữ liệu và xuất tệp
    /// </summary>
    Generating = 1,

    /// <summary>
    /// Đã hoàn thành xuất báo cáo thành công
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Xảy ra lỗi trong quá trình tổng hợp hoặc lưu tệp
    /// </summary>
    Failed = 3
}
