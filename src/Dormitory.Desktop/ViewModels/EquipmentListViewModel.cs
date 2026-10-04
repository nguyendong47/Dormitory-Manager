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
/// ViewModel quản lý danh sách trang thiết bị / tài sản phòng ở và các nghiệp vụ kiểm kê, bảo trì
/// </summary>
public partial class EquipmentListViewModel : ViewModelBase
{
    private readonly IEquipmentService _equipmentService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IUserSession? _userSession;

    [ObservableProperty]
    private ObservableCollection<EquipmentDto> _equipments = new();

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private RoomDto? _selectedRoomFilter;

    [ObservableProperty]
    private EquipmentStatus? _selectedStatusFilter;

    [ObservableProperty]
    private string _selectedStatusOption = "Tất cả";

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditEquipmentCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteEquipmentCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReportIssueCommand))]
    private EquipmentDto? _selectedEquipment;

    [ObservableProperty]
    private bool _isLoading;

    // KPI Counters
    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _goodCount;

    [ObservableProperty]
    private int _needsRepairCount;

    [ObservableProperty]
    private int _brokenCount;

    /// <summary>
    /// Danh sách các tùy chọn lọc trạng thái cho ComboBox
    /// </summary>
    public IReadOnlyList<string> StatusFilterOptions { get; } = new[]
    {
        "Tất cả",
        "Hoạt động tốt",
        "Cần bảo trì",
        "Hỏng hóc"
    };

    public EquipmentListViewModel(
        IEquipmentService equipmentService,
        IRoomService roomService,
        IDialogService dialogService,
        IUserSession? userSession = null)
    {
        _equipmentService = equipmentService;
        _roomService = roomService;
        _dialogService = dialogService;
        _userSession = userSession;
    }

    /// <summary>
    /// Điều kiện để kích hoạt thao tác chỉnh sửa, báo hỏng hoặc xóa từ thanh công cụ
    /// </summary>
    private bool CanEditOrDeleteEquipment => SelectedEquipment != null;

    /// <summary>
    /// Tự động cập nhật bộ lọc Enum và tải lại dữ liệu khi người dùng chọn tùy chọn trạng thái
    /// </summary>
    partial void OnSelectedStatusOptionChanged(string value)
    {
        SelectedStatusFilter = value switch
        {
            "Hoạt động tốt" => EquipmentStatus.Good,
            "Cần bảo trì" => EquipmentStatus.NeedsRepair,
            "Hỏng hóc" => EquipmentStatus.Broken,
            _ => null
        };
        _ = LoadEquipmentsAsync();
    }

    /// <summary>
    /// Tự động nạp lại danh sách khi người dùng chọn lọc theo phòng
    /// </summary>
    partial void OnSelectedRoomFilterChanged(RoomDto? value)
    {
        _ = LoadEquipmentsAsync();
    }

    /// <summary>
    /// Tải danh sách phòng và nạp danh sách thiết bị theo bộ lọc hiện tại, tính toán các chỉ số KPI
    /// </summary>
    [RelayCommand]
    public async Task LoadEquipmentsAsync()
    {
        IsLoading = true;
        try
        {
            if (Rooms.Count == 0)
            {
                var roomList = await _roomService.GetAllRoomsAsync();
                Rooms = new ObservableCollection<RoomDto>(roomList);
            }

            var list = await _equipmentService.GetAllEquipmentsAsync(
                searchTerm: SearchText,
                roomId: SelectedRoomFilter?.Id,
                status: SelectedStatusFilter);

            Equipments = new ObservableCollection<EquipmentDto>(list);

            // Tính toán thống kê KPI theo số lượng thực tế
            TotalCount = list.Sum(e => e.Quantity);
            GoodCount = list.Where(e => e.Status == EquipmentStatus.Good).Sum(e => e.Quantity);
            NeedsRepairCount = list.Where(e => e.Status == EquipmentStatus.NeedsRepair).Sum(e => e.Quantity);
            BrokenCount = list.Where(e => e.Status == EquipmentStatus.Broken).Sum(e => e.Quantity);
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
    /// Mở hộp thoại thêm thiết bị mới
    /// </summary>
    [RelayCommand]
    public async Task AddEquipmentAsync()
    {
        var dialogVm = new EquipmentDialogViewModel(_equipmentService, _roomService);
        await dialogVm.InitializeAsync();
        var dialog = new EquipmentDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadEquipmentsAsync();
        }
    }

    /// <summary>
    /// Mở hộp thoại chỉnh sửa thông tin thiết bị đang được chọn từ thanh công cụ
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteEquipment))]
    public async Task EditEquipmentAsync()
    {
        if (SelectedEquipment == null) return;
        await EditEquipmentInternalAsync(SelectedEquipment);
    }

    /// <summary>
    /// Chỉnh sửa thiết bị cụ thể từ nút thao tác trên dòng DataGrid
    /// </summary>
    [RelayCommand]
    public async Task EditEquipmentItemAsync(EquipmentDto item)
    {
        if (item == null) return;
        await EditEquipmentInternalAsync(item);
    }

    private async Task EditEquipmentInternalAsync(EquipmentDto item)
    {
        var dialogVm = new EquipmentDialogViewModel(_equipmentService, _roomService);
        await dialogVm.InitializeAsync(item);
        var dialog = new EquipmentDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadEquipmentsAsync();
        }
    }

    /// <summary>
    /// Xóa thiết bị đang được chọn sau khi xác nhận
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteEquipment))]
    public async Task DeleteEquipmentAsync()
    {
        if (SelectedEquipment == null) return;
        await DeleteEquipmentInternalAsync(SelectedEquipment);
    }

    /// <summary>
    /// Xóa thiết bị cụ thể từ nút thao tác trên dòng DataGrid
    /// </summary>
    [RelayCommand]
    public async Task DeleteEquipmentItemAsync(EquipmentDto item)
    {
        if (item == null) return;
        await DeleteEquipmentInternalAsync(item);
    }

    private async Task DeleteEquipmentInternalAsync(EquipmentDto item)
    {
        // Ràng buộc RBAC: Chỉ Admin mới có quyền xóa thiết bị
        if (_userSession != null && !_userSession.IsAdmin)
        {
            await _dialogService.ShowMessageAsync(
                "Từ chối truy cập",
                "Chỉ Quản trị viên (Admin) mới có quyền xóa tài sản, thiết bị phòng.");
            return;
        }

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận xóa thiết bị",
            $"Bạn có chắc chắn muốn xóa thiết bị '{item.Name}' (Mã: {item.EquipmentCode}) khỏi phòng {item.RoomNumber} không?");

        if (!confirmed) return;

        try
        {
            var success = await _equipmentService.DeleteEquipmentAsync(item.Id);
            if (success)
            {
                await LoadEquipmentsAsync();
                if (SelectedEquipment?.Id == item.Id)
                {
                    SelectedEquipment = null;
                }
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không tìm thấy thiết bị cần xóa.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Báo cáo sự cố / chuyển trạng thái thiết bị sang Cần sửa chữa hoặc Hỏng hóc
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteEquipment))]
    public async Task ReportIssueAsync()
    {
        if (SelectedEquipment == null) return;
        await ReportIssueInternalAsync(SelectedEquipment);
    }

    /// <summary>
    /// Báo sự cố thiết bị từ nút thao tác trên dòng DataGrid
    /// </summary>
    [RelayCommand]
    public async Task ReportIssueItemAsync(EquipmentDto item)
    {
        if (item == null) return;
        await ReportIssueInternalAsync(item);
    }

    private async Task ReportIssueInternalAsync(EquipmentDto item)
    {
        var nextStatus = item.Status == EquipmentStatus.NeedsRepair
            ? EquipmentStatus.Broken
            : EquipmentStatus.NeedsRepair;

        var statusName = nextStatus == EquipmentStatus.Broken ? "Hỏng hóc" : "Cần bảo trì / sửa chữa";

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Báo sự cố thiết bị",
            $"Chuyển trạng thái thiết bị '{item.Name}' ({item.EquipmentCode}) sang '{statusName}'?");

        if (!confirmed) return;

        try
        {
            var success = await _equipmentService.UpdateStatusAsync(
                item.Id,
                nextStatus,
                $"Ghi nhận sự cố: {statusName} lúc {DateTime.Now:dd/MM/yyyy HH:mm}");

            if (success)
            {
                await LoadEquipmentsAsync();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể cập nhật trạng thái thiết bị.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Đặt lại toàn bộ bộ lọc về trạng thái mặc định
    /// </summary>
    [RelayCommand]
    public async Task ClearFiltersAsync()
    {
        SearchText = string.Empty;
        SelectedRoomFilter = null;
        SelectedStatusOption = "Tất cả";
        SelectedStatusFilter = null;
        await LoadEquipmentsAsync();
    }
}
