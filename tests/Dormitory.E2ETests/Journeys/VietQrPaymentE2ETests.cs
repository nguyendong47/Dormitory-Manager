using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.ViewModels;
using Dormitory.Desktop.Views;
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

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
