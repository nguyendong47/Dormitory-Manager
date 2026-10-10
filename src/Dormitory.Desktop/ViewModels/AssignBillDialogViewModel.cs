using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel cho hộp thoại chọn hóa đơn chưa thanh toán để gán thủ công vào giao dịch ngân hàng chưa khớp
/// </summary>
public partial class AssignBillDialogViewModel : ViewModelBase
{
    private readonly IBillService _billService;
    private readonly IPaymentReconciliationService _reconciliationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private PaymentTransactionDto _transaction;

    public ObservableCollection<BillDto> UnpaidBills { get; } = new();

    [ObservableProperty]
    private BillDto? _selectedBill;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Action đóng dialog (dành cho Avalonia Window code-behind)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Async delegate đóng dialog (hỗ trợ kiểm thử Headless Unit Test)
    /// </summary>
    public Func<bool, Task>? CloseDialogHandler { get; set; }

    public AssignBillDialogViewModel(
        PaymentTransactionDto transaction,
        IBillService billService,
        IPaymentReconciliationService reconciliationService,
        IDialogService dialogService)
    {
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        _billService = billService ?? throw new ArgumentNullException(nameof(billService));
        _reconciliationService = reconciliationService ?? throw new ArgumentNullException(nameof(reconciliationService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        _ = LoadUnpaidBillsAsync();
    }

    /// <summary>
    /// Tải danh sách các hóa đơn chưa thanh toán
    /// </summary>
    [RelayCommand]
    public async Task LoadUnpaidBillsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang tải danh sách hóa đơn chưa thanh toán...";
            UnpaidBills.Clear();

            var bills = await _billService.GetAllBillsAsync(status: BillStatus.Unpaid);
            foreach (var bill in bills)
            {
                UnpaidBills.Add(bill);
            }

            StatusMessage = UnpaidBills.Count > 0
                ? $"Tìm thấy {UnpaidBills.Count} hóa đơn chưa thanh toán."
                : "Không có hóa đơn chưa thanh toán nào trong hệ thống.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi khi tải hóa đơn: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tải danh sách hóa đơn: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Xác nhận gán hóa đơn đã chọn cho giao dịch hiện tại
    /// </summary>
    [RelayCommand]
    public async Task ConfirmAssignAsync()
    {
        if (SelectedBill == null)
        {
            await _dialogService.ShowMessageAsync("Thông báo", "Vui lòng chọn một hóa đơn cần gán.");
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Đang gán hóa đơn vào giao dịch...";

            var success = await _reconciliationService.ManuallyAssignBillAsync(
                Transaction.Id,
                SelectedBill.Id,
                Note);

            if (success)
            {
                StatusMessage = "Gán hóa đơn thành công!";
                CloseAction?.Invoke(true);
                if (CloseDialogHandler != null)
                {
                    await CloseDialogHandler.Invoke(true);
                }
            }
            else
            {
                StatusMessage = "Không thể gán hóa đơn cho giao dịch.";
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể gán hóa đơn cho giao dịch này. Vui lòng kiểm tra lại trạng thái hóa đơn.");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Gặp lỗi khi gán hóa đơn: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Hủy bỏ thao tác và đóng hộp thoại
    /// </summary>
    [RelayCommand]
    public async Task CancelAsync()
    {
        CloseAction?.Invoke(false);
        if (CloseDialogHandler != null)
        {
            await CloseDialogHandler.Invoke(false);
        }
    }
}
