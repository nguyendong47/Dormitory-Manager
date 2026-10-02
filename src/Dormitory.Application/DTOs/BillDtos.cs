using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO thông tin chi tiết hóa đơn
/// </summary>
public class BillDto
{
    public int Id { get; set; }
    public string BillCode { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal RoomFee { get; set; }
    public decimal OldElectricIndex { get; set; }
    public decimal NewElectricIndex { get; set; }
    public decimal ElectricRate { get; set; }
    public decimal ElectricUsage => Math.Max(0, NewElectricIndex - OldElectricIndex);
    public decimal ElectricFee => ElectricUsage * ElectricRate;
    public decimal OldWaterIndex { get; set; }
    public decimal NewWaterIndex { get; set; }
    public decimal WaterRate { get; set; }
    public decimal WaterUsage => Math.Max(0, NewWaterIndex - OldWaterIndex);
    public decimal WaterFee => WaterUsage * WaterRate;
    public decimal OtherServiceFee { get; set; }
    public decimal TotalAmount => RoomFee + ElectricFee + WaterFee + OtherServiceFee;
    public BillStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO yêu cầu lập hóa đơn điện nước mới cho phòng
/// </summary>
public class CreateBillRequest
{
    public int RoomId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal RoomFee { get; set; }
    public decimal OldElectricIndex { get; set; }
    public decimal NewElectricIndex { get; set; }
    public decimal ElectricRate { get; set; } = 3500;
    public decimal OldWaterIndex { get; set; }
    public decimal NewWaterIndex { get; set; }
    public decimal WaterRate { get; set; } = 15000;
    public decimal OtherServiceFee { get; set; } = 50000;
    public DateTime DueDate { get; set; }
}
