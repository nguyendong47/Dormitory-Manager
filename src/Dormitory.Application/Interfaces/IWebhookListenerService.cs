using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện máy chủ nhúng tiếp nhận và xử lý Webhook biến động số dư ngân hàng
/// </summary>
public interface IWebhookListenerService
{
    /// <summary>
    /// Cho biết dịch vụ máy chủ Webhook có đang hoạt động lắng nghe hay không
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Cổng mạng thực tế đang lắng nghe (0 nếu dịch vụ đang dừng)
    /// </summary>
    int ActivePort { get; }

    /// <summary>
    /// Khởi động máy chủ lắng nghe Webhook dựa trên cấu hình hiện tại
    /// </summary>
    /// <returns>True nếu khởi động thành công, False nếu cấu hình tắt hoặc gặp lỗi</returns>
    Task<bool> StartAsync();

    /// <summary>
    /// Dừng máy chủ lắng nghe Webhook và giải phóng cổng mạng
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Lấy thông tin cấu hình Webhook hiện tại
    /// </summary>
    Task<WebhookSettingsDto> GetSettingsAsync();

    /// <summary>
    /// Lưu thông tin cấu hình Webhook mới và áp dụng
    /// </summary>
    /// <param name="settings">Dữ liệu cấu hình Webhook mới</param>
    /// <returns>True nếu lưu thành công</returns>
    Task<bool> SaveSettingsAsync(WebhookSettingsDto settings);

    /// <summary>
    /// Kiểm thử xử lý dữ liệu Webhook giả lập mà không cần mở kết nối mạng từ ngoài
    /// </summary>
    /// <param name="testPayload">Dữ liệu Webhook cần kiểm thử</param>
    /// <returns>Kết quả đối soát giao dịch</returns>
    Task<PaymentReconciliationResultDto> TestWebhookAsync(WebhookPayloadDto testPayload);

    /// <summary>
    /// Xác thực tính toàn vẹn và chống giả mạo của Webhook qua chữ ký HMAC-SHA256 hoặc Secret Token
    /// </summary>
    /// <param name="payload">Nội dung thô JSON nhận từ webhook</param>
    /// <param name="signature">Chữ ký số nhận từ header hoặc body</param>
    /// <param name="secretKey">Khóa bí mật cấu hình trong hệ thống</param>
    /// <returns>True nếu chữ ký hợp lệ hoặc không cấu hình secret, False nếu sai chữ ký</returns>
    bool ValidateSignature(string payload, string signature, string secretKey);

    /// <summary>
    /// Phân tích và chuẩn hóa chuỗi JSON Webhook từ các nhà cung cấp khác nhau (PayOS, Casso, Generic)
    /// </summary>
    /// <param name="rawJson">Chuỗi JSON gốc nhận từ Webhook</param>
    /// <param name="provider">Tên nhà cung cấp ("PayOS", "Casso", "Generic")</param>
    /// <returns>Đối tượng WebhookPayloadDto đã chuẩn hóa, hoặc null nếu JSON không hợp lệ</returns>
    WebhookPayloadDto? ParseIncomingPayload(string rawJson, string provider);
}
