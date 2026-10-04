using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể đại diện cho trang thiết bị, tài sản gắn liền với phòng ở
/// </summary>
public class Equipment
{
    /// <summary>
    /// Mã định danh thiết bị (Khóa chính)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Mã phòng ở chứa thiết bị (Khóa ngoại)
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Đối tượng phòng ở chứa thiết bị (Navigation property)
    /// </summary>
    public Room? Room { get; set; }

    /// <summary>
    /// Mã tài sản / thiết bị (Ví dụ: "TB001", "DH-101")
    /// </summary>
    public string EquipmentCode { get; set; } = string.Empty;

    /// <summary>
    /// Tên thiết bị (Ví dụ: "Điều hòa Daikin 12000BTU", "Bình nóng lạnh Ariston")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái hoạt động của thiết bị (Tốt, Cần sửa, Hỏng)
    /// </summary>
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Good;

    /// <summary>
    /// Số lượng thiết bị
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Đơn giá thiết bị (VND)
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Ghi chú thêm về thiết bị (nhãn hiệu, hiện trạng chi tiết, nhà cung cấp)
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Thời điểm bàn giao / trang bị vào phòng
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm bảo trì / kiểm tra gần nhất
    /// </summary>
    public DateTime? LastMaintainedAt { get; set; }
}
