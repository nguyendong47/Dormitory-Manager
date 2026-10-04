namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái hoạt động của trang thiết bị, tài sản phòng ký túc xá
/// </summary>
public enum EquipmentStatus
{
    /// <summary>
    /// Hoạt động tốt
    /// </summary>
    Good = 1,

    /// <summary>
    /// Cần bảo trì / sửa chữa
    /// </summary>
    NeedsRepair = 2,

    /// <summary>
    /// Hỏng hóc / Không sử dụng được
    /// </summary>
    Broken = 3
}
