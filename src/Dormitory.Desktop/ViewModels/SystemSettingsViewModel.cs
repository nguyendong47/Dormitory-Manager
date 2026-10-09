using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.Common;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý cài đặt hệ thống và quản trị cơ sở dữ liệu SQLite: sao lưu, phục hồi và kiểm tra thông tin CSDL
/// </summary>
public partial class SystemSettingsViewModel : ViewModelBase
{
    private readonly IDatabaseService _databaseService;
    private readonly IFileService _fileService;
    private readonly IDialogService _dialogService;
    private readonly IUserSession _userSession;
    private readonly IEmailService _emailService;
    private readonly IBankSettingsService _bankSettingsService;
    private readonly IVietQrService _vietQrService;

    [ObservableProperty]
    private DatabaseInfoDto _databaseInfo = new();

    [ObservableProperty]
    private EmailSettingsDto _emailSettings = new();

    [ObservableProperty]
    private BankSettingsDto _bankSettings = new();

    public ObservableCollection<BankInfoDto> AvailableBanks { get; } = new(VietQrBankDirectory.GetAllBanks());

    [ObservableProperty]
    private BankInfoDto? _selectedBank;

    public ObservableCollection<string> AvailableTemplates { get; } = new()
    {
        "compact",
        "compact2",
        "qr_only",
        "print"
    };

    [ObservableProperty]
    private string _bankStatusMessage = string.Empty;

    [ObservableProperty]
    private Bitmap? _previewQrBitmap;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmailTesting;

    public bool IsAdmin => _userSession.IsAdmin;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _emailStatusMessage = string.Empty;

    public SystemSettingsViewModel(
        IDatabaseService databaseService,
        IFileService fileService,
        IDialogService dialogService,
        IUserSession userSession,
        IEmailService emailService,
        IBankSettingsService bankSettingsService,
        IVietQrService vietQrService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _bankSettingsService = bankSettingsService ?? throw new ArgumentNullException(nameof(bankSettingsService));
        _vietQrService = vietQrService ?? throw new ArgumentNullException(nameof(vietQrService));

        _userSession.SessionChanged += () => OnPropertyChanged(nameof(IsAdmin));

        _ = LoadEmailSettingsAsync();
        _ = LoadBankSettingsAsync();
    }

    /// <summary>
    /// Xử lý cập nhật thông tin ngân hàng trong cấu hình khi người dùng chọn ngân hàng khác
    /// </summary>
    partial void OnSelectedBankChanged(BankInfoDto? value)
    {
        if (value != null && BankSettings != null)
        {
            BankSettings.BankBin = value.Bin;
            BankSettings.BankName = value.Name;
            BankSettings.BankShortName = value.ShortName;
        }
    }

    /// <summary>
    /// Tải thông tin thống kê tổng quan của tệp tin CSDL SQLite
    /// </summary>
    [RelayCommand]
    public async Task LoadDatabaseInfoAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang tải thông tin cơ sở dữ liệu...";
            DatabaseInfo = await _databaseService.GetDatabaseInfoAsync();
            StatusMessage = "Đã cập nhật thông tin cơ sở dữ liệu.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi khi tải thông tin CSDL: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tải thông tin CSDL: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Alias làm mới thông tin CSDL
    /// </summary>
    [RelayCommand]
    public Task RefreshDbInfoAsync() => LoadDatabaseInfoAsync();

    /// <summary>
    /// Tạo bản snapshot sao lưu CSDL và lưu ra tệp .bak/.db thông qua File Dialog
    /// </summary>
    [RelayCommand]
    public async Task BackupAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang tạo bản sao lưu cơ sở dữ liệu...";
            byte[] backupBytes = await _databaseService.BackupDatabaseAsync();
            bool saved = await _fileService.SaveFileAsync("dormitory_backup", "bak", "Backup Files (*.bak;*.db)", backupBytes);
            if (saved)
            {
                StatusMessage = "Sao lưu cơ sở dữ liệu thành công.";
            }
            else
            {
                StatusMessage = "Đã hủy thao tác sao lưu.";
            }
            await LoadDatabaseInfoAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi sao lưu: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi sao lưu", $"Không thể sao lưu cơ sở dữ liệu: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Phục hồi cơ sở dữ liệu từ tệp sao lưu được chọn, yêu cầu quyền Quản trị viên (Admin)
    /// </summary>
    [RelayCommand]
    public async Task RestoreAsync()
    {
        if (!IsAdmin)
        {
            await _dialogService.ShowMessageAsync("Từ chối quyền truy cập", "Chỉ Quản trị viên (Admin) mới có quyền phục hồi cơ sở dữ liệu.");
            return;
        }

        try
        {
            var bytes = await _fileService.OpenFileAsync("Chọn tệp sao lưu CSDL", new[] { "bak", "db" });
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            var confirm = await _dialogService.ShowConfirmAsync(
                "⚠️ XÁC NHẬN PHỤC HỒI DỮ LIỆU",
                "CẢNH BÁO: Toàn bộ dữ liệu hiện tại trong hệ thống sẽ được thay thế bằng dữ liệu từ tệp sao lưu này.\n\nBạn có chắc chắn muốn tiếp tục?");

            if (!confirm)
            {
                return;
            }

            IsLoading = true;
            StatusMessage = "Đang kiểm tra và phục hồi cơ sở dữ liệu...";

            bool success = await _databaseService.RestoreDatabaseAsync(bytes);
            if (success)
            {
                StatusMessage = "Phục hồi cơ sở dữ liệu thành công.";
                await _dialogService.ShowMessageAsync("Phục hồi thành công", "Đã khôi phục cơ sở dữ liệu thành công! Dữ liệu đã sẵn sàng. Vui lòng chuyển tab hoặc khởi động lại ứng dụng để làm mới toàn bộ dữ liệu.");
                await LoadDatabaseInfoAsync();
            }
            else
            {
                StatusMessage = "Phục hồi cơ sở dữ liệu thất bại.";
                await _dialogService.ShowMessageAsync("Lỗi phục hồi", "Tệp sao lưu không hợp lệ hoặc bị lỗi toàn vẹn dữ liệu. CSDL hiện tại vẫn được giữ nguyên.");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi phục hồi: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi phục hồi", $"Có lỗi xảy ra trong quá trình phục hồi: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Tải thông số cấu hình máy chủ SMTP hiện tại
    /// </summary>
    [RelayCommand]
    public async Task LoadEmailSettingsAsync()
    {
        try
        {
            EmailSettings = await _emailService.GetEmailSettingsAsync();
            EmailStatusMessage = "Đã tải cấu hình email SMTP.";
        }
        catch (Exception ex)
        {
            EmailStatusMessage = $"Lỗi khi tải cấu hình email: {ex.Message}";
        }
    }

    /// <summary>
    /// Lưu thông số cấu hình máy chủ SMTP vào hệ thống
    /// </summary>
    [RelayCommand]
    public async Task SaveEmailSettingsAsync()
    {
        try
        {
            IsLoading = true;
            var success = await _emailService.SaveEmailSettingsAsync(EmailSettings);
            if (success)
            {
                EmailStatusMessage = "Lưu cấu hình email SMTP thành công.";
                await _dialogService.ShowMessageAsync("Thành công", "Đã lưu thông số cấu hình máy chủ gửi email SMTP thành công!");
            }
            else
            {
                EmailStatusMessage = "Lỗi khi lưu tệp cấu hình email.";
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể lưu tệp cấu hình email.");
            }
        }
        catch (Exception ex)
        {
            EmailStatusMessage = $"Lỗi: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Lỗi khi lưu cấu hình email: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Thử nghiệm kết nối tới máy chủ SMTP theo cấu hình hiện tại
    /// </summary>
    [RelayCommand]
    public async Task TestSmtpConnectionAsync()
    {
        try
        {
            IsEmailTesting = true;
            EmailStatusMessage = "Đang kiểm tra kết nối tới máy chủ SMTP...";

            var result = await _emailService.TestSmtpConnectionAsync(EmailSettings);
            if (result.Success)
            {
                EmailStatusMessage = "Kết nối máy chủ SMTP thành công!";
                await _dialogService.ShowMessageAsync("Kết nối thành công", "Kiểm tra kết nối tới máy chủ gửi thư SMTP thành công! Hệ thống đã sẵn sàng gửi email thông báo hóa đơn.");
            }
            else
            {
                EmailStatusMessage = $"Kết nối thất bại: {result.ErrorMessage}";
                await _dialogService.ShowMessageAsync("Kết nối thất bại", $"Không thể kết nối tới máy chủ SMTP:\n\n{result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            EmailStatusMessage = $"Lỗi kết nối: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi kết nối", $"Lỗi kết nối tới máy chủ SMTP: {ex.Message}");
        }
        finally
        {
            IsEmailTesting = false;
        }
    }

    /// <summary>
    /// Tải thông số cấu hình tài khoản ngân hàng thụ hưởng và VietQR hiện tại
    /// </summary>
    [RelayCommand]
    public async Task LoadBankSettingsAsync()
    {
        try
        {
            BankSettings = await _bankSettingsService.GetBankSettingsAsync();
            if (BankSettings != null && !string.IsNullOrEmpty(BankSettings.BankBin))
            {
                SelectedBank = AvailableBanks.FirstOrDefault(b => b.Bin == BankSettings.BankBin);
            }
            BankStatusMessage = "Đã tải cấu hình tài khoản ngân hàng và VietQR.";
        }
        catch (Exception ex)
        {
            BankStatusMessage = $"Lỗi khi tải cấu hình ngân hàng: {ex.Message}";
        }
    }

    /// <summary>
    /// Lưu thông số cấu hình tài khoản ngân hàng thụ hưởng và VietQR vào hệ thống
    /// </summary>
    [RelayCommand]
    public async Task SaveBankSettingsAsync()
    {
        try
        {
            IsLoading = true;
            BankStatusMessage = "Đang lưu cấu hình ngân hàng...";
            var success = await _bankSettingsService.SaveBankSettingsAsync(BankSettings);
            if (success)
            {
                BankStatusMessage = "Lưu cấu hình tài khoản ngân hàng thành công.";
                await _dialogService.ShowMessageAsync("Thành công", "Đã lưu cấu hình tài khoản ngân hàng thụ hưởng và VietQR thành công!");
            }
            else
            {
                BankStatusMessage = "Lỗi khi lưu tệp cấu hình ngân hàng.";
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể lưu tệp cấu hình ngân hàng.");
            }
        }
        catch (Exception ex)
        {
            BankStatusMessage = $"Lỗi: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Lỗi khi lưu cấu hình ngân hàng: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Sinh mã QR thử nghiệm với số tiền mẫu 100.000 đ và nội dung kiểm tra
    /// </summary>
    [RelayCommand]
    public async Task TestGenerateQrAsync()
    {
        try
        {
            BankStatusMessage = "Đang tạo mã QR thử nghiệm...";
            var prefix = !string.IsNullOrWhiteSpace(BankSettings.TransferPrefix) ? BankSettings.TransferPrefix.Trim() : "KTX";
            var testContent = $"{prefix} TEST".Trim();

            var payload = _vietQrService.GeneratePayload(
                BankSettings.BankBin,
                BankSettings.AccountNumber,
                BankSettings.AccountHolder,
                100000m,
                testContent,
                "TEST",
                BankSettings.QrTemplate);

            var qrBytes = _vietQrService.GenerateQrCodePng(payload.EmvCoPayload, 10);
            if (qrBytes != null && qrBytes.Length > 0)
            {
                try
                {
                    using var stream = new MemoryStream(qrBytes);
                    PreviewQrBitmap = new Bitmap(stream);
                }
                catch
                {
                    PreviewQrBitmap = null;
                }
                BankStatusMessage = "Đã tạo mã QR thử nghiệm thành công (100.000 đ)!";
                await _dialogService.ShowMessageAsync("Thành công", "Sinh mã QR VietQR thử nghiệm thành công! Bạn có thể xem ảnh xem trước bên dưới.");
            }
        }
        catch (Exception ex)
        {
            BankStatusMessage = $"Lỗi tạo mã QR: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tạo mã QR thử nghiệm: {ex.Message}");
        }
    }
}
