using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Giao diện danh sách quản lý phòng ở
/// </summary>
public partial class RoomListView : UserControl
{
    public RoomListView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Xử lý sự kiện nhấp đúp chuột vào dòng trong DataGrid để kích hoạt chỉnh sửa phòng
    /// </summary>
    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is RoomListViewModel vm && vm.SelectedRoom != null)
        {
            if (vm.EditRoomCommand.CanExecute(null))
            {
                vm.EditRoomCommand.Execute(null);
            }
        }
    }
}
