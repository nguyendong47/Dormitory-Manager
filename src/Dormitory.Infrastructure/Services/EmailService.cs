using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ gửi email tự động hóa đơn tiền phòng & điện nước kèm tệp PDF phiếu thu sử dụng MailKit/MimeKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly IDormitoryDbContext _context;
    private readonly IConfiguration? _configuration;
    private readonly string? _customSettingsPath;
    private readonly IBankSettingsService? _bankSettingsService;
    private readonly IVietQrService? _vietQrService;

    /// <summary>
    /// Nội dung HTML của hóa đơn được sinh gần nhất (phục vụ kiểm thử và kiểm tra kết quả)
    /// </summary>
    public string? LastGeneratedHtmlBody { get; private set; }

    public EmailService(
        IDormitoryDbContext context,
        IConfiguration? configuration = null,
        string? customSettingsPath = null,
        IBankSettingsService? bankSettingsService = null,
        IVietQrService? vietQrService = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration;
        _customSettingsPath = customSettingsPath;
        _bankSettingsService = bankSettingsService;
        _vietQrService = vietQrService;
    }

    /// <summary>
    /// Constructor hỗ trợ chỉ định tệp cấu hình tùy chỉnh (dùng cho kiểm thử đơn vị độc lập)
    /// </summary>
    public EmailService(
        IDormitoryDbContext context,
        string customSettingsPath,
        IBankSettingsService? bankSettingsService = null,
        IVietQrService? vietQrService = null)
        : this(context, null, customSettingsPath, bankSettingsService, vietQrService)
    {
    }

    /// <summary>
    /// Lấy đường dẫn tuyệt đối tới tệp cấu hình emailsettings.json
    /// </summary>
    private string GetSettingsFilePath()
    {
        if (!string.IsNullOrWhiteSpace(_customSettingsPath))
        {
            var dir = Path.GetDirectoryName(_customSettingsPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return _customSettingsPath;
        }

        var configPath = _configuration?["EmailSettingsFilePath"];
        if (!string.IsNullOrWhiteSpace(configPath))
        {
            return Path.GetFullPath(configPath);
        }

        return DatabasePathResolver.ResolveDatabasePath("emailsettings.json");
    }

    /// <summary>
    /// Đọc cấu hình máy chủ SMTP từ tệp json
    /// </summary>
    public async Task<EmailSettingsDto> GetEmailSettingsAsync()
    {
        var filePath = GetSettingsFilePath();
        if (!File.Exists(filePath))
        {
            return new EmailSettingsDto();
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var settings = JsonSerializer.Deserialize<EmailSettingsDto>(json);
            return settings ?? new EmailSettingsDto();
        }
        catch
        {
            return new EmailSettingsDto();
        }
    }

    /// <summary>
    /// Lưu cấu hình máy chủ SMTP vào tệp json
    /// </summary>
    public async Task<bool> SaveEmailSettingsAsync(EmailSettingsDto settings)
    {
        try
        {
            var filePath = GetSettingsFilePath();
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(settings, options);
            await File.WriteAllTextAsync(filePath, json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra kết nối thử nghiệm tới máy chủ SMTP
    /// </summary>
    public async Task<SendEmailResult> TestSmtpConnectionAsync(EmailSettingsDto settings)
    {
        // Khi cấu hình giả lập (Mock mode) hoặc chưa kích hoạt gửi thật
        if (!settings.IsEnabled || string.Equals(settings.SmtpServer, "mock", StringComparison.OrdinalIgnoreCase))
        {
            return SendEmailResult.Ok();
        }

        if (string.IsNullOrWhiteSpace(settings.SmtpServer))
        {
            return SendEmailResult.Fail("Địa chỉ máy chủ SMTP không được để trống.");
        }

        try
        {
            using var client = new SmtpClient();
            client.Timeout = 10000; // 10 giây chờ kết nối

            var secureSocketOptions = settings.EnableSsl
                ? (settings.SmtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                : SecureSocketOptions.None;

            await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, secureSocketOptions);

            if (!string.IsNullOrWhiteSpace(settings.Username) && !string.IsNullOrWhiteSpace(settings.Password))
            {
                await client.AuthenticateAsync(settings.Username, settings.Password);
            }

            await client.DisconnectAsync(true);
            return SendEmailResult.Ok();
        }
        catch (Exception ex)
        {
            return SendEmailResult.Fail($"Kết nối tới máy chủ SMTP thất bại: {ex.Message}");
        }
    }

    /// <summary>
    /// Gửi email hóa đơn tiền phòng và dịch vụ kèm PDF phiếu thu tới sinh viên
    /// </summary>
    public async Task<SendEmailResult> SendBillInvoiceEmailAsync(
        int billId,
        string recipientEmail,
        string recipientName,
        byte[] pdfReceiptBytes)
    {
        // 1. Kiểm tra tính hợp lệ của tham số
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            return SendEmailResult.Fail("Địa chỉ email người nhận không được để trống.");
        }

        if (!recipientEmail.Contains('@'))
        {
            return SendEmailResult.Fail("Địa chỉ email người nhận không hợp lệ.");
        }

        if (pdfReceiptBytes == null || pdfReceiptBytes.Length == 0)
        {
            return SendEmailResult.Fail("Tệp PDF phiếu thu đính kèm không hợp lệ hoặc rỗng.");
        }

        // 2. Truy vấn hóa đơn từ CSDL
        var bill = await _context.Bills
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == billId);

        if (bill == null)
        {
            return SendEmailResult.Fail($"Không tìm thấy hóa đơn có ID = {billId}.");
        }

        // 3. Tải cấu hình SMTP và cấu hình tài khoản ngân hàng VietQR
        var settings = await GetEmailSettingsAsync();
        var bankSettings = _bankSettingsService != null
            ? await _bankSettingsService.GetBankSettingsAsync()
            : new BankSettingsDto();

        VietQrPayloadDto? vietQrPayload = null;
        if (bankSettings.IsEnabled && _vietQrService != null)
        {
            vietQrPayload = _vietQrService.GeneratePayloadForBill(bill, recipientName, bankSettings);
        }

        var htmlBody = GenerateBillInvoiceHtml(bill, recipientName, bankSettings, vietQrPayload);
        LastGeneratedHtmlBody = htmlBody;

        // 4. Safe Mode / Mock Mode: Giả lập gửi thành công khi chưa bật SMTP thật hoặc email kiểm thử
        if (!settings.IsEnabled ||
            string.Equals(settings.SmtpServer, "mock", StringComparison.OrdinalIgnoreCase) ||
            recipientEmail.EndsWith("@example.com", StringComparison.OrdinalIgnoreCase))
        {
            return SendEmailResult.Ok();
        }

        // 5. Chế độ gửi email thực tế qua MailKit
        try
        {
            var message = new MimeMessage();
            var fromEmail = string.IsNullOrWhiteSpace(settings.SenderEmail) ? "noreply@ktx.edu.vn" : settings.SenderEmail;
            var fromName = string.IsNullOrWhiteSpace(settings.SenderName) ? "Ban Quản lý Ký túc xá" : settings.SenderName;

            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(recipientName, recipientEmail));
            message.Subject = $"[KTX] Thông báo chi phí tiền phòng và dịch vụ tháng {bill.Month}/{bill.Year} - Hóa đơn {bill.BillCode}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            // Đính kèm tệp PDF phiếu thu
            bodyBuilder.Attachments.Add($"PhieuThu_{bill.BillCode}.pdf", pdfReceiptBytes, new ContentType("application", "pdf"));

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Timeout = 15000;

            var secureSocketOptions = settings.EnableSsl
                ? (settings.SmtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                : SecureSocketOptions.None;

            await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, secureSocketOptions);

            if (!string.IsNullOrWhiteSpace(settings.Username) && !string.IsNullOrWhiteSpace(settings.Password))
            {
                await client.AuthenticateAsync(settings.Username, settings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return SendEmailResult.Ok();
        }
        catch (Exception ex)
        {
            return SendEmailResult.Fail($"Lỗi khi gửi email qua SMTP: {ex.Message}");
        }
    }

    /// <summary>
    /// Sinh nội dung HTML thông báo hóa đơn trang nhã, đầy đủ thông tin chi tiết và mã thanh toán VietQR
    /// </summary>
    public static string GenerateBillInvoiceHtml(
        Bill bill,
        string recipientName,
        BankSettingsDto? bankSettings = null,
        VietQrPayloadDto? payload = null)
    {
        bankSettings ??= new BankSettingsDto();

        var roomNumber = bill.Room?.RoomNumber ?? bill.RoomId.ToString();
        var building = bill.Room?.Building ?? "KTX";
        var statusText = bill.Status == BillStatus.Paid ? "ĐÃ THANH TOÁN" : "CHƯA THANH TOÁN (CÒN NỢ)";
        var statusColor = bill.Status == BillStatus.Paid ? "#107C41" : "#D83B01";

        string bankPaymentSectionHtml;

        if (bankSettings.IsEnabled && payload != null)
        {
            bankPaymentSectionHtml = $@"
            <div class=""bank-info"">
                <h3>💳 THÔNG TIN CHUYỂN KHOẢN THANH TOÁN (VIETQR NAPAS 24/7):</h3>
                <table style=""width: 100%; border-collapse: collapse; margin-top: 8px;"">
                    <tr>
                        <td style=""width: 155px; vertical-align: top; text-align: center; padding-right: 15px;"">
                            <img src=""{payload.QuickLinkUrl}"" alt=""Mã QR thanh toán VietQR"" style=""width: 145px; height: 145px; border: 1px solid #c7e0f4; border-radius: 6px; padding: 4px; background: #ffffff; display: block; margin: 0 auto;"" />
                            <div style=""font-size: 11px; color: #605e5c; margin-top: 6px;"">Quét mã để thanh toán</div>
                        </td>
                        <td style=""vertical-align: top;"">
                            <ul style=""margin: 0; padding-left: 18px; font-size: 13px; line-height: 1.8;"">
                                <li><strong>Ngân hàng thụ hưởng:</strong> {bankSettings.BankName} ({bankSettings.BankShortName})</li>
                                <li><strong>Số tài khoản:</strong> <span style=""font-weight: bold; color: #242424;"">{bankSettings.AccountNumber}</span></li>
                                <li><strong>Chủ tài khoản:</strong> <span style=""font-weight: bold; color: #242424;"">{bankSettings.AccountHolder}</span></li>
                                <li><strong>Số tiền:</strong> <span style=""font-weight: bold; color: #D83B01;"">{bill.TotalAmount:N0} VNĐ</span></li>
                                <li><strong>Nội dung CK (Bắt buộc):</strong> <span style=""color: #0078D4; font-weight: bold; background: #e0eeff; padding: 2px 6px; border-radius: 4px;"">{payload.TransferContent}</span></li>
                            </ul>
                            <div style=""font-size: 12px; color: #605e5c; margin-top: 8px; font-style: italic;"">
                                💡 Quét mã qua bất kỳ ứng dụng ngân hàng hoặc ví điện tử để tự động điền đúng số tiền và nội dung.
                            </div>
                        </td>
                    </tr>
                </table>
            </div>";
        }
        else
        {
            var transferContentFallback = $"{bill.BillCode} {roomNumber}".Trim();
            bankPaymentSectionHtml = $@"
            <div class=""bank-info"">
                <h3>💳 THÔNG TIN CHUYỂN KHOẢN THANH TOÁN:</h3>
                <ul>
                    <li><strong>Ngân hàng thụ hưởng:</strong> {bankSettings.BankName} ({bankSettings.BankShortName})</li>
                    <li><strong>Số tài khoản:</strong> {bankSettings.AccountNumber}</li>
                    <li><strong>Tên chủ tài khoản:</strong> {bankSettings.AccountHolder}</li>
                    <li><strong>Nội dung chuyển khoản (Bắt buộc):</strong> <span style=""color: #0078D4; font-weight: bold;"">{transferContentFallback}</span></li>
                </ul>
            </div>";
        }

        return $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333333; margin: 0; padding: 20px; background-color: #f4f6f9; }}
        .container {{ max-width: 650px; margin: 0 auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border: 1px solid #e1dfdd; }}
        .header {{ background-color: #0078D4; color: #ffffff; padding: 24px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 20px; font-weight: 600; letter-spacing: 0.5px; }}
        .header p {{ margin: 6px 0 0 0; font-size: 13px; opacity: 0.9; }}
        .content {{ padding: 28px 24px; }}
        .greeting {{ font-size: 15px; margin-bottom: 16px; }}
        .badge {{ display: inline-block; padding: 4px 12px; border-radius: 4px; font-size: 12px; font-weight: bold; color: white; background-color: {statusColor}; margin-top: 4px; }}
        .info-card {{ background-color: #f8f9fa; border: 1px solid #edebe9; border-radius: 6px; padding: 14px 18px; margin-bottom: 20px; }}
        .table {{ width: 100%; border-collapse: collapse; margin-top: 16px; margin-bottom: 20px; font-size: 14px; }}
        .table th {{ background-color: #f3f2f1; text-align: left; padding: 10px 12px; border-bottom: 2px solid #0078D4; font-weight: 600; }}
        .table td {{ padding: 10px 12px; border-bottom: 1px solid #edebe9; }}
        .table .num {{ text-align: right; }}
        .total-row td {{ font-weight: bold; font-size: 16px; color: #0078D4; background-color: #f8f9fa; border-top: 2px solid #0078D4; }}
        .bank-info {{ background-color: #e8f3fc; border: 1px solid #c7e0f4; border-radius: 6px; padding: 16px; margin-top: 20px; }}
        .bank-info h3 {{ margin: 0 0 8px 0; font-size: 14px; color: #0078D4; }}
        .bank-info ul {{ margin: 0; padding-left: 20px; font-size: 13px; }}
        .footer {{ background-color: #f8f9fa; padding: 16px 24px; text-align: center; font-size: 12px; color: #605e5c; border-top: 1px solid #edebe9; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>BAN QUẢN LÝ KÝ TÚC XÁ</h1>
            <p>HỆ THỐNG THÔNG BÁO CHI PHÍ LƯU TRÚ VÀ DỊCH VỤ</p>
        </div>
        <div class=""content"">
            <div class=""greeting"">
                Kính gửi bạn: <strong>{recipientName}</strong>,
            </div>
            <p>Ban Quản lý Ký túc xá xin gửi tới bạn thông báo chi phí lưu trú và sử dụng dịch vụ tháng <strong>{bill.Month}/{bill.Year}</strong> như sau:</p>
            
            <div class=""info-card"">
                <table style=""width: 100%; font-size: 13px;"">
                    <tr>
                        <td style=""width: 50%;""><strong>Mã hóa đơn:</strong> {bill.BillCode}</td>
                        <td><strong>Phòng:</strong> {roomNumber} - {building}</td>
                    </tr>
                    <tr>
                        <td><strong>Hạn nộp tiền:</strong> {bill.DueDate:dd/MM/yyyy}</td>
                        <td><strong>Trạng thái:</strong> <span class=""badge"">{statusText}</span></td>
                    </tr>
                </table>
            </div>

            <table class=""table"">
                <thead>
                    <tr>
                        <th>Khoản mục chi phí</th>
                        <th class=""num"">Số lượng</th>
                        <th class=""num"">Đơn giá (đ)</th>
                        <th class=""num"">Thành tiền (đ)</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Tiền thuê phòng tháng {bill.Month}/{bill.Year}</td>
                        <td class=""num"">1 tháng</td>
                        <td class=""num"">{bill.RoomFee:N0}</td>
                        <td class=""num"">{bill.RoomFee:N0}</td>
                    </tr>
                    <tr>
                        <td>Tiền điện ({bill.OldElectricIndex:N0} ➔ {bill.NewElectricIndex:N0} kWh)</td>
                        <td class=""num"">{bill.ElectricUsage:N0} kWh</td>
                        <td class=""num"">{bill.ElectricRate:N0}</td>
                        <td class=""num"">{bill.ElectricFee:N0}</td>
                    </tr>
                    <tr>
                        <td>Tiền nước ({bill.OldWaterIndex:N0} ➔ {bill.NewWaterIndex:N0} m³)</td>
                        <td class=""num"">{bill.WaterUsage:N0} m³</td>
                        <td class=""num"">{bill.WaterRate:N0}</td>
                        <td class=""num"">{bill.WaterFee:N0}</td>
                    </tr>
                    <tr>
                        <td>Phụ phí dịch vụ khác (vệ sinh, internet, an ninh)</td>
                        <td class=""num"">-</td>
                        <td class=""num"">-</td>
                        <td class=""num"">{bill.OtherServiceFee:N0}</td>
                    </tr>
                    <tr class=""total-row"">
                        <td colspan=""3"">TỔNG CỘNG THANH TOÁN:</td>
                        <td class=""num"">{bill.TotalAmount:N0} đ</td>
                    </tr>
                </tbody>
            </table>

            {bankPaymentSectionHtml}

            <p style=""font-size: 13px; color: #605e5c; margin-top: 16px;"">
                📎 <em>Lưu ý: Tệp PDF phiếu thu chi tiết đã được đính kèm vào email này (<strong>PhieuThu_{bill.BillCode}.pdf</strong>). Quý sinh viên vui lòng thanh toán đúng hạn trước ngày {bill.DueDate:dd/MM/yyyy} để tránh phát sinh phạt nộp muộn.</em>
            </p>
        </div>
        <div class=""footer"">
            Ban Quản lý Ký túc xá sinh viên • Hotline hỗ trợ: (024) 3869 1234 • Email: ktx@university.edu.vn<br/>
            <em>Đây là email tự động từ hệ thống quản lý KTX, vui lòng không phản hồi trực tiếp vào hòm thư này.</em>
        </div>
    </div>
</body>
</html>";
    }
}
