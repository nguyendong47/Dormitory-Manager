using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal hỗ trợ nhân viên đăng ký khách vào thăm sinh viên KTX (Check-in)
/// </summary>
public partial class VisitorCheckInDialogWindow : Window
{
    public VisitorCheckInDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public VisitorCheckInDialogWindow(VisitorCheckInDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        SetupViewModel(viewModel);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is VisitorCheckInDialogViewModel vm)
        {
            SetupViewModel(vm);
        }
    }

    private void SetupViewModel(VisitorCheckInDialogViewModel vm)
    {
        vm.CloseAction = result => Close(result);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
        {
            Close(false);
        }
    }
}
