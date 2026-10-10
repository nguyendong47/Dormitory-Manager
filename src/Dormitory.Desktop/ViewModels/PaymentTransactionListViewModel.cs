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
using Dormitory.Desktop.Views;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý màn hình danh sách lịch sử giao dịch đối soát ngân hàng và gán thủ công hóa đơn
/// </summary>
public partial class PaymentTransactionListViewModel : ViewModelBase
{
    private readonly IPaymentReconciliationService _reconciliationService;
    private readonly IBillService _billService;
    private readonly IDialogService _dialogService;

    public ObservableCollection<PaymentTransactionDto> Transactions { get; } = new();

    public ObservableCollection<string> StatusFilterOptions { get; } = new()
    {
        "Tất cả",
        "Thành công",
        "Chưa khớp",
        "Thanh toán một phần",
        "Trùng lặp",
        "Thất bại"
    };

    [ObservableProperty]
    private string _selectedStatusFilter = "Tất cả";

    [ObservableProperty]
    private DateTime? _fromDate;

    [ObservableProperty]
    private DateTime? _toDate;

    [ObservableProperty]
    private string? _searchKeyword;

    [ObservableProperty]
    private PaymentTransactionDto? _selectedTransaction;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    // KPI Cards Properties
    [ObservableProperty]
    private decimal _totalReceivedAmount;

    [ObservableProperty]
    private int _successCount;

    [ObservableProperty]
    private int _unmatchedCount;

    /// <summary>
    /// Điều kiện cho phép gán hóa đơn: Giao dịch được chọn và có trạng thái Chưa khớp (Unmatched)
    /// </summary>
    public bool CanAssignBill => SelectedTransaction != null && SelectedTransaction.Status == PaymentTransactionStatus.Unmatched;

    /// <summary>
    /// Delegate hỗ trợ hiển thị dialog gán hóa đơn cho môi trường kiểm thử Headless Unit Test
    /// </summary>
    public Func<AssignBillDialogViewModel, Task<bool>>? ShowAssignBillDialogHandler { get; set; }

    public PaymentTransactionListViewModel(
        IPaymentReconciliationService reconciliationService,
        IBillService billService,
        IDialogService dialogService)
    {
        _reconciliationService = reconciliationService ?? throw new ArgumentNullException(nameof(reconciliationService));
        _billService = billService ?? throw new ArgumentNullException(nameof(billService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        _ = LoadTransactionsAsync();
    }

    partial void OnSelectedTransactionChanged(PaymentTransactionDto? value)
    {
        OnPropertyChanged(nameof(CanAssignBill));
    }

    /// <summary>
    /// Tải danh sách giao dịch từ cơ sở dữ liệu theo các điều kiện lọc và cập nhật các thẻ KPI
    /// </summary>
    [RelayCommand]
    public async Task LoadTransactionsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang tải dữ liệu giao dịch đối soát...";

            PaymentTransactionStatus? statusFilter = SelectedStatusFilter switch
            {
                "Thành công" => PaymentTransactionStatus.Success,
                "Chưa khớp" => PaymentTransactionStatus.Unmatched,
                "Thanh toán một phần" => PaymentTransactionStatus.PartiallyPaid,
                "Trùng lặp" => PaymentTransactionStatus.Duplicate,
                "Thất bại" => PaymentTransactionStatus.Failed,
                _ => null
            };

            var filter = new PaymentTransactionFilterDto
            {
                Status = statusFilter,
                FromDate = FromDate,
                ToDate = ToDate,
                SearchKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ? null : SearchKeyword.Trim()
            };

            var list = await _reconciliationService.GetTransactionsAsync(filter);

            Transactions.Clear();
            foreach (var item in list)
            {
                Transactions.Add(item);
            }

            // Tính toán số liệu thống kê KPI
            TotalReceivedAmount = Transactions
                .Where(t => t.Status == PaymentTransactionStatus.Success || t.Status == PaymentTransactionStatus.PartiallyPaid)
                .Sum(t => t.Amount);

            SuccessCount = Transactions.Count(t => t.Status == PaymentTransactionStatus.Success);
            UnmatchedCount = Transactions.Count(t => t.Status == PaymentTransactionStatus.Unmatched);

            StatusMessage = $"Đã tải {Transactions.Count} giao dịch đối soát ngân hàng.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi tải giao dịch: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tải danh sách giao dịch: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Mở hộp thoại gán hóa đơn thủ công cho giao dịch đang chọn
    /// </summary>
    [RelayCommand]
    public async Task AssignBillAsync()
    {
        if (!CanAssignBill || SelectedTransaction == null)
        {
            await _dialogService.ShowMessageAsync("Thông báo", "Vui lòng chọn một giao dịch chưa khớp để thực hiện gán hóa đơn.");
            return;
        }

        var dialogVm = new AssignBillDialogViewModel(
            SelectedTransaction,
            _billService,
            _reconciliationService,
            _dialogService);

        bool isAssigned = false;

        if (ShowAssignBillDialogHandler != null)
        {
            isAssigned = await ShowAssignBillDialogHandler.Invoke(dialogVm);
        }
        else
        {
            var dialogWindow = new AssignBillDialogWindow(dialogVm);
            isAssigned = await _dialogService.ShowDialogAsync<bool>(dialogWindow);
        }

        if (isAssigned)
        {
            await LoadTransactionsAsync();
            await _dialogService.ShowMessageAsync("Thành công", "Đã gán hóa đơn và cập nhật trạng thái đối soát thành công!");
        }
    }
}
