using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.Views;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý danh sách và tìm kiếm nhân viên kèm các thao tác CRUD
/// </summary>
public partial class EmployeeListViewModel : ViewModelBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<EmployeeDto> _employees = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditEmployeeCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteEmployeeCommand))]
    private EmployeeDto? _selectedEmployee;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string? _selectedDepartment;

    [ObservableProperty]
    private ObservableCollection<string> _departments = new()
    {
        "Tất cả phòng ban",
        "Ban Quản lý Ký túc xá",
        "Tổ Kỹ thuật - Bảo trì",
        "Tổ Bảo vệ",
        "Tổ Vệ sinh",
        "Phòng Kế toán"
    };

    [ObservableProperty]
    private bool _isLoading;

    public EmployeeListViewModel(IEmployeeService employeeService, IDialogService dialogService)
    {
        _employeeService = employeeService;
        _dialogService = dialogService;
        _selectedDepartment = "Tất cả phòng ban";

        LoadEmployeesCommand = new AsyncRelayCommand(LoadEmployeesAsync);
        SearchCommand = new AsyncRelayCommand(LoadEmployeesAsync);
    }

    public IAsyncRelayCommand LoadEmployeesCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }

    /// <summary>
    /// Điều kiện để kích hoạt chỉnh sửa hoặc xóa nhân viên
    /// </summary>
    private bool CanEditOrDeleteEmployee => SelectedEmployee != null;

    /// <summary>
    /// Xử lý khi phòng ban được chọn thay đổi
    /// </summary>
    partial void OnSelectedDepartmentChanged(string? value)
    {
        _ = LoadEmployeesAsync();
    }

    /// <summary>
    /// Mở hộp thoại thêm nhân viên mới
    /// </summary>
    [RelayCommand]
    public async Task AddEmployeeAsync()
    {
        var dialogVm = new EmployeeDialogViewModel(_employeeService);
        var dialog = new EmployeeDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadEmployeesAsync();
        }
    }

    /// <summary>
    /// Mở hộp thoại chỉnh sửa thông tin nhân viên đang được chọn
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteEmployee))]
    public async Task EditEmployeeAsync()
    {
        if (SelectedEmployee == null) return;

        var dialogVm = new EmployeeDialogViewModel(_employeeService, SelectedEmployee);
        var dialog = new EmployeeDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadEmployeesAsync();
        }
    }

    /// <summary>
    /// Xóa nhân viên đang chọn sau khi người dùng xác nhận
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteEmployee))]
    public async Task DeleteEmployeeAsync()
    {
        if (SelectedEmployee == null) return;

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận xóa nhân viên",
            $"Bạn có chắc chắn muốn xóa hồ sơ nhân viên {SelectedEmployee.FullName} (Mã NV: {SelectedEmployee.EmployeeCode}) không?");

        if (!confirmed) return;

        try
        {
            var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.Id);
            if (success)
            {
                await LoadEmployeesAsync();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không tìm thấy hồ sơ nhân viên cần xóa.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Tải danh sách nhân viên theo từ khóa tìm kiếm và bộ lọc phòng ban
    /// </summary>
    public async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        try
        {
            var deptFilter = string.IsNullOrWhiteSpace(SelectedDepartment) || SelectedDepartment == "Tất cả phòng ban"
                ? null
                : SelectedDepartment;

            var list = await _employeeService.GetAllEmployeesAsync(SearchQuery, deptFilter);
            Employees = new ObservableCollection<EmployeeDto>(list);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tải danh sách nhân viên: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
