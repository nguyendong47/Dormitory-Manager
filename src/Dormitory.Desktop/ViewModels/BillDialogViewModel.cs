using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel cho cửa sổ hộp thoại lập hóa đơn điện nước và dịch vụ hàng tháng
/// </summary>
public partial class BillDialogViewModel : ViewModelBase
{
    private readonly IBillService _billService;
    private readonly IRoomService _roomService;

    [ObservableProperty]
    private string _title = "Lập hóa đơn tiền phòng & dịch vụ";

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    [ObservableProperty]
    private int _month = DateTime.Today.Month;

    [ObservableProperty]
    private int _year = DateTime.Today.Year;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _roomFee;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedElectricUsage))]
    [NotifyPropertyChangedFor(nameof(CalculatedElectricAmount))]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _oldElectricIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedElectricUsage))]
    [NotifyPropertyChangedFor(nameof(CalculatedElectricAmount))]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _newElectricIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedElectricAmount))]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _electricRate = 3500m;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedWaterUsage))]
    [NotifyPropertyChangedFor(nameof(CalculatedWaterAmount))]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _oldWaterIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedWaterUsage))]
    [NotifyPropertyChangedFor(nameof(CalculatedWaterAmount))]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _newWaterIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalculatedWaterAmount))]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _waterRate = 12000m;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalAmount))]
    private decimal _otherServiceFee = 50000m;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Hành động đóng dialog trả về kết quả (true nếu lưu thành công, false nếu hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Lượng điện tiêu thụ tự động tính toán (kWh)
    /// </summary>
    public decimal CalculatedElectricUsage => Math.Max(0, NewElectricIndex - OldElectricIndex);

    /// <summary>
    /// Thành tiền điện tiêu thụ (VNĐ)
    /// </summary>
    public decimal CalculatedElectricAmount => CalculatedElectricUsage * ElectricRate;

    /// <summary>
    /// Lượng nước tiêu thụ tự động tính toán (m3)
    /// </summary>
    public decimal CalculatedWaterUsage => Math.Max(0, NewWaterIndex - OldWaterIndex);

    /// <summary>
    /// Thành tiền nước tiêu thụ (VNĐ)
    /// </summary>
    public decimal CalculatedWaterAmount => CalculatedWaterUsage * WaterRate;

    /// <summary>
    /// Tổng tiền hóa đơn tự động tính toán theo thời gian thực (VNĐ)
    /// </summary>
    public decimal TotalAmount => RoomFee + CalculatedElectricAmount + CalculatedWaterAmount + OtherServiceFee;

    public BillDialogViewModel(IBillService billService, IRoomService roomService)
    {
        _billService = billService;
        _roomService = roomService;
    }

    /// <summary>
    /// Khi chọn phòng khác, tự động cập nhật tiền phòng theo giá niêm yết
    /// </summary>
    partial void OnSelectedRoomChanged(RoomDto? value)
    {
        if (value != null)
        {
            RoomFee = value.PricePerMonth;
        }
    }

    /// <summary>
    /// Tải danh sách phòng để chọn khi lập hóa đơn
    /// </summary>
    public async Task LoadInitialDataAsync()
    {
        try
        {
            var roomList = await _roomService.GetAllRoomsAsync();
            Rooms = new ObservableCollection<RoomDto>(roomList);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Không thể nạp danh sách phòng: {ex.Message}";
        }
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ và lưu hóa đơn mới vào cơ sở dữ liệu
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // 1. Kiểm tra validation bắt buộc
            if (SelectedRoom == null)
            {
                ErrorMessage = "Vui lòng chọn phòng cần lập hóa đơn.";
                return;
            }

            if (Month < 1 || Month > 12)
            {
                ErrorMessage = "Tháng không hợp lệ (phải từ 1 đến 12).";
                return;
            }

            if (Year < 2000 || Year > 2100)
            {
                ErrorMessage = "Năm không hợp lệ.";
                return;
            }

            if (NewElectricIndex < OldElectricIndex)
            {
                ErrorMessage = "Chỉ số điện mới không được nhỏ hơn chỉ số cũ.";
                return;
            }

            if (NewWaterIndex < OldWaterIndex)
            {
                ErrorMessage = "Chỉ số nước mới không được nhỏ hơn chỉ số cũ.";
                return;
            }

            if (RoomFee < 0)
            {
                ErrorMessage = "Tiền phòng không được nhỏ hơn 0.";
                return;
            }

            if (ElectricRate < 0)
            {
                ErrorMessage = "Đơn giá điện không được nhỏ hơn 0.";
                return;
            }

            if (WaterRate < 0)
            {
                ErrorMessage = "Đơn giá nước không được nhỏ hơn 0.";
                return;
            }

            if (OtherServiceFee < 0)
            {
                ErrorMessage = "Phí dịch vụ khác không được nhỏ hơn 0.";
                return;
            }

            ErrorMessage = null;

            // Ngày đến hạn mặc định là ngày 10 của tháng sau hoặc 15 ngày sau khi lập
            DateTime dueDate;
            try
            {
                dueDate = new DateTime(Year, Month, 1).AddMonths(1).AddDays(9);
            }
            catch
            {
                dueDate = DateTime.Today.AddDays(15);
            }

            // 2. Chuẩn bị request dữ liệu
            var request = new CreateBillRequest
            {
                RoomId = SelectedRoom.Id,
                Month = Month,
                Year = Year,
                RoomFee = RoomFee,
                OldElectricIndex = OldElectricIndex,
                NewElectricIndex = NewElectricIndex,
                ElectricRate = ElectricRate,
                OldWaterIndex = OldWaterIndex,
                NewWaterIndex = NewWaterIndex,
                WaterRate = WaterRate,
                OtherServiceFee = OtherServiceFee,
                Note = Notes,
                DueDate = dueDate
            };

            await _billService.CreateBillAsync(request);
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
