using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty]
    private DatabaseInfoDto _databaseInfo = new();

    [ObservableProperty]
    private bool _isLoading;

    public bool IsAdmin => _userSession.IsAdmin;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public SystemSettingsViewModel(
        IDatabaseService databaseService,
        IFileService fileService,
        IDialogService dialogService,
        IUserSession userSession)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));

        _userSession.SessionChanged += () => OnPropertyChanged(nameof(IsAdmin));
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
                await _dialogService.ShowMessageAsync("Phục hồi thành công", "Đã khôi phục cơ sở dữ liệu thành công! Dữ liệu đã sẵn sàng.");
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
}
