namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chuẩn hóa dữ liệu biến động số dư nhận từ Webhook ngân hàng / cổng thanh toán (PayOS, Casso, Open Banking, v.v.)
/// </summary>
public class WebhookPayloadDto
{
    /// <summary>
    /// Tên cổng thanh toán hoặc ngân hàng cung cấp Webhook (PayOS, Casso, Generic, v.v.)
    /// </summary>
    public string Gateway { get; set; } = string.Empty;

    /// <summary>
    /// Mã giao dịch duy nhất từ ngân hàng hoặc cổng thanh toán
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền chuyển khoản (VND)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Nội dung tin nhắn chuyển khoản
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Số tài khoản nhận tiền
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh ngân hàng nhận (BIN code Napas)
    /// </summary>
    public string BankBin { get; set; } = string.Empty;

    /// <summary>
    /// Thời gian xảy ra giao dịch tại ngân hàng
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Chữ ký số HMAC hoặc token xác thực từ webhook để chống giả mạo
    /// </summary>
    public string Signature { get; set; } = string.Empty;

    /// <summary>
    /// Chuỗi dữ liệu JSON nguyên bản nhận được trong body của request
    /// </summary>
    public string? RawData { get; set; }
}
