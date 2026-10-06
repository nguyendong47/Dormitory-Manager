using System.Text.Encodings.Web;
using System.Text.Json;
using Dormitory.Application.Common;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Infrastructure.Data;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ quản lý cấu hình tài khoản ngân hàng KTX phục vụ tạo mã VietQR
/// Lưu trữ bền vững dưới tệp JSON và có cơ chế in-memory caching
/// </summary>
public class BankSettingsService : IBankSettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private BankSettingsDto? _cachedSettings;

    /// <summary>
    /// Khởi tạo BankSettingsService. Cho phép truyền đường dẫn tùy chỉnh để phục vụ kiểm thử đơn vị.
    /// </summary>
    /// <param name="customFilePath">Đường dẫn tệp cấu hình JSON (nếu null sẽ dùng DatabasePathResolver)</param>
    public BankSettingsService(string? customFilePath = null)
    {
        _filePath = string.IsNullOrWhiteSpace(customFilePath)
            ? DatabasePathResolver.ResolveDatabasePath("banksettings.json")
            : customFilePath;
    }

    /// <summary>
    /// Tạo cấu hình ngân hàng mặc định cho Ký túc xá (Vietcombank)
    /// </summary>
    public static BankSettingsDto CreateDefaultSettings() => new()
    {
        BankBin = "970436",
        BankName = "Ngân hàng TMCP Ngoại thương Việt Nam",
        BankShortName = "Vietcombank",
        AccountNumber = "0123456789",
        AccountHolder = "BAN QUAN LY KTX",
        QrTemplate = "compact",
        TransferPrefix = "KTX",
        IsEnabled = true
    };

    /// <summary>
    /// Lấy cấu hình ngân hàng hiện tại (đọc từ cache hoặc nạp từ file JSON; fallback sang mặc định nếu chưa có)
    /// </summary>
    public async Task<BankSettingsDto> GetBankSettingsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_cachedSettings != null)
            {
                return Clone(_cachedSettings);
            }

            if (File.Exists(_filePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    var settings = JsonSerializer.Deserialize<BankSettingsDto>(json, JsonOptions);
                    if (settings != null)
                    {
                        _cachedSettings = settings;
                        return Clone(settings);
                    }
                }
                catch
                {
                    // Trường hợp tệp bị hỏng, fallback về mặc định
                }
            }

            // Nếu tệp chưa tồn tại hoặc rỗng, trả về cấu hình mặc định
            var defaultSettings = CreateDefaultSettings();
            _cachedSettings = defaultSettings;
            return Clone(defaultSettings);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Lưu cấu hình tài khoản ngân hàng mới vào tệp JSON và cập nhật bộ nhớ cache
    /// </summary>
    public async Task<bool> SaveBankSettingsAsync(BankSettingsDto settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        await _semaphore.WaitAsync();
        try
        {
            // Tự động điền BankName / BankShortName từ danh mục nếu đang trống
            if (string.IsNullOrWhiteSpace(settings.BankName) || string.IsNullOrWhiteSpace(settings.BankShortName))
            {
                var bankInfo = VietQrBankDirectory.FindByBin(settings.BankBin);
                if (bankInfo != null)
                {
                    if (string.IsNullOrWhiteSpace(settings.BankName))
                        settings.BankName = bankInfo.Name;
                    if (string.IsNullOrWhiteSpace(settings.BankShortName))
                        settings.BankShortName = bankInfo.ShortName;
                }
            }

            // Chuẩn hóa dữ liệu
            var normalized = new BankSettingsDto
            {
                BankBin = settings.BankBin?.Trim() ?? string.Empty,
                BankName = settings.BankName?.Trim() ?? string.Empty,
                BankShortName = settings.BankShortName?.Trim() ?? string.Empty,
                AccountNumber = settings.AccountNumber?.Trim() ?? string.Empty,
                AccountHolder = settings.AccountHolder?.Trim().ToUpperInvariant() ?? string.Empty,
                QrTemplate = string.IsNullOrWhiteSpace(settings.QrTemplate) ? "compact" : settings.QrTemplate.Trim(),
                TransferPrefix = string.IsNullOrWhiteSpace(settings.TransferPrefix) ? "KTX" : settings.TransferPrefix.Trim(),
                IsEnabled = settings.IsEnabled
            };

            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(normalized, JsonOptions);
            await File.WriteAllTextAsync(_filePath, json);

            _cachedSettings = normalized;
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Lấy danh mục tất cả ngân hàng hỗ trợ chuẩn Napas / VietQR
    /// </summary>
    public Task<IReadOnlyList<BankInfoDto>> GetSupportedBanksAsync()
    {
        return Task.FromResult(VietQrBankDirectory.GetSupportedBanks());
    }

    private static BankSettingsDto Clone(BankSettingsDto source) => new()
    {
        BankBin = source.BankBin,
        BankName = source.BankName,
        BankShortName = source.BankShortName,
        AccountNumber = source.AccountNumber,
        AccountHolder = source.AccountHolder,
        QrTemplate = source.QrTemplate,
        TransferPrefix = source.TransferPrefix,
        IsEnabled = source.IsEnabled
    };
}
