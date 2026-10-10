using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.E2ETests.Journeys;

/// <summary>
/// Kiểm thử hành trình E2E Headless cho luồng Webhook ngân hàng và đối soát tự động hóa đơn ký túc xá
/// </summary>
public class PaymentAutoReconciliationE2ETests : IDisposable
{
    private readonly TestFixture _fixture = new();

    /// <summary>
    /// Kịch bản 1: Đối soát tự động thành công qua Webhook giả lập, gạch nợ hóa đơn và cập nhật giao diện
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Auto_Reconcile_And_Mark_Bill_Paid_Via_Webhook_Successfully()
    {
        // 1. Chuẩn bị một hóa đơn chưa thanh toán từ CSDL mẫu đã seed
        var bill = await _fixture.Context.Bills.FirstAsync(b => b.Status == BillStatus.Unpaid);
        bill.Should().NotBeNull();
        var unpaidBillId = bill.Id;
        var unpaidBillCode = bill.BillCode;
        var totalAmount = bill.TotalAmount;

        // 2. Gửi Webhook payload chứa cú pháp KTX {bill.BillCode} và đủ số tiền
        var webhookService = _fixture.GetService<IWebhookListenerService>();
        var testTxId = $"TX_{Guid.NewGuid():N}";
        var testPayload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = testTxId,
            Amount = totalAmount,
            Description = $"KTX {unpaidBillCode} thanh toan tien phong",
            AccountNumber = "123456789",
            BankBin = "970422",
            TransactionDate = DateTime.UtcNow,
            RawData = "{\"mock\": true}"
        };

        var result = await webhookService.TestWebhookAsync(testPayload);

        // 3. Xác minh kết quả trả về IsSuccess == true và Status == PaymentTransactionStatus.Success
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(PaymentTransactionStatus.Success);
        result.BillId.Should().Be(unpaidBillId);
        result.BillCode.Should().Be(unpaidBillCode);
        result.Amount.Should().Be(totalAmount);

        // 4. Xác minh hóa đơn trong CSDL tự động chuyển sang BillStatus.Paid
        var updatedBill = await _fixture.Context.Bills.AsNoTracking().FirstOrDefaultAsync(b => b.Id == unpaidBillId);
        updatedBill.Should().NotBeNull();
        updatedBill!.Status.Should().Be(BillStatus.Paid);
        updatedBill.PaidDate.Should().NotBeNull();

        // 5. Xác minh PaymentTransaction được lưu vào CSDL với Status == Success, BillId == bill.Id
        var txInDb = await _fixture.Context.PaymentTransactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionId == testTxId);
        txInDb.Should().NotBeNull();
        txInDb!.Status.Should().Be(PaymentTransactionStatus.Success);
        txInDb.BillId.Should().Be(unpaidBillId);
        txInDb.BillCode.Should().Be(unpaidBillCode);
        txInDb.Amount.Should().Be(totalAmount);

        // 6. Xác minh BillListViewModel sau khi nạp lại hiển thị hóa đơn đó ở trạng thái Paid
        var billListVm = _fixture.CreateBillListViewModel();
        await billListVm.LoadBillsCommand.ExecuteAsync(null);
        var vmBill = billListVm.Bills.FirstOrDefault(b => b.Id == unpaidBillId);
        vmBill.Should().NotBeNull();
        vmBill!.Status.Should().Be(BillStatus.Paid);
    }

    /// <summary>
    /// Kịch bản 2: Xử lý giao dịch chưa khớp (Unmatched) khi không có mã hóa đơn, và hỗ trợ gán thủ công vào hóa đơn
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Handle_Unmatched_Webhook_And_Allow_Manual_Assignment()
    {
        // 1. Chuẩn bị hóa đơn chưa thanh toán từ CSDL
        var bill = await _fixture.Context.Bills.FirstAsync(b => b.Status == BillStatus.Unpaid);
        bill.Should().NotBeNull();
        var targetBillId = bill.Id;
        var targetBillCode = bill.BillCode;
        var targetAmount = bill.TotalAmount;

        // 2. Gửi Webhook payload với nội dung chuyển khoản không chứa mã hóa đơn nào hợp lệ
        var webhookService = _fixture.GetService<IWebhookListenerService>();
        var unmatchedTxId = $"TX_UNMATCHED_{Guid.NewGuid():N}";
        var testPayload = new WebhookPayloadDto
        {
            Gateway = "Casso",
            TransactionId = unmatchedTxId,
            Amount = targetAmount,
            Description = "Tien phong thang 10",
            AccountNumber = "987654321",
            BankBin = "970418",
            TransactionDate = DateTime.UtcNow,
            RawData = "{\"mock\": true}"
        };

        var result = await webhookService.TestWebhookAsync(testPayload);

        // 3. Xác minh giao dịch được lưu với Status == PaymentTransactionStatus.Unmatched, BillId == null
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        result.BillId.Should().BeNull();

        var txInDb = await _fixture.Context.PaymentTransactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionId == unmatchedTxId);
        txInDb.Should().NotBeNull();
        txInDb!.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        txInDb.BillId.Should().BeNull();

        // 4. Khởi tạo PaymentTransactionListViewModel, nạp danh sách, kiểm tra KPI UnmatchedCount >= 1
        var txListVm = _fixture.CreatePaymentTransactionListViewModel();
        await txListVm.LoadTransactionsCommand.ExecuteAsync(null);

        txListVm.Transactions.Should().NotBeEmpty();
        txListVm.UnmatchedCount.Should().BeGreaterOrEqualTo(1);

        var vmTx = txListVm.Transactions.FirstOrDefault(t => t.TransactionId == unmatchedTxId);
        vmTx.Should().NotBeNull();
        vmTx!.Status.Should().Be(PaymentTransactionStatus.Unmatched);

        // 5. Chọn giao dịch đó và thực hiện gán thủ công vào hóa đơn chưa thanh toán
        txListVm.SelectedTransaction = vmTx;
        txListVm.CanAssignBill.Should().BeTrue();

        txListVm.ShowAssignBillDialogHandler = async dialogVm =>
        {
            await dialogVm.LoadUnpaidBillsAsync();
            dialogVm.UnpaidBills.Should().NotBeEmpty();

            var billOption = dialogVm.UnpaidBills.FirstOrDefault(b => b.Id == targetBillId);
            billOption.Should().NotBeNull();

            dialogVm.SelectedBill = billOption;
            dialogVm.Note = "Đối soát gán thủ công từ kiểm thử E2E";
            await dialogVm.ConfirmAssignCommand.ExecuteAsync(null);
            return true;
        };

        await txListVm.AssignBillCommand.ExecuteAsync(null);

        // 6. Xác minh sau khi gán: Hóa đơn chuyển sang Paid, giao dịch chuyển sang Success có gắn BillId
        var updatedBill = await _fixture.Context.Bills.AsNoTracking().FirstOrDefaultAsync(b => b.Id == targetBillId);
        updatedBill.Should().NotBeNull();
        updatedBill!.Status.Should().Be(BillStatus.Paid);

        var assignedTxInDb = await _fixture.Context.PaymentTransactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionId == unmatchedTxId);
        assignedTxInDb.Should().NotBeNull();
        assignedTxInDb!.Status.Should().Be(PaymentTransactionStatus.Success);
        assignedTxInDb.BillId.Should().Be(targetBillId);
        assignedTxInDb.BillCode.Should().Be(targetBillCode);
    }

    /// <summary>
    /// Kịch bản 3: Từ chối Webhook khi chữ ký số HMAC-SHA256 hoặc Secret Key không hợp lệ
    /// </summary>
    [AvaloniaFact]
    public void Should_Reject_Webhook_With_Invalid_Signature_Or_Secret_Key()
    {
        var webhookService = _fixture.GetService<IWebhookListenerService>();
        var payloadJson = "{\"transactionId\":\"TX_9999\",\"amount\":500000,\"description\":\"KTX HD001\"}";
        var validSecret = "SecretKey_E2E_Test_2026";
        var wrongSecret = "WrongSecretKey_9999";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(validSecret));
        var validBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadJson));
        var validSignature = Convert.ToHexString(validBytes);

        // 1. Chữ ký hợp lệ và secret đúng -> true
        var isValid = webhookService.ValidateSignature(payloadJson, validSignature, validSecret);
        isValid.Should().BeTrue("Chữ ký HMAC hợp lệ và secret đúng phải được chấp nhận");

        // 2. Chữ ký không hợp lệ -> false
        var isInvalidSig = webhookService.ValidateSignature(payloadJson, "BAD_SIGNATURE_HEX", validSecret);
        isInvalidSig.Should().BeFalse("Chữ ký sai lệch phải bị từ chối");

        // 3. Secret key sai -> false
        var isWrongSecret = webhookService.ValidateSignature(payloadJson, validSignature, wrongSecret);
        isWrongSecret.Should().BeFalse("Secret key sai phải bị từ chối");
    }

    /// <summary>
    /// Kịch bản 4: Xử lý giao dịch nộp thiếu tiền (thanh toán một phần), hóa đơn giữ nguyên Unpaid
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Handle_Partial_Payment_Correctly()
    {
        // 1. Lấy hóa đơn chưa thanh toán từ CSDL
        var bill = await _fixture.Context.Bills.FirstAsync(b => b.Status == BillStatus.Unpaid);
        bill.Should().NotBeNull();
        var billId = bill.Id;
        var billCode = bill.BillCode;
        var fullAmount = bill.TotalAmount;

        // 2. Gửi Webhook payload với số tiền nhỏ hơn tổng tiền hóa đơn (ví dụ 100,000 đ)
        var partialAmount = 100_000m;
        fullAmount.Should().BeGreaterThan(partialAmount);

        var webhookService = _fixture.GetService<IWebhookListenerService>();
        var partialTxId = $"TX_PARTIAL_{Guid.NewGuid():N}";
        var testPayload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = partialTxId,
            Amount = partialAmount,
            Description = $"KTX {billCode} thanh toan dot 1",
            AccountNumber = "123456789",
            TransactionDate = DateTime.UtcNow,
            RawData = "{\"mock\": true}"
        };

        var result = await webhookService.TestWebhookAsync(testPayload);

        // 3. Xác minh giao dịch được đánh dấu PartiallyPaid
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.PartiallyPaid);
        result.BillId.Should().Be(billId);
        result.BillCode.Should().Be(billCode);
        result.Amount.Should().Be(partialAmount);

        // 4. Hóa đơn vẫn giữ nguyên BillStatus.Unpaid
        var billInDb = await _fixture.Context.Bills.AsNoTracking().FirstOrDefaultAsync(b => b.Id == billId);
        billInDb.Should().NotBeNull();
        billInDb!.Status.Should().Be(BillStatus.Unpaid);
        billInDb.PaidDate.Should().BeNull();

        // 5. Giao dịch được lưu với Status == PartiallyPaid
        var txInDb = await _fixture.Context.PaymentTransactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionId == partialTxId);
        txInDb.Should().NotBeNull();
        txInDb!.Status.Should().Be(PaymentTransactionStatus.PartiallyPaid);
        txInDb.BillId.Should().Be(billId);
        txInDb.Amount.Should().Be(partialAmount);
    }

    /// <summary>
    /// Kịch bản 5: Phát hiện và từ chối xử lý lại giao dịch ngân hàng trùng lặp TransactionId
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Detect_And_Reject_Duplicate_Transaction_Id()
    {
        // 1. Chuẩn bị hóa đơn chưa thanh toán
        var bill = await _fixture.Context.Bills.FirstAsync(b => b.Status == BillStatus.Unpaid);
        bill.Should().NotBeNull();
        var billCode = bill.BillCode;
        var amount = bill.TotalAmount;

        var sharedTxId = $"TX_DUP_{Guid.NewGuid():N}";
        var webhookService = _fixture.GetService<IWebhookListenerService>();

        var payload1 = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = sharedTxId,
            Amount = amount,
            Description = $"KTX {billCode} nop tien",
            AccountNumber = "123456789",
            TransactionDate = DateTime.UtcNow,
            RawData = "{\"mock\": true}"
        };

        // 2. Gửi Webhook lần 1 -> Success
        var result1 = await webhookService.TestWebhookAsync(payload1);
        result1.Should().NotBeNull();
        result1.IsSuccess.Should().BeTrue();
        result1.Status.Should().Be(PaymentTransactionStatus.Success);

        // 3. Gửi lại Webhook lần 2 với cùng TransactionId -> Duplicate
        var payload2 = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = sharedTxId,
            Amount = amount,
            Description = $"KTX {billCode} nop tien lan 2",
            AccountNumber = "123456789",
            TransactionDate = DateTime.UtcNow,
            RawData = "{\"mock\": true}"
        };

        var result2 = await webhookService.TestWebhookAsync(payload2);

        // 4. Xác minh kết quả trả về Duplicate, không trừ nợ thêm
        result2.Should().NotBeNull();
        result2.IsSuccess.Should().BeFalse();
        result2.Status.Should().Be(PaymentTransactionStatus.Duplicate);

        // 5. Xác minh chỉ có đúng 1 bản ghi giao dịch trong CSDL với mã TransactionId này
        var txCount = await _fixture.Context.PaymentTransactions.CountAsync(t => t.TransactionId == sharedTxId);
        txCount.Should().Be(1);
    }

    /// <summary>
    /// Kịch bản 6: Điều hướng tới màn hình Quản lý Giao dịch Đối soát từ thanh Sidebar trong MainWindow
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Navigate_To_Transactions_View_From_Sidebar_In_MainWindow()
    {
        // 1. Khởi tạo MainWindowViewModel với UserSession đã đăng nhập
        var vm = _fixture.CreateMainWindowViewModel();

        vm.LoginVm.Username = "admin";
        vm.LoginVm.Password = "Admin@123456";
        await vm.LoginVm.LoginCommand.ExecuteAsync(null);

        vm.IsLoggedIn.Should().BeTrue();

        // 2. Kích hoạt command NavigateToTransactionsCommand
        vm.NavigateToTransactionsCommand.Execute(null);

        // 3. Xác minh ActiveMenu == "Transactions" và CurrentView là PaymentTransactionListViewModel
        vm.ActiveMenu.Should().Be("Transactions");
        vm.CurrentView.Should().BeOfType<PaymentTransactionListViewModel>();

        var txListVm = vm.CurrentView as PaymentTransactionListViewModel;
        txListVm.Should().NotBeNull();
        txListVm!.StatusFilterOptions.Should().Contain("Tất cả");
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
