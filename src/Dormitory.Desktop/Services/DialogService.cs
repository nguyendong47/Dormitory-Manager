using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Dormitory.Desktop.Views;

namespace Dormitory.Desktop.Services;

/// <summary>
/// Triển khai dịch vụ điều phối các cửa sổ modal dialog.
/// </summary>
public class DialogService : IDialogService
{
    private Window? GetMainWindow()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    /// <inheritdoc/>
    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => ShowConfirmAsync(title, message));
        }

        var dialog = new ConfirmDialogWindow(title, message);
        var mainWindow = GetMainWindow();

        if (mainWindow != null)
        {
            return await dialog.ShowDialog<bool>(mainWindow);
        }

        // Trường hợp chạy không có MainWindow, mở cửa sổ trực tiếp
        var tcs = new TaskCompletionSource<bool>();
        dialog.Closed += (_, _) => tcs.TrySetResult(dialog.Result);
        dialog.Show();
        return await tcs.Task;
    }

    /// <inheritdoc/>
    public async Task ShowMessageAsync(string title, string message)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            await Dispatcher.UIThread.InvokeAsync(() => ShowMessageAsync(title, message));
            return;
        }

        var dialog = new MessageDialogWindow(title, message);
        var mainWindow = GetMainWindow();

        if (mainWindow != null)
        {
            await dialog.ShowDialog(mainWindow);
            return;
        }

        // Trường hợp chạy không có MainWindow, mở cửa sổ trực tiếp
        var tcs = new TaskCompletionSource<bool>();
        dialog.Closed += (_, _) => tcs.TrySetResult(true);
        dialog.Show();
        await tcs.Task;
    }

    /// <inheritdoc/>
    public async Task<TResult?> ShowDialogAsync<TResult>(Window dialog)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            return await Dispatcher.UIThread.InvokeAsync(() => ShowDialogAsync<TResult>(dialog));
        }

        var mainWindow = GetMainWindow();
        if (mainWindow != null)
        {
            return await dialog.ShowDialog<TResult>(mainWindow);
        }

        throw new InvalidOperationException("Không tìm thấy cửa sổ chính (MainWindow) để hiển thị hộp thoại.");
    }
}
