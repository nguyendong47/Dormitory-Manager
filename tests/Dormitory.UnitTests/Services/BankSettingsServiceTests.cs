using Dormitory.Application.Common;
using Dormitory.Application.DTOs;
using Dormitory.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị cho dịch vụ cấu hình tài khoản ngân hàng KTX và danh mục VietQR (BankSettingsService & VietQrBankDirectory)
/// </summary>
public class BankSettingsServiceTests : IDisposable
{
    private readonly string _tempFilePath;
    private readonly BankSettingsService _service;

    public BankSettingsServiceTests()
    {
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"banksettings_test_{Guid.NewGuid():N}.json");
        _service = new BankSettingsService(_tempFilePath);
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
        catch
        {
            // Bỏ qua lỗi dọn dẹp file tạm
        }
    }

    [Fact]
    public async Task GetBankSettingsAsync_WhenFileDoesNotExist_ReturnsDefaultSettings()
    {
        // Act
        var settings = await _service.GetBankSettingsAsync();

        // Assert
        settings.Should().NotBeNull();
        settings.BankBin.Should().Be("970436");
        settings.BankName.Should().Be("Ngân hàng TMCP Ngoại thương Việt Nam");
        settings.BankShortName.Should().Be("Vietcombank");
        settings.AccountNumber.Should().Be("0123456789");
        settings.AccountHolder.Should().Be("BAN QUAN LY KTX");
        settings.QrTemplate.Should().Be("compact");
        settings.TransferPrefix.Should().Be("KTX");
        settings.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task SaveBankSettingsAsync_SavesToFileAndUpdatesCache()
    {
        // Arrange
        var newSettings = new BankSettingsDto
        {
            BankBin = "970422",
            BankName = "Ngân hàng TMCP Quân đội",
            BankShortName = "MBBank",
            AccountNumber = "0987654321",
            AccountHolder = "KTX KHU A",
            QrTemplate = "compact2",
            TransferPrefix = "KTX_A",
            IsEnabled = true
        };

        // Act
        var saveResult = await _service.SaveBankSettingsAsync(newSettings);
        var loaded = await _service.GetBankSettingsAsync();

        // Assert
        saveResult.Should().BeTrue();
        File.Exists(_tempFilePath).Should().BeTrue();
        loaded.BankBin.Should().Be("970422");
        loaded.BankShortName.Should().Be("MBBank");
        loaded.AccountNumber.Should().Be("0987654321");
        loaded.AccountHolder.Should().Be("KTX KHU A");
        loaded.QrTemplate.Should().Be("compact2");
        loaded.TransferPrefix.Should().Be("KTX_A");
        loaded.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task SaveBankSettingsAsync_AndNewServiceInstance_ReadsPersistedSettings()
    {
        // Arrange
        var custom = new BankSettingsDto
        {
            BankBin = "970415",
            BankName = "Ngân hàng TMCP Công thương Việt Nam",
            BankShortName = "VietinBank",
            AccountNumber = "101009876543",
            AccountHolder = "TRUONG DAI HOC KTX",
            QrTemplate = "qr_only",
            TransferPrefix = "DORM",
            IsEnabled = false
        };

        await _service.SaveBankSettingsAsync(custom);

        // Act: Khởi tạo instance mới cùng trỏ tới file tạm để kiểm tra tính bền vững
        var newServiceInstance = new BankSettingsService(_tempFilePath);
        var loaded = await newServiceInstance.GetBankSettingsAsync();

        // Assert
        loaded.Should().NotBeNull();
        loaded.BankBin.Should().Be("970415");
        loaded.BankShortName.Should().Be("VietinBank");
        loaded.AccountNumber.Should().Be("101009876543");
        loaded.AccountHolder.Should().Be("TRUONG DAI HOC KTX");
        loaded.QrTemplate.Should().Be("qr_only");
        loaded.TransferPrefix.Should().Be("DORM");
        loaded.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task SaveBankSettingsAsync_WhenBankNameOrShortNameMissing_AutoPopulatesFromDirectory()
    {
        // Arrange: Cung cấp BIN 970407 (Techcombank) nhưng để trống tên ngân hàng
        var incompleteSettings = new BankSettingsDto
        {
            BankBin = "970407",
            BankName = "",
            BankShortName = "",
            AccountNumber = "190300123456",
            AccountHolder = "KTX DHQG",
            QrTemplate = "compact",
            TransferPrefix = "KTX",
            IsEnabled = true
        };

        // Act
        var saveResult = await _service.SaveBankSettingsAsync(incompleteSettings);
        var loaded = await _service.GetBankSettingsAsync();

        // Assert
        saveResult.Should().BeTrue();
        loaded.BankShortName.Should().Be("Techcombank");
        loaded.BankName.Should().Be("Ngân hàng TMCP Kỹ thương Việt Nam");
    }

    [Fact]
    public async Task SaveBankSettingsAsync_NormalizesAccountHolderToUpperCaseAndTrims()
    {
        // Arrange
        var settings = new BankSettingsDto
        {
            BankBin = "970436",
            AccountNumber = " 0123456789 ",
            AccountHolder = " ban quan ly ktx khu b ",
            TransferPrefix = " ktx ",
            QrTemplate = " compact "
        };

        // Act
        await _service.SaveBankSettingsAsync(settings);
        var loaded = await _service.GetBankSettingsAsync();

        // Assert
        loaded.AccountNumber.Should().Be("0123456789");
        loaded.AccountHolder.Should().Be("BAN QUAN LY KTX KHU B");
        loaded.TransferPrefix.Should().Be("ktx");
        loaded.QrTemplate.Should().Be("compact");
    }

    [Fact]
    public async Task SaveBankSettingsAsync_WithNullSettings_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SaveBankSettingsAsync(null!));
    }

    [Fact]
    public async Task GetSupportedBanksAsync_ReturnsAllBanks()
    {
        // Act
        var banks = await _service.GetSupportedBanksAsync();

        // Assert
        banks.Should().NotBeNull();
        banks.Count.Should().BeGreaterThanOrEqualTo(35);
    }

    [Fact]
    public void VietQrBankDirectory_ContainsAtLeast35Banks_WithValidProperties()
    {
        // Act
        var banks = VietQrBankDirectory.GetSupportedBanks();

        // Assert
        banks.Should().NotBeNull();
        banks.Count.Should().BeGreaterThanOrEqualTo(35);

        foreach (var bank in banks)
        {
            bank.Bin.Should().NotBeNullOrWhiteSpace();
            bank.Bin.Should().HaveLength(6);
            bank.ShortName.Should().NotBeNullOrWhiteSpace();
            bank.Name.Should().NotBeNullOrWhiteSpace();
            bank.Code.Should().NotBeNullOrWhiteSpace();
        }

        // Tất cả mã BIN phải là duy nhất
        var binCodes = banks.Select(b => b.Bin).ToList();
        binCodes.Should().OnlyHaveUniqueItems();
    }

    [Theory]
    [InlineData("970436", "Vietcombank")]
    [InlineData("970415", "VietinBank")]
    [InlineData("970418", "BIDV")]
    [InlineData("970405", "Agribank")]
    [InlineData("970422", "MBBank")]
    [InlineData("970407", "Techcombank")]
    [InlineData("970432", "VPBank")]
    [InlineData("970416", "ACB")]
    [InlineData("970423", "TPBank")]
    [InlineData("970403", "Sacombank")]
    public void VietQrBankDirectory_FindByBin_ReturnsCorrectBank(string bin, string expectedShortName)
    {
        // Act
        var bank = VietQrBankDirectory.FindByBin(bin);

        // Assert
        bank.Should().NotBeNull();
        bank!.ShortName.Should().Be(expectedShortName);
    }

    [Fact]
    public void VietQrBankDirectory_FindByBin_WithWhitespace_ReturnsCorrectBank()
    {
        // Act
        var bank = VietQrBankDirectory.FindByBin("  970436  ");

        // Assert
        bank.Should().NotBeNull();
        bank!.ShortName.Should().Be("Vietcombank");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("000000")]
    [InlineData("999999")]
    public void VietQrBankDirectory_FindByBin_WhenInvalidOrNotFound_ReturnsNull(string? bin)
    {
        // Act
        var bank = VietQrBankDirectory.FindByBin(bin);

        // Assert
        bank.Should().BeNull();
    }

    [Theory]
    [InlineData("VCB", "Vietcombank")]
    [InlineData("vcb", "Vietcombank")]
    [InlineData("MB", "MBBank")]
    [InlineData("mb", "MBBank")]
    [InlineData("TCB", "Techcombank")]
    [InlineData("Vietcombank", "Vietcombank")]
    [InlineData("mbbank", "MBBank")]
    public void VietQrBankDirectory_FindByCode_ReturnsCorrectBank(string code, string expectedShortName)
    {
        // Act
        var bank = VietQrBankDirectory.FindByCode(code);

        // Assert
        bank.Should().NotBeNull();
        bank!.ShortName.Should().Be(expectedShortName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("UNKNOWN_BANK_CODE")]
    public void VietQrBankDirectory_FindByCode_WhenInvalidOrNotFound_ReturnsNull(string? code)
    {
        // Act
        var bank = VietQrBankDirectory.FindByCode(code);

        // Assert
        bank.Should().BeNull();
    }

    #region Mở rộng kiểm thử: Xử lý tệp JSON hỏng và nạp cấu hình đa dạng

    [Theory]
    [InlineData("{ corrupted-json-without-closing-brace: [1, 2, ")]
    [InlineData("MALFORMED JSON CONTENT !@#$%^&*()")]
    [InlineData("{\"BankBin\": 970436, \"Broken\": ")]
    public async Task GetBankSettingsAsync_WhenFileIsCorruptedJson_SafelyFallsBackToDefaultSettings(string corruptedContent)
    {
        // Arrange: Ghi nội dung JSON bị hỏng vào file cấu hình tạm
        await File.WriteAllTextAsync(_tempFilePath, corruptedContent);

        // Khởi tạo instance mới trỏ tới file hỏng
        var serviceWithCorruptedFile = new BankSettingsService(_tempFilePath);

        // Act
        var settings = await serviceWithCorruptedFile.GetBankSettingsAsync();

        // Assert: Không được phát sinh ngoại lệ và fallback về cấu hình mặc định an toàn
        settings.Should().NotBeNull();
        settings.BankBin.Should().Be("970436");
        settings.BankShortName.Should().Be("Vietcombank");
        settings.AccountNumber.Should().Be("0123456789");
        settings.AccountHolder.Should().Be("BAN QUAN LY KTX");
        settings.QrTemplate.Should().Be("compact");
        settings.TransferPrefix.Should().Be("KTX");
        settings.IsEnabled.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("   \r\n\t   ")]
    public async Task GetBankSettingsAsync_WhenFileIsEmptyOrWhitespace_SafelyFallsBackToDefaultSettings(string emptyContent)
    {
        // Arrange
        await File.WriteAllTextAsync(_tempFilePath, emptyContent);

        var serviceWithEmptyFile = new BankSettingsService(_tempFilePath);

        // Act
        var settings = await serviceWithEmptyFile.GetBankSettingsAsync();

        // Assert
        settings.Should().NotBeNull();
        settings.BankBin.Should().Be("970436");
        settings.BankShortName.Should().Be("Vietcombank");
        settings.IsEnabled.Should().BeTrue();
    }

    [Theory]
    [InlineData("compact2", "970418", "BIDV", "1234567890", "BAN QUAN LY KTX KHU B", "KTX_B", true)]
    [InlineData("qr_only", "970422", "MBBank", "0987654321", "KTX DAI HOC QUOC GIA", "KTX_DHQG", false)]
    [InlineData("print", "970405", "Agribank", "2200101234567", "PHONG TAI VU KTX", "DORM_FEE", true)]
    [InlineData("compact", "970415", "VietinBank", "101009876543", "TRUONG DAI HOC KTX", "KTX_2026", true)]
    public async Task SaveBankSettingsAsync_WithVariousTemplatesAndRichFields_PreservesAndReloadsAccurately(
        string template,
        string bankBin,
        string expectedShortName,
        string accountNumber,
        string accountHolder,
        string prefix,
        bool isEnabled)
    {
        // Arrange
        var customSettings = new BankSettingsDto
        {
            BankBin = bankBin,
            BankShortName = expectedShortName,
            BankName = VietQrBankDirectory.FindByBin(bankBin)?.Name ?? string.Empty,
            AccountNumber = accountNumber,
            AccountHolder = accountHolder,
            QrTemplate = template,
            TransferPrefix = prefix,
            IsEnabled = isEnabled
        };

        // Act: Lưu qua service hiện tại
        var saveResult = await _service.SaveBankSettingsAsync(customSettings);
        saveResult.Should().BeTrue();

        // Nạp lại qua một instance mới độc lập hoàn toàn (không dùng in-memory cache của service cũ)
        var freshService = new BankSettingsService(_tempFilePath);
        var loaded = await freshService.GetBankSettingsAsync();

        // Assert: Xác minh tất cả các trường thông tin phong phú
        loaded.Should().NotBeNull();
        loaded.BankBin.Should().Be(bankBin);
        loaded.BankShortName.Should().Be(expectedShortName);
        loaded.AccountNumber.Should().Be(accountNumber);
        loaded.AccountHolder.Should().Be(accountHolder);
        loaded.QrTemplate.Should().Be(template);
        loaded.TransferPrefix.Should().Be(prefix);
        loaded.IsEnabled.Should().Be(isEnabled);
    }

    #endregion
}
