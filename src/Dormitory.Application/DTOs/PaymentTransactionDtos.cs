using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO thông tin chi tiết giao dịch thanh toán ngân hàng
/// </summary>
public class PaymentTransactionDto
{
    /// <summary>
    /// ID định danh bản ghi giao dịch
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Mã giao dịch ngân hàng
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh ngân hàng nhận (BIN code)
    /// </summary>
    public string BankBin { get; set; } = string.Empty;

    /// <summary>
    /// Số tài khoản nhận tiền
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
    /// Thời gian thực hiện giao dịch tại ngân hàng
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// ID hóa đơn khớp được (nếu có)
    /// </summary>
    public int? BillId { get; set; }

    /// <summary>
    /// Mã hóa đơn bóc tách được từ nội dung chuyển khoản
    /// </summary>
    public string BillCode { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái đối soát của giao dịch
    /// </summary>
    public PaymentTransactionStatus Status { get; set; }

    /// <summary>
    /// Dữ liệu JSON gốc từ Webhook
    /// </summary>
    public string RawPayload { get; set; } = string.Empty;

    /// <summary>
    /// Ghi chú đối soát
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm lưu bản ghi vào hệ thống KTX
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO bộ lọc tìm kiếm và phân trang giao dịch thanh toán ngân hàng
/// </summary>
public class PaymentTransactionFilterDto
{
    /// <summary>
    /// Lọc theo trạng thái đối soát
    /// </summary>
    public PaymentTransactionStatus? Status { get; set; }

    /// <summary>
    /// Lọc từ ngày
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Lọc đến ngày
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Từ khóa tìm kiếm (mã giao dịch, nội dung, mã hóa đơn, số tài khoản)
    /// </summary>
    public string? SearchKeyword { get; set; }

    /// <summary>
    /// Lọc theo ID hóa đơn cụ thể
    /// </summary>
    public int? BillId { get; set; }
}

/// <summary>
/// DTO kết quả đối soát tự động một giao dịch thanh toán ngân hàng
/// </summary>
public class PaymentReconciliationResultDto
{
    /// <summary>
    /// Đối soát và xử lý thành công hay không
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Trạng thái phân loại của giao dịch sau khi đối soát
    /// </summary>
    public PaymentTransactionStatus Status { get; set; }

    /// <summary>
    /// Mã giao dịch ngân hàng
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// ID hóa đơn khớp được (nếu có)
    /// </summary>
    public int? BillId { get; set; }

    /// <summary>
    /// Mã hóa đơn bóc tách được (nếu có)
    /// </summary>
    public string BillCode { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền giao dịch (VND)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Thông điệp kết quả xử lý đối soát
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
