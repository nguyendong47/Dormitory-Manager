using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel cho cửa sổ hộp thoại Thêm mới hoặc Chỉnh sửa thông tin sinh viên
/// </summary>
public partial class StudentDialogViewModel : ViewModelBase
{
    private readonly IStudentService _studentService;
    private readonly IRoomService? _roomService;
    private readonly int? _studentId;

    /// <summary>
    /// Cho biết đang ở chế độ chỉnh sửa hay thêm mới
    /// </summary>
    public bool IsEditMode => _studentId.HasValue;

    [ObservableProperty]
    private string _title = "Thêm sinh viên mới";

    [ObservableProperty]
    private string _studentCode = string.Empty;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _dateOfBirth = new DateTimeOffset(new DateTime(2004, 1, 1), TimeSpan.Zero);

    [ObservableProperty]
    private Gender _selectedGender = Gender.Male;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string? _email;

    [ObservableProperty]
    private string _identityCard = string.Empty;

    [ObservableProperty]
    private string _className = string.Empty;

    [ObservableProperty]
    private string _faculty = string.Empty;

    [ObservableProperty]
    private string _hometown = string.Empty;

    /// <summary>
    /// Thuộc tính bí danh đồng bộ với HomeTown của DTO
    /// </summary>
    public string HomeTown
    {
        get => Hometown;
        set => Hometown = value;
    }

    [ObservableProperty]
    private string? _parentName;

    [ObservableProperty]
    private string? _parentPhoneNumber;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private int? _selectedRoomId;

    [ObservableProperty]
    private ObservableCollection<RoomDto> _availableRooms = new();

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    /// <summary>
    /// Hành động đóng dialog trả về kết quả (true nếu lưu thành công, false nếu hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Danh sách giới tính cho ComboBox
    /// </summary>
    public IReadOnlyList<Gender> Genders => Enum.GetValues<Gender>();

    /// <summary>
    /// Khởi tạo cho chế độ Thêm mới sinh viên
    /// </summary>
    public StudentDialogViewModel(IStudentService studentService, IRoomService? roomService = null)
    {
        _studentService = studentService;
        _roomService = roomService;
        _studentId = null;
        Title = "Thêm sinh viên mới";
        DateOfBirth = new DateTimeOffset(new DateTime(2004, 1, 1), TimeSpan.Zero);
    }

    /// <summary>
    /// Khởi tạo cho chế độ Chỉnh sửa sinh viên hiện có
    /// </summary>
    public StudentDialogViewModel(IStudentService studentService, StudentDto student, IRoomService? roomService = null)
    {
        _studentService = studentService;
        _roomService = roomService;
        _studentId = student.Id;
        Title = $"Chỉnh sửa sinh viên {student.StudentCode}";

        StudentCode = student.StudentCode;
        FullName = student.FullName;
        DateOfBirth = new DateTimeOffset(student.DateOfBirth, TimeSpan.Zero);
        SelectedGender = student.Gender;
        IdentityCard = student.IdentityCard;
        PhoneNumber = student.PhoneNumber;
        Email = student.Email;
        Hometown = student.HomeTown;
        ClassName = student.ClassName;
        Faculty = student.Faculty;
        ParentName = student.ParentName;
        ParentPhoneNumber = student.ParentPhoneNumber;
        Address = student.Address;
        SelectedRoomId = student.CurrentRoomId;
    }

    /// <summary>
    /// Lưu thông tin sinh viên (Thêm mới hoặc Cập nhật)
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // 1. Kiểm tra validation bắt buộc
            if (string.IsNullOrWhiteSpace(StudentCode))
            {
                ErrorMessage = "Vui lòng nhập mã sinh viên.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Vui lòng nhập họ và tên sinh viên.";
                return;
            }

            if (string.IsNullOrWhiteSpace(IdentityCard))
            {
                ErrorMessage = "Vui lòng nhập số CCCD/CMND.";
                return;
            }

            if (!DateOfBirth.HasValue)
            {
                ErrorMessage = "Vui lòng chọn ngày sinh.";
                return;
            }

            ErrorMessage = null;

            // 2. Chuẩn bị request dữ liệu
            var request = new CreateOrUpdateStudentRequest
            {
                StudentCode = StudentCode.Trim(),
                FullName = FullName.Trim(),
                DateOfBirth = DateOfBirth.Value.DateTime,
                Gender = SelectedGender,
                IdentityCard = IdentityCard.Trim(),
                PhoneNumber = PhoneNumber?.Trim() ?? string.Empty,
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                HomeTown = Hometown?.Trim() ?? string.Empty,
                ClassName = ClassName?.Trim() ?? string.Empty,
                Faculty = Faculty?.Trim() ?? string.Empty,
                ParentName = ParentName?.Trim() ?? string.Empty,
                ParentPhoneNumber = ParentPhoneNumber?.Trim() ?? string.Empty,
                Address = Address?.Trim()
            };

            if (IsEditMode && _studentId.HasValue)
            {
                var updated = await _studentService.UpdateStudentAsync(_studentId.Value, request);
                if (!updated)
                {
                    ErrorMessage = "Không tìm thấy hồ sơ sinh viên để cập nhật.";
                    return;
                }
            }
            else
            {
                await _studentService.CreateStudentAsync(request);
            }

            // Đóng dialog với kết quả thành công
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            // Hiển thị thông báo lỗi (ví dụ: trùng MSSV hoặc CCCD)
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Hủy bỏ và đóng hộp thoại
    /// </summary>
    [RelayCommand]
    public void Cancel()
    {
        CloseAction?.Invoke(false);
    }
}
