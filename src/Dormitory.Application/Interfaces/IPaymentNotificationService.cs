using System;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Dữ liệu sự kiện khi nhận được thanh toán chuyển khoản ngân hàng thành công
/// </summary>
public class PaymentReceivedEventArgs : EventArgs
{
    /// <summary>
    /// ID hóa đơn được gạch nợ thành công
    /// </summary>
    public int BillId { get; set; }

    /// <summary>
    /// Mã số hóa đơn được gạch nợ
    /// </summary>
    public string BillCode { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền thực nhận từ giao dịch chuyển khoản (VND)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Mã giao dịch ngân hàng
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm nhận được thông báo thanh toán
    /// </summary>
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Giao diện dịch vụ phát sự kiện thông báo thanh toán ngân hàng thời gian thực sang UI thread
/// </summary>
public interface IPaymentNotificationService
{
    /// <summary>
    /// Sự kiện kích hoạt khi có giao dịch thanh toán ngân hàng được đối soát và gạch nợ thành công
    /// </summary>
    event EventHandler<PaymentReceivedEventArgs>? OnPaymentReceived;

    /// <summary>
    /// Phát sự kiện thông báo thanh toán thành công tới toàn bộ các subscribers
    /// </summary>
    /// <param name="eventArgs">Thông tin chi tiết giao dịch thanh toán</param>
    void NotifyPaymentReceived(PaymentReceivedEventArgs eventArgs);
}
