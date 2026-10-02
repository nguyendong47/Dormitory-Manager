using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể đại diện cho sinh viên nội trú trong ký túc xá
/// </summary>
public class Student
{
    /// <summary>
    /// Khóa chính sinh viên
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Mã số sinh viên (Ví dụ: "SV2021001")
    /// </summary>
    public string StudentCode { get; set; } = string.Empty;

    /// <summary>
    /// Họ và tên đầy đủ của sinh viên
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Giới tính
    /// </summary>
    public Gender Gender { get; set; } = Gender.Male;

    /// <summary>
    /// Số CMND hoặc CCCD
    /// </summary>
    public string IdentityCard { get; set; } = string.Empty;

    /// <summary>
    /// Số điện thoại liên hệ của sinh viên
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Email sinh viên (nếu có)
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Quê quán / Tỉnh thành
    /// </summary>
    public string HomeTown { get; set; } = string.Empty;

    /// <summary>
    /// Lớp học của sinh viên tại trường
    /// </summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// Khoa / Viện đào tạo
    /// </summary>
    public string Faculty { get; set; } = string.Empty;

    /// <summary>
    /// Họ tên phụ huynh hoặc người bảo hộ
    /// </summary>
    public string ParentName { get; set; } = string.Empty;

    /// <summary>
    /// Số điện thoại liên hệ khẩn cấp của phụ huynh
    /// </summary>
    public string ParentPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Địa chỉ thường trú chi tiết
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// ID của phòng hiện tại đang ở (nếu đang có hợp đồng hiệu lực)
    /// </summary>
    public int? CurrentRoomId { get; set; }

    /// <summary>
    /// Phòng hiện tại đang ở
    /// </summary>
    public Room? CurrentRoom { get; set; }

    /// <summary>
    /// Lịch sử các hợp đồng thuê phòng của sinh viên
    /// </summary>
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
