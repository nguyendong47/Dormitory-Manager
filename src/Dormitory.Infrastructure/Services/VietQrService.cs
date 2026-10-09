using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using QRCoder;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ khởi tạo mã thanh toán VietQR động chuẩn NAPAS 247 (EMVCo QR Code Specification)
/// </summary>
public class VietQrService : IVietQrService
{
    private const string NapasGuid = "A000000727";
    private const string ServiceCode = "QRIBFTTA";
    private const string CurrencyCodeVnd = "704";
    private const string CountryCodeVn = "VN";

    /// <inheritdoc />
    public string BuildEmvCoPayload(string bankBin, string accountNumber, decimal amount, string transferContent)
    {
        var cleanBin = (bankBin ?? string.Empty).Trim();
        var cleanAcc = (accountNumber ?? string.Empty).Trim();
        var cleanContent = SanitizeTransferContent(transferContent);

        // Sub-tag của Tag 38 (Merchant Account Information)
        // Sub-tag 00: GUID NAPAS
        var subTag00 = FormatTlv("00", NapasGuid);

        // Sub-tag 01: Beneficiary Organization (Chứa Sub 00: Bank BIN, Sub 01: Số tài khoản)
        var binSub = FormatTlv("00", cleanBin);
        var accSub = FormatTlv("01", cleanAcc);
        var beneficiaryOrg = binSub + accSub;
        var subTag01 = FormatTlv("01", beneficiaryOrg);

        // Sub-tag 02: Service Code
        var subTag02 = FormatTlv("02", ServiceCode);

        // Ghép Tag 38 hoàn chỉnh
        var tag38Value = subTag00 + subTag01 + subTag02;
        var tag38 = FormatTlv("38", tag38Value);

        // Tag 00: Payload Format Indicator
        var tag00 = FormatTlv("00", "01");

        // Tag 01: Point of Initiation Method (12: Dynamic QR code trong VietQR)
        var tag01 = FormatTlv("01", "12");

        // Tag 53: Transaction Currency (704 = VNĐ)
        var tag53 = FormatTlv("53", CurrencyCodeVnd);

        // Tag 54: Transaction Amount (Chỉ thêm nếu amount > 0, làm tròn số nguyên không phân cách)
        var tag54 = string.Empty;
        if (amount > 0)
        {
            var amountStr = ((long)Math.Round(amount, MidpointRounding.AwayFromZero)).ToString(CultureInfo.InvariantCulture);
            tag54 = FormatTlv("54", amountStr);
        }

        // Tag 58: Country Code (VN)
        var tag58 = FormatTlv("58", CountryCodeVn);

        // Tag 62: Additional Data Field Template (Sub-tag 08: Nội dung chuyển khoản)
        var tag62 = string.Empty;
        if (!string.IsNullOrWhiteSpace(cleanContent))
        {
            var subTag08 = FormatTlv("08", cleanContent);
            tag62 = FormatTlv("62", subTag08);
        }

        // Ghép chuỗi tiền tố chuẩn bị tính checksum CRC-16
        // Tag 63 với độ dài 04 luôn ở cuối cùng
        var payloadWithoutCrc = tag00 + tag01 + tag38 + tag53 + tag54 + tag58 + tag62 + "6304";

        // Tính toán CRC-16/CCITT-FALSE
        var crcHex = CalculateCrc16(payloadWithoutCrc);

        return payloadWithoutCrc + crcHex;
    }

    /// <inheritdoc />
    public string CalculateCrc16(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return "0000";
        }

        byte[] bytes = Encoding.ASCII.GetBytes(data);
        ushort crc = 0xFFFF;
        const ushort polynomial = 0x1021;

        foreach (byte b in bytes)
        {
            crc ^= (ushort)(b << 8);
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) != 0)
                {
                    crc = (ushort)((crc << 1) ^ polynomial);
                }
                else
                {
                    crc = (ushort)(crc << 1);
                }
            }
        }

        return crc.ToString("X4");
    }

    /// <inheritdoc />
    public bool ValidateEmvCoPayload(string emvCoPayload)
    {
        if (string.IsNullOrWhiteSpace(emvCoPayload) || emvCoPayload.Length < 12)
        {
            return false;
        }

        // Phải bắt đầu bằng Payload Format Indicator "000201"
        if (!emvCoPayload.StartsWith("000201", StringComparison.Ordinal))
        {
            return false;
        }

        // Kiểm tra vị trí Tag 63: 8 ký tự cuối cùng phải có dạng "6304XXXX"
        var crcTagPos = emvCoPayload.Length - 8;
        if (emvCoPayload.Substring(crcTagPos, 4) != "6304")
        {
            return false;
        }

        var expectedCrc = emvCoPayload.Substring(emvCoPayload.Length - 4);
        var dataToHash = emvCoPayload.Substring(0, emvCoPayload.Length - 4);

        var actualCrc = CalculateCrc16(dataToHash);
        if (!actualCrc.Equals(expectedCrc, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Kiểm tra cấu trúc TLV của toàn bộ chuỗi
        int index = 0;
        while (index < emvCoPayload.Length)
        {
            if (index + 4 > emvCoPayload.Length)
            {
                return false;
            }

            if (!int.TryParse(emvCoPayload.Substring(index + 2, 2), out int len) || len < 0)
            {
                return false;
            }

            index += 4;
            if (index + len > emvCoPayload.Length)
            {
                return false;
            }

            index += len;
        }

        return index == emvCoPayload.Length;
    }

    /// <inheritdoc />
    public string GenerateQuickLinkUrl(
        string bankBin,
        string accountNumber,
        decimal amount,
        string transferContent,
        string accountHolder = "",
        string template = "compact")
    {
        var cleanTemplate = string.IsNullOrWhiteSpace(template) ? "compact" : template.Trim();
        var cleanBin = (bankBin ?? string.Empty).Trim();
        var cleanAcc = (accountNumber ?? string.Empty).Trim();
        var amountStr = ((long)Math.Round(amount, MidpointRounding.AwayFromZero)).ToString(CultureInfo.InvariantCulture);
        var escapedContent = Uri.EscapeDataString(transferContent ?? string.Empty);
        var escapedHolder = Uri.EscapeDataString(accountHolder ?? string.Empty);

        return $"https://img.vietqr.io/image/{cleanBin}-{cleanAcc}-{cleanTemplate}.png?amount={amountStr}&addInfo={escapedContent}&accountName={escapedHolder}";
    }

    /// <inheritdoc />
    public VietQrPayloadDto GeneratePayload(
        string bankBin,
        string accountNumber,
        string accountHolder,
        decimal amount,
        string transferContent,
        string billCode = "",
        string template = "compact")
    {
        var emvCo = BuildEmvCoPayload(bankBin, accountNumber, amount, transferContent);
        var quickLink = GenerateQuickLinkUrl(bankBin, accountNumber, amount, transferContent, accountHolder, template);

        return new VietQrPayloadDto
        {
            EmvCoPayload = emvCo,
            QuickLinkUrl = quickLink,
            BankBin = bankBin?.Trim() ?? string.Empty,
            AccountNumber = accountNumber?.Trim() ?? string.Empty,
            AccountHolder = accountHolder?.Trim() ?? string.Empty,
            Amount = amount,
            TransferContent = transferContent ?? string.Empty,
            BillCode = billCode ?? string.Empty
        };
    }

    /// <inheritdoc />
    public VietQrPayloadDto GeneratePayloadForBill(Bill bill, string studentName, BankSettingsDto bankSettings)
    {
        ArgumentNullException.ThrowIfNull(bill);
        ArgumentNullException.ThrowIfNull(bankSettings);

        var billIdentifier = !string.IsNullOrWhiteSpace(bill.BillCode) ? bill.BillCode.Trim() : bill.Id.ToString();
        var prefix = !string.IsNullOrWhiteSpace(bankSettings.TransferPrefix) ? bankSettings.TransferPrefix.Trim() : "KTX";
        var cleanStudent = SanitizeTransferContent(studentName);

        var transferContent = !string.IsNullOrWhiteSpace(cleanStudent)
            ? $"{prefix} {billIdentifier} {cleanStudent}".Trim()
            : $"{prefix} {billIdentifier}".Trim();

        return GeneratePayload(
            bankSettings.BankBin,
            bankSettings.AccountNumber,
            bankSettings.AccountHolder,
            bill.TotalAmount,
            transferContent,
            billIdentifier,
            bankSettings.QrTemplate);
    }

    /// <inheritdoc />
    public byte[] GenerateQrCodePng(string qrContent, int pixelsPerModule = 10)
    {
        if (string.IsNullOrEmpty(qrContent))
        {
            return Array.Empty<byte>();
        }

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule);
    }

    /// <summary>
    /// Định dạng dữ liệu theo chuẩn TLV (Tag - Length - Value)
    /// </summary>
    private static string FormatTlv(string tag, string value)
    {
        var lenStr = value.Length.ToString("D2");
        return $"{tag}{lenStr}{value}";
    }

    /// <summary>
    /// Làm sạch nội dung chuyển khoản: loại bỏ dấu tiếng Việt, ký tự đặc biệt, chuẩn hóa khoảng trắng và viết hoa
    /// </summary>
    private static string SanitizeTransferContent(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var withoutDiacritics = RemoveDiacritics(input);
        var sb = new StringBuilder();

        foreach (char c in withoutDiacritics)
        {
            if (char.IsLetterOrDigit(c) || c == '-')
            {
                sb.Append(char.ToUpperInvariant(c));
            }
            else
            {
                sb.Append(' ');
            }
        }

        return Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
    }

    /// <summary>
    /// Loại bỏ dấu thanh tiếng Việt (ví dụ: "Nguyễn" -> "Nguyen")
    /// </summary>
    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        var result = sb.ToString().Normalize(NormalizationForm.FormC);
        return result.Replace("Đ", "D").Replace("đ", "d");
    }
}
