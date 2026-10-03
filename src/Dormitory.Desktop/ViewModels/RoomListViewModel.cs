using System;
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
/// ViewModel quản lý danh sách phòng ở ký túc xá và các thao tác CRUD
/// </summary>
public partial class RoomListViewModel : ViewModelBase
{
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IExportService _exportService;
    private readonly IFileService _fileService;

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditRoomCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteRoomCommand))]
    private RoomDto? _selectedRoom;

    [ObservableProperty]
    private string? _selectedBuilding;

    [ObservableProperty]
    private RoomStatus? _selectedStatus;

    [ObservableProperty]
    private bool _isLoading;

    private readonly IUserSession? _userSession;

    public RoomListViewModel(
        IRoomService roomService,
        IDialogService dialogService,
        IExportService exportService,
        IFileService fileService,
        IUserSession? userSession = null)
    {
        _roomService = roomService;
        _dialogService = dialogService;
        _exportService = exportService;
        _fileService = fileService;
        _userSession = userSession;
        LoadRoomsCommand = new AsyncRelayCommand(LoadRoomsAsync);
    }

    public IAsyncRelayCommand LoadRoomsCommand { get; }

    /// <summary>
    /// Xuất danh sách phòng ở ra file Excel
    /// </summary>
    [RelayCommand]
    public async Task ExportToExcelAsync()
    {
        IsLoading = true;
        try
        {
            var list = Rooms.ToList();
            var bytes = await _exportService.ExportRoomsToExcelAsync(list);
            await _fileService.SaveFileAsync("Danh_Sach_Phong", "xlsx", "Excel Files", bytes);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", $"Lỗi khi xuất file Excel: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Điều kiện để kích hoạt chỉnh sửa hoặc xóa phòng
    /// </summary>
    private bool CanEditOrDeleteRoom => SelectedRoom != null;

    /// <summary>
    /// Mở hộp thoại thêm phòng mới
    /// </summary>
    [RelayCommand]
    public async Task AddRoomAsync()
    {
        var dialogVm = new RoomDialogViewModel(_roomService);
        var dialog = new RoomDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadRoomsAsync();
        }
    }

    /// <summary>
    /// Mở hộp thoại chỉnh sửa thông tin phòng đang được chọn
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteRoom))]
    public async Task EditRoomAsync()
    {
        if (SelectedRoom == null) return;

        var dialogVm = new RoomDialogViewModel(_roomService, SelectedRoom);
        var dialog = new RoomDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadRoomsAsync();
        }
    }

    /// <summary>
    /// Xóa phòng đang chọn sau khi người dùng xác nhận
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEditOrDeleteRoom))]
    public async Task DeleteRoomAsync()
    {
        if (SelectedRoom == null) return;

        // Ràng buộc RBAC: Chỉ Admin mới có quyền xóa phòng
        if (_userSession != null && !_userSession.IsAdmin)
        {
            await _dialogService.ShowMessageAsync(
                "Từ chối truy cập",
                "Chỉ Quản trị viên (Admin) mới có quyền xóa phòng ở.");
            return;
        }

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận xóa phòng",
            $"Bạn có chắc chắn muốn xóa phòng {SelectedRoom.RoomNumber} thuộc {SelectedRoom.Building} không?");

        if (!confirmed) return;

        try
        {
            var success = await _roomService.DeleteRoomAsync(SelectedRoom.Id);
            if (success)
            {
                await LoadRoomsAsync();
                SelectedRoom = null;
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không tìm thấy phòng cần xóa.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Tải danh sách phòng từ cơ sở dữ liệu
    /// </summary>
    public async Task LoadRoomsAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _roomService.GetAllRoomsAsync(SelectedBuilding, SelectedStatus);
            Rooms = new ObservableCollection<RoomDto>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
