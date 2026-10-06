using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý cấu hình tài khoản ngân hàng ký túc xá cho phân hệ thanh toán VietQR
/// </summary>
public interface IBankSettingsService
{
    /// <summary>
    /// Lấy cấu hình tài khoản ngân hàng hiện tại của ký túc xá
    /// </summary>
    Task<BankSettingsDto> GetBankSettingsAsync();

    /// <summary>
    /// Lưu cấu hình tài khoản ngân hàng mới
    /// </summary>
    Task<bool> SaveBankSettingsAsync(BankSettingsDto settings);

    /// <summary>
    /// Lấy danh sách các ngân hàng hỗ trợ chuẩn VietQR/Napas
    /// </summary>
    Task<IReadOnlyList<BankInfoDto>> GetSupportedBanksAsync();
}
