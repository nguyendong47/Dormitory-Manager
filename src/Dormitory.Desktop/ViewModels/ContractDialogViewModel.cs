using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel cho cửa sổ hộp thoại Lập hợp đồng thuê phòng KTX mới
/// </summary>
public partial class ContractDialogViewModel : ViewModelBase
{
    private readonly IContractService _contractService;
    private readonly IStudentService _studentService;
    private readonly IRoomService _roomService;

    [ObservableProperty]
    private string _title = "Lập hợp đồng thuê phòng mới";

    [ObservableProperty]
    private ObservableCollection<StudentDto> _students = new();

    [ObservableProperty]
    private StudentDto? _selectedStudent;

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    [ObservableProperty]
    private DateTimeOffset? _startDate = new DateTimeOffset(DateTime.Today);

    [ObservableProperty]
    private DateTimeOffset? _endDate = new DateTimeOffset(DateTime.Today.AddMonths(6));

    [ObservableProperty]
    private decimal _monthlyRate;

    [ObservableProperty]
    private decimal _depositAmount;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Hành động đóng dialog trả về kết quả (true nếu tạo thành công, false nếu hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    public ContractDialogViewModel(
        IContractService contractService,
        IStudentService studentService,
        IRoomService roomService)
    {
        _contractService = contractService;
        _studentService = studentService;
        _roomService = roomService;
    }

    /// <summary>
    /// Khi người dùng chọn một phòng, tự động cập nhật đơn giá thuê tháng theo giá niêm yết của phòng đó
    /// </summary>
    partial void OnSelectedRoomChanged(RoomDto? value)
    {
        if (value != null)
        {
            MonthlyRate = value.PricePerMonth;
        }
    }

    /// <summary>
    /// Nạp dữ liệu ban đầu bao gồm danh sách tất cả sinh viên và các phòng còn chỗ trống
    /// </summary>
    public async Task LoadInitialDataAsync()
    {
        try
        {
            var studentList = await _studentService.GetAllStudentsAsync();
            Students = new ObservableCollection<StudentDto>(studentList);

            var roomList = await _roomService.GetAllRoomsAsync(status: RoomStatus.Available);
            Rooms = new ObservableCollection<RoomDto>(roomList);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải dữ liệu khởi tạo: {ex.Message}";
        }
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ và thực hiện ký hợp đồng mới
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        // 1. Kiểm tra validation bắt buộc
        if (SelectedStudent == null)
        {
            ErrorMessage = "Vui lòng chọn sinh viên ký hợp đồng.";
            return;
        }

        if (SelectedRoom == null)
        {
            ErrorMessage = "Vui lòng chọn phòng ở.";
            return;
        }

        if (!StartDate.HasValue)
        {
            ErrorMessage = "Vui lòng chọn ngày bắt đầu hợp đồng.";
            return;
        }

        if (!EndDate.HasValue)
        {
            ErrorMessage = "Vui lòng chọn ngày kết thúc hợp đồng.";
            return;
        }

        if (EndDate.Value <= StartDate.Value)
        {
            ErrorMessage = "Ngày kết thúc hợp đồng phải sau ngày bắt đầu.";
            return;
        }

        if (MonthlyRate <= 0)
        {
            ErrorMessage = "Đơn giá thuê tháng phải lớn hơn 0.";
            return;
        }

        if (DepositAmount < 0)
        {
            ErrorMessage = "Tiền đặt cọc không được nhỏ hơn 0.";
            return;
        }

        ErrorMessage = null;

        // 2. Chuẩn bị request dữ liệu tạo hợp đồng
        var request = new CreateContractRequest
        {
            StudentId = SelectedStudent.Id,
            RoomId = SelectedRoom.Id,
            StartDate = StartDate.Value.DateTime,
            EndDate = EndDate.Value.DateTime,
            DepositAmount = DepositAmount,
            MonthlyRate = MonthlyRate,
            Notes = Notes?.Trim()
        };

        try
        {
            await _contractService.CreateContractAsync(request);
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            // Hiển thị thông báo lỗi nghiệp vụ từ backend (ví dụ: SV đã có HĐ, giới tính không hợp lệ,...)
            ErrorMessage = ex.Message;
        }
    }

    /// <summary>
    /// Hủy bỏ thao tác lập hợp đồng và đóng hộp thoại
    /// </summary>
    [RelayCommand]
    public void Cancel()
    {
        CloseAction?.Invoke(false);
    }
}
