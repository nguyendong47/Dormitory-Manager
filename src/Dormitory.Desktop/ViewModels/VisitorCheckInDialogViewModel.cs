using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý hộp thoại đăng ký lượt khách vào thăm sinh viên KTX (Check-in)
/// </summary>
public partial class VisitorCheckInDialogViewModel : ViewModelBase
{
    private readonly IVisitorService _visitorService;
    private readonly IRoomService _roomService;
    private readonly IStudentService _studentService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _visitorName = string.Empty;

    [ObservableProperty]
    private string _identificationNumber = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _note = string.Empty;

    public ObservableCollection<RoomDto> Rooms { get; } = new();

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    public ObservableCollection<StudentDto> StudentsInRoom { get; } = new();

    [ObservableProperty]
    private StudentDto? _selectedStudent;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Action đóng dialog (kết nối với Avalonia Window code-behind)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Delegate bất đồng bộ hỗ trợ kiểm thử Headless Unit Test
    /// </summary>
    public Func<bool, Task>? CloseDialogHandler { get; set; }

    public VisitorCheckInDialogViewModel(
        IVisitorService visitorService,
        IRoomService roomService,
        IStudentService studentService,
        IDialogService dialogService)
    {
        _visitorService = visitorService ?? throw new ArgumentNullException(nameof(visitorService));
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        _ = LoadRoomsAsync();
    }

    /// <summary>
    /// Xử lý sự kiện khi phòng được chọn thay đổi, tự động tải danh sách sinh viên thuộc phòng đó
    /// </summary>
    async partial void OnSelectedRoomChanged(RoomDto? value)
    {
        StudentsInRoom.Clear();
        SelectedStudent = null;

        if (value == null)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var students = await _studentService.GetAllStudentsAsync(roomId: value.Id);
            foreach (var student in students)
            {
                StudentsInRoom.Add(student);
            }

            if (StudentsInRoom.Count > 0)
            {
                SelectedStudent = StudentsInRoom[0];
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải danh sách sinh viên: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Tải danh sách tất cả các phòng để người dùng lựa chọn
    /// </summary>
    [RelayCommand]
    public async Task LoadRoomsAsync()
    {
        try
        {
            IsLoading = true;
            Rooms.Clear();
            var rooms = await _roomService.GetAllRoomsAsync();
            foreach (var room in rooms)
            {
                Rooms.Add(room);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi tải danh sách phòng: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Xác nhận đăng ký check-in khách sau khi kiểm tra tính hợp lệ của dữ liệu
    /// </summary>
    [RelayCommand]
    public async Task ConfirmCheckInAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(VisitorName))
        {
            ErrorMessage = "Vui lòng nhập họ và tên khách.";
            await _dialogService.ShowMessageAsync("Thông báo", ErrorMessage);
            return;
        }

        if (string.IsNullOrWhiteSpace(IdentificationNumber))
        {
            ErrorMessage = "Vui lòng nhập số CCCD / CMND của khách.";
            await _dialogService.ShowMessageAsync("Thông báo", ErrorMessage);
            return;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ErrorMessage = "Vui lòng nhập số điện thoại liên hệ của khách.";
            await _dialogService.ShowMessageAsync("Thông báo", ErrorMessage);
            return;
        }

        if (SelectedRoom == null)
        {
            ErrorMessage = "Vui lòng chọn phòng cần đến thăm.";
            await _dialogService.ShowMessageAsync("Thông báo", ErrorMessage);
            return;
        }

        if (SelectedStudent == null)
        {
            ErrorMessage = "Vui lòng chọn sinh viên cần đến thăm.";
            await _dialogService.ShowMessageAsync("Thông báo", ErrorMessage);
            return;
        }

        try
        {
            IsLoading = true;

            var dto = new CreateVisitorLogDto
            {
                VisitorName = VisitorName.Trim(),
                IdentificationNumber = IdentificationNumber.Trim(),
                PhoneNumber = PhoneNumber.Trim(),
                RoomId = SelectedRoom.Id,
                StudentId = SelectedStudent.Id,
                Note = Note?.Trim() ?? string.Empty
            };

            await _visitorService.CheckInAsync(dto);

            CloseAction?.Invoke(true);
            if (CloseDialogHandler != null)
            {
                await CloseDialogHandler.Invoke(true);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi đăng ký khách: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", ErrorMessage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Hủy bỏ đăng ký và đóng hộp thoại
    /// </summary>
    [RelayCommand]
    public async Task CancelAsync()
    {
        CloseAction?.Invoke(false);
        if (CloseDialogHandler != null)
        {
            await CloseDialogHandler.Invoke(false);
        }
    }
}
