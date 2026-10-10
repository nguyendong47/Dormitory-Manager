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
public partial class BillListViewModel : ViewModelBase, IDisposable
{
    private readonly IBillService _billService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IExportService _exportService;
    private readonly IFileService _fileService;
    private readonly IPdfExportService _pdfExportService;
    private readonly IEmailService _emailService;
    private readonly IContractService _contractService;
    private readonly IStudentService _studentService;
    private readonly IBankSettingsService? _bankSettingsService;
    private readonly IVietQrService? _vietQrService;
    private readonly IPaymentNotificationService? _paymentNotificationService;

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
    [NotifyCanExecuteChangedFor(nameof(SendBillEmailCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenVietQrDialogCommand))]
    private BillDto? _selectedBill;

    [ObservableProperty]
    private int? _selectedMonth;

    [ObservableProperty]
    private int? _selectedYear;

    [ObservableProperty]
    private BillStatus? _selectedStatus;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _autoPaymentNotification;

    public BillListViewModel(
        IBillService billService,
        IRoomService roomService,
        IDialogService dialogService,
        IExportService exportService,
        IFileService fileService,
        IPdfExportService pdfExportService,
        IEmailService emailService,
        IContractService contractService,
        IStudentService studentService,
        IBankSettingsService? bankSettingsService = null,
        IVietQrService? vietQrService = null,
        IPaymentNotificationService? paymentNotificationService = null)
    {
        _billService = billService;
        _roomService = roomService;
        _dialogService = dialogService;
        _exportService = exportService;
        _fileService = fileService;
        _pdfExportService = pdfExportService;
        _emailService = emailService;
        _contractService = contractService;
        _studentService = studentService;
        _bankSettingsService = bankSettingsService;
        _vietQrService = vietQrService;
        _paymentNotificationService = paymentNotificationService;

        if (_paymentNotificationService != null)
        {
            _paymentNotificationService.OnPaymentReceived += HandlePaymentReceived;
        }
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
    /// Gửi email thông báo hóa đơn kèm tệp PDF phiếu thu tới sinh viên thuê phòng
    /// </summary>
    [RelayCommand]
    public async Task SendBillEmailAsync(BillDto? bill = null)
    {
        var target = bill ?? SelectedBill;
        if (target == null)
        {
            await _dialogService.ShowMessageAsync("Thông báo", "Vui lòng chọn một hóa đơn cần gửi email.");
            return;
        }

        IsLoading = true;
        try
        {
            // 1. Tìm thông tin sinh viên thuê phòng của hóa đơn
            string recipientEmail = string.Empty;
            string recipientName = string.Empty;

            var contracts = await _contractService.GetAllContractsAsync(ContractStatus.Active, roomId: target.RoomId);
            var activeContract = contracts.FirstOrDefault();

            if (activeContract != null)
            {
                var student = await _studentService.GetStudentByIdAsync(activeContract.StudentId);
                if (student != null)
                {
                    recipientName = student.FullName;
                    recipientEmail = student.Email?.Trim() ?? string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                var roomStudents = await _studentService.GetAllStudentsAsync(roomId: target.RoomId);
                var studentWithEmail = roomStudents.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s.Email));
                if (studentWithEmail != null)
                {
                    recipientName = studentWithEmail.FullName;
                    recipientEmail = studentWithEmail.Email!.Trim();
                }
                else if (roomStudents.Any())
                {
                    recipientName = roomStudents.First().FullName;
                }
            }

            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                await _dialogService.ShowMessageAsync(
                    "Không tìm thấy email",
                    $"Phòng {target.RoomNumber} (Hóa đơn {target.BillCode}) hiện chưa có sinh viên nào được cập nhật địa chỉ email trong hồ sơ.\n\nVui lòng cập nhật email sinh viên trong danh mục Quản lý sinh viên trước khi gửi thông báo.");
                return;
            }

            // 2. Sinh dữ liệu phiếu thu định dạng PDF
            var pdfBytes = await _pdfExportService.GenerateBillReceiptPdfAsync(target.Id);

            // 3. Gửi email qua dịch vụ IEmailService
            var sendResult = await _emailService.SendBillInvoiceEmailAsync(target.Id, recipientEmail, recipientName, pdfBytes);

            if (sendResult.Success)
            {
                await _dialogService.ShowMessageAsync(
                    "Gửi email thành công",
                    $"Đã gửi hóa đơn {target.BillCode} kèm tệp PDF phiếu thu tới sinh viên {recipientName} ({recipientEmail}) thành công!");
            }
            else
            {
                await _dialogService.ShowMessageAsync(
                    "Gửi email thất bại",
                    $"Không thể gửi email hóa đơn: {sendResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi gửi email", $"Có lỗi xảy ra khi gửi email: {ex.Message}");
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
    /// Lấy họ tên sinh viên đang thuê hoặc cư trú tại phòng của hóa đơn
    /// </summary>
    private async Task<string> GetStudentNameForRoomAsync(int roomId)
    {
        var contracts = await _contractService.GetAllContractsAsync(ContractStatus.Active, roomId: roomId);
        var activeContract = contracts.FirstOrDefault();

        if (activeContract != null)
        {
            var student = await _studentService.GetStudentByIdAsync(activeContract.StudentId);
            if (student != null && !string.IsNullOrWhiteSpace(student.FullName))
            {
                return student.FullName;
            }
        }

        var roomStudents = await _studentService.GetAllStudentsAsync(roomId: roomId);
        var firstStudent = roomStudents.FirstOrDefault();
        return firstStudent?.FullName ?? string.Empty;
    }

    /// <summary>
    /// Ủy nhiệm hiển thị cửa sổ VietQR (cho phép thay thế hoặc mock trong unit test)
    /// </summary>
    public Func<VietQrDialogViewModel, Task<bool>>? ShowVietQrDialogHandler { get; set; }

    /// <summary>
    /// Mở cửa sổ quét mã VietQR động tại quầy thu ngân cho hóa đơn
    /// </summary>
    [RelayCommand]
    public async Task OpenVietQrDialogAsync(BillDto? bill = null)
    {
        var target = bill ?? SelectedBill;
        if (target == null)
        {
            await _dialogService.ShowMessageAsync("Thông báo", "Vui lòng chọn một hóa đơn cần quét mã VietQR.");
            return;
        }

        if (_bankSettingsService == null || _vietQrService == null)
        {
            await _dialogService.ShowMessageAsync("Lỗi", "Dịch vụ ngân hàng và VietQR chưa được cấu hình.");
            return;
        }

        try
        {
            string studentName = await GetStudentNameForRoomAsync(target.RoomId);

            var dialogVm = new VietQrDialogViewModel(
                target,
                studentName,
                _bankSettingsService,
                _vietQrService,
                _billService,
                _fileService,
                _dialogService);

            await dialogVm.InitializeAsync();

            bool result;
            if (ShowVietQrDialogHandler != null)
            {
                result = await ShowVietQrDialogHandler(dialogVm);
            }
            else
            {
                var dialog = new VietQrDialogWindow(dialogVm);
                result = await _dialogService.ShowDialogAsync<bool>(dialog);
            }

            if (result)
            {
                await LoadBillsAsync();
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể mở cửa sổ VietQR: {ex.Message}");
        }
    }

    /// <summary>
    /// Alias tương thích ngược cho MarkPaidCommand
    /// </summary>
    public IAsyncRelayCommand MarkAsPaidCommand => MarkPaidCommand;

    /// <summary>
    /// Xử lý sự kiện nhận thông báo thanh toán ngân hàng thành công qua Webhook theo thời gian thực
    /// </summary>
    private void HandlePaymentReceived(object? sender, PaymentReceivedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
        {
            AutoPaymentNotification = $"Hóa đơn {e.BillCode} đã được tự động thanh toán {e.Amount:N0} VND qua chuyển khoản ngân hàng!";
            await LoadBillsAsync();
        });
    }

    /// <summary>
    /// Hủy đăng ký sự kiện thanh toán khi ViewModel giải phóng tránh memory leak
    /// </summary>
    public void Dispose()
    {
        if (_paymentNotificationService != null)
        {
            _paymentNotificationService.OnPaymentReceived -= HandlePaymentReceived;
        }
    }
}
