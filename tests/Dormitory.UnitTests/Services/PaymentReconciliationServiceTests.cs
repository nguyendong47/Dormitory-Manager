using System;
using System.Collections.Generic;
using System.Linq;
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
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị theo chuẩn TDD cho IPaymentReconciliationService và thuật toán đối soát tự động
/// </summary>
public class PaymentReconciliationServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly IPaymentReconciliationService _service;

    public PaymentReconciliationServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _service = new PaymentReconciliationService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    #region 1. Unit Tests cho ExtractBillCode

    [Theory]
    [InlineData("KTX HD001 NGUYEN VAN A", "HD001")]
    [InlineData("KTX-HD-2024-10-P101", "HD-2024-10-P101")]
    [InlineData("KTXHD005", "HD005")]
    [InlineData("ktxhd123", "hd123")]
    [InlineData("Chuyen tien phong KTX HD002", "HD002")]
    [InlineData("SV NGUYEN VAN B KTX_HD_999 NOP TIEN", "HD_999")]
    [InlineData("Thanh toan tien KTX: HD-888-2026", "HD-888-2026")]
    public void ExtractBillCode_WithValidSyntax_ShouldReturnExtractedCode(string description, string expectedCode)
    {
        // Act
        var result = _service.ExtractBillCode(description);

        // Assert
        result.Should().NotBeNull();
        result!.ToLowerInvariant().Should().Be(expectedCode.ToLowerInvariant());
    }

    [Theory]
    [InlineData("Chuyen tien phong thang 10")]
    [InlineData("Nguyen Van A chuyen khoan")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("KTX")]
    [InlineData("KTX   ")]
    [InlineData("KTX - ")]
    public void ExtractBillCode_WithInvalidOrMissingSyntax_ShouldReturnNull(string description)
    {
        // Act
        var result = _service.ExtractBillCode(description);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractBillCode_WithCustomPrefix_ShouldExtractAccordingly()
    {
        // Act
        var result = _service.ExtractBillCode("DORM BILL-00123 LE THI C", prefix: "DORM");

        // Assert
        result.Should().Be("BILL-00123");
    }

    [Fact]
    public void ExtractBillCode_WithLongOrComplexPayload_ShouldSafelyHandleWithoutHanging()
    {
        // Arrange: Chuỗi dài với nhiều ký tự lặp lại kiểm tra an toàn ReDoS
        var maliciousString = "KTX " + new string('-', 1000) + " HD001 " + new string('A', 2000);

        // Act
        var result = _service.ExtractBillCode(maliciousString);

        // Assert
        // Không bị treo hoặc ném ngoại lệ
        result.Should().Be("HD001");
    }

    #endregion

    #region 2. Unit Tests cho ProcessTransactionAsync

    [Fact]
    public async Task ProcessTransactionAsync_DuplicateTransactionId_ShouldReturnDuplicateStatus()
    {
        // Arrange
        var existingTx = new PaymentTransaction
        {
            TransactionId = "TXN_DUPLICATE_001",
            Amount = 1000000m,
            Description = "KTX HD001",
            TransactionDate = DateTime.UtcNow.AddMinutes(-30),
            Status = PaymentTransactionStatus.Success
        };
        _context.PaymentTransactions.Add(existingTx);
        await _context.SaveChangesAsync();

        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_DUPLICATE_001",
            Amount = 1000000m,
            Description = "KTX HD001",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.Duplicate);
        result.TransactionId.Should().Be("TXN_DUPLICATE_001");
        result.Message.Should().Contain("trùng lặp");

        var totalTxCount = await _context.PaymentTransactions.CountAsync();
        totalTxCount.Should().Be(1);
    }

    [Fact]
    public async Task ProcessTransactionAsync_NoBillCodeFound_ShouldRecordUnmatchedTransaction()
    {
        // Arrange
        var payload = new WebhookPayloadDto
        {
            Gateway = "Casso",
            TransactionId = "TXN_NO_CODE_001",
            Amount = 1200000m,
            Description = "Chuyen tien phong khong ghi ma hoa don",
            AccountNumber = "123456789",
            BankBin = "970422",
            TransactionDate = DateTime.UtcNow,
            RawData = "{\"mock\":\"json\"}"
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        result.BillId.Should().BeNull();
        result.BillCode.Should().BeEmpty();

        var savedTx = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.TransactionId == "TXN_NO_CODE_001");
        savedTx.Should().NotBeNull();
        savedTx!.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        savedTx.BillId.Should().BeNull();
        savedTx.Note.Should().Contain("Không tìm thấy mã hóa đơn");
    }

    [Fact]
    public async Task ProcessTransactionAsync_BillCodeNotFoundInDb_ShouldRecordUnmatchedTransaction()
    {
        // Arrange
        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_NOT_FOUND_BILL_001",
            Amount = 1500000m,
            Description = "KTX HD_NON_EXISTENT_9999",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        result.BillCode.Should().Be("HD_NON_EXISTENT_9999");

        var savedTx = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.TransactionId == "TXN_NOT_FOUND_BILL_001");
        savedTx.Should().NotBeNull();
        savedTx!.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        savedTx.Note.Should().Contain("không tồn tại");
    }

    [Fact]
    public async Task ProcessTransactionAsync_ExactAmount_ShouldReconcileAndMarkBillPaid()
    {
        // Arrange
        var room = new Room { RoomNumber = "P201", Building = "Tòa A", PricePerMonth = 1500000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-2026-10-P201",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1500000m,
            OldElectricIndex = 100,
            NewElectricIndex = 120, // 20 kWh * 3500 = 70,000
            ElectricRate = 3500,
            OldWaterIndex = 10,
            NewWaterIndex = 12, // 2 m3 * 15000 = 30,000
            WaterRate = 15000,
            OtherServiceFee = 50000,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(5)
        };
        // TotalAmount = 1500000 + 70000 + 30000 + 50000 = 1650000
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_EXACT_001",
            Amount = 1650000m,
            Description = "KTX HD-2026-10-P201 NGUYEN VAN A",
            AccountNumber = "0987654321",
            BankBin = "970422",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(PaymentTransactionStatus.Success);
        result.BillId.Should().Be(bill.Id);
        result.BillCode.Should().Be("HD-2026-10-P201");
        result.Amount.Should().Be(1650000m);

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Paid);
        updatedBill.PaidDate.Should().NotBeNull();
        updatedBill.PaidDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedTx = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.TransactionId == "TXN_EXACT_001");
        savedTx.Should().NotBeNull();
        savedTx!.Status.Should().Be(PaymentTransactionStatus.Success);
        savedTx.BillId.Should().Be(bill.Id);
    }

    [Fact]
    public async Task ProcessTransactionAsync_OverpaidAmount_ShouldReconcileMarkBillPaidAndAddNote()
    {
        // Arrange
        var room = new Room { RoomNumber = "P202", Building = "Tòa A", PricePerMonth = 1000000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-2026-10-P202",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1000000m,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(5)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_OVERPAID_001",
            Amount = 1200000m, // Hóa đơn 1,000,000 nhưng chuyển 1,200,000 (thừa 200,000)
            Description = "KTX HD-2026-10-P202 chuyen du tien",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(PaymentTransactionStatus.Success);

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Paid);

        var savedTx = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.TransactionId == "TXN_OVERPAID_001");
        savedTx.Should().NotBeNull();
        savedTx!.Status.Should().Be(PaymentTransactionStatus.Success);
        savedTx.Note.Should().Contain("thừa 200,000");
    }

    [Fact]
    public async Task ProcessTransactionAsync_UnderpaidAmount_ShouldMarkPartiallyPaidAndKeepBillUnpaid()
    {
        // Arrange
        var room = new Room { RoomNumber = "P203", Building = "Tòa A", PricePerMonth = 2000000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-2026-10-P203",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 2000000m,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(5)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var payload = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_UNDERPAID_001",
            Amount = 1000000m, // Cần 2,000,000 nhưng chỉ chuyển 1,000,000 (thiếu 1,000,000)
            Description = "KTX HD-2026-10-P203 nop truoc 1 nua",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.ProcessTransactionAsync(payload);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(PaymentTransactionStatus.PartiallyPaid);
        result.Message.Should().Contain("thiếu");

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Unpaid); // Vẫn chưa thanh toán đủ
        updatedBill.PaidDate.Should().BeNull();

        var savedTx = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.TransactionId == "TXN_UNDERPAID_001");
        savedTx.Should().NotBeNull();
        savedTx!.Status.Should().Be(PaymentTransactionStatus.PartiallyPaid);
        savedTx.BillId.Should().Be(bill.Id);
        savedTx.Note.Should().Contain("thiếu 1,000,000");
    }

    #endregion

    #region 3. Unit Tests cho ManuallyAssignBillAsync

    [Fact]
    public async Task ManuallyAssignBillAsync_ValidAssignment_ShouldUpdateBillAndTransaction()
    {
        // Arrange
        var room = new Room { RoomNumber = "P301", Building = "Tòa B", PricePerMonth = 1500000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-MANUAL-001",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1500000m,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var unmatchedTx = new PaymentTransaction
        {
            TransactionId = "TXN_MANUAL_001",
            Amount = 1500000m,
            Description = "Chuyen tien phong khong ro ma",
            Status = PaymentTransactionStatus.Unmatched,
            TransactionDate = DateTime.UtcNow
        };
        _context.PaymentTransactions.Add(unmatchedTx);
        await _context.SaveChangesAsync();

        // Act
        var success = await _service.ManuallyAssignBillAsync(unmatchedTx.Id, bill.Id, "Kế toán xác nhận thủ công");

        // Assert
        success.Should().BeTrue();

        var updatedTx = await _context.PaymentTransactions.FindAsync(unmatchedTx.Id);
        updatedTx!.Status.Should().Be(PaymentTransactionStatus.Success);
        updatedTx.BillId.Should().Be(bill.Id);
        updatedTx.BillCode.Should().Be(bill.BillCode);
        updatedTx.Note.Should().Contain("Kế toán xác nhận thủ công");

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Paid);
        updatedBill.PaidDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ManuallyAssignBillAsync_UnderpaidAmount_ShouldMarkTransactionPartiallyPaidAndKeepBillUnpaid()
    {
        // Arrange
        var room = new Room { RoomNumber = "P302", Building = "Tòa B", PricePerMonth = 2000000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-MANUAL-002",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 2000000m,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var tx = new PaymentTransaction
        {
            TransactionId = "TXN_MANUAL_002",
            Amount = 800000m, // Thiếu tiền
            Description = "Chuyen tien dot 1",
            Status = PaymentTransactionStatus.Unmatched,
            TransactionDate = DateTime.UtcNow
        };
        _context.PaymentTransactions.Add(tx);
        await _context.SaveChangesAsync();

        // Act
        var success = await _service.ManuallyAssignBillAsync(tx.Id, bill.Id, "Gán giao dịch nộp thiếu");

        // Assert
        success.Should().BeTrue();

        var updatedTx = await _context.PaymentTransactions.FindAsync(tx.Id);
        updatedTx!.Status.Should().Be(PaymentTransactionStatus.PartiallyPaid);
        updatedTx.BillId.Should().Be(bill.Id);

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Unpaid);
    }

    [Fact]
    public async Task ManuallyAssignBillAsync_NonExistentEntities_ShouldReturnFalse()
    {
        // Act
        var result1 = await _service.ManuallyAssignBillAsync(9999, 1);
        var result2 = await _service.ManuallyAssignBillAsync(1, 9999);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    #endregion

    #region 4. Unit Tests cho GetTransactionsAsync

    [Fact]
    public async Task GetTransactionsAsync_FilteringAndSearch_ShouldReturnMatchingTransactions()
    {
        // Arrange
        var room = new Room { RoomNumber = "P401", Building = "Tòa C", PricePerMonth = 1500000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill1 = new Bill { BillCode = "HD001", RoomId = room.Id, Month = 10, Year = 2026, RoomFee = 1500000m, DueDate = DateTime.UtcNow.AddDays(7) };
        var bill2 = new Bill { BillCode = "HD002", RoomId = room.Id, Month = 10, Year = 2026, RoomFee = 1500000m, DueDate = DateTime.UtcNow.AddDays(7) };
        _context.Bills.AddRange(bill1, bill2);
        await _context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var list = new List<PaymentTransaction>
        {
            new() { TransactionId = "TX_A", Amount = 100000m, Description = "KTX HD001", Status = PaymentTransactionStatus.Success, TransactionDate = now.AddDays(-5), BillCode = "HD001", BillId = bill1.Id },
            new() { TransactionId = "TX_B", Amount = 200000m, Description = "KTX HD002", Status = PaymentTransactionStatus.PartiallyPaid, TransactionDate = now.AddDays(-3), BillCode = "HD002", BillId = bill2.Id },
            new() { TransactionId = "TX_C", Amount = 300000m, Description = "Chuyen tien sai", Status = PaymentTransactionStatus.Unmatched, TransactionDate = now.AddDays(-1), BillCode = "" }
        };
        _context.PaymentTransactions.AddRange(list);
        await _context.SaveChangesAsync();

        // Act 1: Lọc theo Status
        var filterByStatus = new PaymentTransactionFilterDto { Status = PaymentTransactionStatus.Success };
        var resultsStatus = await _service.GetTransactionsAsync(filterByStatus);
        resultsStatus.Should().HaveCount(1);
        resultsStatus.First().TransactionId.Should().Be("TX_A");

        // Act 2: Lọc theo SearchKeyword
        var filterBySearch = new PaymentTransactionFilterDto { SearchKeyword = "HD002" };
        var resultsSearch = await _service.GetTransactionsAsync(filterBySearch);
        resultsSearch.Should().HaveCount(1);
        resultsSearch.First().TransactionId.Should().Be("TX_B");

        // Act 3: Lọc theo BillId
        var filterByBill = new PaymentTransactionFilterDto { BillId = bill1.Id };
        var resultsBill = await _service.GetTransactionsAsync(filterByBill);
        resultsBill.Should().HaveCount(1);
        resultsBill.First().TransactionId.Should().Be("TX_A");

        // Act 4: Lọc theo khoảng thời gian
        var filterByDate = new PaymentTransactionFilterDto
        {
            FromDate = now.AddDays(-4),
            ToDate = now
        };
        var resultsDate = await _service.GetTransactionsAsync(filterByDate);
        resultsDate.Should().HaveCount(2);
        resultsDate.Select(t => t.TransactionId).Should().Contain(new[] { "TX_B", "TX_C" });
    }

    #endregion
}
