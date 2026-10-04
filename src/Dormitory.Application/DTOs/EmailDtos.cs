namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO cấu hình thông số kết nối máy chủ gửi email SMTP
/// </summary>
public class EmailSettingsDto
{
    /// <summary>
    /// Địa chỉ máy chủ SMTP (ví dụ: smtp.gmail.com hoặc "mock" cho chế độ giả lập)
    /// </summary>
    public string SmtpServer { get; set; } = "smtp.gmail.com";

    /// <summary>
    /// Cổng kết nối SMTP (mặc định 587 cho TLS/STARTTLS, 465 cho SSL)
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Địa chỉ email người gửi
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị người gửi
    /// </summary>
    public string SenderName { get; set; } = "Ban Quản lý Ký túc xá";

    /// <summary>
    /// Tên đăng nhập SMTP (thường là email)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Mật khẩu SMTP hoặc App Password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Có sử dụng mã hóa SSL/TLS không
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Kích hoạt chức năng gửi email tự động (false = chế độ Mock / Safe Mode)
    /// </summary>
    public bool IsEnabled { get; set; } = false;
}

/// <summary>
/// Kết quả thực hiện gửi email
/// </summary>
public class SendEmailResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public static SendEmailResult Ok() => new() { Success = true };
    public static SendEmailResult Fail(string error) => new() { Success = false, ErrorMessage = error };
}
