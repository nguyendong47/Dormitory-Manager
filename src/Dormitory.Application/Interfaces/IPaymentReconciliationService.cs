using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ bóc tách cú pháp chuyển khoản và tự động đối soát hóa đơn qua biến động số dư ngân hàng
/// </summary>
public interface IPaymentReconciliationService
{
    /// <summary>
    /// Xử lý biến động số dư ngân hàng từ Webhook: bóc tách nội dung, đối soát số tiền và tự động gạch nợ hóa đơn
    /// </summary>
    /// <param name="payload">Dữ liệu chuẩn hóa từ webhook ngân hàng / cổng thanh toán</param>
    /// <returns>Kết quả đối soát giao dịch</returns>
    Task<PaymentReconciliationResultDto> ProcessTransactionAsync(WebhookPayloadDto payload);

    /// <summary>
    /// Trích xuất mã hóa đơn từ nội dung tin nhắn chuyển khoản ngân hàng bằng Regex thông minh
    /// </summary>
    /// <param name="description">Nội dung chi tiết chuyển khoản ngân hàng</param>
    /// <param name="prefix">Tiền tố nhận diện chuyển khoản ký túc xá (mặc định: "KTX")</param>
    /// <returns>Mã hóa đơn nếu tìm thấy, null nếu không trích xuất được</returns>
    string? ExtractBillCode(string description, string prefix = "KTX");

    /// <summary>
    /// Kế toán / thủ quỹ gán thủ công một giao dịch chưa khớp (Unmatched) vào hóa đơn cụ thể
    /// </summary>
    /// <param name="transactionId">ID bản ghi PaymentTransaction</param>
    /// <param name="billId">ID hóa đơn cần gạch nợ</param>
    /// <param name="note">Ghi chú đối soát thủ công</param>
    /// <returns>True nếu gán thành công, False nếu thất bại</returns>
    Task<bool> ManuallyAssignBillAsync(int transactionId, int billId, string note = "");

    /// <summary>
    /// Lấy danh sách lịch sử các giao dịch thanh toán ngân hàng theo bộ lọc
    /// </summary>
    /// <param name="filter">Bộ lọc điều kiện tra cứu giao dịch</param>
    /// <returns>Danh sách DTO các giao dịch phù hợp</returns>
    Task<List<PaymentTransactionDto>> GetTransactionsAsync(PaymentTransactionFilterDto filter);
}
