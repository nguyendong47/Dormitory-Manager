using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal nhập liệu tạo mới và chỉnh sửa thông tin trang thiết bị, tài sản phòng ở ký túc xá.
/// </summary>
public partial class EquipmentDialogWindow : Window
{
    public EquipmentDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public EquipmentDialogWindow(EquipmentDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = result => Close(result);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is EquipmentDialogViewModel vm)
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
