namespace Dormitory.Application.DTOs;

/// <summary>
/// Cấu hình dịch vụ máy chủ nhúng tiếp nhận Webhook biến động số dư ngân hàng
/// </summary>
public class WebhookSettingsDto
{
    /// <summary>
    /// Trạng thái bật/tắt dịch vụ lắng nghe Webhook trong ứng dụng
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Cổng mạng nội bộ để lắng nghe HTTP Webhook (Mặc định: 5005)
    /// </summary>
    public int Port { get; set; } = 5005;

    /// <summary>
    /// Đường dẫn tương đối endpoint nhận Webhook (Mặc định: "/api/webhook/payment")
    /// </summary>
    public string Path { get; set; } = "/api/webhook/payment";

    /// <summary>
    /// Khóa bí mật (Secret Key / Webhook Token) dùng để xác thực chữ ký HMAC-SHA256 hoặc xác thực token
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Cổng thanh toán hoặc nhà cung cấp Webhook ngân hàng ("PayOS", "Casso", "Generic")
    /// </summary>
    public string Provider { get; set; } = "PayOS";

    /// <summary>
    /// Tiền tố nhận diện nội dung chuyển khoản phục vụ đối soát (Mặc định: "KTX")
    /// </summary>
    public string TransferPrefix { get; set; } = "KTX";
}
