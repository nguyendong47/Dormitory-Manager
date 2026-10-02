using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO thông tin phòng ở
/// </summary>
public class RoomDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public int CurrentOccupancy { get; set; }
    public decimal PricePerMonth { get; set; }
    public RoomStatus Status { get; set; }
    public RoomType Type { get; set; }
    public Gender AllowedGender { get; set; }
    public string? Description { get; set; }
    public bool HasVacancy => Status == RoomStatus.Available && CurrentOccupancy < Capacity;
}

/// <summary>
/// DTO yêu cầu tạo hoặc cập nhật phòng
/// </summary>
public class CreateOrUpdateRoomRequest
{
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerMonth { get; set; }
    public RoomType Type { get; set; } = RoomType.Standard;
    public Gender AllowedGender { get; set; } = Gender.Male;
    public string? Description { get; set; }
}
