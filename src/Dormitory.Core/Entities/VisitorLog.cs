using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể ghi nhận lịch sử khách ra vào thăm sinh viên tại ký túc xá
/// </summary>
public class VisitorLog
{
    /// <summary>
    /// Mã định danh bản ghi (Khóa chính)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Họ và tên khách đến thăm
    /// </summary>
    public string VisitorName { get; set; } = string.Empty;

    /// <summary>
    /// Số CCCD / CMND / Hộ chiếu của khách
    /// </summary>
    public string IdentificationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Số điện thoại liên hệ của khách
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh sinh viên được đến thăm (Khóa ngoại)
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Mã định danh phòng sinh viên cư trú (Khóa ngoại)
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Thời điểm khách check-in vào KTX
    /// </summary>
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm khách check-out rời KTX (null nếu chưa rời)
    /// </summary>
    public DateTime? CheckOutTime { get; set; }

    /// <summary>
    /// Trạng thái lưu trú của khách (mặc định InDormitory)
    /// </summary>
    public VisitorStatus Status { get; set; } = VisitorStatus.InDormitory;

    /// <summary>
    /// Ghi chú lý do thăm / đồ vật gửi lại tại cổng bảo vệ
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Thông tin sinh viên được đến thăm
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Thông tin phòng sinh viên cư trú
    /// </summary>
    public Room? Room { get; set; }
}
