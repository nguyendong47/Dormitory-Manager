using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal nhập liệu tạo mới và chỉnh sửa thông tin hồ sơ nhân viên
/// </summary>
public partial class EmployeeDialogWindow : Window
{
    public EmployeeDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public EmployeeDialogWindow(EmployeeDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = result => Close(result);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is EmployeeDialogViewModel vm)
        {
            vm.CloseAction = result => Close(result);
        }
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
