using System;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

/// <summary>
/// Kiểm thử đơn vị các chức năng quản lý Webhook trong SystemSettingsViewModel sử dụng NSubstitute
/// </summary>
public class SystemSettingsWebhookTests
{
    private readonly IDatabaseService _databaseService;
    private readonly IFileService _fileService;
    private readonly IDialogService _dialogService;
    private readonly IUserSession _userSession;
    private readonly IEmailService _emailService;
    private readonly IBankSettingsService _bankSettingsService;
    private readonly IVietQrService _vietQrService;
    private readonly IWebhookListenerService _webhookListenerService;

    public SystemSettingsWebhookTests()
    {
        _databaseService = Substitute.For<IDatabaseService>();
        _fileService = Substitute.For<IFileService>();
        _dialogService = Substitute.For<IDialogService>();
        _userSession = Substitute.For<IUserSession>();
        _emailService = Substitute.For<IEmailService>();
        _bankSettingsService = Substitute.For<IBankSettingsService>();
        _vietQrService = Substitute.For<IVietQrService>();
        _webhookListenerService = Substitute.For<IWebhookListenerService>();

        _userSession.IsAdmin.Returns(true);
        _bankSettingsService.GetBankSettingsAsync().Returns(new BankSettingsDto());
        _emailService.GetEmailSettingsAsync().Returns(new EmailSettingsDto());
    }

    private SystemSettingsViewModel CreateViewModel(bool withWebhookService = true)
    {
        return new SystemSettingsViewModel(
            _databaseService,
            _fileService,
            _dialogService,
            _userSession,
            _emailService,
            _bankSettingsService,
            _vietQrService,
            withWebhookService ? _webhookListenerService : null);
    }

    [Fact]
    public async Task Should_LoadWebhookSettingsAsync_When_ServiceProvided()
    {
        // Arrange
        var settings = new WebhookSettingsDto
        {
            IsEnabled = true,
            Port = 5005,
            Provider = "PayOS",
            SecretKey = "secret123",
            TransferPrefix = "KTX"
        };

        _webhookListenerService.GetSettingsAsync().Returns(settings);
        _webhookListenerService.IsRunning.Returns(true);
        _webhookListenerService.ActivePort.Returns(5005);

        var vm = CreateViewModel();

        // Act
        await vm.LoadWebhookSettingsAsync();

        // Assert
        vm.WebhookSettings.IsEnabled.Should().BeTrue();
        vm.WebhookSettings.Port.Should().Be(5005);
        vm.WebhookSettings.Provider.Should().Be("PayOS");
        vm.IsWebhookRunning.Should().BeTrue();
        vm.WebhookPort.Should().Be(5005);
        vm.WebhookStatusMessage.Should().Contain("5005");
    }

    [Fact]
    public async Task Should_SaveWebhookSettingsAsync_And_StartService_When_Enabled()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.WebhookSettings = new WebhookSettingsDto
        {
            IsEnabled = true,
            Port = 5005,
            Provider = "PayOS"
        };

        _webhookListenerService.SaveSettingsAsync(Arg.Any<WebhookSettingsDto>()).Returns(true);
        _webhookListenerService.StartAsync().Returns(true);
        _webhookListenerService.IsRunning.Returns(true);
        _webhookListenerService.ActivePort.Returns(5005);

        // Act
        await vm.SaveWebhookSettingsAsync();

        // Assert
        await _webhookListenerService.Received(1).SaveSettingsAsync(Arg.Is<WebhookSettingsDto>(s => s.IsEnabled && s.Port == 5005));
        await _webhookListenerService.Received(1).StartAsync();
        await _dialogService.Received(1).ShowMessageAsync("Thành công", Arg.Any<string>());
        vm.IsWebhookRunning.Should().BeTrue();
    }

    [Fact]
    public async Task Should_SaveWebhookSettingsAsync_And_StopService_When_Disabled()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.WebhookSettings = new WebhookSettingsDto
        {
            IsEnabled = false,
            Port = 5005,
            Provider = "Casso"
        };

        _webhookListenerService.SaveSettingsAsync(Arg.Any<WebhookSettingsDto>()).Returns(true);
        _webhookListenerService.IsRunning.Returns(false);
        _webhookListenerService.ActivePort.Returns(0);

        // Act
        await vm.SaveWebhookSettingsAsync();

        // Assert
        await _webhookListenerService.Received(1).SaveSettingsAsync(Arg.Is<WebhookSettingsDto>(s => !s.IsEnabled));
        await _webhookListenerService.Received(1).StopAsync();
        await _dialogService.Received(1).ShowMessageAsync("Thành công", Arg.Any<string>());
        vm.IsWebhookRunning.Should().BeFalse();
    }

    [Fact]
    public async Task Should_TestWebhookAsync_Call_Service_And_ShowMessage()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.WebhookSettings = new WebhookSettingsDto
        {
            Provider = "PayOS",
            TransferPrefix = "KTX"
        };

        var expectedResult = new PaymentReconciliationResultDto
        {
            IsSuccess = true,
            TransactionId = "TEST_123",
            Amount = 100000m,
            Message = "Đối soát thành công"
        };

        _webhookListenerService.TestWebhookAsync(Arg.Any<WebhookPayloadDto>()).Returns(expectedResult);

        // Act
        await vm.TestWebhookAsync();

        // Assert
        await _webhookListenerService.Received(1).TestWebhookAsync(Arg.Is<WebhookPayloadDto>(p => p.Amount == 100000m && p.Gateway == "PayOS"));
        vm.WebhookStatusMessage.Should().Contain("Thành công");
        await _dialogService.Received(1).ShowMessageAsync("Kết quả kiểm thử Webhook", Arg.Any<string>());
    }

    [Fact]
    public void SampleWebhookUrl_Should_Update_Based_On_Port()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        vm.WebhookSettings = new WebhookSettingsDto
        {
            Port = 8080
        };

        // Assert
        vm.SampleWebhookUrl.Should().Be("http://localhost:8080/api/webhook/payment");
    }

    [Fact]
    public async Task Should_Handle_Null_WebhookService_Gracefully()
    {
        // Arrange
        var vm = CreateViewModel(withWebhookService: false);

        // Act & Assert
        await vm.LoadWebhookSettingsAsync(); // Không ném exception
        await vm.SaveWebhookSettingsAsync(); // Thông báo lỗi nhẹ qua DialogService
        await _dialogService.Received(1).ShowMessageAsync("Lỗi", Arg.Any<string>());
    }
}
