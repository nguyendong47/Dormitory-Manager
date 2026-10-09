using System;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

public class VietQrDialogViewModelTests
{
    private readonly Mock<IBankSettingsService> _mockBankSettingsService;
    private readonly Mock<IVietQrService> _mockVietQrService;
    private readonly Mock<IBillService> _mockBillService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<IDialogService> _mockDialogService;

    public VietQrDialogViewModelTests()
    {
        _mockBankSettingsService = new Mock<IBankSettingsService>();
        _mockVietQrService = new Mock<IVietQrService>();
        _mockBillService = new Mock<IBillService>();
        _mockFileService = new Mock<IFileService>();
        _mockDialogService = new Mock<IDialogService>();
    }

    private VietQrDialogViewModel CreateViewModel(BillDto bill, string studentName = "Nguyen Van A")
    {
        return new VietQrDialogViewModel(
            bill,
            studentName,
            _mockBankSettingsService.Object,
            _mockVietQrService.Object,
            _mockBillService.Object,
            _mockFileService.Object,
            _mockDialogService.Object);
    }

    [Fact]
    public async Task InitializeAsync_WhenBillIsUnpaid_GeneratesPayloadAndReadyStatus()
    {
        // Arrange
        var bill = new BillDto
        {
            Id = 1,
            BillCode = "HD001",
            RoomNumber = "101",
            RoomFee = 500000,
            Status = BillStatus.Unpaid
        };

        var bankSettings = new BankSettingsDto
        {
            BankBin = "970436",
            BankName = "Vietcombank",
            AccountNumber = "0123456789",
            AccountHolder = "BAN QUAN LY KTX",
            TransferPrefix = "KTX"
        };

        var payload = new VietQrPayloadDto
        {
            BankBin = "970436",
            AccountNumber = "0123456789",
            AccountHolder = "BAN QUAN LY KTX",
            Amount = 500000,
            TransferContent = "KTX HD001 NGUYEN VAN A",
            EmvCoPayload = "00020101021238540010A00000072701240006970436011001234567890208QRIBFTTA530370454065000005802VN62260822KTX HD001 NGUYEN VAN A63041234"
        };

        _mockBankSettingsService
            .Setup(s => s.GetBankSettingsAsync())
            .ReturnsAsync(bankSettings);

        _mockVietQrService
            .Setup(v => v.GeneratePayloadForBill(bill, "Nguyen Van A", bankSettings))
            .Returns(payload);

        _mockVietQrService
            .Setup(v => v.GenerateQrCodePng(payload.EmvCoPayload, 10))
            .Returns(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        var vm = CreateViewModel(bill, "Nguyen Van A");

        // Act
        await vm.InitializeAsync();

        // Assert
        vm.Payload.Should().NotBeNull();
        vm.Payload!.Amount.Should().Be(500000);
        vm.Payload.TransferContent.Should().Be("KTX HD001 NGUYEN Van A".ToUpperInvariant());
        vm.IsPaid.Should().BeFalse();
        vm.CanConfirmPayment.Should().BeTrue();
        vm.StatusMessage.Should().Contain("sẵn sàng");
    }

    [Fact]
    public async Task InitializeAsync_WhenBillIsPaid_SetsIsPaidTrueAndDisablesConfirmPayment()
    {
        // Arrange
        var bill = new BillDto
        {
            Id = 2,
            BillCode = "HD002",
            RoomNumber = "102",
            RoomFee = 750000,
            Status = BillStatus.Paid
        };

        var bankSettings = new BankSettingsDto();
        var payload = new VietQrPayloadDto { Amount = 750000 };

        _mockBankSettingsService.Setup(s => s.GetBankSettingsAsync()).ReturnsAsync(bankSettings);
        _mockVietQrService.Setup(v => v.GeneratePayloadForBill(bill, "Tran Thi B", bankSettings)).Returns(payload);

        var vm = CreateViewModel(bill, "Tran Thi B");

        // Act
        await vm.InitializeAsync();

        // Assert
        vm.IsPaid.Should().BeTrue();
        vm.CanConfirmPayment.Should().BeFalse();
        vm.StatusMessage.Should().Contain("đã được thanh toán");
    }

    [Fact]
    public async Task CopyAccountNumberCommand_InvokesClipboardCallback()
    {
        // Arrange
        var bill = new BillDto { Id = 1, BillCode = "HD001", Status = BillStatus.Unpaid };
        var vm = CreateViewModel(bill);
        vm.Payload = new VietQrPayloadDto { AccountNumber = "9876543210" };

        string copiedText = string.Empty;
        vm.CopyToClipboardAction = text =>
        {
            copiedText = text;
            return Task.CompletedTask;
        };

        // Act
        await vm.CopyAccountNumberCommand.ExecuteAsync(null);

        // Assert
        copiedText.Should().Be("9876543210");
        vm.StatusMessage.Should().Contain("sao chép số tài khoản");
    }

    [Fact]
    public async Task CopyTransferContentCommand_InvokesClipboardCallback()
    {
        // Arrange
        var bill = new BillDto { Id = 1, BillCode = "HD001", Status = BillStatus.Unpaid };
        var vm = CreateViewModel(bill);
        vm.Payload = new VietQrPayloadDto { TransferContent = "KTX HD001 NGUYEN VAN A" };

        string copiedText = string.Empty;
        vm.CopyToClipboardAction = text =>
        {
            copiedText = text;
            return Task.CompletedTask;
        };

        // Act
        await vm.CopyTransferContentCommand.ExecuteAsync(null);

        // Assert
        copiedText.Should().Be("KTX HD001 NGUYEN VAN A");
        vm.StatusMessage.Should().Contain("sao chép nội dung chuyển khoản");
    }

    [Fact]
    public async Task SaveQrImageCommand_CallsFileServiceWithPng()
    {
        // Arrange
        var bill = new BillDto { Id = 1, BillCode = "HD001", Status = BillStatus.Unpaid };
        var vm = CreateViewModel(bill);
        vm.Payload = new VietQrPayloadDto { EmvCoPayload = "000201..." };

        byte[] fakeQrBytes = new byte[] { 1, 2, 3, 4 };
        _mockVietQrService
            .Setup(v => v.GenerateQrCodePng("000201...", 10))
            .Returns(fakeQrBytes);

        _mockFileService
            .Setup(f => f.SaveFileAsync("VietQR_HD001", "png", It.IsAny<string>(), fakeQrBytes))
            .ReturnsAsync(true);

        // Act
        await vm.SaveQrImageCommand.ExecuteAsync(null);

        // Assert
        _mockFileService.Verify(f => f.SaveFileAsync("VietQR_HD001", "png", It.IsAny<string>(), fakeQrBytes), Times.Once);
        vm.StatusMessage.ToLowerInvariant().Should().Contain("lưu ảnh mã vietqr thành công");
    }

    [Fact]
    public async Task ConfirmPaymentCommand_WhenConfirmed_MarksAsPaidAndClosesWithTrue()
    {
        // Arrange
        var bill = new BillDto { Id = 5, BillCode = "HD005", RoomFee = 300000, Status = BillStatus.Unpaid };
        var vm = CreateViewModel(bill);

        bool? closeResult = null;
        vm.CloseAction = res => closeResult = res;

        _mockDialogService
            .Setup(d => d.ShowConfirmAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockBillService
            .Setup(b => b.MarkAsPaidAsync(5))
            .ReturnsAsync(true);

        // Act
        await vm.ConfirmPaymentCommand.ExecuteAsync(null);

        // Assert
        _mockBillService.Verify(b => b.MarkAsPaidAsync(5), Times.Once);
        vm.IsPaid.Should().BeTrue();
        closeResult.Should().BeTrue();
        vm.StatusMessage.Should().Contain("thành công");
    }

    [Fact]
    public async Task ConfirmPaymentCommand_WhenUserCancels_DoesNotMarkAsPaid()
    {
        // Arrange
        var bill = new BillDto { Id = 5, BillCode = "HD005", RoomFee = 300000, Status = BillStatus.Unpaid };
        var vm = CreateViewModel(bill);

        bool? closeResult = null;
        vm.CloseAction = res => closeResult = res;

        _mockDialogService
            .Setup(d => d.ShowConfirmAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        await vm.ConfirmPaymentCommand.ExecuteAsync(null);

        // Assert
        _mockBillService.Verify(b => b.MarkAsPaidAsync(It.IsAny<int>()), Times.Never);
        vm.IsPaid.Should().BeFalse();
        closeResult.Should().BeNull();
    }

    [Fact]
    public void CloseCommand_InvokesCloseActionWithIsPaidState()
    {
        // Arrange
        var bill = new BillDto { Id = 1, BillCode = "HD001", Status = BillStatus.Unpaid };
        var vm = CreateViewModel(bill);

        bool? closeResult = null;
        vm.CloseAction = res => closeResult = res;

        // Act
        vm.CloseCommand.Execute(null);

        // Assert
        closeResult.Should().BeFalse();
    }
}
