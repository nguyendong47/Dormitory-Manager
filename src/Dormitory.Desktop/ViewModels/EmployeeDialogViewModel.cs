using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel cho cửa sổ hộp thoại Thêm mới hoặc Chỉnh sửa thông tin nhân viên
/// </summary>
public partial class EmployeeDialogViewModel : ViewModelBase
{
    private readonly IEmployeeService _employeeService;
    private readonly int? _employeeId;
    private readonly int? _userId;

    /// <summary>
    /// Cho biết đang ở chế độ chỉnh sửa hay thêm mới
    /// </summary>
    public bool IsEditMode => _employeeId.HasValue;

    [ObservableProperty]
    private string _title = "Thêm nhân viên mới";

    [ObservableProperty]
    private string _employeeCode = string.Empty;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _dateOfBirth = new DateTimeOffset(new DateTime(1990, 1, 1), TimeSpan.Zero);

    [ObservableProperty]
    private Gender _selectedGender = Gender.Male;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _identityCard = string.Empty;

    [ObservableProperty]
    private string _position = string.Empty;

    [ObservableProperty]
    private string _department = string.Empty;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Hành động đóng dialog trả về kết quả (true nếu lưu thành công, false nếu hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Danh sách giới tính cho ComboBox
    /// </summary>
    public IReadOnlyList<Gender> Genders => Enum.GetValues<Gender>();

    /// <summary>
    /// Khởi tạo cho chế độ Thêm mới nhân viên
    /// </summary>
    public EmployeeDialogViewModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
        _employeeId = null;
        _userId = null;
        Title = "Thêm nhân viên mới";
        DateOfBirth = new DateTimeOffset(new DateTime(1990, 1, 1), TimeSpan.Zero);
    }

    /// <summary>
    /// Khởi tạo cho chế độ Chỉnh sửa nhân viên hiện có
    /// </summary>
    public EmployeeDialogViewModel(IEmployeeService employeeService, EmployeeDto employee)
    {
        _employeeService = employeeService;
        _employeeId = employee.Id;
        _userId = employee.UserId;
        Title = $"Chỉnh sửa nhân viên {employee.EmployeeCode}";

        EmployeeCode = employee.EmployeeCode;
        FullName = employee.FullName;
        DateOfBirth = new DateTimeOffset(employee.DateOfBirth, TimeSpan.Zero);
        SelectedGender = employee.Gender;
        PhoneNumber = employee.PhoneNumber;
        IdentityCard = employee.IdentityCard;
        Position = employee.Position;
        Department = employee.Department;
        Address = employee.Address;
    }

    /// <summary>
    /// Lưu thông tin nhân viên (Thêm mới hoặc Cập nhật)
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        // 1. Kiểm tra validation bắt buộc
        if (string.IsNullOrWhiteSpace(EmployeeCode))
        {
            ErrorMessage = "Vui lòng nhập mã nhân viên.";
            return;
        }

        if (string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "Vui lòng nhập họ và tên nhân viên.";
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
        var request = new CreateOrUpdateEmployeeRequest
        {
            EmployeeCode = EmployeeCode.Trim(),
            FullName = FullName.Trim(),
            DateOfBirth = DateOfBirth.Value.DateTime,
            Gender = SelectedGender,
            PhoneNumber = PhoneNumber?.Trim() ?? string.Empty,
            IdentityCard = IdentityCard.Trim(),
            Position = Position?.Trim() ?? string.Empty,
            Department = Department?.Trim() ?? string.Empty,
            Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
            UserId = _userId
        };

        try
        {
            if (IsEditMode && _employeeId.HasValue)
            {
                var updated = await _employeeService.UpdateEmployeeAsync(_employeeId.Value, request);
                if (!updated)
                {
                    ErrorMessage = "Không tìm thấy hồ sơ nhân viên để cập nhật.";
                    return;
                }
            }
            else
            {
                await _employeeService.CreateEmployeeAsync(request);
            }

            // Đóng dialog với kết quả thành công
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            // Hiển thị thông báo lỗi (ví dụ: trùng mã nhân viên hoặc CCCD)
            ErrorMessage = ex.Message;
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
