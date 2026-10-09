using Dormitory.Application.DTOs;
using Dormitory.Core.Entities;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ tạo và xử lý mã QR thanh toán VietQR chuẩn NAPAS 247 (EMVCo)
/// </summary>
public interface IVietQrService
{
    /// <summary>
    /// Xây dựng chuỗi dữ liệu mã QR theo tiêu chuẩn EMVCo QR Code Specification (NAPAS 247)
    /// </summary>
    /// <param name="bankBin">Mã BIN 6 chữ số của ngân hàng thụ hưởng (Napas BIN)</param>
    /// <param name="accountNumber">Số tài khoản thụ hưởng</param>
    /// <param name="amount">Số tiền cần thanh toán (VNĐ)</param>
    /// <param name="transferContent">Nội dung chuyển khoản</param>
    /// <returns>Chuỗi payload EMVCo hoàn chỉnh có kèm mã kiểm tra CRC-16</returns>
    string BuildEmvCoPayload(string bankBin, string accountNumber, decimal amount, string transferContent);

    /// <summary>
    /// Tính toán mã kiểm tra CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF) cho chuỗi dữ liệu đầu vào
    /// </summary>
    /// <param name="data">Chuỗi dữ liệu cần tính CRC</param>
    /// <returns>Mã kiểm tra CRC-16 dạng chuỗi hexa 4 ký tự viết hoa</returns>
    string CalculateCrc16(string data);

    /// <summary>
    /// Kiểm tra tính hợp lệ của chuỗi payload EMVCo bằng cách kiểm tra cấu trúc TLV và mã CRC-16
    /// </summary>
    /// <param name="emvCoPayload">Chuỗi payload EMVCo cần kiểm tra</param>
    /// <returns>True nếu payload hợp lệ và checksum chính xác; ngược lại False</returns>
    bool ValidateEmvCoPayload(string emvCoPayload);

    /// <summary>
    /// Sinh URL hình ảnh VietQR QuickLink (dịch vụ api img.vietqr.io)
    /// </summary>
    /// <param name="bankBin">Mã BIN ngân hàng thụ hưởng</param>
    /// <param name="accountNumber">Số tài khoản thụ hưởng</param>
    /// <param name="amount">Số tiền cần thanh toán</param>
    /// <param name="transferContent">Nội dung chuyển khoản</param>
    /// <param name="accountHolder">Tên chủ tài khoản thụ hưởng</param>
    /// <param name="template">Mẫu hiển thị hình ảnh QR (mặc định: "compact")</param>
    /// <returns>Đường dẫn URL tải ảnh mã QR VietQR</returns>
    string GenerateQuickLinkUrl(string bankBin, string accountNumber, decimal amount, string transferContent, string accountHolder = "", string template = "compact");

    /// <summary>
    /// Khởi tạo dữ liệu DTO VietQrPayloadDto hoàn chỉnh cho thông tin thanh toán tự do
    /// </summary>
    /// <param name="bankBin">Mã BIN ngân hàng</param>
    /// <param name="accountNumber">Số tài khoản</param>
    /// <param name="accountHolder">Tên chủ tài khoản</param>
    /// <param name="amount">Số tiền thanh toán</param>
    /// <param name="transferContent">Nội dung chuyển khoản</param>
    /// <param name="billCode">Mã hóa đơn liên kết (nếu có)</param>
    /// <param name="template">Mẫu hình ảnh QR (mặc định: "compact")</param>
    /// <returns>Đối tượng VietQrPayloadDto chứa payload EMVCo và QuickLink URL</returns>
    VietQrPayloadDto GeneratePayload(string bankBin, string accountNumber, string accountHolder, decimal amount, string transferContent, string billCode = "", string template = "compact");

    /// <summary>
    /// Khởi tạo dữ liệu DTO VietQrPayloadDto tự động cho hóa đơn phòng và thông tin sinh viên
    /// </summary>
    /// <param name="bill">Đối tượng hóa đơn cần thanh toán</param>
    /// <param name="studentName">Họ và tên sinh viên thanh toán</param>
    /// <param name="bankSettings">Cấu hình tài khoản ngân hàng thụ hưởng của Ký túc xá</param>
    /// <returns>Đối tượng VietQrPayloadDto đã được điền đầy đủ dữ liệu thanh toán</returns>
    VietQrPayloadDto GeneratePayloadForBill(Bill bill, string studentName, BankSettingsDto bankSettings);

    /// <summary>
    /// Khởi tạo dữ liệu DTO VietQrPayloadDto tự động cho thông tin hóa đơn (BillDto) và thông tin sinh viên
    /// </summary>
    /// <param name="bill">Đối tượng DTO hóa đơn cần thanh toán</param>
    /// <param name="studentName">Họ và tên sinh viên thanh toán</param>
    /// <param name="bankSettings">Cấu hình tài khoản ngân hàng thụ hưởng của Ký túc xá</param>
    /// <returns>Đối tượng VietQrPayloadDto đã được điền đầy đủ dữ liệu thanh toán</returns>
    VietQrPayloadDto GeneratePayloadForBill(BillDto bill, string studentName, BankSettingsDto bankSettings);

    /// <summary>
    /// Sinh mảng byte hình ảnh PNG mã QR offline bằng thư viện QRCoder
    /// </summary>
    /// <param name="qrContent">Nội dung cần mã hóa thành QR Code</param>
    /// <param name="pixelsPerModule">Kích thước số điểm ảnh (pixels) cho mỗi ô mã QR (mặc định: 10)</param>
    /// <returns>Mảng byte dữ liệu tệp ảnh định dạng PNG</returns>
    byte[] GenerateQrCodePng(string qrContent, int pixelsPerModule = 10);
}
