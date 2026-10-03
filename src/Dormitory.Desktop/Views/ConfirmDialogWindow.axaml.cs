using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal xác nhận thao tác của người dùng.
/// </summary>
public partial class ConfirmDialogWindow : Window
{
    /// <summary>
    /// Kết quả xác nhận (true: Đồng ý, false: Hủy).
    /// </summary>
    public bool Result { get; private set; }

    public ConfirmDialogWindow()
    {
        InitializeComponent();
    }

    public ConfirmDialogWindow(string title, string message) : this()
    {
        Title = title;
        TitleTextBlock.Text = title;
        MessageTextBlock.Text = message;
    }

    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        Result = true;
        Close(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Result = false;
        Close(false);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            Result = false;
            Close(false);
        }
    }
}
