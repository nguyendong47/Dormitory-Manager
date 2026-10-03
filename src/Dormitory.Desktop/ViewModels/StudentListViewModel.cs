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
/// ViewModel quản lý danh sách và tìm kiếm sinh viên kèm các thao tác CRUD
/// </summary>
public partial class StudentListViewModel : ViewModelBase
{
    private readonly IStudentService _studentService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<StudentDto> _students = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditStudentCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteStudentCommand))]
    private StudentDto? _selectedStudent;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public StudentListViewModel(IStudentService studentService, IDialogService dialogService)
    {
        _studentService = studentService;
        _dialogService = dialogService;
        LoadStudentsCommand = new AsyncRelayCommand(LoadStudentsAsync);
        SearchCommand = new AsyncRelayCommand(LoadStudentsAsync);
    }

    public IAsyncRelayCommand LoadStudentsCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }

    /// <summary>
    /// Điều kiện để kích hoạt chỉnh sửa hoặc xóa sinh viên
    /// </summary>
    private bool CanEditOrDeleteStudent => SelectedStudent != null;

    /// <summary>
    /// Mở hộp thoại thêm sinh viên mới
    /// </summary>
    [RelayCommand]
    public async Task AddStudentAsync()
    {
        var dialogVm = new StudentDialogViewModel(_studentService);
        var dialog = new StudentDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadStudentsAsync();
        }
    }

    /// <summary>
    /// Mở hộp thoại chỉnh sửa thông tin sinh viên đang được chọn
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteStudent))]
    public async Task EditStudentAsync()
    {
        if (SelectedStudent == null) return;

        var dialogVm = new StudentDialogViewModel(_studentService, SelectedStudent);
        var dialog = new StudentDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadStudentsAsync();
        }
    }

    /// <summary>
    /// Xóa sinh viên đang chọn sau khi người dùng xác nhận
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteStudent))]
    public async Task DeleteStudentAsync()
    {
        if (SelectedStudent == null) return;

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận xóa sinh viên",
            $"Bạn có chắc chắn muốn xóa hồ sơ sinh viên {SelectedStudent.FullName} (MSSV: {SelectedStudent.StudentCode}) không?");

        if (!confirmed) return;

        try
        {
            var success = await _studentService.DeleteStudentAsync(SelectedStudent.Id);
            if (success)
            {
                await LoadStudentsAsync();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không tìm thấy hồ sơ sinh viên cần xóa.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Tải danh sách sinh viên theo từ khóa tìm kiếm
    /// </summary>
    public async Task LoadStudentsAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _studentService.GetAllStudentsAsync(SearchQuery);
            Students = new ObservableCollection<StudentDto>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
