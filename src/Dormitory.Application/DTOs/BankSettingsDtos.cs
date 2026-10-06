namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chứa thông tin tra cứu cơ bản của một ngân hàng trong hệ thống Napas/VietQR
/// </summary>
public class BankInfoDto
{
    /// <summary>
    /// Mã định danh ngân hàng Napas BIN (Bank Identification Number - 6 chữ số)
    /// </summary>
    public string Bin { get; set; } = string.Empty;

    /// <summary>
    /// Tên viết tắt của ngân hàng (ví dụ: Vietcombank, MBBank, BIDV, ACB)
    /// </summary>
    public string ShortName { get; set; } = string.Empty;

    /// <summary>
    /// Tên đầy đủ của ngân hàng
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã hệ thống hoặc mã chứng khoán / mã giao dịch (ví dụ: VCB, MB, CTG, TCB)
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// DTO lưu trữ cấu hình tài khoản ngân hàng thụ hưởng của Ký túc xá phục vụ thanh toán VietQR
/// </summary>
public class BankSettingsDto
{
    /// <summary>
    /// Mã BIN ngân hàng thụ hưởng (mặc định 970436 - Vietcombank)
    /// </summary>
    public string BankBin { get; set; } = "970436";

    /// <summary>
    /// Tên đầy đủ của ngân hàng thụ hưởng
    /// </summary>
    public string BankName { get; set; } = "Ngân hàng TMCP Ngoại thương Việt Nam";

    /// <summary>
    /// Tên viết tắt của ngân hàng thụ hưởng
    /// </summary>
    public string BankShortName { get; set; } = "Vietcombank";

    /// <summary>
    /// Số tài khoản ngân hàng thụ hưởng của Ký túc xá
    /// </summary>
    public string AccountNumber { get; set; } = "0123456789";

    /// <summary>
    /// Tên chủ tài khoản thụ hưởng (viết hoa không dấu hoặc có dấu)
    /// </summary>
    public string AccountHolder { get; set; } = "BAN QUAN LY KTX";

    /// <summary>
    /// Mẫu hiển thị mã QR VietQR (ví dụ: "compact", "compact2", "qr_only", "print")
    /// </summary>
    public string QrTemplate { get; set; } = "compact";

    /// <summary>
    /// Tiền tố trong nội dung chuyển khoản tự động (mặc định "KTX")
    /// </summary>
    public string TransferPrefix { get; set; } = "KTX";

    /// <summary>
    /// Cờ bật/tắt tính năng thanh toán quét mã VietQR trong toàn hệ thống
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// DTO đại diện cho dữ liệu mã QR VietQR đã được khởi tạo cho một hóa đơn
/// </summary>
public class VietQrPayloadDto
{
    /// <summary>
    /// Chuỗi dữ liệu mã QR theo tiêu chuẩn EMVCo QR Code Specification
    /// </summary>
    public string EmvCoPayload { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn hình ảnh mã QR VietQR QuickLink (ví dụ qua api https://img.vietqr.io/...)
    /// </summary>
    public string QuickLinkUrl { get; set; } = string.Empty;

    /// <summary>
    /// Mã BIN ngân hàng thụ hưởng
    /// </summary>
    public string BankBin { get; set; } = string.Empty;

    /// <summary>
    /// Số tài khoản thụ hưởng
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tên chủ tài khoản thụ hưởng
    /// </summary>
    public string AccountHolder { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền cần thanh toán theo hóa đơn (VNĐ)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Nội dung chuyển khoản chuẩn hóa (ví dụ: "KTX HD001 NGUYEN VAN A")
    /// </summary>
    public string TransferContent { get; set; } = string.Empty;

    /// <summary>
    /// Mã hóa đơn liên kết
    /// </summary>
    public string BillCode { get; set; } = string.Empty;
}
