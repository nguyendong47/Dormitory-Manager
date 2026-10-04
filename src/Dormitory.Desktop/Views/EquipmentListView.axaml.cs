using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Giao diện danh sách trang thiết bị, tài sản phòng ở kèm bảng thống kê và công cụ tìm kiếm
/// </summary>
public partial class EquipmentListView : UserControl
{
    public EquipmentListView()
    {
        InitializeComponent();
    }

    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is EquipmentListViewModel vm && vm.SelectedEquipment != null)
        {
            _ = vm.EditEquipmentAsync();
        }
    }
}
