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
/// ViewModel cho cửa sổ hộp thoại Thêm mới hoặc Chỉnh sửa thông tin phòng ở
/// </summary>
public partial class RoomDialogViewModel : ViewModelBase
{
    private readonly IRoomService _roomService;
    private readonly int? _roomId;

    /// <summary>
    /// Cho biết đang ở chế độ chỉnh sửa hay thêm mới
    /// </summary>
    public bool IsEditMode => _roomId.HasValue;

    [ObservableProperty]
    private string _title = "Thêm phòng mới";

    [ObservableProperty]
    private string _roomNumber = string.Empty;

    [ObservableProperty]
    private string _building = string.Empty;

    [ObservableProperty]
    private int _floor = 1;

    [ObservableProperty]
    private int _capacity = 4;

    [ObservableProperty]
    private decimal _pricePerMonth = 500_000m;

    [ObservableProperty]
    private RoomType _selectedType = RoomType.Standard;

    [ObservableProperty]
    private RoomStatus _selectedStatus = RoomStatus.Available;

    [ObservableProperty]
    private Gender _selectedGender = Gender.Male;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Hành động đóng dialog trả về kết quả (true nếu lưu thành công, false nếu hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Danh sách các loại phòng cho ComboBox
    /// </summary>
    public IReadOnlyList<RoomType> RoomTypes => Enum.GetValues<RoomType>();

    /// <summary>
    /// Danh sách các trạng thái phòng cho ComboBox
    /// </summary>
    public IReadOnlyList<RoomStatus> RoomStatuses => Enum.GetValues<RoomStatus>();

    /// <summary>
    /// Danh sách giới tính áp dụng cho ComboBox
    /// </summary>
    public IReadOnlyList<Gender> RoomGenders => Enum.GetValues<Gender>();

    /// <summary>
    /// Khởi tạo cho chế độ Thêm mới phòng
    /// </summary>
    public RoomDialogViewModel(IRoomService roomService)
    {
        _roomService = roomService;
        _roomId = null;
        Title = "Thêm phòng mới";
    }

    /// <summary>
    /// Khởi tạo cho chế độ Chỉnh sửa phòng hiện có
    /// </summary>
    public RoomDialogViewModel(IRoomService roomService, RoomDto room)
    {
        _roomService = roomService;
        _roomId = room.Id;
        Title = $"Chỉnh sửa phòng {room.RoomNumber}";

        RoomNumber = room.RoomNumber;
        Building = room.Building;
        Floor = room.Floor;
        Capacity = room.Capacity;
        PricePerMonth = room.PricePerMonth;
        SelectedType = room.Type;
        SelectedStatus = room.Status;
        SelectedGender = room.AllowedGender;
        Description = room.Description;
    }

    /// <summary>
    /// Lưu thông tin phòng (Thêm mới hoặc Cập nhật)
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // 1. Kiểm tra validation bắt buộc
            if (string.IsNullOrWhiteSpace(RoomNumber))
            {
                ErrorMessage = "Vui lòng nhập số phòng.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Building))
            {
                ErrorMessage = "Vui lòng nhập tên tòa nhà.";
                return;
            }

            if (Capacity <= 0)
            {
                ErrorMessage = "Sức chứa phòng phải lớn hơn 0.";
                return;
            }

            if (PricePerMonth < 0)
            {
                ErrorMessage = "Đơn giá thuê tháng phải lớn hơn hoặc bằng 0.";
                return;
            }

            ErrorMessage = null;

            // 2. Chuẩn bị request dữ liệu
            var request = new CreateOrUpdateRoomRequest
            {
                RoomNumber = RoomNumber.Trim(),
                Building = Building.Trim(),
                Floor = Floor,
                Capacity = Capacity,
                PricePerMonth = PricePerMonth,
                Type = SelectedType,
                AllowedGender = SelectedGender,
                Description = Description?.Trim()
            };

            if (IsEditMode && _roomId.HasValue)
            {
                var updated = await _roomService.UpdateRoomAsync(_roomId.Value, request);
                if (!updated)
                {
                    ErrorMessage = "Không tìm thấy phòng để cập nhật.";
                    return;
                }

                // Cập nhật trạng thái phòng nếu có thay đổi
                await _roomService.SetRoomStatusAsync(_roomId.Value, SelectedStatus);
            }
            else
            {
                var created = await _roomService.CreateRoomAsync(request);
                // Nếu người dùng chọn trạng thái khác Available khi tạo mới, cập nhật lại trạng thái
                if (SelectedStatus != RoomStatus.Available)
                {
                    await _roomService.SetRoomStatusAsync(created.Id, SelectedStatus);
                }
            }

            // Đóng dialog với kết quả thành công
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            // Hiển thị thông báo lỗi (ví dụ: trùng số phòng)
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
