namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái đối soát giao dịch thanh toán ngân hàng
/// </summary>
public enum PaymentTransactionStatus
{
    /// <summary>
    /// Đối soát thành công, đã gạch nợ hóa đơn
    /// </summary>
    Success = 1,

    /// <summary>
    /// Đã thanh toán một phần / nộp thiếu tiền
    /// </summary>
    PartiallyPaid = 2,

    /// <summary>
    /// Chưa khớp hóa đơn nào / sai cú pháp chuyển khoản
    /// </summary>
    Unmatched = 3,

    /// <summary>
    /// Giao dịch trùng lặp mã giao dịch ngân hàng
    /// </summary>
    Duplicate = 4,

    /// <summary>
    /// Giao dịch thất bại / lỗi xử lý
    /// </summary>
    Failed = 5
}
