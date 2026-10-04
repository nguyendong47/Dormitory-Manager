using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Giao diện danh sách biên bản vi phạm kỷ luật KTX kèm bảng thống kê KPI và bộ lọc tìm kiếm
/// </summary>
public partial class ViolationListView : UserControl
{
    public ViolationListView()
    {
        InitializeComponent();
    }

    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is ViolationListViewModel vm && vm.SelectedViolation != null)
        {
            _ = vm.EditViolationAsync();
        }
    }
}
