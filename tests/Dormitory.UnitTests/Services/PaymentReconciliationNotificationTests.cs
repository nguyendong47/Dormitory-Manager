using System;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using Dormitory.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử tích hợp giữa PaymentReconciliationService và IPaymentNotificationService (NSubstitute)
/// </summary>
public class PaymentReconciliationNotificationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly IPaymentNotificationService _mockNotificationService;
    private readonly PaymentReconciliationService _service;

    public PaymentReconciliationNotificationTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _mockNotificationService = Substitute.For<IPaymentNotificationService>();
        _service = new PaymentReconciliationService(_context, _mockNotificationService);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private async Task<(Room Room, Bill Bill)> SeedRoomAndBillAsync(string billCode, decimal amount = 1500000m)
    {
        var room = new Room
        {
            RoomNumber = "P301",
            Building = "Tòa B",
            PricePerMonth = amount
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = billCode,
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = amount,
            OldElectricIndex = 0,
            NewElectricIndex = 0,
            ElectricRate = 0,
            OldWaterIndex = 0,
            NewWaterIndex = 0,
            WaterRate = 0,
            OtherServiceFee = 0,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        return (room, bill);
    }

    /// <summary>
    /// Kiểm tra khi đối soát tự động thành công (thanh toán đủ hoặc thừa tiền), hệ thống phát sự kiện thông báo thanh toán
    /// </summary>
    [Fact]
    public async Task ReconcileAsync_OnSuccessfulMatch_NotifiesPaymentReceived()
    {
        // Arrange
        var (_, bill) = await SeedRoomAndBillAsync("HD001", 1500000m);

        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_MATCH_001",
            Amount = 1500000m,
            Description = "KTX HD001 NGUYEN VAN A",
            AccountNumber = "123456789",
            BankBin = "970422",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(PaymentTransactionStatus.Success);

        _mockNotificationService.Received(1).NotifyPaymentReceived(Arg.Is<PaymentReceivedEventArgs>(e =>
            e.BillId == bill.Id &&
            e.BillCode == bill.BillCode &&
            e.Amount == payload.Amount &&
            e.TransactionId == payload.TransactionId));
    }

    /// <summary>
    /// Kiểm tra khi IPaymentNotificationService là null (không được cấu hình), quá trình đối soát vẫn hoàn tất thành công và không ném ngoại lệ
    /// </summary>
    [Fact]
    public async Task ReconcileAsync_WhenNotificationServiceIsNull_DoesNotThrow()
    {
        // Arrange
        var serviceWithoutNotification = new PaymentReconciliationService(_context, null);
        var (_, bill) = await SeedRoomAndBillAsync("HD002", 1000000m);

        var payload = new WebhookPayloadDto
        {
            Gateway = "Casso",
            TransactionId = "TXN_NULL_NOTIFY_001",
            Amount = 1000000m,
            Description = "KTX HD002 TRAN THI B",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var act = async () => await serviceWithoutNotification.ProcessTransactionAsync(payload);

        // Assert
        var result = await act.Should().NotThrowAsync();
        result.Subject.IsSuccess.Should().BeTrue();
        result.Subject.Status.Should().Be(PaymentTransactionStatus.Success);

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Paid);
    }

    /// <summary>
    /// Kiểm tra khi giao dịch không khớp được hóa đơn (Unmatched), không phát thông báo thanh toán
    /// </summary>
    [Fact]
    public async Task ReconcileAsync_OnUnmatched_DoesNotNotify()
    {
        // Arrange - Nội dung không chứa mã hóa đơn hợp lệ
        var payloadNoCode = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_UNMATCHED_001",
            Amount = 500000m,
            Description = "Chuyen tien phong khong ro nguon goc",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var resultNoCode = await _service.ProcessTransactionAsync(payloadNoCode);

        // Assert
        resultNoCode.IsSuccess.Should().BeFalse();
        resultNoCode.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        _mockNotificationService.DidNotReceive().NotifyPaymentReceived(Arg.Any<PaymentReceivedEventArgs>());

        // Arrange - Chứa mã hóa đơn nhưng không tồn tại trong hệ thống
        var payloadCodeNotFound = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_UNMATCHED_002",
            Amount = 500000m,
            Description = "KTX HD_NON_EXISTENT_999",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var resultCodeNotFound = await _service.ProcessTransactionAsync(payloadCodeNotFound);

        // Assert
        resultCodeNotFound.IsSuccess.Should().BeFalse();
        resultCodeNotFound.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        _mockNotificationService.DidNotReceive().NotifyPaymentReceived(Arg.Any<PaymentReceivedEventArgs>());
    }

    /// <summary>
    /// Kiểm tra khi giao dịch bị trùng lặp mã TransactionId, hệ thống từ chối và không phát thông báo thanh toán
    /// </summary>
    [Fact]
    public async Task ReconcileAsync_OnDuplicate_DoesNotNotify()
    {
        // Arrange - Đã tồn tại một giao dịch với TransactionId này trong DB
        var existingTx = new PaymentTransaction
        {
            TransactionId = "TXN_DUPLICATE_001",
            Amount = 1200000m,
            Description = "KTX HD003",
            TransactionDate = DateTime.UtcNow,
            Status = PaymentTransactionStatus.Success,
            CreatedAt = DateTime.UtcNow
        };
        _context.PaymentTransactions.Add(existingTx);
        await _context.SaveChangesAsync();

        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_DUPLICATE_001",
            Amount = 1200000m,
            Description = "KTX HD003",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.Duplicate);
        _mockNotificationService.DidNotReceive().NotifyPaymentReceived(Arg.Any<PaymentReceivedEventArgs>());
    }

    /// <summary>
    /// Kiểm tra khi kế toán xác nhận đối soát thủ công thành công và đủ số tiền, hệ thống phát thông báo thanh toán
    /// </summary>
    [Fact]
    public async Task ManualMatchAsync_OnSuccessfulMatch_NotifiesPaymentReceived()
    {
        // Arrange
        var (_, bill) = await SeedRoomAndBillAsync("HD005", 2000000m);

        var unmatchedTx = new PaymentTransaction
        {
            TransactionId = "TXN_MANUAL_MATCH_001",
            Amount = 2000000m,
            Description = "Tien phong thang 10",
            TransactionDate = DateTime.UtcNow,
            Status = PaymentTransactionStatus.Unmatched,
            CreatedAt = DateTime.UtcNow
        };
        _context.PaymentTransactions.Add(unmatchedTx);
        await _context.SaveChangesAsync();

        // Act
        var success = await _service.ManuallyAssignBillAsync(unmatchedTx.Id, bill.Id, "Kế toán duyệt thủ công");

        // Assert
        success.Should().BeTrue();

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Paid);

        _mockNotificationService.Received(1).NotifyPaymentReceived(Arg.Is<PaymentReceivedEventArgs>(e =>
            e.BillId == bill.Id &&
            e.BillCode == bill.BillCode &&
            e.Amount == unmatchedTx.Amount &&
            e.TransactionId == unmatchedTx.TransactionId));
    }

    /// <summary>
    /// Kiểm tra khi kế toán đối soát thủ công nhưng số tiền nộp thiếu so với hóa đơn, không phát thông báo thanh toán
    /// </summary>
    [Fact]
    public async Task ManualMatchAsync_OnUnderpaidMatch_DoesNotNotify()
    {
        // Arrange
        var (_, bill) = await SeedRoomAndBillAsync("HD006", 2000000m);

        var underpaidTx = new PaymentTransaction
        {
            TransactionId = "TXN_MANUAL_UNDERPAID_001",
            Amount = 1000000m, // Thiếu 1.000.000 VND
            Description = "Tien phong tra mot nua",
            TransactionDate = DateTime.UtcNow,
            Status = PaymentTransactionStatus.Unmatched,
            CreatedAt = DateTime.UtcNow
        };
        _context.PaymentTransactions.Add(underpaidTx);
        await _context.SaveChangesAsync();

        // Act
        var success = await _service.ManuallyAssignBillAsync(underpaidTx.Id, bill.Id, "Kế toán duyệt nộp 1 phần");

        // Assert
        success.Should().BeTrue();

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Unpaid); // Vẫn chưa thanh toán đủ

        _mockNotificationService.DidNotReceive().NotifyPaymentReceived(Arg.Any<PaymentReceivedEventArgs>());
    }

    /// <summary>
    /// Kiểm tra khi đối soát tự động thanh toán một phần (nộp thiếu tiền), không phát thông báo thanh toán
    /// </summary>
    [Fact]
    public async Task ReconcileAsync_OnPartiallyPaid_DoesNotNotify()
    {
        // Arrange
        var (_, bill) = await SeedRoomAndBillAsync("HD007", 1500000m);

        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_PARTIAL_001",
            Amount = 1000000m, // Thiếu 500.000 VND
            Description = "KTX HD007 NOP THIEU TIEN",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.PartiallyPaid);

        _mockNotificationService.DidNotReceive().NotifyPaymentReceived(Arg.Any<PaymentReceivedEventArgs>());
    }
}
