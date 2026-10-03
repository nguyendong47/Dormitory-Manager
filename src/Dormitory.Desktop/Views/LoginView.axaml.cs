using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Code-behind cho LoginView
/// </summary>
public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Xử lý nhấn phím Enter để thực hiện đăng nhập
    /// </summary>
    private void OnInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
        {
            vm.LoginCommand.Execute(null);
        }
    }
}
