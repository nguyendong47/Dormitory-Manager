using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel cho cửa sổ hộp thoại quét mã thanh toán VietQR động tại quầy thu ngân
/// </summary>
public partial class VietQrDialogViewModel : ViewModelBase
{
    private readonly IBankSettingsService _bankSettingsService;
    private readonly IVietQrService _vietQrService;
    private readonly IBillService _billService;
    private readonly IFileService _fileService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private BillDto _bill;

    [ObservableProperty]
    private string _studentName = string.Empty;

    [ObservableProperty]
    private VietQrPayloadDto? _payload;

    [ObservableProperty]
    private Bitmap? _qrBitmap;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirmPayment))]
    private bool _isPaid;

    public bool CanConfirmPayment => !IsPaid;

    /// <summary>
    /// Action đóng cửa sổ hộp thoại kèm kết quả (true nếu đã xác nhận thanh toán)
    /// </summary>
    public Action<bool>? CloseAction { get; set; }

    /// <summary>
    /// Ủy nhiệm sao chép vào bộ nhớ tạm (phục vụ kiểm thử và gọi native Avalonia Clipboard)
    /// </summary>
    public Func<string, Task>? CopyToClipboardAction { get; set; }

    public VietQrDialogViewModel(
        BillDto bill,
        string studentName,
        IBankSettingsService bankSettingsService,
        IVietQrService vietQrService,
        IBillService billService,
        IFileService fileService,
        IDialogService dialogService)
    {
        _bill = bill ?? throw new ArgumentNullException(nameof(bill));
        _studentName = studentName ?? string.Empty;
        _bankSettingsService = bankSettingsService ?? throw new ArgumentNullException(nameof(bankSettingsService));
        _vietQrService = vietQrService ?? throw new ArgumentNullException(nameof(vietQrService));
        _billService = billService ?? throw new ArgumentNullException(nameof(billService));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        _isPaid = bill.Status == BillStatus.Paid;
    }

    /// <summary>
    /// Khởi tạo dữ liệu mã QR: nạp cấu hình ngân hàng, tính payload EMVCo và tạo ảnh Bitmap
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            StatusMessage = "Đang tải cấu hình ngân hàng và sinh mã QR...";
            var bankSettings = await _bankSettingsService.GetBankSettingsAsync();
            Payload = _vietQrService.GeneratePayloadForBill(Bill, StudentName, bankSettings);

            if (Payload != null && !string.IsNullOrWhiteSpace(Payload.EmvCoPayload))
            {
                var qrBytes = _vietQrService.GenerateQrCodePng(Payload.EmvCoPayload, 10);
                if (qrBytes != null && qrBytes.Length > 0)
                {
                    try
                    {
                        using var stream = new MemoryStream(qrBytes);
                        QrBitmap = new Bitmap(stream);
                    }
                    catch
                    {
                        QrBitmap = null;
                    }
                }
            }

            StatusMessage = IsPaid
                ? "Hóa đơn này đã được thanh toán trước đó."
                : "Mã QR thanh toán VietQR sẵn sàng để quét tại quầy.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi khởi tạo VietQR: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể tạo mã VietQR: {ex.Message}");
        }
    }

    /// <summary>
    /// Sao chép chuỗi văn bản vào bộ nhớ tạm Clipboard
    /// </summary>
    private async Task CopyToClipboardAsync(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (CopyToClipboardAction != null)
        {
            await CopyToClipboardAction(text);
            return;
        }

        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
            if (topLevel?.Clipboard != null)
            {
                await topLevel.Clipboard.SetTextAsync(text);
            }
        }
    }

    /// <summary>
    /// Sao chép số tài khoản thụ hưởng vào clipboard
    /// </summary>
    [RelayCommand]
    public async Task CopyAccountNumberAsync()
    {
        if (Payload != null && !string.IsNullOrWhiteSpace(Payload.AccountNumber))
        {
            await CopyToClipboardAsync(Payload.AccountNumber);
            StatusMessage = "Đã sao chép số tài khoản vào bộ nhớ tạm.";
        }
    }

    /// <summary>
    /// Sao chép nội dung chuyển khoản vào clipboard
    /// </summary>
    [RelayCommand]
    public async Task CopyTransferContentAsync()
    {
        if (Payload != null && !string.IsNullOrWhiteSpace(Payload.TransferContent))
        {
            await CopyToClipboardAsync(Payload.TransferContent);
            StatusMessage = "Đã sao chép nội dung chuyển khoản vào bộ nhớ tạm.";
        }
    }

    /// <summary>
    /// Lưu tệp ảnh mã QR định dạng PNG ra máy tính
    /// </summary>
    [RelayCommand]
    public async Task SaveQrImageAsync()
    {
        if (Payload == null || string.IsNullOrWhiteSpace(Payload.EmvCoPayload)) return;

        try
        {
            var qrBytes = _vietQrService.GenerateQrCodePng(Payload.EmvCoPayload, 10);
            var fileName = $"VietQR_{Bill.BillCode}";
            var saved = await _fileService.SaveFileAsync(fileName, "png", "PNG Image (*.png)|*.png", qrBytes);
            if (saved)
            {
                StatusMessage = "Đã lưu ảnh mã VietQR thành công.";
                await _dialogService.ShowMessageAsync("Thành công", "Đã lưu ảnh mã VietQR thành công.");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi lưu ảnh: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", $"Không thể lưu ảnh mã QR: {ex.Message}");
        }
    }

    /// <summary>
    /// Xác nhận đã thu tiền tại quầy và cập nhật trạng thái hóa đơn thành Đã thanh toán
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanConfirmPayment))]
    public async Task ConfirmPaymentAsync()
    {
        if (Bill == null || IsPaid) return;

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận đã thu tiền",
            $"Xác nhận sinh viên đã chuyển khoản thành công {Bill.TotalAmount:N0} đ cho hóa đơn {Bill.BillCode}?");

        if (!confirmed) return;

        try
        {
            var success = await _billService.MarkAsPaidAsync(Bill.Id);
            if (success)
            {
                IsPaid = true;
                StatusMessage = "Đã xác nhận thu tiền thành công!";
                await _dialogService.ShowMessageAsync("Thành công", "Đã cập nhật trạng thái hóa đơn thành Đã thanh toán.");
                CloseAction?.Invoke(true);
            }
            else
            {
                StatusMessage = "Lỗi khi cập nhật trạng thái hóa đơn.";
                await _dialogService.ShowMessageAsync("Lỗi", "Không thể cập nhật trạng thái hóa đơn.");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi: {ex.Message}";
            await _dialogService.ShowMessageAsync("Lỗi", ex.Message);
        }
    }

    /// <summary>
    /// Đóng hộp thoại VietQR
    /// </summary>
    [RelayCommand]
    public void Close()
    {
        CloseAction?.Invoke(IsPaid);
    }
}
