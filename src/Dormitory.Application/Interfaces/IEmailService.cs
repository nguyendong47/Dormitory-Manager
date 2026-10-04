using System.Threading.Tasks;
using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ gửi email thông báo và cấu hình máy chủ SMTP
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Gửi email thông báo chi phí hóa đơn tiền phòng & điện nước đính kèm tệp PDF phiếu thu tới sinh viên
    /// </summary>
    /// <param name="billId">Mã định danh hóa đơn</param>
    /// <param name="recipientEmail">Địa chỉ email người nhận</param>
    /// <param name="recipientName">Họ và tên người nhận</param>
    /// <param name="pdfReceiptBytes">Mảng byte tệp PDF phiếu thu đính kèm</param>
    /// <returns>Kết quả gửi email (thành công hoặc thông báo lỗi)</returns>
    Task<SendEmailResult> SendBillInvoiceEmailAsync(int billId, string recipientEmail, string recipientName, byte[] pdfReceiptBytes);

    /// <summary>
    /// Kiểm tra kết nối thử nghiệm tới máy chủ SMTP theo thông số cấu hình
    /// </summary>
    /// <param name="settings">Thông số cấu hình máy chủ SMTP</param>
    /// <returns>Kết quả kiểm tra kết nối</returns>
    Task<SendEmailResult> TestSmtpConnectionAsync(EmailSettingsDto settings);

    /// <summary>
    /// Lấy cấu hình email SMTP hiện tại của hệ thống
    /// </summary>
    /// <returns>Thông số cấu hình EmailSettingsDto</returns>
    Task<EmailSettingsDto> GetEmailSettingsAsync();

    /// <summary>
    /// Lưu cấu hình email SMTP mới vào tệp cấu hình hệ thống
    /// </summary>
    /// <param name="settings">Thông số cấu hình EmailSettingsDto</param>
    /// <returns>True nếu lưu thành công, ngược lại False</returns>
    Task<bool> SaveEmailSettingsAsync(EmailSettingsDto settings);
}
