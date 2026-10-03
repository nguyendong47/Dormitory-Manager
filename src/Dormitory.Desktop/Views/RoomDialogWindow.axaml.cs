using System;
using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Cửa sổ modal nhập liệu tạo mới và chỉnh sửa thông tin phòng ở ký túc xá.
/// </summary>
public partial class RoomDialogWindow : Window
{
    public RoomDialogWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    public RoomDialogWindow(RoomDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = result => Close(result);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is RoomDialogViewModel vm)
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
