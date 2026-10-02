using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý màn hình Dashboard tổng quan ký túc xá
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService;

    [ObservableProperty]
    private DashboardStatsDto _stats = new();

    [ObservableProperty]
    private bool _isLoading;

    public DashboardViewModel(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
        LoadStatsCommand = new AsyncRelayCommand(LoadStatsAsync);
    }

    public IAsyncRelayCommand LoadStatsCommand { get; }

    /// <summary>
    /// Tải dữ liệu thống kê từ cơ sở dữ liệu
    /// </summary>
    public async Task LoadStatsAsync()
    {
        IsLoading = true;
        try
        {
            Stats = await _dashboardService.GetStatsAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
