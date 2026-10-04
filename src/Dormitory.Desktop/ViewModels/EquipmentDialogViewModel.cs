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
/// ViewModel cho hộp thoại modal thêm mới hoặc chỉnh sửa trang thiết bị, tài sản phòng ở
/// </summary>
public partial class EquipmentDialogViewModel : ViewModelBase
{
    private readonly IEquipmentService _equipmentService;
    private readonly IRoomService _roomService;

    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _roomId;

    [ObservableProperty]
    private string _equipmentCode = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private EquipmentStatus _status = EquipmentStatus.Good;

    [ObservableProperty]
    private int _quantity = 1;

    [ObservableProperty]
    private decimal _price = 0m;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _title = "Thêm thiết bị mới";

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    /// <summary>
    /// Hành động đóng dialog trả về kết quả (true nếu lưu thành công, false nếu hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Danh sách các trạng thái thiết bị cho ComboBox
    /// </summary>
    public IReadOnlyList<EquipmentStatus> EquipmentStatuses => Enum.GetValues<EquipmentStatus>();

    public EquipmentDialogViewModel(IEquipmentService equipmentService, IRoomService roomService)
    {
        _equipmentService = equipmentService;
        _roomService = roomService;
    }

    /// <summary>
    /// Tự động cập nhật RoomId khi người dùng chọn phòng từ danh sách
    /// </summary>
    partial void OnSelectedRoomChanged(RoomDto? value)
    {
        if (value != null)
        {
            RoomId = value.Id;
        }
    }

    /// <summary>
    /// Nạp danh sách phòng và điền dữ liệu thiết bị nếu ở chế độ chỉnh sửa
    /// </summary>
    public async Task InitializeAsync(EquipmentDto? existing = null)
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var roomList = await _roomService.GetAllRoomsAsync();
            Rooms = new ObservableCollection<RoomDto>(roomList);

            if (existing != null)
            {
                Id = existing.Id;
                RoomId = existing.RoomId;
                EquipmentCode = existing.EquipmentCode;
                Name = existing.Name;
                Status = existing.Status;
                Quantity = existing.Quantity;
                Price = existing.Price;
                Notes = existing.Notes;
                IsEditMode = true;
                Title = $"Chỉnh sửa thiết bị {existing.EquipmentCode}";

                SelectedRoom = Rooms.FirstOrDefault(r => r.Id == existing.RoomId);
            }
            else
            {
                Id = 0;
                EquipmentCode = string.Empty;
                Name = string.Empty;
                Status = EquipmentStatus.Good;
                Quantity = 1;
                Price = 0m;
                Notes = null;
                IsEditMode = false;
                Title = "Thêm thiết bị mới";

                if (Rooms.Count > 0)
                {
                    SelectedRoom = Rooms[0];
                    RoomId = SelectedRoom.Id;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể tải danh sách phòng: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ và lưu thông tin trang thiết bị
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsBusy) return;

        // 1. Kiểm tra dữ liệu bắt buộc
        if (SelectedRoom == null || RoomId <= 0)
        {
            ErrorMessage = "Vui lòng chọn phòng đặt thiết bị.";
            return;
        }

        if (string.IsNullOrWhiteSpace(EquipmentCode))
        {
            ErrorMessage = "Mã thiết bị không được để trống.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Tên thiết bị không được để trống.";
            return;
        }

        if (Quantity <= 0)
        {
            ErrorMessage = "Số lượng thiết bị phải lớn hơn 0.";
            return;
        }

        if (Price < 0)
        {
            ErrorMessage = "Đơn giá thiết bị không được âm.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsEditMode)
            {
                var updateDto = new UpdateEquipmentDto
                {
                    RoomId = RoomId,
                    EquipmentCode = EquipmentCode.Trim(),
                    Name = Name.Trim(),
                    Status = Status,
                    Quantity = Quantity,
                    Price = Price,
                    Notes = Notes?.Trim()
                };

                await _equipmentService.UpdateEquipmentAsync(Id, updateDto);
            }
            else
            {
                var createDto = new CreateEquipmentDto
                {
                    RoomId = RoomId,
                    EquipmentCode = EquipmentCode.Trim(),
                    Name = Name.Trim(),
                    Status = Status,
                    Quantity = Quantity,
                    Price = Price,
                    Notes = Notes?.Trim()
                };

                await _equipmentService.CreateEquipmentAsync(createDto);
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
    /// Hủy bỏ thao tác và đóng hộp thoại
    /// </summary>
    [RelayCommand]
    public void Cancel()
    {
        CloseAction?.Invoke(false);
    }
}
