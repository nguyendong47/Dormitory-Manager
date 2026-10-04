using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chứa thông tin chi tiết trang thiết bị / tài sản phòng ở
/// </summary>
public class EquipmentDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMaintainedAt { get; set; }
}

/// <summary>
/// DTO yêu cầu thêm mới trang thiết bị cho phòng
/// </summary>
public class CreateEquipmentDto
{
    public int RoomId { get; set; }
    public string EquipmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Good;
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO yêu cầu cập nhật thông tin trang thiết bị phòng
/// </summary>
public class UpdateEquipmentDto
{
    public int RoomId { get; set; }
    public string EquipmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Good;
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}
