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
/// ViewModel quản lý danh sách hóa đơn điện, nước và dịch vụ phòng
/// </summary>
public partial class BillListViewModel : ViewModelBase
{
    private readonly IBillService _billService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IExportService _exportService;
    private readonly IFileService _fileService;
    private readonly IPdfExportService _pdfExportService;

    public ObservableCollection<string> StatusFilterOptions { get; } = new()
    {
        "Tất cả",
        "Chưa thanh toán (Còn nợ)",
        "Đã thanh toán"
    };

    [ObservableProperty]
    private string _selectedStatusFilter = "Tất cả";

    [ObservableProperty]
    private ObservableCollection<BillDto> _bills = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MarkPaidCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteBillCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportBillPdfCommand))]
    private BillDto? _selectedBill;

    [ObservableProperty]
    private int? _selectedMonth;

    [ObservableProperty]
    private int? _selectedYear;

    [ObservableProperty]
    private BillStatus? _selectedStatus;

    [ObservableProperty]
    private bool _isLoading;

    public BillListViewModel(
        IBillService billService,
        IRoomService roomService,
        IDialogService dialogService,
        IExportService exportService,
        IFileService fileService,
        IPdfExportService pdfExportService)
    {
        _billService = billService;
        _roomService = roomService;
        _dialogService = dialogService;
        _exportService = exportService;
        _fileService = fileService;
        _pdfExportService = pdfExportService;
    }

    /// <summary>
    /// Xử lý khi bộ lọc trạng thái hóa đơn thay đổi: tự động nạp lại danh sách hóa đơn
    /// </summary>
    partial void OnSelectedStatusFilterChanged(string value)
    {
        _ = LoadBillsAsync();
    }

    /// <summary>
    /// Xuất danh sách hóa đơn ra file Excel
    /// </summary>
    [RelayCommand]
    public async Task ExportToExcelAsync()
    {
        IsLoading = true;
        try
        {
            var list = Bills.ToList();
            var bytes = await _exportService.ExportBillsToExcelAsync(list);
            await _fileService.SaveFileAsync("Danh_Sach_Hoa_Don", "xlsx", "Excel Files", bytes);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", $"Lỗi khi xuất file Excel: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Xuất phiếu thu tiền phòng & dịch vụ ra định dạng PDF in ấn chuyên nghiệp
    /// </summary>
    [RelayCommand]
    public async Task ExportBillPdfAsync(BillDto? bill = null)
    {
        var target = bill ?? SelectedBill;
        if (target == null) return;

        IsLoading = true;
        try
        {
            var bytes = await _pdfExportService.GenerateBillReceiptPdfAsync(target.Id);
            await _fileService.SaveFileAsync($"PhieuThu_{target.BillCode}", "pdf", "PDF Documents (*.pdf)|*.pdf", bytes);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", $"Lỗi khi xuất phiếu thu PDF: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Điều kiện đánh dấu đã thanh toán (chỉ khi hóa đơn chưa thanh toán)
    /// </summary>
    private bool CanMarkPaid => SelectedBill != null && SelectedBill.Status == BillStatus.Unpaid;

    /// <summary>
    /// Điều kiện xóa hóa đơn (khi đã chọn một hóa đơn)
    /// </summary>
    private bool CanDeleteBill => SelectedBill != null;

    /// <summary>
    /// Tải danh sách hóa đơn theo tháng, năm và trạng thái thanh toán
    /// </summary>
    [RelayCommand]
    public async Task LoadBillsAsync()
    {
        IsLoading = true;
        try
        {
            BillStatus? status = SelectedStatusFilter switch
            {
                "Chưa thanh toán (Còn nợ)" => BillStatus.Unpaid,
                "Đã thanh toán" => BillStatus.Paid,
                _ => null
            };
            SelectedStatus = status;

            var list = await _billService.GetAllBillsAsync(roomId: null, month: SelectedMonth, year: SelectedYear, status: SelectedStatus);
            Bills = new ObservableCollection<BillDto>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Mở hộp thoại lập hóa đơn điện nước mới cho phòng
    /// </summary>
    [RelayCommand]
    public async Task CreateBillAsync()
    {
        var dialogVm = new BillDialogViewModel(_billService, _roomService);
        await dialogVm.LoadInitialDataAsync();
        var dialog = new BillDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadBillsAsync();
        }
    }

    /// <summary>
    /// Xác nhận thu tiền / Đánh dấu đã thanh toán cho hóa đơn được chọn
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanMarkPaid))]
    public async Task MarkPaidAsync()
    {
        if (SelectedBill == null || SelectedBill.Status != BillStatus.Unpaid) return;

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận thanh toán",
            $"Xác nhận thu tiền và đánh dấu đã thanh toán cho hóa đơn {SelectedBill.BillCode} (Phòng {SelectedBill.RoomNumber}) số tiền {SelectedBill.TotalAmount:N0} đ?");

        if (!confirmed) return;

        try
        {
            var success = await _billService.MarkAsPaidAsync(SelectedBill.Id);
            if (success)
            {
                await LoadBillsAsync();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể cập nhật trạng thái thanh toán.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Xóa hóa đơn được chọn
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDeleteBill))]
    public async Task DeleteBillAsync()
    {
        if (SelectedBill == null) return;

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận xóa hóa đơn",
            $"Bạn có chắc chắn muốn xóa hóa đơn {SelectedBill.BillCode} của phòng {SelectedBill.RoomNumber} không? Thao tác này không thể hoàn tác.");

        if (!confirmed) return;

        try
        {
            var success = await _billService.DeleteBillAsync(SelectedBill.Id);
            if (success)
            {
                await LoadBillsAsync();
                SelectedBill = null;
            }
            else
            {
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể xóa hóa đơn.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Alias tương thích ngược cho MarkPaidCommand
    /// </summary>
    public IAsyncRelayCommand MarkAsPaidCommand => MarkPaidCommand;
}
