using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal hiển thị mã thanh toán VietQR động (NAPAS 247) tại quầy thu ngân.
/// </summary>
public partial class VietQrDialogWindow : Window
{
    public VietQrDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public VietQrDialogWindow(VietQrDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        SetupViewModel(viewModel);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is VietQrDialogViewModel vm)
        {
            SetupViewModel(vm);
        }
    }

    private void SetupViewModel(VietQrDialogViewModel vm)
    {
        vm.CloseAction = result => Close(result);
        vm.CopyToClipboardAction = async text =>
        {
            if (Clipboard != null)
            {
                await Clipboard.SetTextAsync(text);
            }
        };
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            if (DataContext is VietQrDialogViewModel vm)
            {
                Close(vm.IsPaid);
            }
            else
            {
                Close(false);
            }
        }
    }
}
