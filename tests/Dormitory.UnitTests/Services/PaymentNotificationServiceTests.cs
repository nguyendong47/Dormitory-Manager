using System;
using System.Reflection;
using Dormitory.Application.Interfaces;
using Dormitory.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị cho IPaymentNotificationService và PaymentNotificationService
/// </summary>
public class PaymentNotificationServiceTests
{
    private readonly PaymentNotificationService _service;

    public PaymentNotificationServiceTests()
    {
        _service = new PaymentNotificationService();
    }

    /// <summary>
    /// Kiểm tra phát sự kiện thành công khi truyền tham số hợp lệ
    /// </summary>
    [Fact]
    public void NotifyPaymentReceived_WithValidArgs_InvokesEvent()
    {
        // Arrange
        bool eventFired = false;
        object? capturedSender = null;
        PaymentReceivedEventArgs? capturedArgs = null;

        _service.OnPaymentReceived += (sender, args) =>
        {
            eventFired = true;
            capturedSender = sender;
            capturedArgs = args;
        };

        var expectedArgs = new PaymentReceivedEventArgs
        {
            BillId = 10,
            BillCode = "HD010",
            Amount = 1500000m,
            TransactionId = "TXN_123456",
            ReceivedAt = DateTime.UtcNow
        };

        // Act
        _service.NotifyPaymentReceived(expectedArgs);

        // Assert
        eventFired.Should().BeTrue();
        capturedSender.Should().BeSameAs(_service);
        capturedArgs.Should().BeSameAs(expectedArgs);
    }

    /// <summary>
    /// Kiểm tra ngoại lệ ArgumentNullException khi truyền tham số null vào NotifyPaymentReceived
    /// </summary>
    [Fact]
    public void NotifyPaymentReceived_WithNullArgs_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _service.NotifyPaymentReceived(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("eventArgs");
    }

    /// <summary>
    /// Kiểm tra khi không có subscriber nào đăng ký sự kiện, phương thức NotifyPaymentReceived không gây lỗi
    /// </summary>
    [Fact]
    public void OnPaymentReceived_NoSubscribers_DoesNotThrow()
    {
        // Arrange
        var args = new PaymentReceivedEventArgs
        {
            BillId = 1,
            BillCode = "HD001",
            Amount = 500000m,
            TransactionId = "TXN_001"
        };

        // Act
        Action act = () => _service.NotifyPaymentReceived(args);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Kiểm tra khi có nhiều subscriber cùng lắng nghe, tất cả đều được thông báo khi có sự kiện
    /// </summary>
    [Fact]
    public void OnPaymentReceived_MultipleSubscribers_AllCalled()
    {
        // Arrange
        int callCount1 = 0;
        int callCount2 = 0;
        int callCount3 = 0;

        _service.OnPaymentReceived += (s, e) => callCount1++;
        _service.OnPaymentReceived += (s, e) => callCount2++;
        _service.OnPaymentReceived += (s, e) => callCount3++;

        var args = new PaymentReceivedEventArgs
        {
            BillId = 2,
            BillCode = "HD002",
            Amount = 800000m,
            TransactionId = "TXN_MULTI_001"
        };

        // Act
        _service.NotifyPaymentReceived(args);

        // Assert
        callCount1.Should().Be(1);
        callCount2.Should().Be(1);
        callCount3.Should().Be(1);
    }

    /// <summary>
    /// Kiểm tra các giá trị thuộc tính trong PaymentReceivedEventArgs được truyền chính xác tới subscriber
    /// </summary>
    [Fact]
    public void NotifyPaymentReceived_EventArgs_HasCorrectValues()
    {
        // Arrange
        PaymentReceivedEventArgs? capturedArgs = null;
        _service.OnPaymentReceived += (s, e) => capturedArgs = e;

        var now = DateTime.UtcNow;
        var args = new PaymentReceivedEventArgs
        {
            BillId = 99,
            BillCode = "HD-2026-99",
            Amount = 2500000.50m,
            TransactionId = "BANK_TXN_888",
            ReceivedAt = now
        };

        // Act
        _service.NotifyPaymentReceived(args);

        // Assert
        capturedArgs.Should().NotBeNull();
        capturedArgs!.BillId.Should().Be(99);
        capturedArgs.BillCode.Should().Be("HD-2026-99");
        capturedArgs.Amount.Should().Be(2500000.50m);
        capturedArgs.TransactionId.Should().Be("BANK_TXN_888");
        capturedArgs.ReceivedAt.Should().Be(now);
    }

    /// <summary>
    /// Kiểm tra sau khi hủy đăng ký (unsubscribe), subscriber sẽ không nhận được sự kiện nữa
    /// </summary>
    [Fact]
    public void NotifyPaymentReceived_AfterUnsubscribe_EventNotCalled()
    {
        // Arrange
        int callCount = 0;
        EventHandler<PaymentReceivedEventArgs> handler = (s, e) => callCount++;

        _service.OnPaymentReceived += handler;
        _service.NotifyPaymentReceived(new PaymentReceivedEventArgs { BillId = 1 });
        callCount.Should().Be(1);

        // Act
        _service.OnPaymentReceived -= handler;
        _service.NotifyPaymentReceived(new PaymentReceivedEventArgs { BillId = 2 });

        // Assert
        callCount.Should().Be(1);
    }

    /// <summary>
    /// Kiểm tra sự kiện OnPaymentReceived ban đầu chưa có subscriber nào (null)
    /// </summary>
    [Fact]
    public void OnPaymentReceived_InitiallyNull()
    {
        // Arrange & Act
        var service = new PaymentNotificationService();

        // Sử dụng Reflection kiểm tra delegate field ban đầu là null
        var eventField = typeof(PaymentNotificationService)
            .GetField("OnPaymentReceived", BindingFlags.Instance | BindingFlags.NonPublic);

        var delegateValue = eventField?.GetValue(service) as Delegate;

        // Assert
        delegateValue.Should().BeNull();
    }

    /// <summary>
    /// Kiểm tra thuộc tính ReceivedAt mặc định có giá trị gần với thời điểm hiện tại UtcNow
    /// </summary>
    [Fact]
    public void NotifyPaymentReceived_ReceivedAt_IsReasonablyNow()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var args = new PaymentReceivedEventArgs();
        var after = DateTime.UtcNow;

        // Assert
        args.ReceivedAt.Should().BeOnOrAfter(before.AddMilliseconds(-50));
        args.ReceivedAt.Should().BeOnOrBefore(after.AddMilliseconds(50));
    }
}
