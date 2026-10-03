using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace Dormitory.Desktop.Services;

/// <summary>
/// Triển khai dịch vụ lưu file và hiển thị hộp thoại chọn file của Avalonia UI.
/// </summary>
public class FileService : IFileService
{
    private readonly IDialogService _dialogService;

    public FileService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    private static Window? GetMainWindow()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    /// <inheritdoc/>
    public async Task<bool> SaveFileAsync(string defaultFileName, string extension, string fileTypeFilter, byte[] content)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => SaveFileAsync(defaultFileName, extension, fileTypeFilter, content));
        }

        var mainWindow = GetMainWindow();
        if (mainWindow == null)
        {
            await _dialogService.ShowMessageAsync("Lỗi", "Không tìm thấy cửa sổ chính để mở hộp thoại lưu file.");
            return false;
        }

        var cleanExtension = extension.TrimStart('.');
        var options = new FilePickerSaveOptions
        {
            Title = "Lưu file",
            SuggestedFileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.{cleanExtension}",
            DefaultExtension = cleanExtension,
            FileTypeChoices = new[]
            {
                new FilePickerFileType(fileTypeFilter)
                {
                    Patterns = new[] { $"*.{cleanExtension}" }
                }
            }
        };

        var file = await mainWindow.StorageProvider.SaveFilePickerAsync(options);
        if (file == null)
        {
            // Người dùng nhấn Hủy (Cancel)
            return false;
        }

        try
        {
            await using var stream = await file.OpenWriteAsync();
            await stream.WriteAsync(content);
            await _dialogService.ShowMessageAsync("Thành công", "Xuất file Excel thành công!");
            return true;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", $"Lỗi khi lưu file: {ex.Message}");
            return false;
        }
    }
}
