using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể đại diện cho hợp đồng thuê phòng ký túc xá giữa sinh viên và ban quản lý
/// </summary>
public class Contract
{
    /// <summary>
    /// Mã định danh hợp đồng (Khóa chính)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Số hợp đồng (Ví dụ: "HD2024-001")
    /// </summary>
    public string ContractNumber { get; set; } = string.Empty;

    /// <summary>
    /// ID sinh viên ký hợp đồng
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Thông tin sinh viên
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// ID phòng ký túc xá được thuê
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Thông tin phòng được thuê
    /// </summary>
    public Room? Room { get; set; }

    /// <summary>
    /// Ngày bắt đầu hợp đồng có hiệu lực
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc hợp đồng
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Tiền đặt cọc phòng (VND)
    /// </summary>
    public decimal DepositAmount { get; set; }

    /// <summary>
    /// Giá thuê phòng thỏa thuận mỗi tháng (VND)
    /// </summary>
    public decimal MonthlyRate { get; set; }

    /// <summary>
    /// Trạng thái hợp đồng (Có hiệu lực, Hết hạn, Đã chấm dứt)
    /// </summary>
    public ContractStatus Status { get; set; } = ContractStatus.Active;

    /// <summary>
    /// Ghi chú điều khoản hợp đồng hoặc lý do kết thúc
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Ngày tạo hợp đồng
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
