using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
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

    [ObservableProperty]
    private DashboardStatsDto _stats = new();

    [ObservableProperty]
    private ISeries[] _buildingOccupancySeries = Array.Empty<ISeries>();

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
        IBillService billService)
    {
        _dashboardService = dashboardService;
        _contractService = contractService;
        _billService = billService;
        LoadStatsCommand = new AsyncRelayCommand(LoadStatsAsync);
        UpdateAlertMessage();
    }

    public IAsyncRelayCommand LoadStatsCommand { get; }

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
                    Values = new int[] { b.OccupiedBeds > 0 ? b.OccupiedBeds : (b.TotalBeds > 0 ? 0 : 1) },
                    Fill = new SolidColorPaint(palette[i % palette.Length]),
                    ToolTipLabelFormatter = point => $"{b.BuildingName}: {b.OccupiedBeds}/{b.TotalBeds} chỗ ({b.OccupancyRate:F1}%)"
                }).ToArray();
            }
            else
            {
                BuildingOccupancySeries = Array.Empty<ISeries>();
            }

            UpdateAlertMessage();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
