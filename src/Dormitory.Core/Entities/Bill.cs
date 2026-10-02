using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể hóa đơn tiền phòng và dịch vụ điện nước hàng tháng
/// </summary>
public class Bill
{
    /// <summary>
    /// Khóa chính hóa đơn
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Mã số hóa đơn (Ví dụ: "HD-2024-10-P101")
    /// </summary>
    public string BillCode { get; set; } = string.Empty;

    /// <summary>
    /// ID của phòng tương ứng
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Thông tin phòng
    /// </summary>
    public Room? Room { get; set; }

    /// <summary>
    /// Tháng xuất hóa đơn
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Năm xuất hóa đơn
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Tiền thuê phòng (VND)
    /// </summary>
    public decimal RoomFee { get; set; }

    /// <summary>
    /// Chỉ số công tơ điện cũ (kWh)
    /// </summary>
    public decimal OldElectricIndex { get; set; }

    /// <summary>
    /// Chỉ số công tơ điện mới (kWh)
    /// </summary>
    public decimal NewElectricIndex { get; set; }

    /// <summary>
    /// Đơn giá điện (VND / kWh)
    /// </summary>
    public decimal ElectricRate { get; set; } = 3500;

    /// <summary>
    /// Số kWh điện tiêu thụ trong tháng
    /// </summary>
    public decimal ElectricUsage => Math.Max(0, NewElectricIndex - OldElectricIndex);

    /// <summary>
    /// Thành tiền điện (VND)
    /// </summary>
    public decimal ElectricFee => ElectricUsage * ElectricRate;

    /// <summary>
    /// Chỉ số đồng hồ nước cũ (m3)
    /// </summary>
    public decimal OldWaterIndex { get; set; }

    /// <summary>
    /// Chỉ số đồng hồ nước mới (m3)
    /// </summary>
    public decimal NewWaterIndex { get; set; }

    /// <summary>
    /// Đơn giá nước (VND / m3)
    /// </summary>
    public decimal WaterRate { get; set; } = 15000;

    /// <summary>
    /// Khối lượng nước tiêu thụ trong tháng (m3)
    /// </summary>
    public decimal WaterUsage => Math.Max(0, NewWaterIndex - OldWaterIndex);

    /// <summary>
    /// Thành tiền nước (VND)
    /// </summary>
    public decimal WaterFee => WaterUsage * WaterRate;

    /// <summary>
    /// Phụ phí dịch vụ khác (vệ sinh, internet, bảo vệ)
    /// </summary>
    public decimal OtherServiceFee { get; set; }

    /// <summary>
    /// Tổng số tiền cần thanh toán
    /// </summary>
    public decimal TotalAmount => RoomFee + ElectricFee + WaterFee + OtherServiceFee;

    /// <summary>
    /// Trạng thái thanh toán của hóa đơn
    /// </summary>
    public BillStatus Status { get; set; } = BillStatus.Unpaid;

    /// <summary>
    /// Ngày đến hạn thanh toán
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Ngày thực tế sinh viên thanh toán hóa đơn
    /// </summary>
    public DateTime? PaidDate { get; set; }

    /// <summary>
    /// Ngày lập hóa đơn
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
