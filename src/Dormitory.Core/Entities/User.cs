using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể tài khoản đăng nhập vào hệ thống
/// </summary>
public class User
{
    /// <summary>
    /// Khóa chính tài khoản
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tên đăng nhập (Username)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Mật khẩu đã được băm an toàn bằng BCrypt (không lưu plaintext)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Họ và tên chủ tài khoản
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email liên hệ
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Vai trò người dùng (Admin, Manager, Staff)
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Staff;

    /// <summary>
    /// Trạng thái kích hoạt tài khoản
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Thời điểm khởi tạo tài khoản
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm đăng nhập gần nhất
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
