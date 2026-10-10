using System;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

/// <summary>
/// Kiểm thử đơn vị cho việc tích hợp IPaymentNotificationService trong BillListViewModel
/// </summary>
public class BillListViewModelNotificationTests
{
    private readonly IBillService _billService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IExportService _exportService;
    private readonly IFileService _fileService;
    private readonly IPdfExportService _pdfExportService;
    private readonly IEmailService _emailService;
    private readonly IContractService _contractService;
    private readonly IStudentService _studentService;
    private readonly IBankSettingsService _bankSettingsService;
    private readonly IVietQrService _vietQrService;

    /// <summary>
    /// Helper mock triển khai IPaymentNotificationService để kiểm tra việc đăng ký/hủy đăng ký sự kiện OnPaymentReceived
    /// </summary>
    private class TestPaymentNotificationService : IPaymentNotificationService
    {
        public event EventHandler<PaymentReceivedEventArgs>? OnPaymentReceived;
        public bool HasSubscribers => OnPaymentReceived != null;

        public void NotifyPaymentReceived(PaymentReceivedEventArgs eventArgs)
        {
            OnPaymentReceived?.Invoke(this, eventArgs);
        }
    }

    public BillListViewModelNotificationTests()
    {
        _billService = Substitute.For<IBillService>();
        _roomService = Substitute.For<IRoomService>();
        _dialogService = Substitute.For<IDialogService>();
        _exportService = Substitute.For<IExportService>();
        _fileService = Substitute.For<IFileService>();
        _pdfExportService = Substitute.For<IPdfExportService>();
        _emailService = Substitute.For<IEmailService>();
        _contractService = Substitute.For<IContractService>();
        _studentService = Substitute.For<IStudentService>();
        _bankSettingsService = Substitute.For<IBankSettingsService>();
        _vietQrService = Substitute.For<IVietQrService>();
    }

    private BillListViewModel CreateViewModel(IPaymentNotificationService? notificationService = null)
    {
        return new BillListViewModel(
            _billService,
            _roomService,
            _dialogService,
            _exportService,
            _fileService,
            _pdfExportService,
            _emailService,
            _contractService,
            _studentService,
            _bankSettingsService,
            _vietQrService,
            notificationService);
    }

    /// <summary>
    /// Kiểm tra khi ViewModel được khởi tạo với notification service, ViewModel đăng ký lắng nghe sự kiện OnPaymentReceived
    /// </summary>
    [Fact]
    public void Constructor_WithNotificationService_SubscribesToOnPaymentReceived()
    {
        // Arrange
        var testNotificationService = new TestPaymentNotificationService();

        // Act
        var vm = CreateViewModel(testNotificationService);

        // Assert
        testNotificationService.HasSubscribers.Should().BeTrue();
    }

    /// <summary>
    /// Kiểm tra khi gọi Dispose(), ViewModel hủy đăng ký sự kiện OnPaymentReceived để tránh rò rỉ bộ nhớ
    /// </summary>
    [Fact]
    public void Dispose_WithNotificationService_UnsubscribesFromOnPaymentReceived()
    {
        // Arrange
        var testNotificationService = new TestPaymentNotificationService();
        var vm = CreateViewModel(testNotificationService);

        // Act
        vm.Dispose();

        // Assert
        testNotificationService.HasSubscribers.Should().BeFalse();
    }

    /// <summary>
    /// Kiểm tra khi khởi tạo ViewModel với paymentNotificationService = null thì không phát sinh ngoại lệ
    /// </summary>
    [Fact]
    public void Constructor_WithNullNotificationService_DoesNotThrow()
    {
        // Arrange & Act
        var act = () => CreateViewModel(null);

        // Assert
        act.Should().NotThrow();
    }
}
