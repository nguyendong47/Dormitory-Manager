using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý danh sách hóa đơn điện, nước và dịch vụ phòng
/// </summary>
public partial class BillListViewModel : ViewModelBase
{
    private readonly IBillService _billService;

    [ObservableProperty]
    private ObservableCollection<BillDto> _bills = new();

    [ObservableProperty]
    private BillDto? _selectedBill;

    [ObservableProperty]
    private int? _selectedMonth;

    [ObservableProperty]
    private int? _selectedYear;

    [ObservableProperty]
    private BillStatus? _selectedStatus;

    [ObservableProperty]
    private bool _isLoading;

    public BillListViewModel(IBillService billService)
    {
        _billService = billService;
        SelectedMonth = DateTime.UtcNow.Month;
        SelectedYear = DateTime.UtcNow.Year;

        LoadBillsCommand = new AsyncRelayCommand(LoadBillsAsync);
        MarkAsPaidCommand = new AsyncRelayCommand(MarkAsPaidSelectedAsync, () => SelectedBill != null && SelectedBill.Status != BillStatus.Paid);
    }

    public IAsyncRelayCommand LoadBillsCommand { get; }
    public IAsyncRelayCommand MarkAsPaidCommand { get; }

    /// <summary>
    /// Tải danh sách hóa đơn theo tháng, năm và trạng thái
    /// </summary>
    public async Task LoadBillsAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _billService.GetAllBillsAsync(month: SelectedMonth, year: SelectedYear, status: SelectedStatus);
            Bills = new ObservableCollection<BillDto>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Xác nhận thu tiền / Đánh dấu đã thanh toán cho hóa đơn được chọn
    /// </summary>
    private async Task MarkAsPaidSelectedAsync()
    {
        if (SelectedBill == null) return;

        var success = await _billService.MarkAsPaidAsync(SelectedBill.Id);
        if (success)
        {
            await LoadBillsAsync();
        }
    }
}
