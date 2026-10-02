using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý danh sách phòng ở ký túc xá
/// </summary>
public partial class RoomListViewModel : ViewModelBase
{
    private readonly IRoomService _roomService;

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    [ObservableProperty]
    private string? _selectedBuilding;

    [ObservableProperty]
    private RoomStatus? _selectedStatus;

    [ObservableProperty]
    private bool _isLoading;

    public RoomListViewModel(IRoomService roomService)
    {
        _roomService = roomService;
        LoadRoomsCommand = new AsyncRelayCommand(LoadRoomsAsync);
    }

    public IAsyncRelayCommand LoadRoomsCommand { get; }

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
