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

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// Mục tùy chọn mức độ vi phạm hiển thị trên giao diện ComboBox
/// </summary>
public record SeverityOptionItem(ViolationSeverity Value, string DisplayName);

/// <summary>
/// Mục tùy chọn trạng thái xử lý vi phạm hiển thị trên giao diện ComboBox
/// </summary>
public record StatusOptionItem(ViolationStatus Value, string DisplayName);

/// <summary>
/// ViewModel cho cửa sổ hộp thoại Lập, Cập nhật và Xử lý biên bản vi phạm KTX
/// </summary>
public partial class ViolationDialogViewModel : ViewModelBase
{
    private readonly IViolationService _violationService;
    private readonly IStudentService _studentService;
    private readonly IRoomService _roomService;

    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _violationCode = string.Empty;

    [ObservableProperty]
    private int _studentId;

    [ObservableProperty]
    private int _roomId;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private ViolationSeverity _severity = ViolationSeverity.Minor;

    [ObservableProperty]
    private ViolationStatus _status = ViolationStatus.Pending;

    [ObservableProperty]
    private decimal _fineAmount;

    [ObservableProperty]
    private int _demeritPoints = 5;

    [ObservableProperty]
    private DateTimeOffset? _violationDate = DateTimeOffset.Now;

    [ObservableProperty]
    private string? _resolutionNotes;

    [ObservableProperty]
    private string? _recordedBy;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _isResolveMode;

    [ObservableProperty]
    private string _dialogTitle = "Lập biên bản vi phạm";

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<StudentDto> _students = new();

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private StudentDto? _selectedStudent;

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    [ObservableProperty]
    private SeverityOptionItem? _selectedSeverityItem;

    [ObservableProperty]
    private StatusOptionItem? _selectedStatusItem;

    /// <summary>
    /// Danh sách các tùy chọn mức độ nghiêm trọng
    /// </summary>
    public IReadOnlyList<SeverityOptionItem> SeverityOptions { get; } = new[]
    {
        new SeverityOptionItem(ViolationSeverity.Minor, "Nhắc nhở (Mức nhẹ)"),
        new SeverityOptionItem(ViolationSeverity.Moderate, "Khiển trách (Mức vừa)"),
        new SeverityOptionItem(ViolationSeverity.Severe, "Cảnh cáo (Nghiêm trọng)"),
        new SeverityOptionItem(ViolationSeverity.Critical, "Buộc rời KTX (Rất nghiêm trọng)")
    };

    /// <summary>
    /// Danh sách các tùy chọn trạng thái xử lý
    /// </summary>
    public IReadOnlyList<StatusOptionItem> StatusOptions { get; } = new[]
    {
        new StatusOptionItem(ViolationStatus.Pending, "Chờ xử lý"),
        new StatusOptionItem(ViolationStatus.Resolved, "Đã xử lý"),
        new StatusOptionItem(ViolationStatus.Dismissed, "Hủy bỏ")
    };

    /// <summary>
    /// Hành động đóng dialog trả về kết quả (true nếu lưu thành công, false nếu hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    public ViolationDialogViewModel(
        IViolationService violationService,
        IStudentService studentService,
        IRoomService roomService)
    {
        _violationService = violationService;
        _studentService = studentService;
        _roomService = roomService;

        _selectedSeverityItem = SeverityOptions[0];
        _selectedStatusItem = StatusOptions[0];
    }

    /// <summary>
    /// Khi chọn sinh viên, tự động gán StudentId và gợi ý phòng của sinh viên
    /// </summary>
    partial void OnSelectedStudentChanged(StudentDto? value)
    {
        if (value != null)
        {
            StudentId = value.Id;

            // Nếu tạo mới và sinh viên đã có phòng, tự động chọn phòng tương ứng
            if (!IsEditMode && !IsResolveMode && value.CurrentRoomId.HasValue)
            {
                var matchingRoom = Rooms.FirstOrDefault(r => r.Id == value.CurrentRoomId.Value);
                if (matchingRoom != null)
                {
                    SelectedRoom = matchingRoom;
                }
            }
        }
    }

    /// <summary>
    /// Khi chọn phòng, tự động gán RoomId
    /// </summary>
    partial void OnSelectedRoomChanged(RoomDto? value)
    {
        if (value != null)
        {
            RoomId = value.Id;
        }
    }

    /// <summary>
    /// Cập nhật enum Severity khi thay đổi ComboBox
    /// </summary>
    partial void OnSelectedSeverityItemChanged(SeverityOptionItem? value)
    {
        if (value != null)
        {
            Severity = value.Value;
        }
    }

    /// <summary>
    /// Cập nhật enum Status khi thay đổi ComboBox
    /// </summary>
    partial void OnSelectedStatusItemChanged(StatusOptionItem? value)
    {
        if (value != null)
        {
            Status = value.Value;
        }
    }

    /// <summary>
    /// Nạp dữ liệu ban đầu cho danh sách sinh viên, phòng và biên bản cần sửa/xử lý
    /// </summary>
    public async Task InitializeAsync(ViolationDto? existing = null, bool isResolveMode = false)
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var studentTask = _studentService.GetAllStudentsAsync();
            var roomTask = _roomService.GetAllRoomsAsync();

            await Task.WhenAll(studentTask, roomTask);

            Students = new ObservableCollection<StudentDto>(studentTask.Result);
            Rooms = new ObservableCollection<RoomDto>(roomTask.Result);

            IsResolveMode = isResolveMode;
            IsEditMode = existing != null && !isResolveMode;

            if (existing != null)
            {
                Id = existing.Id;
                ViolationCode = existing.ViolationCode;
                StudentId = existing.StudentId;
                RoomId = existing.RoomId;
                Title = existing.Title;
                Description = existing.Description;
                Severity = existing.Severity;
                Status = existing.Status;
                FineAmount = existing.FineAmount;
                DemeritPoints = existing.DemeritPoints;
                ViolationDate = new DateTimeOffset(existing.ViolationDate);
                ResolutionNotes = existing.ResolutionNotes;
                RecordedBy = existing.RecordedBy;

                SelectedStudent = Students.FirstOrDefault(s => s.Id == existing.StudentId);
                SelectedRoom = Rooms.FirstOrDefault(r => r.Id == existing.RoomId);
                SelectedSeverityItem = SeverityOptions.FirstOrDefault(s => s.Value == existing.Severity) ?? SeverityOptions[0];
                SelectedStatusItem = StatusOptions.FirstOrDefault(s => s.Value == existing.Status) ?? StatusOptions[0];

                if (isResolveMode)
                {
                    DialogTitle = $"⚖️ Xử lý biên bản vi phạm {existing.ViolationCode}";
                    // Mặc định chuyển sang Đã xử lý nếu biên bản đang Chờ xử lý
                    if (Status == ViolationStatus.Pending)
                    {
                        SelectedStatusItem = StatusOptions.FirstOrDefault(s => s.Value == ViolationStatus.Resolved) ?? StatusOptions[1];
                        Status = ViolationStatus.Resolved;
                    }
                }
                else
                {
                    DialogTitle = $"✏️ Chỉnh sửa biên bản {existing.ViolationCode}";
                }
            }
            else
            {
                Id = 0;
                ViolationCode = string.Empty;
                StudentId = 0;
                RoomId = 0;
                Title = string.Empty;
                Description = null;
                Severity = ViolationSeverity.Minor;
                Status = ViolationStatus.Pending;
                FineAmount = 0m;
                DemeritPoints = 5;
                ViolationDate = DateTimeOffset.Now;
                ResolutionNotes = null;
                RecordedBy = null;
                DialogTitle = "⚖️ Lập biên bản vi phạm nội quy mới";

                SelectedSeverityItem = SeverityOptions[0];
                SelectedStatusItem = StatusOptions[0];

                if (Students.Count > 0)
                {
                    SelectedStudent = Students[0];
                }
                if (Rooms.Count > 0)
                {
                    SelectedRoom = Rooms[0];
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể nạp dữ liệu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Lưu thông tin biên bản vi phạm (thêm mới, chỉnh sửa hoặc giải quyết)
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsBusy) return;

        // Kiểm tra dữ liệu bắt buộc
        if (SelectedStudent == null || StudentId <= 0)
        {
            ErrorMessage = "Vui lòng chọn sinh viên vi phạm.";
            return;
        }

        if (SelectedRoom == null || RoomId <= 0)
        {
            ErrorMessage = "Vui lòng chọn phòng xảy ra vi phạm.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Tiêu đề vi phạm không được để trống.";
            return;
        }

        if (DemeritPoints < 0)
        {
            ErrorMessage = "Điểm rèn luyện bị trừ không được âm.";
            return;
        }

        if (FineAmount < 0)
        {
            ErrorMessage = "Tiền phạt không được âm.";
            return;
        }

        if (IsResolveMode && string.IsNullOrWhiteSpace(ResolutionNotes))
        {
            ErrorMessage = "Vui lòng nhập ghi chú kết luận / biện pháp xử lý vi phạm.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsResolveMode)
            {
                var resolveDto = new ResolveViolationDto
                {
                    Status = Status,
                    ResolutionNotes = ResolutionNotes?.Trim()
                };

                var success = await _violationService.ResolveViolationAsync(Id, resolveDto);
                if (!success)
                {
                    ErrorMessage = "Không thể cập nhật kết quả xử lý biên bản.";
                    return;
                }
            }
            else if (IsEditMode)
            {
                var updateDto = new UpdateViolationDto
                {
                    StudentId = StudentId,
                    RoomId = RoomId,
                    Title = Title.Trim(),
                    Description = Description?.Trim(),
                    Severity = Severity,
                    Status = Status,
                    FineAmount = FineAmount,
                    DemeritPoints = DemeritPoints,
                    ViolationDate = ViolationDate?.DateTime ?? DateTime.Now,
                    ResolutionNotes = ResolutionNotes?.Trim()
                };

                await _violationService.UpdateViolationAsync(Id, updateDto);
            }
            else
            {
                var createDto = new CreateViolationDto
                {
                    ViolationCode = string.IsNullOrWhiteSpace(ViolationCode) ? null : ViolationCode.Trim(),
                    StudentId = StudentId,
                    RoomId = RoomId,
                    Title = Title.Trim(),
                    Description = Description?.Trim(),
                    Severity = Severity,
                    FineAmount = FineAmount,
                    DemeritPoints = DemeritPoints,
                    ViolationDate = ViolationDate?.DateTime ?? DateTime.Now,
                    RecordedBy = RecordedBy?.Trim()
                };

                await _violationService.CreateViolationAsync(createDto);
            }

            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
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
