using System;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

/// <summary>
/// Kiểm thử đơn vị cho việc tích hợp IPaymentNotificationService trong BillListViewModel sử dụng NSubstitute
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
    private readonly IPaymentNotificationService _notificationService;

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
        _notificationService = Substitute.For<IPaymentNotificationService>();
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
        // Arrange & Act
        var vm = CreateViewModel(_notificationService);

        // Assert
        _notificationService.Received().OnPaymentReceived += Arg.Any<EventHandler<PaymentReceivedEventArgs>>();
    }

    /// <summary>
    /// Kiểm tra khi gọi Dispose(), ViewModel hủy đăng ký sự kiện OnPaymentReceived để tránh rò rỉ bộ nhớ
    /// </summary>
    [Fact]
    public void Dispose_WithNotificationService_UnsubscribesFromOnPaymentReceived()
    {
        // Arrange
        var vm = CreateViewModel(_notificationService);

        // Act
        vm.Dispose();

        // Assert
        _notificationService.Received().OnPaymentReceived -= Arg.Any<EventHandler<PaymentReceivedEventArgs>>();
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
