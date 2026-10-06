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
/// Tùy chọn mục hiển thị cho ComboBox
/// </summary>
public record ReportTypeOption(ReportType Value, string DisplayName);
public record ReportFormatOption(ReportFormat Value, string DisplayName);
public record SeverityFilterOption(ViolationSeverity? Value, string DisplayName);
public record ViolationStatusFilterOption(ViolationStatus? Value, string DisplayName);
public record RoomTypeFilterOption(RoomType? Value, string DisplayName);
public record EquipmentStatusFilterOption(EquipmentStatus? Value, string DisplayName);

/// <summary>
/// ViewModel cho hộp thoại cấu hình và sinh báo cáo KTX
/// </summary>
public partial class ReportGenerateDialogViewModel : ViewModelBase
{
    private readonly IReportService _reportService;
    private readonly IRoomService? _roomService;
    private List<RoomDto> _allRooms = new();
    private bool _isTitleCustomized = false;

    [ObservableProperty]
    private ReportType _selectedReportType = ReportType.Violations;

    [ObservableProperty]
    private ReportTypeOption? _selectedReportTypeOption;

    [ObservableProperty]
    private ReportFormat _selectedFormat = ReportFormat.Pdf;

    [ObservableProperty]
    private ReportFormatOption? _selectedFormatOption;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _fromDate;

    [ObservableProperty]
    private DateTimeOffset? _toDate;

    [ObservableProperty]
    private int _selectedMonth = DateTime.Today.Month;

    [ObservableProperty]
    private int _selectedYear = DateTime.Today.Year;

    [ObservableProperty]
    private ObservableCollection<BuildingDto> _buildings = new();

    [ObservableProperty]
    private BuildingDto? _selectedBuilding;

    [ObservableProperty]
    private ObservableCollection<RoomDto> _rooms = new();

    [ObservableProperty]
    private RoomDto? _selectedRoom;

    // Bộ lọc chuyên biệt Vi phạm
    [ObservableProperty]
    private ViolationSeverity? _selectedSeverity;

    [ObservableProperty]
    private SeverityFilterOption? _selectedSeverityOption;

    [ObservableProperty]
    private ViolationStatus? _selectedViolationStatus;

    [ObservableProperty]
    private ViolationStatusFilterOption? _selectedViolationStatusOption;

    // Bộ lọc chuyên biệt Tài chính
    [ObservableProperty]
    private bool _onlyOverdue;

    // Bộ lọc chuyên biệt Lấp đầy
    [ObservableProperty]
    private RoomType? _selectedRoomType;

    [ObservableProperty]
    private RoomTypeFilterOption? _selectedRoomTypeOption;

    [ObservableProperty]
    private bool _onlyVacant;

    // Bộ lọc chuyên biệt Thiết bị
    [ObservableProperty]
    private EquipmentStatus? _selectedEquipmentStatus;

    [ObservableProperty]
    private EquipmentStatusFilterOption? _selectedEquipmentStatusOption;

    // Trạng thái giao diện
    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Các cờ nhận diện loại báo cáo phục vụ hiển thị động trên UI
    /// </summary>
    public bool IsViolationsReport => SelectedReportType == ReportType.Violations;
    public bool IsFinancialReport => SelectedReportType == ReportType.Financial;
    public bool IsOccupancyReport => SelectedReportType == ReportType.Occupancy;
    public bool IsEquipmentReport => SelectedReportType == ReportType.Equipment;

    /// <summary>
    /// Hành động đóng dialog kèm cờ kết quả (true = thành công, false = hủy)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    // Danh sách tùy chọn hiển thị
    public IReadOnlyList<ReportTypeOption> ReportTypeOptions { get; } = new[]
    {
        new ReportTypeOption(ReportType.Violations, "⚖️ Vi phạm nội quy & Kỷ luật"),
        new ReportTypeOption(ReportType.Financial, "💵 Tài chính & Thu phí KTX"),
        new ReportTypeOption(ReportType.Occupancy, "🏠 Tỷ lệ lấp đầy & Sức chứa"),
        new ReportTypeOption(ReportType.Equipment, "🛋️ Kiểm kê tài sản & Thiết bị")
    };

    public IReadOnlyList<ReportFormatOption> ReportFormatOptions { get; } = new[]
    {
        new ReportFormatOption(ReportFormat.Pdf, "📄 Adobe PDF (.pdf)"),
        new ReportFormatOption(ReportFormat.Excel, "📊 Microsoft Excel (.xlsx)")
    };

    public IReadOnlyList<int> MonthOptions { get; } = Enumerable.Range(1, 12).ToList();
    public IReadOnlyList<int> YearOptions { get; } = Enumerable.Range(DateTime.Today.Year - 3, 5).ToList();

    public IReadOnlyList<SeverityFilterOption> SeverityOptions { get; } = new[]
    {
        new SeverityFilterOption(null, "-- Tất cả mức độ --"),
        new SeverityFilterOption(ViolationSeverity.Minor, "Nhắc nhở (Mức nhẹ)"),
        new SeverityFilterOption(ViolationSeverity.Moderate, "Khiển trách (Mức vừa)"),
        new SeverityFilterOption(ViolationSeverity.Severe, "Cảnh cáo (Nghiêm trọng)"),
        new SeverityFilterOption(ViolationSeverity.Critical, "Buộc rời KTX (Rất nghiêm trọng)")
    };

    public IReadOnlyList<ViolationStatusFilterOption> ViolationStatusOptions { get; } = new[]
    {
        new ViolationStatusFilterOption(null, "-- Tất cả trạng thái --"),
        new ViolationStatusFilterOption(ViolationStatus.Pending, "Chờ xử lý"),
        new ViolationStatusFilterOption(ViolationStatus.Resolved, "Đã xử lý"),
        new ViolationStatusFilterOption(ViolationStatus.Dismissed, "Hủy bỏ")
    };

    public IReadOnlyList<RoomTypeFilterOption> RoomTypeOptions { get; } = new[]
    {
        new RoomTypeFilterOption(null, "-- Tất cả loại phòng --"),
        new RoomTypeFilterOption(RoomType.Standard, "Tiêu chuẩn"),
        new RoomTypeFilterOption(RoomType.Premium, "Chất lượng cao"),
        new RoomTypeFilterOption(RoomType.Vip, "VIP / Đặc biệt")
    };

    public IReadOnlyList<EquipmentStatusFilterOption> EquipmentStatusOptions { get; } = new[]
    {
        new EquipmentStatusFilterOption(null, "-- Tất cả tình trạng --"),
        new EquipmentStatusFilterOption(EquipmentStatus.Good, "Hoạt động tốt"),
        new EquipmentStatusFilterOption(EquipmentStatus.NeedsRepair, "Cần bảo trì / sửa chữa"),
        new EquipmentStatusFilterOption(EquipmentStatus.Broken, "Hỏng hóc")
    };

    public ReportGenerateDialogViewModel(
        IReportService reportService,
        IRoomService? roomService = null)
    {
        _reportService = reportService;
        _roomService = roomService;

        _selectedReportTypeOption = ReportTypeOptions[0];
        _selectedFormatOption = ReportFormatOptions[0];
        _selectedSeverityOption = SeverityOptions[0];
        _selectedViolationStatusOption = ViolationStatusOptions[0];
        _selectedRoomTypeOption = RoomTypeOptions[0];
        _selectedEquipmentStatusOption = EquipmentStatusOptions[0];
    }

    /// <summary>
    /// Khởi tạo dữ liệu và cấu hình ban đầu
    /// </summary>
    public async Task InitializeAsync(ReportType initialType = ReportType.Violations)
    {
        SelectedReportType = initialType;
        SelectedReportTypeOption = ReportTypeOptions.FirstOrDefault(o => o.Value == initialType) ?? ReportTypeOptions[0];
        SelectedFormatOption = ReportFormatOptions.FirstOrDefault(o => o.Value == SelectedFormat) ?? ReportFormatOptions[0];

        ApplyPresetThisMonth();

        if (_roomService != null)
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                _allRooms = rooms;

                var buildingNames = rooms
                    .Select(r => r.Building)
                    .Where(b => !string.IsNullOrWhiteSpace(b))
                    .Distinct()
                    .OrderBy(b => b)
                    .ToList();

                var buildingList = new List<BuildingDto>
                {
                    new BuildingDto("", "-- Tất cả tòa nhà --")
                };

                foreach (var b in buildingNames)
                {
                    buildingList.Add(new BuildingDto(b));
                }

                Buildings = new ObservableCollection<BuildingDto>(buildingList);
                SelectedBuilding = Buildings.FirstOrDefault();

                UpdateRoomCollection();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải dữ liệu phòng & tòa nhà: {ex.Message}";
            }
        }

        UpdateSuggestedTitle();
    }

    partial void OnSelectedReportTypeChanged(ReportType value)
    {
        if (SelectedReportTypeOption?.Value != value)
        {
            SelectedReportTypeOption = ReportTypeOptions.FirstOrDefault(o => o.Value == value);
        }

        OnPropertyChanged(nameof(IsViolationsReport));
        OnPropertyChanged(nameof(IsFinancialReport));
        OnPropertyChanged(nameof(IsOccupancyReport));
        OnPropertyChanged(nameof(IsEquipmentReport));

        _isTitleCustomized = false;
        UpdateSuggestedTitle();
    }

    partial void OnSelectedReportTypeOptionChanged(ReportTypeOption? value)
    {
        if (value != null && SelectedReportType != value.Value)
        {
            SelectedReportType = value.Value;
        }
    }

    partial void OnSelectedFormatChanged(ReportFormat value)
    {
        if (SelectedFormatOption?.Value != value)
        {
            SelectedFormatOption = ReportFormatOptions.FirstOrDefault(o => o.Value == value);
        }
    }

    partial void OnSelectedFormatOptionChanged(ReportFormatOption? value)
    {
        if (value != null && SelectedFormat != value.Value)
        {
            SelectedFormat = value.Value;
        }
    }

    partial void OnSelectedSeverityOptionChanged(SeverityFilterOption? value)
    {
        SelectedSeverity = value?.Value;
    }

    partial void OnSelectedViolationStatusOptionChanged(ViolationStatusFilterOption? value)
    {
        SelectedViolationStatus = value?.Value;
    }

    partial void OnSelectedRoomTypeOptionChanged(RoomTypeFilterOption? value)
    {
        SelectedRoomType = value?.Value;
    }

    partial void OnSelectedEquipmentStatusOptionChanged(EquipmentStatusFilterOption? value)
    {
        SelectedEquipmentStatus = value?.Value;
    }

    partial void OnSelectedMonthChanged(int value)
    {
        UpdateSuggestedTitle();
    }

    partial void OnSelectedYearChanged(int value)
    {
        UpdateSuggestedTitle();
    }

    partial void OnSelectedBuildingChanged(BuildingDto? value)
    {
        UpdateRoomCollection();
    }

    private void UpdateRoomCollection()
    {
        if (_allRooms.Count == 0) return;

        var filtered = string.IsNullOrEmpty(SelectedBuilding?.Name)
            ? _allRooms
            : _allRooms.Where(r => r.Building == SelectedBuilding.Name).ToList();

        var roomList = new List<RoomDto>
        {
            new RoomDto { Id = 0, RoomNumber = "-- Tất cả phòng --" }
        };
        roomList.AddRange(filtered);

        Rooms = new ObservableCollection<RoomDto>(roomList);
        SelectedRoom = Rooms.FirstOrDefault();
    }

    partial void OnTitleChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value != GetSuggestedTitle())
        {
            _isTitleCustomized = true;
        }
    }

    /// <summary>
    /// Lấy tiêu đề mặc định gợi ý theo cấu hình hiện tại
    /// </summary>
    public string GetSuggestedTitle()
    {
        return SelectedReportType switch
        {
            ReportType.Violations => $"Báo cáo vi phạm kỷ luật tháng {SelectedMonth:D2}/{SelectedYear}",
            ReportType.Financial => $"Báo cáo tài chính & thu phí tháng {SelectedMonth:D2}/{SelectedYear}",
            ReportType.Occupancy => $"Báo cáo tỷ lệ lấp đầy KTX tháng {SelectedMonth:D2}/{SelectedYear}",
            ReportType.Equipment => $"Báo cáo kiểm kê tài sản thiết bị năm {SelectedYear}",
            _ => $"Báo cáo tổng hợp tháng {SelectedMonth:D2}/{SelectedYear}"
        };
    }

    /// <summary>
    /// Tự động sinh tiêu đề báo cáo gợi ý
    /// </summary>
    public void UpdateSuggestedTitle()
    {
        if (_isTitleCustomized) return;
        Title = GetSuggestedTitle();
    }

    /// <summary>
    /// Áp dụng mẫu nhanh: Tháng này
    /// </summary>
    [RelayCommand]
    public void ApplyPresetThisMonth()
    {
        var now = DateTime.Today;
        SelectedMonth = now.Month;
        SelectedYear = now.Year;
        FromDate = new DateTimeOffset(new DateTime(now.Year, now.Month, 1));
        ToDate = DateTimeOffset.Now;
        _isTitleCustomized = false;
        UpdateSuggestedTitle();
    }

    /// <summary>
    /// Áp dụng mẫu nhanh: Tháng trước
    /// </summary>
    [RelayCommand]
    public void ApplyPresetLastMonth()
    {
        var last = DateTime.Today.AddMonths(-1);
        SelectedMonth = last.Month;
        SelectedYear = last.Year;
        FromDate = new DateTimeOffset(new DateTime(last.Year, last.Month, 1));
        ToDate = new DateTimeOffset(new DateTime(last.Year, last.Month, DateTime.DaysInMonth(last.Year, last.Month), 23, 59, 59));
        _isTitleCustomized = false;
        UpdateSuggestedTitle();
    }

    /// <summary>
    /// Áp dụng mẫu nhanh: Năm nay
    /// </summary>
    [RelayCommand]
    public void ApplyPresetThisYear()
    {
        var now = DateTime.Today;
        SelectedYear = now.Year;
        FromDate = new DateTimeOffset(new DateTime(now.Year, 1, 1));
        ToDate = DateTimeOffset.Now;
        _isTitleCustomized = false;
        UpdateSuggestedTitle();
    }

    /// <summary>
    /// Lệnh khởi tạo và lưu báo cáo
    /// </summary>
    [RelayCommand]
    public async Task GenerateAsync()
    {
        if (IsGenerating) return;

        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Tiêu đề báo cáo không được để trống.";
            return;
        }

        if (FromDate.HasValue && ToDate.HasValue && FromDate.Value > ToDate.Value)
        {
            ErrorMessage = "Từ ngày không được lớn hơn Đến ngày.";
            return;
        }

        IsGenerating = true;
        ErrorMessage = null;

        try
        {
            var request = new GenerateReportRequestDto
            {
                ReportType = SelectedReportType,
                Format = SelectedFormat,
                Title = Title.Trim(),
                FromDate = FromDate?.DateTime,
                ToDate = ToDate?.DateTime,
                Month = SelectedMonth > 0 ? SelectedMonth : null,
                Year = SelectedYear > 0 ? SelectedYear : null,
                Building = string.IsNullOrEmpty(SelectedBuilding?.Name) ? null : SelectedBuilding.Name,
                RoomId = SelectedRoom?.Id > 0 ? SelectedRoom.Id : null,
                Severity = SelectedReportType == ReportType.Violations ? SelectedSeverity : null,
                ViolationStatus = SelectedReportType == ReportType.Violations ? SelectedViolationStatus : null,
                OnlyOverdue = SelectedReportType == ReportType.Financial ? OnlyOverdue : null,
                RoomType = SelectedReportType == ReportType.Occupancy ? SelectedRoomType : null,
                OnlyVacant = SelectedReportType == ReportType.Occupancy ? OnlyVacant : null,
                EquipmentStatus = SelectedReportType == ReportType.Equipment ? SelectedEquipmentStatus : null
            };

            await _reportService.GenerateAndSaveReportAsync(request);
            CloseAction?.Invoke(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi tạo báo cáo: {ex.Message}";
        }
        finally
        {
            IsGenerating = false;
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
