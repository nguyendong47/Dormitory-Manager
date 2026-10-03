using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal nhập liệu tạo mới và chỉnh sửa thông tin hồ sơ sinh viên
/// </summary>
public partial class StudentDialogWindow : Window
{
    public StudentDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public StudentDialogWindow(StudentDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = result => Close(result);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is StudentDialogViewModel vm)
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
