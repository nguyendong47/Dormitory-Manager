using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý danh sách và tìm kiếm sinh viên
/// </summary>
public partial class StudentListViewModel : ViewModelBase
{
    private readonly IStudentService _studentService;

    [ObservableProperty]
    private ObservableCollection<StudentDto> _students = new();

    [ObservableProperty]
    private StudentDto? _selectedStudent;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public StudentListViewModel(IStudentService studentService)
    {
        _studentService = studentService;
        LoadStudentsCommand = new AsyncRelayCommand(LoadStudentsAsync);
        SearchCommand = new AsyncRelayCommand(LoadStudentsAsync);
    }

    public IAsyncRelayCommand LoadStudentsCommand { get; }
    public IAsyncRelayCommand SearchCommand { get; }

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
