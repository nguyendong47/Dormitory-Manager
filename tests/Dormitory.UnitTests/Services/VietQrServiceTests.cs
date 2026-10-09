using Dormitory.Application.DTOs;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị toàn diện cho dịch vụ sinh mã VietQR chuẩn NAPAS 247 EMVCo (VietQrService)
/// </summary>
public class VietQrServiceTests
{
    private readonly VietQrService _service;

    public VietQrServiceTests()
    {
        _service = new VietQrService();
    }

    #region 1. Kiểm thử thuật toán CRC-16/CCITT-FALSE

    [Fact]
    public void CalculateCrc16_WithStandardTestVector_ReturnsExpectedChecksum()
    {
        // Vector chuẩn quốc tế kiểm tra thuật toán CRC-16/CCITT-FALSE: "123456789" -> 0x29B1
        // Act
        var result = _service.CalculateCrc16("123456789");

        // Assert
        result.Should().Be("29B1");
    }

    [Theory]
    [InlineData("", "0000")]
    [InlineData(null, "0000")]
    public void CalculateCrc16_WithNullOrEmpty_ReturnsZeroHex(string? input, string expected)
    {
        // Act
        var result = _service.CalculateCrc16(input!);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateCrc16_ReturnsUppercaseFourCharacterHex()
    {
        // Act
        var result = _service.CalculateCrc16("VietQR Napas 247 Dormitory");

        // Assert
        result.Should().HaveLength(4);
        result.Should().MatchRegex("^[0-9A-F]{4}$");
    }

    [Fact]
    public void CalculateCrc16_SingleByteChange_ProducesDifferentChecksum()
    {
        // Arrange
        var text1 = "00020101021238540010A0000007276304";
        var text2 = "00020101021238540010A0000007286304";

        // Act
        var crc1 = _service.CalculateCrc16(text1);
        var crc2 = _service.CalculateCrc16(text2);

        // Assert
        crc1.Should().NotBe(crc2);
    }

    #endregion

    #region 2. Kiểm thử BuildEmvCoPayload

    [Fact]
    public void BuildEmvCoPayload_WithValidInputs_GeneratesStandardEmvCoString()
    {
        // Arrange
        var bankBin = "970436"; // Vietcombank
        var accountNumber = "1018593456";
        var amount = 1500000m;
        var transferContent = "KTX HD001 NGUYEN VAN A";

        // Act
        var payload = _service.BuildEmvCoPayload(bankBin, accountNumber, amount, transferContent);

        // Assert
        payload.Should().NotBeNullOrWhiteSpace();

        // Tag 00: Payload Format Indicator ("000201")
        payload.Should().StartWith("000201");

        // Tag 01: Point of Initiation Method ("010212")
        payload.Should().Contain("010212");

        // Tag 38: Merchant Account Information chứa GUID NAPAS "A000000727" và Service Code "QRIBFTTA"
        payload.Should().Contain("0010A000000727");
        payload.Should().Contain("0006970436"); // Sub 00 chứa Bank BIN
        payload.Should().Contain("01101018593456"); // Sub 01 chứa STK 10 ký tự
        payload.Should().Contain("0208QRIBFTTA"); // Sub 02 chứa mã dịch vụ

        // Tag 53: Currency VNĐ (704) -> "5303704"
        payload.Should().Contain("5303704");

        // Tag 54: Amount 1500000 -> 7 ký tự -> "54071500000"
        payload.Should().Contain("54071500000");

        // Tag 58: Country VN -> "5802VN"
        payload.Should().Contain("5802VN");

        // Tag 62: Additional Data với Sub 08
        payload.Should().Contain("62");
        payload.Should().Contain("0822KTX HD001 NGUYEN VAN A");

        // Tag 63: CRC16 ở cuối ("6304XXXX")
        payload.Should().Contain("6304");
        payload.Length.Should().BeGreaterThan(50);

        // Kiểm tra tính hợp lệ
        _service.ValidateEmvCoPayload(payload).Should().BeTrue();
    }

    [Fact]
    public void BuildEmvCoPayload_WhenAmountIsZero_OmitsTag54()
    {
        // Arrange
        var bankBin = "970422"; // MBBank
        var accountNumber = "0987654321";

        // Act
        var payload = _service.BuildEmvCoPayload(bankBin, accountNumber, 0m, "KTX TIEN PHONG");

        // Assert - Khi amount = 0, Tag 54 không xuất hiện (Tag 53 đi liền ngay trước Tag 58)
        payload.Should().Contain("53037045802VN");
        _service.ValidateEmvCoPayload(payload).Should().BeTrue();
    }

    [Fact]
    public void BuildEmvCoPayload_RemovesDiacriticsAndSanitizesTransferContent()
    {
        // Arrange
        var bankBin = "970436";
        var accountNumber = "123456789";
        var transferContent = "Tiền phòng KTX Đỗ Đức Đạt #102!";

        // Act
        var payload = _service.BuildEmvCoPayload(bankBin, accountNumber, 200000m, transferContent);

        // Assert
        // Dấu tiếng Việt "Đỗ Đức Đạt" được chuyển thành "DO DUC DAT", "Tiền phòng" -> "TIEN PHONG"
        payload.Should().Contain("TIEN PHONG KTX DO DUC DAT 102");
        payload.Should().NotContain("Tiền");
        payload.Should().NotContain("Đạt");
        payload.Should().NotContain("!");
        _service.ValidateEmvCoPayload(payload).Should().BeTrue();
    }

    [Fact]
    public void BuildEmvCoPayload_WhenTransferContentIsEmpty_OmitsTag62()
    {
        // Act
        var payload = _service.BuildEmvCoPayload("970436", "123456789", 50000m, "   ");

        // Assert
        payload.Should().NotContain("62");
        _service.ValidateEmvCoPayload(payload).Should().BeTrue();
    }

    #endregion

    #region 3. Kiểm thử ValidateEmvCoPayload

    [Fact]
    public void ValidateEmvCoPayload_WithGeneratedPayload_ReturnsTrue()
    {
        // Arrange
        var payload = _service.BuildEmvCoPayload("970436", "0123456789", 350000m, "KTX HD001");

        // Act & Assert
        _service.ValidateEmvCoPayload(payload).Should().BeTrue();
    }

    [Fact]
    public void ValidateEmvCoPayload_WithTamperedByte_ReturnsFalse()
    {
        // Arrange
        var payload = _service.BuildEmvCoPayload("970436", "0123456789", 350000m, "KTX HD001");
        // Thay đổi 1 ký tự số tiền từ '3' thành '4'
        var tampered = payload.Replace("350000", "450000");

        // Act & Assert
        _service.ValidateEmvCoPayload(tampered).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("000201")] // Quá ngắn
    [InlineData("9902010102123854")] // Không bắt đầu bằng 000201
    [InlineData("00020101021238540010A000000727")] // Không có Tag 6304
    [InlineData("00020101021238540010A0000007276304ZZZZ")] // CRC không hợp lệ
    [InlineData("00990101021263041234")] // Chiều dài TLV vượt quá độ dài chuỗi
    public void ValidateEmvCoPayload_WithMalformedPayload_ReturnsFalse(string? malformed)
    {
        // Act & Assert
        _service.ValidateEmvCoPayload(malformed!).Should().BeFalse();
    }

    #endregion

    #region 4. Kiểm thử GenerateQuickLinkUrl

    [Fact]
    public void GenerateQuickLinkUrl_WithParameters_BuildsCorrectUrl()
    {
        // Arrange
        var bankBin = "970436";
        var accountNumber = "0123456789";
        var amount = 1250000m;
        var transferContent = "KTX HD005 NGUYEN VAN B";
        var accountHolder = "BAN QUAN LY KTX";
        var template = "compact2";

        // Act
        var url = _service.GenerateQuickLinkUrl(bankBin, accountNumber, amount, transferContent, accountHolder, template);

        // Assert
        url.Should().StartWith("https://img.vietqr.io/image/970436-0123456789-compact2.png?");
        url.Should().Contain("amount=1250000");
        url.Should().Contain("addInfo=" + Uri.EscapeDataString(transferContent));
        url.Should().Contain("accountName=" + Uri.EscapeDataString(accountHolder));
    }

    [Fact]
    public void GenerateQuickLinkUrl_WhenTemplateIsEmpty_DefaultsToCompact()
    {
        // Act
        var url = _service.GenerateQuickLinkUrl("970436", "123456", 500000m, "KTX", "KTX", "");

        // Assert
        url.Should().StartWith("https://img.vietqr.io/image/970436-123456-compact.png?");
    }

    [Fact]
    public void GenerateQuickLinkUrl_EscapesSpecialCharactersInQueryParameters()
    {
        // Arrange
        var transferContent = "KTX & HD-001 / PHONG 101";
        var accountHolder = "KÝ TÚC XÁ ĐH KHTN";

        // Act
        var url = _service.GenerateQuickLinkUrl("970436", "123456", 100000m, transferContent, accountHolder);

        // Assert
        url.Should().Contain("addInfo=" + Uri.EscapeDataString(transferContent));
        url.Should().Contain("accountName=" + Uri.EscapeDataString(accountHolder));
        url.Should().NotContain(" "); // Tất cả khoảng trắng trong query param phải được encode
    }

    #endregion

    #region 5. Kiểm thử GeneratePayload

    [Fact]
    public void GeneratePayload_ReturnsCompleteDtoWithBothEmvCoAndQuickLink()
    {
        // Arrange
        var bankBin = "970415"; // VietinBank
        var accountNumber = "1122334455";
        var accountHolder = "KTX KHU A";
        var amount = 750000m;
        var transferContent = "KTX HD012";
        var billCode = "HD012";
        var template = "print";

        // Act
        var dto = _service.GeneratePayload(bankBin, accountNumber, accountHolder, amount, transferContent, billCode, template);

        // Assert
        dto.Should().NotBeNull();
        dto.BankBin.Should().Be("970415");
        dto.AccountNumber.Should().Be("1122334455");
        dto.AccountHolder.Should().Be("KTX KHU A");
        dto.Amount.Should().Be(750000m);
        dto.TransferContent.Should().Be("KTX HD012");
        dto.BillCode.Should().Be("HD012");
        dto.QuickLinkUrl.Should().Contain("https://img.vietqr.io/image/970415-1122334455-print.png");
        dto.EmvCoPayload.Should().StartWith("000201");
        _service.ValidateEmvCoPayload(dto.EmvCoPayload).Should().BeTrue();
    }

    #endregion

    #region 6. Kiểm thử GeneratePayloadForBill

    [Fact]
    public void GeneratePayloadForBill_WithValidBillAndStudent_GeneratesProperContentAndAmount()
    {
        // Arrange
        var bill = new Bill
        {
            Id = 42,
            BillCode = "HD-202410-A101",
            RoomFee = 800000m,
            OldElectricIndex = 100,
            NewElectricIndex = 150,
            ElectricRate = 3500, // 50 * 3500 = 175000
            OldWaterIndex = 10,
            NewWaterIndex = 15,
            WaterRate = 15000, // 5 * 15000 = 75000
            OtherServiceFee = 50000m,
            Status = BillStatus.Unpaid
        };
        // TotalAmount = 800000 + 175000 + 75000 + 50000 = 1100000

        var studentName = "Nguyễn Hoàng Nam";
        var bankSettings = new BankSettingsDto
        {
            BankBin = "970436",
            BankName = "Vietcombank",
            AccountNumber = "0123456789",
            AccountHolder = "BAN QUAN LY KTX",
            TransferPrefix = "KTX",
            QrTemplate = "compact"
        };

        // Act
        var dto = _service.GeneratePayloadForBill(bill, studentName, bankSettings);

        // Assert
        dto.Should().NotBeNull();
        dto.BillCode.Should().Be("HD-202410-A101");
        dto.Amount.Should().Be(1100000m);
        // Tên sinh viên được làm sạch bỏ dấu: "NGUYEN HOANG NAM"
        dto.TransferContent.Should().Be("KTX HD-202410-A101 NGUYEN HOANG NAM");
        dto.BankBin.Should().Be("970436");
        dto.AccountNumber.Should().Be("0123456789");
        dto.AccountHolder.Should().Be("BAN QUAN LY KTX");

        _service.ValidateEmvCoPayload(dto.EmvCoPayload).Should().BeTrue();
        dto.EmvCoPayload.Should().Contain("1100000");
    }

    [Fact]
    public void GeneratePayloadForBill_WhenStudentNameIsEmpty_FormatsWithoutStudentSuffix()
    {
        // Arrange
        var bill = new Bill
        {
            Id = 99,
            BillCode = "HD-99",
            RoomFee = 500000m
        };
        var bankSettings = new BankSettingsDto
        {
            BankBin = "970422",
            AccountNumber = "9999999999",
            TransferPrefix = "KTX"
        };

        // Act
        var dto = _service.GeneratePayloadForBill(bill, "", bankSettings);

        // Assert
        dto.TransferContent.Should().Be("KTX HD-99");
        dto.BillCode.Should().Be("HD-99");
    }

    [Fact]
    public void GeneratePayloadForBill_WhenBillCodeIsEmpty_FallsBackToBillId()
    {
        // Arrange
        var bill = new Bill
        {
            Id = 105,
            BillCode = "",
            RoomFee = 600000m
        };
        var bankSettings = new BankSettingsDto
        {
            BankBin = "970436",
            AccountNumber = "123456",
            TransferPrefix = "KTX"
        };

        // Act
        var dto = _service.GeneratePayloadForBill(bill, "Lê Văn Tám", bankSettings);

        // Assert
        dto.BillCode.Should().Be("105");
        dto.TransferContent.Should().Be("KTX 105 LE VAN TAM");
    }

    [Fact]
    public void GeneratePayloadForBill_NullArguments_ThrowsArgumentNullException()
    {
        // Arrange
        var bill = new Bill { Id = 1 };
        var settings = new BankSettingsDto();

        // Act & Assert
        var act1 = () => _service.GeneratePayloadForBill(null!, "Sinh Vien", settings);
        act1.Should().Throw<ArgumentNullException>();

        var act2 = () => _service.GeneratePayloadForBill(bill, "Sinh Vien", null!);
        act2.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region 7. Kiểm thử GenerateQrCodePng

    [Fact]
    public void GenerateQrCodePng_WithValidContent_ReturnsValidPngBytes()
    {
        // Arrange
        var qrContent = "00020101021238540010A00000072701240006970436011001234567890208QRIBFTTA53037045802VN6304";

        // Act
        var pngBytes = _service.GenerateQrCodePng(qrContent, pixelsPerModule: 5);

        // Assert
        pngBytes.Should().NotBeNull();
        pngBytes.Length.Should().BeGreaterThan(100);

        // Kiểm tra 4 bytes đầu tiên chuẩn định dạng tệp PNG: 0x89, 0x50, 0x4E, 0x47 (\x89PNG)
        pngBytes[0].Should().Be(0x89);
        pngBytes[1].Should().Be(0x50);
        pngBytes[2].Should().Be(0x4E);
        pngBytes[3].Should().Be(0x47);
    }

    [Fact]
    public void GenerateQrCodePng_WithLargerModuleSize_ReturnsLargerImage()
    {
        // Arrange
        var qrContent = "https://vietqr.io";

        // Act
        var smallBytes = _service.GenerateQrCodePng(qrContent, pixelsPerModule: 3);
        var largeBytes = _service.GenerateQrCodePng(qrContent, pixelsPerModule: 10);

        // Assert
        smallBytes.Length.Should().BeGreaterThan(0);
        largeBytes.Length.Should().BeGreaterThan(0);
        largeBytes.Length.Should().BeGreaterThan(smallBytes.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void GenerateQrCodePng_WhenContentIsEmptyOrNull_ReturnsEmptyByteArray(string? content)
    {
        // Act
        var bytes = _service.GenerateQrCodePng(content!);

        // Assert
        bytes.Should().BeEmpty();
    }

    #endregion
}
