using Avalonia.Controls;
using Avalonia.Input;
using Dormitory.Desktop.ViewModels;

namespace Dormitory.Desktop.Views;

/// <summary>
/// Giao diện danh sách quản lý sinh viên
/// </summary>
public partial class StudentListView : UserControl
{
    public StudentListView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Xử lý sự kiện nhấp đúp chuột vào dòng trong DataGrid để kích hoạt chỉnh sửa sinh viên
    /// </summary>
    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is StudentListViewModel vm && vm.SelectedStudent != null)
        {
            if (vm.EditStudentCommand.CanExecute(null))
            {
                vm.EditStudentCommand.Execute(null);
            }
        }
    }
}
