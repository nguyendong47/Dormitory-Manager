using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Giao diện danh sách quản lý nhân viên
/// </summary>
public partial class EmployeeListView : UserControl
{
    public EmployeeListView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Xử lý sự kiện nhấp đúp chuột vào dòng trong DataGrid để kích hoạt chỉnh sửa nhân viên
    /// </summary>
    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is EmployeeListViewModel vm && vm.SelectedEmployee != null)
        {
            if (vm.EditEmployeeCommand.CanExecute(null))
            {
                vm.EditEmployeeCommand.Execute(null);
            }
        }
    }
}
