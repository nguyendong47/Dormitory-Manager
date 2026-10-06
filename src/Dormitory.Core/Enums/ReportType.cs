namespace Dormitory.Core.Enums;

/// <summary>
/// Phân loại các loại báo cáo nghiệp vụ trong hệ thống Quản lý Ký túc xá
/// </summary>
public enum ReportType
{
    /// <summary>
    /// Báo cáo vi phạm nội quy &amp; kỷ luật sinh viên
    /// </summary>
    Violations = 1,

    /// <summary>
    /// Báo cáo tài chính, doanh thu tiền phòng và điện nước
    /// </summary>
    Financial = 2,

    /// <summary>
    /// Báo cáo tỷ lệ lấp đầy, sức chứa và tình trạng phòng ở
    /// </summary>
    Occupancy = 3,

    /// <summary>
    /// Báo cáo kiểm kê tài sản và trang thiết bị phòng ở
    /// </summary>
    Equipment = 4,

    /// <summary>
    /// Bí danh (alias) tương thích cho báo cáo vi phạm
    /// </summary>
    Violation = Violations
}
