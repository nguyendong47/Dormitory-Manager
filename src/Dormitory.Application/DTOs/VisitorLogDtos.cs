using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chứa thông tin chi tiết lịch sử khách đến thăm sinh viên KTX
/// </summary>
public class VisitorLogDto
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
    /// Mã định danh sinh viên được đến thăm
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Tên sinh viên được đến thăm
    /// </summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh phòng sinh viên cư trú
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Số phòng sinh viên cư trú
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm khách check-in vào KTX
    /// </summary>
    public DateTime CheckInTime { get; set; }

    /// <summary>
    /// Thời điểm khách check-out rời KTX (null nếu chưa rời)
    /// </summary>
    public DateTime? CheckOutTime { get; set; }

    /// <summary>
    /// Trạng thái lưu trú của khách
    /// </summary>
    public VisitorStatus Status { get; set; }

    /// <summary>
    /// Ghi chú lý do thăm / đồ gửi lại
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Nhãn hiển thị tiếng Việt của trạng thái khách
    /// </summary>
    public string StatusText => Status switch
    {
        VisitorStatus.InDormitory => "Đang trong KTX",
        VisitorStatus.CheckedOut => "Đã rời KTX",
        VisitorStatus.Overstayed => "Quá giờ quy định",
        _ => "Không rõ"
    };

    /// <summary>
    /// Màu chữ badge trạng thái
    /// </summary>
    public string StatusBadgeColor => Status switch
    {
        VisitorStatus.InDormitory => "#107C41",
        VisitorStatus.CheckedOut => "#5D5A58",
        VisitorStatus.Overstayed => "#A80000",
        _ => "#605E5C"
    };

    /// <summary>
    /// Màu nền badge trạng thái
    /// </summary>
    public string StatusBadgeBackground => Status switch
    {
        VisitorStatus.InDormitory => "#DFF6DD",
        VisitorStatus.CheckedOut => "#F3F2F1",
        VisitorStatus.Overstayed => "#FDE7E9",
        _ => "#F3F2F1"
    };
}

/// <summary>
/// DTO yêu cầu ghi nhận khách đăng ký check-in vào KTX
/// </summary>
public class CreateVisitorLogDto
{
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
    /// Mã định danh sinh viên được đến thăm
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Mã định danh phòng sinh viên cư trú
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Ghi chú lý do thăm / đồ vật gửi lại cổng bảo vệ
    /// </summary>
    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// DTO bộ lọc tìm kiếm và truy vấn danh sách khách ra vào
/// </summary>
public class VisitorFilterDto
{
    /// <summary>
    /// Lọc theo trạng thái lưu trú
    /// </summary>
    public VisitorStatus? Status { get; set; }

    /// <summary>
    /// Lọc từ ngày check-in
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Lọc đến ngày check-in
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Lọc theo phòng cụ thể
    /// </summary>
    public int? RoomId { get; set; }

    /// <summary>
    /// Từ khóa tìm kiếm (tên khách, CCCD, số điện thoại, tên sinh viên)
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// Chỉ hiển thị các lượt khách quá giờ quy định / quá giờ giới nghiêm 22:00
    /// </summary>
    public bool OnlyOverstayed { get; set; }
}
