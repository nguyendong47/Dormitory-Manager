namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái của phòng ở ký túc xá
/// </summary>
public enum RoomStatus
{
    /// <summary>
    /// Phòng còn chỗ trống cho sinh viên đăng ký
    /// </summary>
    Available = 1,

    /// <summary>
    /// Phòng đã đủ số lượng người tối đa
    /// </summary>
    Occupied = 2,

    /// <summary>
    /// Phòng đang trong quá trình sửa chữa, bảo trì
    /// </summary>
    Maintenance = 3
}
