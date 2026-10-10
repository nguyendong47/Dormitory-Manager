using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể lưu trữ thông tin giao dịch thanh toán ngân hàng nhận được qua Webhook
/// </summary>
public class PaymentTransaction
{
    /// <summary>
    /// Khóa chính định danh bản ghi giao dịch
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Mã giao dịch duy nhất từ phía ngân hàng hoặc cổng thanh toán
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh ngân hàng thụ hưởng (BIN code theo chuẩn Napas)
    /// </summary>
    public string BankBin { get; set; } = string.Empty;

    /// <summary>
    /// Số tài khoản ngân hàng nhận tiền
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền giao dịch (VND)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Nội dung chi tiết chuyển khoản
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Thời gian thực tế giao dịch diễn ra tại ngân hàng
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// ID hóa đơn khớp được trong hệ thống (null nếu chưa đối soát hoặc không tìm thấy)
    /// </summary>
    public int? BillId { get; set; }

    /// <summary>
    /// Mã hóa đơn bóc tách được từ nội dung chuyển khoản
    /// </summary>
    public string BillCode { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái đối soát giao dịch
    /// </summary>
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Unmatched;

    /// <summary>
    /// Dữ liệu JSON gốc nhận từ Webhook phục vụ tra cứu kiểm toán
    /// </summary>
    public string RawPayload { get; set; } = string.Empty;

    /// <summary>
    /// Ghi chú đối soát (lý do lỗi, nộp thừa/thiếu tiền, v.v.)
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm bản ghi được ghi nhận vào hệ thống quản lý KTX
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thông tin hóa đơn liên kết tương ứng
    /// </summary>
    public Bill? Bill { get; set; }
}
