using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.ViewModels;
using Dormitory.Desktop.Views;
using Dormitory.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Dormitory.E2ETests.Journeys;

/// <summary>
/// Kiểm thử hành trình E2E cho quy trình quét mã VietQR tại quầy và cấu hình tài khoản ngân hàng thụ hưởng
/// </summary>
public class VietQrPaymentE2ETests : IDisposable
{
    private readonly TestFixture _fixture = new();

    [AvaloniaFact]
    public async Task Should_Initialize_VietQr_Dialog_And_Confirm_Payment_Successfully()
    {
        // 1. Lấy BillListViewModel và nạp danh sách hóa đơn đã seed
        var billListVm = _fixture.CreateBillListViewModel();
        await billListVm.LoadBillsCommand.ExecuteAsync(null);

        billListVm.Bills.Should().NotBeEmpty("Dữ liệu mẫu seed phải có hóa đơn");
        var testBill = billListVm.Bills.First(b => b.Status == BillStatus.Unpaid);
        testBill.Should().NotBeNull();

        // 2. Khởi tạo VietQrDialogViewModel với hóa đơn chưa thanh toán
        var bankSettingsService = _fixture.GetService<IBankSettingsService>();
        var vietQrService = _fixture.GetService<IVietQrService>();
        var billService = _fixture.GetService<IBillService>();

        var qrDialogVm = new VietQrDialogViewModel(
            testBill,
            "Nguyen Van Test",
            bankSettingsService,
            vietQrService,
            billService,
            _fixture.FileService,
            _fixture.DialogService);

        await qrDialogVm.InitializeAsync();

        // 3. Xác minh Payload VietQR và ảnh QR Bitmap
        qrDialogVm.Payload.Should().NotBeNull("Payload VietQR phải được sinh tự động");
        qrDialogVm.Payload!.Amount.Should().Be(testBill.TotalAmount, "Số tiền trên mã QR phải bằng tổng tiền hóa đơn");
        qrDialogVm.Payload.TransferContent.Should().Contain(testBill.BillCode, "Nội dung chuyển khoản phải chứa mã hóa đơn");
        qrDialogVm.QrBitmap.Should().NotBeNull("Ảnh mã QR Bitmap phải được tạo thành công trong môi trường Avalonia");
        qrDialogVm.IsPaid.Should().BeFalse("Hóa đơn chưa thanh toán thì IsPaid phải là false");
        qrDialogVm.CanConfirmPayment.Should().BeTrue("Nút xác nhận thanh toán phải được bật");

        // 4. Kiểm tra sao chép STK và nội dung chuyển khoản
        string copiedAcc = string.Empty;
        qrDialogVm.CopyToClipboardAction = text =>
        {
            copiedAcc = text;
            return Task.CompletedTask;
        };
        await qrDialogVm.CopyAccountNumberCommand.ExecuteAsync(null);
        copiedAcc.Should().Be(qrDialogVm.Payload.AccountNumber);

        // 5. Kiểm tra lưu ảnh mã QR PNG
        await qrDialogVm.SaveQrImageCommand.ExecuteAsync(null);
        _fixture.FileService.SaveCallCount.Should().Be(1, "Phải gọi lưu ảnh QR đúng 1 lần");
        _fixture.FileService.LastSavedExtension.Should().Be("png");
        _fixture.FileService.LastSavedFileName.Should().Contain(testBill.BillCode);
        _fixture.FileService.LastSavedBytes.Should().NotBeNullOrEmpty();

        // 6. Kiểm tra xác nhận đã thu tiền tại quầy
        _fixture.DialogService.ConfirmResult = true;
        bool? dialogClosedResult = null;
        qrDialogVm.CloseAction = res => dialogClosedResult = res;

        await qrDialogVm.ConfirmPaymentCommand.ExecuteAsync(null);

        dialogClosedResult.Should().BeTrue("Sau khi xác nhận thu tiền, dialog phải đóng với kết quả true");
        qrDialogVm.IsPaid.Should().BeTrue("Trạng thái thanh toán phải chuyển thành true");
        qrDialogVm.CanConfirmPayment.Should().BeFalse("Không thể xác nhận lại khi đã thanh toán");

        // 7. Xác minh hóa đơn trong CSDL đã được chuyển thành Paid
        var updatedBills = await billService.GetAllBillsAsync();
        var reloadedBill = updatedBills.FirstOrDefault(b => b.Id == testBill.Id);
        reloadedBill.Should().NotBeNull();
        reloadedBill!.Status.Should().Be(BillStatus.Paid, "Trạng thái hóa đơn trong CSDL phải là Paid");
    }

    [AvaloniaFact]
    public async Task Should_Manage_Bank_Settings_And_Test_Qr_Generation_In_SystemSettings()
    {
        // 1. Khởi tạo SystemSettingsViewModel từ DI Container
        var settingsVm = _fixture.GetService<SystemSettingsViewModel>();

        // 2. Nạp cấu hình ngân hàng
        await settingsVm.LoadBankSettingsAsync();

        settingsVm.BankSettings.Should().NotBeNull();
        settingsVm.AvailableBanks.Should().NotBeEmpty("Danh mục ngân hàng phải có danh sách các ngân hàng Napas");
        settingsVm.AvailableTemplates.Should().Contain("compact");

        // 3. Đổi sang ngân hàng khác (ví dụ BIDV)
        var bidv = settingsVm.AvailableBanks.FirstOrDefault(b => b.Bin == "970418");
        bidv.Should().NotBeNull();
        settingsVm.SelectedBank = bidv;

        settingsVm.BankSettings.BankBin.Should().Be("970418");
        settingsVm.BankSettings.BankShortName.Should().Be("BIDV");

        // 4. Lưu cấu hình ngân hàng mới
        await settingsVm.SaveBankSettingsCommand.ExecuteAsync(null);
        settingsVm.BankStatusMessage.Should().Contain("thành công");

        // 5. Thử nghiệm sinh mã QR kiểm tra
        await settingsVm.TestGenerateQrCommand.ExecuteAsync(null);
        settingsVm.PreviewQrBitmap.Should().NotBeNull("Ảnh xem trước mã QR phải được sinh");
        settingsVm.BankStatusMessage.Should().Contain("100.000 đ");
    }

    [AvaloniaFact]
    public async Task Should_Handle_Already_Paid_Bill_In_VietQr_Dialog()
    {
        // 1. Lấy danh sách hóa đơn và đảm bảo có hóa đơn trạng thái Paid
        var billListVm = _fixture.CreateBillListViewModel();
        await billListVm.LoadBillsCommand.ExecuteAsync(null);

        var billService = _fixture.GetService<IBillService>();
        var paidBill = billListVm.Bills.FirstOrDefault(b => b.Status == BillStatus.Paid);
        if (paidBill == null)
        {
            var unpaidBill = billListVm.Bills.First(b => b.Status == BillStatus.Unpaid);
            await billService.MarkAsPaidAsync(unpaidBill.Id);
            await billListVm.LoadBillsCommand.ExecuteAsync(null);
            paidBill = billListVm.Bills.First(b => b.Id == unpaidBill.Id);
        }

        paidBill.Should().NotBeNull();
        paidBill!.Status.Should().Be(BillStatus.Paid);

        // 2. Khởi tạo VietQrDialogViewModel với hóa đơn đã thanh toán
        var bankSettingsService = _fixture.GetService<IBankSettingsService>();
        var vietQrService = _fixture.GetService<IVietQrService>();

        var qrDialogVm = new VietQrDialogViewModel(
            paidBill,
            "Nguyen Van Test",
            bankSettingsService,
            vietQrService,
            billService,
            _fixture.FileService,
            _fixture.DialogService);

        await qrDialogVm.InitializeAsync();

        // 3. Xác minh IsPaid == true, CanConfirmPayment == false
        qrDialogVm.IsPaid.Should().BeTrue("Hóa đơn đã thanh toán thì IsPaid phải là true");
        qrDialogVm.CanConfirmPayment.Should().BeFalse("Hóa đơn đã thanh toán thì CanConfirmPayment phải là false");
        qrDialogVm.StatusMessage.Should().Contain("đã được thanh toán", "Thông báo trạng thái phải nêu rõ hóa đơn đã được thanh toán");

        // 4. Cố tình gọi ConfirmPaymentCommand và xác minh không thay đổi trạng thái
        bool dialogClosed = false;
        qrDialogVm.CloseAction = res => dialogClosed = true;

        await qrDialogVm.ConfirmPaymentCommand.ExecuteAsync(null);

        dialogClosed.Should().BeFalse("Không đóng hộp thoại khi gọi lệnh với hóa đơn đã thanh toán");
        qrDialogVm.IsPaid.Should().BeTrue("Trạng thái IsPaid vẫn phải là true");
        qrDialogVm.CanConfirmPayment.Should().BeFalse();

        // Xác minh trạng thái trong CSDL không bị thay đổi
        var billInDb = await billService.GetBillByIdAsync(paidBill.Id);
        billInDb.Should().NotBeNull();
        billInDb!.Status.Should().Be(BillStatus.Paid);
    }

    [AvaloniaFact]
    public async Task Should_Handle_Disabled_VietQr_Settings_Gracefully()
    {
        var bankSettingsService = _fixture.GetService<IBankSettingsService>();
        var pdfExportService = _fixture.GetService<IPdfExportService>();
        var emailService = _fixture.GetService<IEmailService>();
        var billService = _fixture.GetService<IBillService>();
        var vietQrService = _fixture.GetService<IVietQrService>();

        var originalSettings = await bankSettingsService.GetBankSettingsAsync();
        var disabledSettings = new BankSettingsDto
        {
            BankBin = originalSettings.BankBin,
            BankName = originalSettings.BankName,
            BankShortName = originalSettings.BankShortName,
            AccountNumber = originalSettings.AccountNumber,
            AccountHolder = originalSettings.AccountHolder,
            QrTemplate = originalSettings.QrTemplate,
            TransferPrefix = originalSettings.TransferPrefix,
            IsEnabled = false
        };

        try
        {
            // 1. Đổi cấu hình BankSettings.IsEnabled = false và lưu lại
            await bankSettingsService.SaveBankSettingsAsync(disabledSettings);
            var reloaded = await bankSettingsService.GetBankSettingsAsync();
            reloaded.IsEnabled.Should().BeFalse("Cấu hình VietQR phải được lưu thành công với IsEnabled = false");

            var allBills = await billService.GetAllBillsAsync();
            var testBill = allBills.First();

            // 2. Kiểm tra PdfExportService xử lý gracefully không sinh mã VietQR
            var pdfBytes = await pdfExportService.GenerateBillReceiptPdfAsync(testBill.Id);
            pdfBytes.Should().NotBeNullOrEmpty("PDF vẫn phải được tạo bình thường khi tắt tính năng VietQR");
            pdfBytes.Length.Should().BeGreaterThan(1000);

            // 3. Kiểm tra EmailService xử lý gracefully không sinh ảnh mã VietQR
            var emailResult = await emailService.SendBillInvoiceEmailAsync(testBill.Id, "student.test@example.com", "Nguyen Van Test", pdfBytes);
            emailResult.Success.Should().BeTrue("Gửi email vẫn phải thành công trong chế độ giả lập");
            var emailConcrete = emailService as EmailService;
            var emailHtml = emailConcrete?.LastGeneratedHtmlBody;
            emailHtml.Should().NotBeNullOrWhiteSpace();
            emailHtml.Should().NotContain("img.vietqr.io", "Email không được nhúng liên kết ảnh VietQR khi IsEnabled = false");
            emailHtml.Should().Contain("THÔNG TIN CHUYỂN KHOẢN THANH TOÁN:", "Email phải hiển thị thông tin chuyển khoản bằng văn bản");

            // 4. Kiểm tra VietQrDialogViewModel xử lý hoặc có thông báo tương ứng
            var qrDialogVm = new VietQrDialogViewModel(
                testBill,
                "Nguyen Van Test",
                bankSettingsService,
                vietQrService,
                billService,
                _fixture.FileService,
                _fixture.DialogService);

            await qrDialogVm.InitializeAsync();
            qrDialogVm.Payload.Should().BeNull("Khi IsEnabled = false, Payload VietQR không được tạo");
            qrDialogVm.QrBitmap.Should().BeNull("Khi IsEnabled = false, ảnh Bitmap QR không được tạo");
            qrDialogVm.StatusMessage.Should().Contain("tắt", "Thông báo phải nêu rõ tính năng VietQR đang tắt");
        }
        finally
        {
            // 5. Khôi phục IsEnabled = true sau test
            await bankSettingsService.SaveBankSettingsAsync(originalSettings);
        }
    }

    [AvaloniaFact]
    public async Task Should_Trigger_OpenVietQrDialogCommand_From_BillListViewModel()
    {
        // 1. Lấy BillListViewModel, chọn hóa đơn chưa thanh toán
        var billListVm = _fixture.CreateBillListViewModel();
        await billListVm.LoadBillsCommand.ExecuteAsync(null);

        billListVm.Bills.Should().NotBeEmpty();
        var unpaidBill = billListVm.Bills.First(b => b.Status == BillStatus.Unpaid);
        billListVm.SelectedBill = unpaidBill;

        // 2. Bắt lời gọi mở hộp thoại thông qua FakeDialogService
        Window? openedDialog = null;
        _fixture.DialogService.OnShowDialogAsync = dialog =>
        {
            openedDialog = dialog;
            return Task.FromResult<object?>(true);
        };

        try
        {
            // 3. Chạy OpenVietQrDialogCommand
            await billListVm.OpenVietQrDialogCommand.ExecuteAsync(unpaidBill);

            // 4. Xác minh dialog được mở thông qua FakeDialogService
            openedDialog.Should().NotBeNull("Hộp thoại VietQrDialogWindow phải được mở qua IDialogService");
            openedDialog.Should().BeOfType<VietQrDialogWindow>();

            var vm = openedDialog!.DataContext as VietQrDialogViewModel;
            vm.Should().NotBeNull("DataContext của cửa sổ phải là VietQrDialogViewModel");
            vm!.Bill.Id.Should().Be(unpaidBill.Id, "Mã ID hóa đơn trong dialog phải khớp với hóa đơn được chọn");
            vm.IsPaid.Should().BeFalse();
        }
        finally
        {
            _fixture.DialogService.OnShowDialogAsync = null;
        }
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
