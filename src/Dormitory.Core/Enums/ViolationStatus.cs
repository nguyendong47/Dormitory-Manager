namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái giải quyết biên bản vi phạm
/// </summary>
public enum ViolationStatus
{
    /// <summary>
    /// Chờ xử lý / Đang lập biên bản
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Đã xử lý / Đã nộp phạt và khắc phục
    /// </summary>
    Resolved = 2,

    /// <summary>
    /// Hủy bỏ / Miễn trừ kỷ luật
    /// </summary>
    Dismissed = 3
}
