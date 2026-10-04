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
using Dormitory.Desktop.Views;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý danh sách biên bản vi phạm kỷ luật nội quy KTX, bộ lọc tìm kiếm và các thao tác lập / xử lý biên bản
/// </summary>
public partial class ViolationListViewModel : ViewModelBase
{
    private readonly IViolationService _violationService;
    private readonly IStudentService _studentService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IUserSession? _userSession;

    [ObservableProperty]
    private ObservableCollection<ViolationDto> _violations = new();

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private RoomDto? _selectedRoomFilter;

    [ObservableProperty]
    private ViolationSeverity? _selectedSeverityFilter;

    [ObservableProperty]
    private string _selectedSeverityOption = "Tất cả";

    [ObservableProperty]
    private ViolationStatus? _selectedStatusFilter;

    [ObservableProperty]
    private string _selectedStatusOption = "Tất cả";

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditViolationCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResolveViolationCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteViolationCommand))]
    private ViolationDto? _selectedViolation;

    [ObservableProperty]
    private bool _isLoading;

    // KPI Counters
    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _pendingCount;

    [ObservableProperty]
    private int _resolvedCount;

    [ObservableProperty]
    private int _criticalCount;

    /// <summary>
    /// Danh sách các tùy chọn lọc mức độ nghiêm trọng
    /// </summary>
    public IReadOnlyList<string> SeverityFilterOptions { get; } = new[]
    {
        "Tất cả",
        "Nhắc nhở (Mức nhẹ)",
        "Khiển trách (Mức vừa)",
        "Cảnh cáo (Nghiêm trọng)",
        "Buộc rời KTX (Rất nghiêm trọng)"
    };

    /// <summary>
    /// Danh sách các tùy chọn lọc trạng thái giải quyết
    /// </summary>
    public IReadOnlyList<string> StatusFilterOptions { get; } = new[]
    {
        "Tất cả",
        "Chờ xử lý",
        "Đã xử lý",
        "Hủy bỏ"
    };

    public ViolationListViewModel(
        IViolationService violationService,
        IStudentService studentService,
        IRoomService roomService,
        IDialogService dialogService,
        IUserSession? userSession = null)
    {
        _violationService = violationService;
        _studentService = studentService;
        _roomService = roomService;
        _dialogService = dialogService;
        _userSession = userSession;
    }

    /// <summary>
    /// Điều kiện để kích hoạt thao tác từ thanh công cụ
    /// </summary>
    private bool CanEditOrDeleteViolation => SelectedViolation != null;

    /// <summary>
    /// Tự động cập nhật bộ lọc mức độ nghiêm trọng khi người dùng chọn ComboBox
    /// </summary>
    partial void OnSelectedSeverityOptionChanged(string value)
    {
        SelectedSeverityFilter = value switch
        {
            "Nhắc nhở (Mức nhẹ)" => ViolationSeverity.Minor,
            "Khiển trách (Mức vừa)" => ViolationSeverity.Moderate,
            "Cảnh cáo (Nghiêm trọng)" => ViolationSeverity.Severe,
            "Buộc rời KTX (Rất nghiêm trọng)" => ViolationSeverity.Critical,
            _ => null
        };
        _ = LoadViolationsAsync();
    }

    /// <summary>
    /// Tự động cập nhật bộ lọc trạng thái khi người dùng chọn ComboBox
    /// </summary>
    partial void OnSelectedStatusOptionChanged(string value)
    {
        SelectedStatusFilter = value switch
        {
            "Chờ xử lý" => ViolationStatus.Pending,
            "Đã xử lý" => ViolationStatus.Resolved,
            "Hủy bỏ" => ViolationStatus.Dismissed,
            _ => null
        };
        _ = LoadViolationsAsync();
    }

    /// <summary>
    /// Tự động nạp lại danh sách khi người dùng chọn lọc theo phòng
    /// </summary>
    partial void OnSelectedRoomFilterChanged(RoomDto? value)
    {
        _ = LoadViolationsAsync();
    }

    /// <summary>
    /// Tải danh sách phòng và nạp danh sách biên bản vi phạm theo bộ lọc hiện tại, tính toán KPI
    /// </summary>
    [RelayCommand]
    public async Task LoadViolationsAsync()
    {
        IsLoading = true;
        try
        {
            if (Rooms.Count == 0)
            {
                var roomList = await _roomService.GetAllRoomsAsync();
                Rooms = new ObservableCollection<RoomDto>(roomList);
            }

            var list = await _violationService.GetAllViolationsAsync(
                searchTerm: SearchText,
                severity: SelectedSeverityFilter,
                status: SelectedStatusFilter,
                roomId: SelectedRoomFilter?.Id);

            Violations = new ObservableCollection<ViolationDto>(list);

            // Cập nhật thống kê KPI
            TotalCount = list.Count;
            PendingCount = list.Count(v => v.Status == ViolationStatus.Pending);
            ResolvedCount = list.Count(v => v.Status == ViolationStatus.Resolved);
            CriticalCount = list.Count(v => v.Severity == ViolationSeverity.Severe || v.Severity == ViolationSeverity.Critical);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi nạp dữ liệu", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Mở hộp thoại lập biên bản vi phạm mới
    /// </summary>
    [RelayCommand]
    public async Task AddViolationAsync()
    {
        var dialogVm = new ViolationDialogViewModel(_violationService, _studentService, _roomService);
        await dialogVm.InitializeAsync();
        var dialog = new ViolationDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadViolationsAsync();
        }
    }

    /// <summary>
    /// Mở hộp thoại chỉnh sửa thông tin biên bản vi phạm đang được chọn
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteViolation))]
    public async Task EditViolationAsync()
    {
        if (SelectedViolation == null) return;
        await EditViolationInternalAsync(SelectedViolation);
    }

    /// <summary>
    /// Chỉnh sửa biên bản vi phạm từ nút thao tác trên dòng DataGrid
    /// </summary>
    [RelayCommand]
    public async Task EditViolationItemAsync(ViolationDto item)
    {
        if (item == null) return;
        await EditViolationInternalAsync(item);
    }

    private async Task EditViolationInternalAsync(ViolationDto item)
    {
        var dialogVm = new ViolationDialogViewModel(_violationService, _studentService, _roomService);
        await dialogVm.InitializeAsync(item);
        var dialog = new ViolationDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadViolationsAsync();
        }
    }

    /// <summary>
    /// Mở hộp thoại xử lý / kết luận biên bản vi phạm đang chọn từ thanh công cụ
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteViolation))]
    public async Task ResolveViolationAsync()
    {
        if (SelectedViolation == null) return;
        await ResolveViolationInternalAsync(SelectedViolation);
    }

    /// <summary>
    /// Xử lý / kết luận biên bản vi phạm từ nút thao tác trên dòng DataGrid
    /// </summary>
    [RelayCommand]
    public async Task ResolveViolationItemAsync(ViolationDto item)
    {
        if (item == null) return;
        await ResolveViolationInternalAsync(item);
    }

    private async Task ResolveViolationInternalAsync(ViolationDto item)
    {
        var dialogVm = new ViolationDialogViewModel(_violationService, _studentService, _roomService);
        await dialogVm.InitializeAsync(item, isResolveMode: true);
        var dialog = new ViolationDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadViolationsAsync();
        }
    }

    /// <summary>
    /// Xóa biên bản vi phạm đang chọn từ thanh công cụ
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteViolation))]
    public async Task DeleteViolationAsync()
    {
        if (SelectedViolation == null) return;
        await DeleteViolationInternalAsync(SelectedViolation);
    }

    /// <summary>
    /// Xóa biên bản vi phạm từ nút thao tác trên dòng DataGrid
    /// </summary>
    [RelayCommand]
    public async Task DeleteViolationItemAsync(ViolationDto item)
    {
        if (item == null) return;
        await DeleteViolationInternalAsync(item);
    }

    private async Task DeleteViolationInternalAsync(ViolationDto item)
    {
        // Ràng buộc RBAC: Chỉ Admin mới có quyền xóa biên bản vi phạm
        if (_userSession != null && !_userSession.IsAdmin)
        {
            await _dialogService.ShowMessageAsync(
                "Từ chối truy cập",
                "Chỉ Quản trị viên (Admin) mới có quyền xóa biên bản kỷ luật / vi phạm.");
            return;
        }

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận xóa biên bản vi phạm",
            $"Bạn có chắc chắn muốn xóa biên bản vi phạm '{item.ViolationCode}' ({item.Title}) của sinh viên {item.StudentName} không?");

        if (!confirmed) return;

        try
        {
            var success = await _violationService.DeleteViolationAsync(item.Id);
            if (success)
            {
                await LoadViolationsAsync();
                if (SelectedViolation?.Id == item.Id)
                {
                    SelectedViolation = null;
                }
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không tìm thấy biên bản cần xóa.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Đặt lại toàn bộ bộ lọc về mặc định và tải lại dữ liệu
    /// </summary>
    [RelayCommand]
    public async Task ClearFiltersAsync()
    {
        SearchText = string.Empty;
        SelectedRoomFilter = null;
        SelectedSeverityOption = "Tất cả";
        SelectedSeverityFilter = null;
        SelectedStatusOption = "Tất cả";
        SelectedStatusFilter = null;
        await LoadViolationsAsync();
    }
}
