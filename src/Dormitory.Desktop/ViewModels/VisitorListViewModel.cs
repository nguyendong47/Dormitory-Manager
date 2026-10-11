using System;
using System.Collections.ObjectModel;
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
/// ViewModel quản lý màn hình danh sách lượt khách ra vào KTX và kiểm soát giờ giấc giới nghiêm
/// </summary>
public partial class VisitorListViewModel : ViewModelBase
{
    private readonly IVisitorService _visitorService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IStudentService? _studentService;

    public ObservableCollection<VisitorLogDto> Visitors { get; } = new();

    public ObservableCollection<string> StatusFilterOptions { get; } = new()
    {
        "Tất cả",
        "Trong KTX",
        "Đã rời KTX",
        "Quá giờ"
    };

    [ObservableProperty]
    private string _selectedStatusFilter = "Tất cả";

    public ObservableCollection<RoomDto> RoomFilterOptions { get; } = new();

    [ObservableProperty]
    private RoomDto? _selectedRoomFilter;

    [ObservableProperty]
    private string? _searchKeyword;

    [ObservableProperty]
    private DateTime? _fromDate;

    [ObservableProperty]
    private DateTime? _toDate;

    [ObservableProperty]
    private bool _onlyOverstayed;

    [ObservableProperty]
    private VisitorLogDto? _selectedVisitor;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    // Các thuộc tính thống kê thẻ KPI
    [ObservableProperty]
    private int _activeVisitorsCount;

    [ObservableProperty]
    private int _overstayedCount;

    [ObservableProperty]
    private int _totalVisitorsToday;

    /// <summary>
    /// Điều kiện cho phép check-out khách: Khách đang được chọn và chưa check-out
    /// </summary>
    public bool CanCheckOut => SelectedVisitor != null && SelectedVisitor.Status != VisitorStatus.CheckedOut;

    /// <summary>
    /// Delegate hỗ trợ hiển thị dialog đăng ký khách cho môi trường kiểm thử Headless E2E / Unit Test
    /// </summary>
    public Func<VisitorCheckInDialogViewModel, Task<bool>>? ShowCheckInDialogHandler { get; set; }

    public VisitorListViewModel(
        IVisitorService visitorService,
        IRoomService roomService,
        IDialogService dialogService,
        IStudentService? studentService = null)
    {
        _visitorService = visitorService ?? throw new ArgumentNullException(nameof(visitorService));
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _studentService = studentService;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadRoomsFilterAsync();
        await LoadVisitorsAsync();
    }

    partial void OnSelectedVisitorChanged(VisitorLogDto? value)
    {
        OnPropertyChanged(nameof(CanCheckOut));
    }

    /// <summary>
    /// Tải danh sách phòng vào bộ lọc tìm kiếm
    /// </summary>
    [RelayCommand]
    public async Task LoadRoomsFilterAsync()
    {
        try
        {
            RoomFilterOptions.Clear();
            var rooms = await _roomService.GetAllRoomsAsync();
            foreach (var room in rooms)
            {
                RoomFilterOptions.Add(room);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi tải bộ lọc phòng: {ex.Message}";
        }
    }

    /// <summary>
    /// Tải danh sách khách ra vào theo các điều kiện lọc và cập nhật số liệu thẻ KPI
    /// </summary>
    [RelayCommand]
    public async Task LoadVisitorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang tải danh sách khách ra vào...";

            VisitorStatus? status = SelectedStatusFilter switch
            {
                "Trong KTX" => VisitorStatus.InDormitory,
                "Đã rời KTX" => VisitorStatus.CheckedOut,
                "Quá giờ" => VisitorStatus.Overstayed,
                _ => null
            };

            var filter = new VisitorFilterDto
            {
                Status = status,
                RoomId = SelectedRoomFilter?.Id,
                SearchKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ? null : SearchKeyword.Trim(),
                FromDate = FromDate,
                ToDate = ToDate,
                OnlyOverstayed = OnlyOverstayed
            };

            var list = await _visitorService.GetVisitorsAsync(filter);

            Visitors.Clear();
            foreach (var item in list)
            {
                Visitors.Add(item);
            }

            // Cập nhật số liệu tổng hợp các thẻ KPI
            var summary = await _visitorService.GetVisitorSummaryAsync();
            ActiveVisitorsCount = summary.ActiveVisitorsCount;
            OverstayedCount = summary.OverstayedCount;
            TotalVisitorsToday = summary.TotalVisitorsToday;

            StatusMessage = $"Đã tải {Visitors.Count} lượt khách.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi tải dữ liệu khách: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tải danh sách khách: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Mở hộp thoại đăng ký khách mới (Check-in)
    /// </summary>
    [RelayCommand]
    public async Task CheckInAsync()
    {
        if (_studentService == null)
        {
            await _dialogService.ShowMessageAsync("Lỗi", "Không tìm thấy dịch vụ quản lý sinh viên.");
            return;
        }

        var dialogVm = new VisitorCheckInDialogViewModel(
            _visitorService,
            _roomService,
            _studentService,
            _dialogService);

        bool isSuccess = false;

        if (ShowCheckInDialogHandler != null)
        {
            isSuccess = await ShowCheckInDialogHandler.Invoke(dialogVm);
        }
        else
        {
            var dialogWindow = new VisitorCheckInDialogWindow(dialogVm);
            isSuccess = await _dialogService.ShowDialogAsync<bool>(dialogWindow);
        }

        if (isSuccess)
        {
            await LoadVisitorsAsync();
            await _dialogService.ShowMessageAsync("Thành công", "Đăng ký khách vào KTX thành công!");
        }
    }

    /// <summary>
    /// Ghi nhận khách rời khỏi KTX (Check-out)
    /// </summary>
    [RelayCommand]
    public async Task CheckOutAsync()
    {
        if (!CanCheckOut || SelectedVisitor == null)
        {
            await _dialogService.ShowMessageAsync("Thông báo", "Vui lòng chọn khách đang lưu trú để check-out.");
            return;
        }

        var confirm = await _dialogService.ShowConfirmAsync(
            "Xác nhận Check-out",
            $"Bạn có chắc chắn muốn ghi nhận khách '{SelectedVisitor.VisitorName}' rời khỏi KTX?");

        if (!confirm)
        {
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Đang xử lý check-out khách...";

            await _visitorService.CheckOutAsync(SelectedVisitor.Id);
            await LoadVisitorsAsync();

            await _dialogService.ShowMessageAsync("Thành công", $"Khách '{SelectedVisitor.VisitorName}' đã rời khỏi KTX thành công!");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi check-out: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể check-out khách: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Quét và tự động đánh dấu các lượt khách lưu trú quá giờ quy định (sau 22:00)
    /// </summary>
    [RelayCommand]
    public async Task MarkOverstayedAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang quét danh sách khách quá giờ quy định...";

            var count = await _visitorService.MarkOverstayedVisitorsAsync();
            await LoadVisitorsAsync();

            await _dialogService.ShowMessageAsync("Quét quá giờ", $"Đã phát hiện và gắn cờ quá giờ cho {count} lượt khách lưu trú sau 22:00.");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi quét quá giờ: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể quét quá giờ: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Làm mới bộ lọc và tải lại dữ liệu danh sách khách
    /// </summary>
    [RelayCommand]
    public async Task RefreshAsync()
    {
        SearchKeyword = string.Empty;
        SelectedStatusFilter = "Tất cả";
        SelectedRoomFilter = null;
        FromDate = null;
        ToDate = null;
        OnlyOverstayed = false;

        await LoadRoomsFilterAsync();
        await LoadVisitorsAsync();
    }
}
