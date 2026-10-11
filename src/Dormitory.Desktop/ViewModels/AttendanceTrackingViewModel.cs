using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// Tùy chọn trạng thái điểm danh hiển thị trong ComboBox
/// </summary>
public class AttendanceStatusOption
{
    public AttendanceStatus Value { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string BadgeColor { get; set; } = string.Empty;
    public string BadgeBackground { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel đại diện cho một sinh viên trong danh sách điểm danh theo phòng
/// </summary>
public partial class StudentAttendanceItemViewModel : ObservableObject
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;

    [ObservableProperty]
    private AttendanceStatus _status = AttendanceStatus.Present;

    [ObservableProperty]
    private string? _reason;

    /// <summary>
    /// Sự kiện thông báo khi trạng thái điểm danh của sinh viên thay đổi để cập nhật KPI
    /// </summary>
    public event EventHandler? StatusChanged;

    /// <summary>
    /// Danh sách 4 tùy chọn trạng thái cho ComboBox
    /// </summary>
    public IReadOnlyList<AttendanceStatusOption> AvailableStatusOptions => AttendanceTrackingViewModel.StatusOptions;

    /// <summary>
    /// Tùy chọn trạng thái đang chọn phục vụ Binding hai chiều với ComboBox
    /// </summary>
    public AttendanceStatusOption SelectedStatusOption
    {
        get => AttendanceTrackingViewModel.StatusOptions.FirstOrDefault(o => o.Value == Status)
               ?? AttendanceTrackingViewModel.StatusOptions[0];
        set
        {
            if (value != null && Status != value.Value)
            {
                Status = value.Value;
            }
        }
    }

    /// <summary>
    /// Nhãn hiển thị tiếng Việt của trạng thái điểm danh
    /// </summary>
    public string StatusText => Status switch
    {
        AttendanceStatus.Present => "Có mặt",
        AttendanceStatus.AbsentWithoutPermission => "Vắng không phép",
        AttendanceStatus.AbsentWithPermission => "Vắng có phép",
        AttendanceStatus.Late => "Về muộn",
        _ => "Không rõ"
    };

    /// <summary>
    /// Màu chữ badge trạng thái
    /// </summary>
    public string StatusBadgeColor => Status switch
    {
        AttendanceStatus.Present => "#107C41",
        AttendanceStatus.AbsentWithoutPermission => "#A80000",
        AttendanceStatus.AbsentWithPermission => "#0078D4",
        AttendanceStatus.Late => "#C67D00",
        _ => "#605E5C"
    };

    /// <summary>
    /// Màu nền badge trạng thái
    /// </summary>
    public string StatusBadgeBackground => Status switch
    {
        AttendanceStatus.Present => "#DFF6DD",
        AttendanceStatus.AbsentWithoutPermission => "#FDE7E9",
        AttendanceStatus.AbsentWithPermission => "#DEECF9",
        AttendanceStatus.Late => "#FFF4CE",
        _ => "#F3F2F1"
    };

    partial void OnStatusChanged(AttendanceStatus value)
    {
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(StatusBadgeColor));
        OnPropertyChanged(nameof(StatusBadgeBackground));
        OnPropertyChanged(nameof(SelectedStatusOption));
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// ViewModel quản lý giao diện điểm danh sinh viên nội trú ban đêm theo phòng
/// </summary>
public partial class AttendanceTrackingViewModel : ViewModelBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IRoomService _roomService;
    private readonly IStudentService _studentService;
    private readonly IDialogService _dialogService;
    private bool _isInternalLoading;

    /// <summary>
    /// Danh sách 4 tùy chọn trạng thái tĩnh dùng chung cho ComboBox
    /// </summary>
    public static IReadOnlyList<AttendanceStatusOption> StatusOptions { get; } = new List<AttendanceStatusOption>
    {
        new() { Value = AttendanceStatus.Present, DisplayName = "Có mặt", BadgeColor = "#107C41", BadgeBackground = "#DFF6DD" },
        new() { Value = AttendanceStatus.AbsentWithoutPermission, DisplayName = "Vắng không phép", BadgeColor = "#A80000", BadgeBackground = "#FDE7E9" },
        new() { Value = AttendanceStatus.AbsentWithPermission, DisplayName = "Vắng có phép", BadgeColor = "#0078D4", BadgeBackground = "#DEECF9" },
        new() { Value = AttendanceStatus.Late, DisplayName = "Về muộn", BadgeColor = "#C67D00", BadgeBackground = "#FFF4CE" }
    };

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    /// <summary>
    /// Thuộc tính hỗ trợ CalendarDatePicker (Avalonia hỗ trợ DateTime?)
    /// </summary>
    public DateTime? SelectedDateNullable
    {
        get => SelectedDate;
        set
        {
            if (value.HasValue && value.Value.Date != SelectedDate.Date)
            {
                SelectedDate = value.Value.Date;
            }
        }
    }

    public ObservableCollection<RoomDto> Rooms { get; } = new();

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    public ObservableCollection<StudentAttendanceItemViewModel> Students { get; } = new();

    [ObservableProperty]
    private string _recordedBy = "Quản lý KTX";

    // Thống kê thẻ KPI
    [ObservableProperty]
    private int _totalStudentsCount;

    [ObservableProperty]
    private int _presentCount;

    [ObservableProperty]
    private int _absentWithoutPermissionCount;

    [ObservableProperty]
    private int _absentWithPermissionCount;

    [ObservableProperty]
    private int _lateCount;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Chỉ cho phép lưu khi có phòng được chọn và danh sách sinh viên không rỗng
    /// </summary>
    public bool CanSave => SelectedRoom != null && Students.Count > 0;

    public AttendanceTrackingViewModel(
        IAttendanceService attendanceService,
        IRoomService roomService,
        IStudentService studentService,
        IDialogService dialogService)
    {
        _attendanceService = attendanceService ?? throw new ArgumentNullException(nameof(attendanceService));
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        _ = LoadRoomsAsync();
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        OnPropertyChanged(nameof(SelectedDateNullable));
        if (!_isInternalLoading)
        {
            _ = LoadAttendanceAsync();
        }
    }

    partial void OnSelectedRoomChanged(RoomDto? value)
    {
        OnPropertyChanged(nameof(CanSave));
        if (!_isInternalLoading)
        {
            _ = LoadAttendanceAsync();
        }
    }

    /// <summary>
    /// Tải danh sách phòng ký túc xá và chọn phòng đầu tiên
    /// </summary>
    [RelayCommand]
    public async Task LoadRoomsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang tải danh sách phòng...";

            var rooms = await _roomService.GetAllRoomsAsync();
            Rooms.Clear();
            foreach (var room in rooms)
            {
                Rooms.Add(room);
            }

            if (Rooms.Count > 0)
            {
                _isInternalLoading = true;
                SelectedRoom = Rooms[0];
                _isInternalLoading = false;

                await LoadAttendanceAsync();
            }
            else
            {
                SelectedRoom = null;
                ClearStudents();
                UpdateKpiStats();
                StatusMessage = "Không có phòng nào trong danh sách.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi tải danh sách phòng: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tải danh sách phòng: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Tải dữ liệu sinh viên trong phòng và kết quả điểm danh theo ngày đã chọn
    /// </summary>
    [RelayCommand]
    public async Task LoadAttendanceAsync()
    {
        if (SelectedRoom == null)
        {
            ClearStudents();
            UpdateKpiStats();
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = $"Đang tải dữ liệu điểm danh phòng {SelectedRoom.RoomNumber}...";

            var studentList = await _studentService.GetAllStudentsAsync(searchQuery: null, roomId: SelectedRoom.Id);
            var existingRecords = await _attendanceService.GetRoomAttendanceAsync(SelectedRoom.Id, SelectedDate);

            var recordMap = existingRecords != null
                ? existingRecords.ToDictionary(r => r.StudentId)
                : new Dictionary<int, AttendanceRecordDto>();

            ClearStudents();

            foreach (var student in studentList)
            {
                var item = new StudentAttendanceItemViewModel
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    StudentCode = student.StudentCode,
                    RoomId = SelectedRoom.Id,
                    RoomNumber = SelectedRoom.RoomNumber
                };

                if (recordMap.TryGetValue(student.Id, out var existingRecord))
                {
                    item.Status = existingRecord.Status;
                    item.Reason = existingRecord.Reason;
                }
                else
                {
                    item.Status = AttendanceStatus.Present;
                    item.Reason = null;
                }

                item.StatusChanged += OnStudentStatusChanged;
                Students.Add(item);
            }

            UpdateKpiStats();
            StatusMessage = $"Đã tải {Students.Count} sinh viên phòng {SelectedRoom.RoomNumber}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi tải dữ liệu điểm danh: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tải dữ liệu điểm danh: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Đánh dấu tất cả sinh viên có mặt
    /// </summary>
    [RelayCommand]
    public void MarkAllPresent()
    {
        foreach (var student in Students)
        {
            student.Status = AttendanceStatus.Present;
        }
        UpdateKpiStats();
    }

    /// <summary>
    /// Đánh dấu tất cả sinh viên vắng không phép
    /// </summary>
    [RelayCommand]
    public void MarkAllAbsent()
    {
        foreach (var student in Students)
        {
            student.Status = AttendanceStatus.AbsentWithoutPermission;
        }
        UpdateKpiStats();
    }

    /// <summary>
    /// Đánh dấu tất cả sinh viên về muộn
    /// </summary>
    [RelayCommand]
    public void MarkAllLate()
    {
        foreach (var student in Students)
        {
            student.Status = AttendanceStatus.Late;
        }
        UpdateKpiStats();
    }

    /// <summary>
    /// Lưu kết quả điểm danh theo lô cho toàn bộ sinh viên trong phòng
    /// </summary>
    [RelayCommand]
    public async Task SaveAttendanceAsync()
    {
        if (SelectedRoom == null || Students.Count == 0)
        {
            await _dialogService.ShowMessageAsync("Thông báo", "Vui lòng chọn phòng có sinh viên để lưu điểm danh.");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Đang lưu kết quả điểm danh...";

            var batchDto = new BatchRoomAttendanceDto
            {
                RoomId = SelectedRoom.Id,
                AttendanceDate = SelectedDate,
                RecordedBy = RecordedBy,
                StudentAttendances = Students.Select(s => new AttendanceRecordDto
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    StudentCode = s.StudentCode,
                    RoomId = s.RoomId,
                    RoomNumber = s.RoomNumber,
                    Status = s.Status,
                    Reason = s.Reason,
                    RecordedBy = RecordedBy,
                    AttendanceDate = SelectedDate
                }).ToList()
            };

            await _attendanceService.RecordBatchRoomAttendanceAsync(batchDto);
            await LoadAttendanceAsync();
            UpdateKpiStats();

            await _dialogService.ShowMessageAsync("Thành công", $"Đã lưu kết quả điểm danh phòng {SelectedRoom.RoomNumber} ngày {SelectedDate:dd/MM/yyyy} thành công!");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi lưu điểm danh: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể lưu kết quả điểm danh: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Làm mới toàn bộ danh sách phòng và nạp lại điểm danh
    /// </summary>
    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadRoomsAsync();
    }

    /// <summary>
    /// Cập nhật các chỉ số KPI chuyên cần và trạng thái CanSave
    /// </summary>
    public void UpdateKpiStats()
    {
        TotalStudentsCount = Students.Count;
        PresentCount = Students.Count(s => s.Status == AttendanceStatus.Present);
        AbsentWithoutPermissionCount = Students.Count(s => s.Status == AttendanceStatus.AbsentWithoutPermission);
        AbsentWithPermissionCount = Students.Count(s => s.Status == AttendanceStatus.AbsentWithPermission);
        LateCount = Students.Count(s => s.Status == AttendanceStatus.Late);
        OnPropertyChanged(nameof(CanSave));
    }

    private void ClearStudents()
    {
        foreach (var item in Students)
        {
            item.StatusChanged -= OnStudentStatusChanged;
        }
        Students.Clear();
    }

    private void OnStudentStatusChanged(object? sender, EventArgs e)
    {
        UpdateKpiStats();
    }
}
