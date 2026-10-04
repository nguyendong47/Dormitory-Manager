using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể đại diện cho phòng ở ký túc xá
/// </summary>
public class Room
{
    /// <summary>
    /// Mã định danh phòng (Khóa chính)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Số phòng (Ví dụ: "P101", "P205")
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tòa nhà (Ví dụ: "Tòa A", "Tòa B", "Tòa C")
    /// </summary>
    public string Building { get; set; } = string.Empty;

    /// <summary>
    /// Tầng của phòng ở
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// Sức chứa tối đa (số giường / số sinh viên)
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Số lượng sinh viên hiện tại đang cư trú trong phòng
    /// </summary>
    public int CurrentOccupancy { get; set; }

    /// <summary>
    /// Đơn giá thuê phòng mỗi tháng (VND)
    /// </summary>
    public decimal PricePerMonth { get; set; }

    /// <summary>
    /// Trạng thái phòng (Còn trống, Đã đầy, Bảo trì)
    /// </summary>
    public RoomStatus Status { get; set; } = RoomStatus.Available;

    /// <summary>
    /// Loại phòng (Tiêu chuẩn, Premium, VIP)
    /// </summary>
    public RoomType Type { get; set; } = RoomType.Standard;

    /// <summary>
    /// Giới tính áp dụng cho phòng (Phòng Nam hoặc Phòng Nữ)
    /// </summary>
    public Gender AllowedGender { get; set; } = Gender.Male;

    /// <summary>
    /// Ghi chú thêm về cơ sở vật chất phòng
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kiểm tra xem phòng còn giường trống hay không
    /// </summary>
    public bool HasVacancy => Status == RoomStatus.Available && CurrentOccupancy < Capacity;

    /// <summary>
    /// Danh sách hợp đồng thuê liên kết với phòng này
    /// </summary>
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    /// <summary>
    /// Danh sách hóa đơn điện, nước, phòng hàng tháng của phòng này
    /// </summary>
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();

    /// <summary>
    /// Danh sách trang thiết bị, tài sản thuộc phòng này
    /// </summary>
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}
