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
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý màn hình Dashboard tổng quan ký túc xá kèm cảnh báo hạn hợp đồng và hóa đơn chưa thanh toán
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IContractService _contractService;
    private readonly IBillService _billService;
    private readonly IExportService _exportService;
    private readonly IFileService _fileService;

    [ObservableProperty]
    private DashboardStatsDto _stats = new();

    [ObservableProperty]
    private ISeries[] _buildingOccupancySeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] _revenueTrendSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] _revenueXAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] _revenueYAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private ObservableCollection<BuildingOccupancyDto> _buildingStats = new();

    [ObservableProperty]
    private int _expiringContractsCount;

    [ObservableProperty]
    private int _unpaidBillsCount;

    [ObservableProperty]
    private ObservableCollection<ContractDto> _expiringContracts = new();

    [ObservableProperty]
    private string _alertMessage = string.Empty;

    [ObservableProperty]
    private bool _hasExpiringContracts;

    [ObservableProperty]
    private bool _hasAlert;

    [ObservableProperty]
    private bool _isLoading;

    public DashboardViewModel(
        IDashboardService dashboardService,
        IContractService contractService,
        IBillService billService,
        IExportService exportService,
        IFileService fileService)
    {
        _dashboardService = dashboardService;
        _contractService = contractService;
        _billService = billService;
        _exportService = exportService;
        _fileService = fileService;
        LoadStatsCommand = new AsyncRelayCommand(LoadStatsAsync);
        ExportDashboardReportCommand = new AsyncRelayCommand(ExportDashboardReportAsync);
        UpdateAlertMessage();
    }

    public IAsyncRelayCommand LoadStatsCommand { get; }
    public IAsyncRelayCommand ExportDashboardReportCommand { get; }

    /// <summary>
    /// Xuất báo cáo tổng hợp Dashboard ra file Excel (.xlsx) và lưu thông qua FileService
    /// </summary>
    public async Task ExportDashboardReportAsync()
    {
        var buildings = BuildingStats.ToList();
        var trends = await _dashboardService.GetRevenueTrendsAsync(6);
        var bytes = await _exportService.ExportDashboardSummaryToExcelAsync(Stats, buildings, trends);
        await _fileService.SaveFileAsync("BaoCao_TongQuan_KTX", "xlsx", "Excel Files (*.xlsx)|*.xlsx", bytes);
    }

    /// <summary>
    /// Cập nhật thông điệp cảnh báo hiển thị trên giao diện
    /// </summary>
    private void UpdateAlertMessage()
    {
        AlertMessage = $"Có {ExpiringContractsCount} hợp đồng sắp hết hạn (trong 30 ngày) và {UnpaidBillsCount} hóa đơn chưa thanh toán cần xử lý!";
        HasExpiringContracts = ExpiringContractsCount > 0;
        HasAlert = ExpiringContractsCount > 0 || UnpaidBillsCount > 0;
    }

    /// <summary>
    /// Tải dữ liệu thống kê, hợp đồng sắp hết hạn và hóa đơn chưa thanh toán
    /// </summary>
    public async Task LoadStatsAsync()
    {
        IsLoading = true;
        try
        {
            Stats = await _dashboardService.GetStatsAsync();

            // Tải danh sách hợp đồng active và lọc hợp đồng sắp hết hạn trong 30 ngày tới
            var activeContracts = await _contractService.GetAllContractsAsync(ContractStatus.Active);
            var today = DateTime.Today;
            var maxDate = today.AddDays(30);
            var expiring = activeContracts
                .Where(c => c.Status == ContractStatus.Active && c.EndDate <= maxDate && c.EndDate >= today)
                .OrderBy(c => c.EndDate)
                .ToList();

            ExpiringContracts = new ObservableCollection<ContractDto>(expiring);
            ExpiringContractsCount = expiring.Count;

            // Tải danh sách hóa đơn chưa thanh toán
            var unpaidBills = await _billService.GetAllBillsAsync(status: BillStatus.Unpaid);
            UnpaidBillsCount = unpaidBills.Count;

            // Tải danh sách thống kê công suất theo từng tòa nhà
            var buildings = await _dashboardService.GetBuildingOccupancyAsync();
            BuildingStats = new ObservableCollection<BuildingOccupancyDto>(buildings);

            if (buildings.Count > 0)
            {
                SKColor[] palette = new[]
                {
                    SKColor.Parse("#0078D4"),
                    SKColor.Parse("#107C41"),
                    SKColor.Parse("#D83B01"),
                    SKColor.Parse("#5C2D91"),
                    SKColor.Parse("#FFB900"),
                    SKColor.Parse("#00B7C3"),
                    SKColor.Parse("#E3008C")
                };

                BuildingOccupancySeries = buildings.Select((b, i) => (ISeries)new PieSeries<int>
                {
                    Name = b.BuildingName,
                    Values = new int[] { Math.Max(0, b.OccupiedBeds) },
                    Fill = new SolidColorPaint(palette[i % palette.Length]),
                    ToolTipLabelFormatter = point => $"{b.BuildingName}: {b.OccupiedBeds}/{b.TotalBeds} chỗ ({b.OccupancyRate:F1}%)"
                }).ToArray();
            }
            else
            {
                BuildingOccupancySeries = Array.Empty<ISeries>();
            }

            // Tải dữ liệu xu hướng doanh thu 6 tháng gần nhất
            var trends = await _dashboardService.GetRevenueTrendsAsync(6);
            if (trends != null && trends.Count > 0)
            {
                RevenueTrendSeries = new ISeries[]
                {
                    new ColumnSeries<decimal>
                    {
                        Name = "Tiền phòng",
                        Values = trends.Select(t => t.RoomFeeRevenue).ToArray(),
                        Fill = new SolidColorPaint(SKColor.Parse("#0078D4")),
                        YToolTipLabelFormatter = point => $"Tiền phòng: {point.Coordinate.PrimaryValue:N0} đ"
                    },
                    new ColumnSeries<decimal>
                    {
                        Name = "Điện nước & Dịch vụ",
                        Values = trends.Select(t => t.UtilityFeeRevenue).ToArray(),
                        Fill = new SolidColorPaint(SKColor.Parse("#107C41")),
                        YToolTipLabelFormatter = point => $"Điện nước & Dịch vụ: {point.Coordinate.PrimaryValue:N0} đ"
                    }
                };

                RevenueXAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = trends.Select(t => t.Label).ToArray(),
                        LabelsRotation = 0,
                        TextSize = 12
                    }
                };

                RevenueYAxes = new Axis[]
                {
                    new Axis
                    {
                        Labeler = value => $"{value:N0} đ",
                        TextSize = 12,
                        MinStep = 500000
                    }
                };
            }
            else
            {
                RevenueTrendSeries = Array.Empty<ISeries>();
                RevenueXAxes = Array.Empty<Axis>();
                RevenueYAxes = Array.Empty<Axis>();
            }

            UpdateAlertMessage();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
