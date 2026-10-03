using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal thông báo cho người dùng.
/// </summary>
public partial class MessageDialogWindow : Window
{
    public MessageDialogWindow()
    {
        InitializeComponent();
    }

    public MessageDialogWindow(string title, string message) : this()
    {
        Title = title;
        TitleTextBlock.Text = title;
        MessageTextBlock.Text = message;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape || e.Key == Key.Enter)
        {
            Close();
        }
    }
}
