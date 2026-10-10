using System;
using Dormitory.Application.Interfaces;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ phát sự kiện thanh toán thời gian thực an toàn cho toàn bộ hệ thống
/// </summary>
public class PaymentNotificationService : IPaymentNotificationService
{
    /// <inheritdoc />
    public event EventHandler<PaymentReceivedEventArgs>? OnPaymentReceived;

    /// <inheritdoc />
    public void NotifyPaymentReceived(PaymentReceivedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        OnPaymentReceived?.Invoke(this, eventArgs);
    }
}
