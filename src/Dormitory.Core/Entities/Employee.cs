using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể thông tin nhân viên làm việc tại ký túc xá
/// </summary>
public class Employee
{
    /// <summary>
    /// Khóa chính nhân viên
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Mã số nhân viên (Ví dụ: "NV001")
    /// </summary>
    public string EmployeeCode { get; set; } = string.Empty;

    /// <summary>
    /// Họ và tên đầy đủ
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
    /// Số điện thoại liên hệ
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Số CMND hoặc CCCD
    /// </summary>
    public string IdentityCard { get; set; } = string.Empty;

    /// <summary>
    /// Vị trí công tác / Chức vụ (Quản lý tòa, Kế toán, Bảo vệ, Kỹ thuật)
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Bộ phận làm việc
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Địa chỉ thường trú
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// ID tài khoản đăng nhập liên kết (nếu có)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Tài khoản đăng nhập tương ứng
    /// </summary>
    public User? User { get; set; }
}
