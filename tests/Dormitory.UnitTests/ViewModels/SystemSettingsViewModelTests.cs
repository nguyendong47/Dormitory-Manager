using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

public class SystemSettingsViewModelTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly Mock<IUserSession> _mockUserSession;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IBankSettingsService> _mockBankSettingsService;
    private readonly Mock<IVietQrService> _mockVietQrService;

    public SystemSettingsViewModelTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockFileService = new Mock<IFileService>();
        _mockDialogService = new Mock<IDialogService>();
        _mockUserSession = new Mock<IUserSession>();
        _mockEmailService = new Mock<IEmailService>();
        _mockBankSettingsService = new Mock<IBankSettingsService>();
        _mockVietQrService = new Mock<IVietQrService>();

        _mockEmailService.Setup(e => e.GetEmailSettingsAsync()).ReturnsAsync(new EmailSettingsDto());
        _mockBankSettingsService.Setup(b => b.GetBankSettingsAsync()).ReturnsAsync(new BankSettingsDto
        {
            BankBin = "970422",
            BankName = "Ngân hàng TMCP Quân đội",
            BankShortName = "MBBank",
            AccountNumber = "123456789",
            AccountHolder = "KTX DORMITORY",
            QrTemplate = "compact",
            TransferPrefix = "KTX"
        });
    }

    private SystemSettingsViewModel CreateViewModel()
    {
        return new SystemSettingsViewModel(
            _mockDatabaseService.Object,
            _mockFileService.Object,
            _mockDialogService.Object,
            _mockUserSession.Object,
            _mockEmailService.Object,
            _mockBankSettingsService.Object,
            _mockVietQrService.Object);
    }

    [Fact]
    public async Task LoadBankSettingsAsync_LoadsSettingsAndMatchesSelectedBank()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        await vm.LoadBankSettingsAsync();

        // Assert
        vm.BankSettings.BankBin.Should().Be("970422");
        vm.SelectedBank.Should().NotBeNull();
        vm.SelectedBank!.Bin.Should().Be("970422");
        vm.SelectedBank.ShortName.Should().Be("MBBank");
        vm.BankStatusMessage.Should().Contain("Đã tải cấu hình");
    }

    [Fact]
    public void SelectedBank_WhenChanged_UpdatesBankSettingsBinAndName()
    {
        // Arrange
        var vm = CreateViewModel();
        var vcb = vm.AvailableBanks.First(b => b.Bin == "970436");

        // Act
        vm.SelectedBank = vcb;

        // Assert
        vm.BankSettings.BankBin.Should().Be("970436");
        vm.BankSettings.BankShortName.Should().Be("Vietcombank");
        vm.BankSettings.BankName.Should().Be("Ngân hàng TMCP Ngoại thương Việt Nam");
    }

    [Fact]
    public async Task SaveBankSettingsCommand_WhenSuccess_ShowsSuccessDialogAndUpdatesMessage()
    {
        // Arrange
        var vm = CreateViewModel();
        _mockBankSettingsService
            .Setup(b => b.SaveBankSettingsAsync(It.IsAny<BankSettingsDto>()))
            .ReturnsAsync(true);

        // Act
        await vm.SaveBankSettingsCommand.ExecuteAsync(null);

        // Assert
        _mockBankSettingsService.Verify(b => b.SaveBankSettingsAsync(vm.BankSettings), Times.Once);
        _mockDialogService.Verify(d => d.ShowMessageAsync("Thành công", It.IsAny<string>()), Times.Once);
        vm.BankStatusMessage.Should().Contain("thành công");
    }

    [Fact]
    public async Task TestGenerateQrCommand_GeneratesPayloadAndQrBytes()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.BankSettings.BankBin = "970436";
        vm.BankSettings.AccountNumber = "0123456789";
        vm.BankSettings.AccountHolder = "BAN QUAN LY KTX";
        vm.BankSettings.TransferPrefix = "KTX";

        var payload = new VietQrPayloadDto
        {
            EmvCoPayload = "000201...",
            Amount = 100000
        };

        _mockVietQrService
            .Setup(v => v.GeneratePayload(
                "970436",
                "0123456789",
                "BAN QUAN LY KTX",
                100000m,
                "KTX TEST",
                "TEST",
                "compact"))
            .Returns(payload);

        _mockVietQrService
            .Setup(v => v.GenerateQrCodePng("000201...", 10))
            .Returns(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        // Act
        await vm.TestGenerateQrCommand.ExecuteAsync(null);

        // Assert
        _mockVietQrService.Verify(v => v.GeneratePayload(
            "970436",
            "0123456789",
            "BAN QUAN LY KTX",
            100000m,
            "KTX TEST",
            "TEST",
            "compact"), Times.Once);

        _mockVietQrService.Verify(v => v.GenerateQrCodePng("000201...", 10), Times.Once);
        vm.BankStatusMessage.Should().Contain("100.000 đ");
    }
}
