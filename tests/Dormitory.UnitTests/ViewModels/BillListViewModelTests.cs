using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using Dormitory.Desktop.Views;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

public class BillListViewModelTests
{
    private readonly Mock<IBillService> _mockBillService;
    private readonly Mock<IRoomService> _mockRoomService;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly Mock<IExportService> _mockExportService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<IPdfExportService> _mockPdfExportService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IContractService> _mockContractService;
    private readonly Mock<IStudentService> _mockStudentService;
    private readonly Mock<IBankSettingsService> _mockBankSettingsService;
    private readonly Mock<IVietQrService> _mockVietQrService;
    private readonly Mock<IPaymentNotificationService> _mockPaymentNotificationService;

    public BillListViewModelTests()
    {
        _mockBillService = new Mock<IBillService>();
        _mockRoomService = new Mock<IRoomService>();
        _mockDialogService = new Mock<IDialogService>();
        _mockExportService = new Mock<IExportService>();
        _mockFileService = new Mock<IFileService>();
        _mockPdfExportService = new Mock<IPdfExportService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockContractService = new Mock<IContractService>();
        _mockStudentService = new Mock<IStudentService>();
        _mockBankSettingsService = new Mock<IBankSettingsService>();
        _mockVietQrService = new Mock<IVietQrService>();
        _mockPaymentNotificationService = new Mock<IPaymentNotificationService>();
    }

    private BillListViewModel CreateViewModel(IPaymentNotificationService? paymentNotificationService = null)
    {
        return new BillListViewModel(
            _mockBillService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockExportService.Object,
            _mockFileService.Object,
            _mockPdfExportService.Object,
            _mockEmailService.Object,
            _mockContractService.Object,
            _mockStudentService.Object,
            _mockBankSettingsService.Object,
            _mockVietQrService.Object,
            paymentNotificationService);
    }

    [Fact]
    public async Task OpenVietQrDialogAsync_WhenNoBillProvidedOrSelected_ShowsWarning()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.SelectedBill = null;

        // Act
        await vm.OpenVietQrDialogCommand.ExecuteAsync(null);

        // Assert
        _mockDialogService.Verify(d => d.ShowMessageAsync("Thông báo", It.Is<string>(s => s.Contains("chọn một hóa đơn"))), Times.Once);
        _mockDialogService.Verify(d => d.ShowDialogAsync<bool>(It.IsAny<Window>()), Times.Never);
    }

    [Fact]
    public async Task OpenVietQrDialogAsync_WhenTargetBillProvided_FindsStudentAndShowsDialog()
    {
        // Arrange
        var bill = new BillDto
        {
            Id = 10,
            BillCode = "HD010",
            RoomId = 1,
            RoomNumber = "101",
            RoomFee = 450000,
            Status = BillStatus.Unpaid
        };

        var student = new StudentDto
        {
            Id = 5,
            FullName = "Le Van C"
        };

        var contract = new ContractDto
        {
            Id = 1,
            RoomId = 1,
            StudentId = 5,
            Status = ContractStatus.Active
        };

        var bankSettings = new BankSettingsDto();
        var payload = new VietQrPayloadDto { Amount = 450000 };

        _mockContractService
            .Setup(c => c.GetAllContractsAsync(ContractStatus.Active, null, 1))
            .ReturnsAsync(new List<ContractDto> { contract });

        _mockStudentService
            .Setup(s => s.GetStudentByIdAsync(5))
            .ReturnsAsync(student);

        _mockBankSettingsService
            .Setup(b => b.GetBankSettingsAsync())
            .ReturnsAsync(bankSettings);

        _mockVietQrService
            .Setup(v => v.GeneratePayloadForBill(bill, "Le Van C", bankSettings))
            .Returns(payload);

        _mockDialogService
            .Setup(d => d.ShowDialogAsync<bool>(It.IsAny<Window>()))
            .ReturnsAsync(true);

        _mockBillService
            .Setup(b => b.GetAllBillsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<BillStatus?>()))
            .ReturnsAsync(new List<BillDto> { bill });

        var vm = CreateViewModel();

        bool handlerCalled = false;
        vm.ShowVietQrDialogHandler = dialogVm =>
        {
            handlerCalled = true;
            dialogVm.Bill.Id.Should().Be(10);
            dialogVm.StudentName.Should().Be("Le Van C");
            return Task.FromResult(true);
        };

        // Act
        await vm.OpenVietQrDialogCommand.ExecuteAsync(bill);

        // Assert
        handlerCalled.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithPaymentNotificationService_SubscribesToOnPaymentReceived()
    {
        // Arrange & Act
        var vm = CreateViewModel(_mockPaymentNotificationService.Object);

        // Assert
        _mockPaymentNotificationService.VerifyAdd(m => m.OnPaymentReceived += It.IsAny<EventHandler<PaymentReceivedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_WithPaymentNotificationService_UnsubscribesFromOnPaymentReceived()
    {
        // Arrange
        var vm = CreateViewModel(_mockPaymentNotificationService.Object);

        // Act
        vm.Dispose();

        // Assert
        _mockPaymentNotificationService.VerifyRemove(m => m.OnPaymentReceived -= It.IsAny<EventHandler<PaymentReceivedEventArgs>>(), Times.Once);
    }
}
