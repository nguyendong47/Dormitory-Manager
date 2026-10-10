using Dormitory.Application.DTOs;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.UnitTests.Entities;

/// <summary>
/// Kiểm thử đơn vị cho thực thể PaymentTransaction, các DTOs liên quan và cấu hình EF Core SQLite
/// </summary>
public class PaymentTransactionEntityTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;

    public PaymentTransactionEntityTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public void PaymentTransaction_DefaultValues_ShouldBeInitializedProperly()
    {
        // Act
        var transaction = new PaymentTransaction();

        // Assert
        transaction.Id.Should().Be(0);
        transaction.TransactionId.Should().BeEmpty();
        transaction.BankBin.Should().BeEmpty();
        transaction.AccountNumber.Should().BeEmpty();
        transaction.Amount.Should().Be(0);
        transaction.Description.Should().BeEmpty();
        transaction.TransactionDate.Should().Be(default);
        transaction.BillId.Should().BeNull();
        transaction.BillCode.Should().BeEmpty();
        transaction.Status.Should().Be(PaymentTransactionStatus.Unmatched);
        transaction.RawPayload.Should().BeEmpty();
        transaction.Note.Should().BeEmpty();
        transaction.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        transaction.Bill.Should().BeNull();
    }

    [Fact]
    public void PaymentTransaction_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var now = DateTime.Now;
        var transactionDate = now.AddMinutes(-10);

        // Act
        var transaction = new PaymentTransaction
        {
            Id = 1,
            TransactionId = "TXN_20261010_001",
            BankBin = "970422",
            AccountNumber = "0123456789",
            Amount = 1500000m,
            Description = "KTX HD2026-10-P101",
            TransactionDate = transactionDate,
            BillId = 10,
            BillCode = "HD2026-10-P101",
            Status = PaymentTransactionStatus.Success,
            RawPayload = "{\"gateway\":\"PayOS\",\"amount\":1500000}",
            Note = "Đối soát tự động thành công",
            CreatedAt = now
        };

        // Assert
        transaction.Id.Should().Be(1);
        transaction.TransactionId.Should().Be("TXN_20261010_001");
        transaction.BankBin.Should().Be("970422");
        transaction.AccountNumber.Should().Be("0123456789");
        transaction.Amount.Should().Be(1500000m);
        transaction.Description.Should().Be("KTX HD2026-10-P101");
        transaction.TransactionDate.Should().Be(transactionDate);
        transaction.BillId.Should().Be(10);
        transaction.BillCode.Should().Be("HD2026-10-P101");
        transaction.Status.Should().Be(PaymentTransactionStatus.Success);
        transaction.RawPayload.Should().Be("{\"gateway\":\"PayOS\",\"amount\":1500000}");
        transaction.Note.Should().Be("Đối soát tự động thành công");
        transaction.CreatedAt.Should().Be(now);
    }

    [Fact]
    public async Task PaymentTransaction_DatabasePersistence_ShouldPersistAndRetrieveCorrectly()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "P101",
            Building = "Tòa A",
            Capacity = 4,
            PricePerMonth = 1500000m,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-2026-10-P101",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1500000m,
            OldElectricIndex = 100,
            NewElectricIndex = 150,
            OldWaterIndex = 20,
            NewWaterIndex = 30,
            DueDate = DateTime.Now.AddDays(7),
            Status = BillStatus.Unpaid
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var transaction = new PaymentTransaction
        {
            TransactionId = "TXN_998877",
            BankBin = "970422",
            AccountNumber = "0987654321",
            Amount = bill.TotalAmount,
            Description = "KTX HD-2026-10-P101",
            TransactionDate = DateTime.Now,
            BillId = bill.Id,
            BillCode = bill.BillCode,
            Status = PaymentTransactionStatus.Success,
            RawPayload = "{\"mock\":\"data\"}",
            Note = "Khớp hóa đơn tự động"
        };
        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var retrieved = await _context.PaymentTransactions
            .Include(pt => pt.Bill)
            .FirstOrDefaultAsync(pt => pt.TransactionId == "TXN_998877");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().BeGreaterThan(0);
        retrieved.TransactionId.Should().Be("TXN_998877");
        retrieved.BillId.Should().Be(bill.Id);
        retrieved.Bill.Should().NotBeNull();
        retrieved.Bill!.BillCode.Should().Be("HD-2026-10-P101");
        retrieved.Status.Should().Be(PaymentTransactionStatus.Success);
    }

    [Fact]
    public async Task Bill_PaymentTransactionsCollection_ShouldTrackTransactions()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "P102",
            Building = "Tòa B",
            Capacity = 4,
            PricePerMonth = 1200000m,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-2026-10-P102",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1200000m,
            DueDate = DateTime.Now.AddDays(7),
            Status = BillStatus.Unpaid
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var tx1 = new PaymentTransaction
        {
            TransactionId = "TXN_PART_1",
            BankBin = "970422",
            AccountNumber = "11223344",
            Amount = 500000m,
            Description = "KTX HD-2026-10-P102 dot 1",
            TransactionDate = DateTime.Now.AddHours(-2),
            BillId = bill.Id,
            BillCode = bill.BillCode,
            Status = PaymentTransactionStatus.PartiallyPaid
        };

        var tx2 = new PaymentTransaction
        {
            TransactionId = "TXN_PART_2",
            BankBin = "970422",
            AccountNumber = "11223344",
            Amount = 700000m,
            Description = "KTX HD-2026-10-P102 dot 2",
            TransactionDate = DateTime.Now.AddMinutes(-30),
            BillId = bill.Id,
            BillCode = bill.BillCode,
            Status = PaymentTransactionStatus.Success
        };

        _context.PaymentTransactions.AddRange(tx1, tx2);
        await _context.SaveChangesAsync();

        // Act
        var retrievedBill = await _context.Bills
            .Include(b => b.PaymentTransactions)
            .FirstOrDefaultAsync(b => b.Id == bill.Id);

        // Assert
        retrievedBill.Should().NotBeNull();
        retrievedBill!.PaymentTransactions.Should().HaveCount(2);
        retrievedBill.PaymentTransactions.Select(t => t.TransactionId)
            .Should().Contain(new[] { "TXN_PART_1", "TXN_PART_2" });
    }

    [Fact]
    public async Task PaymentTransaction_UniqueTransactionId_ShouldThrowOnDuplicate()
    {
        // Arrange
        var tx1 = new PaymentTransaction
        {
            TransactionId = "DUP_TXN_001",
            Amount = 100000m,
            Description = "Test dup 1",
            TransactionDate = DateTime.Now
        };
        _context.PaymentTransactions.Add(tx1);
        await _context.SaveChangesAsync();

        var tx2 = new PaymentTransaction
        {
            TransactionId = "DUP_TXN_001",
            Amount = 100000m,
            Description = "Test dup 2",
            TransactionDate = DateTime.Now
        };
        _context.PaymentTransactions.Add(tx2);

        // Act & Assert
        var act = async () => await _context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public void PaymentTransactionDtos_ShouldStoreAndMapPropertiesCorrectly()
    {
        // Arrange & Act
        var now = DateTime.Now;
        var dto = new PaymentTransactionDto
        {
            Id = 5,
            TransactionId = "TXN_DTO_123",
            BankBin = "970415",
            AccountNumber = "9988776655",
            Amount = 2500000m,
            Description = "KTX HD005 NGUYEN VAN A",
            TransactionDate = now.AddDays(-1),
            BillId = 12,
            BillCode = "HD005",
            Status = PaymentTransactionStatus.Success,
            RawPayload = "{\"test\":true}",
            Note = "Gạch nợ chuẩn",
            CreatedAt = now
        };

        var filter = new PaymentTransactionFilterDto
        {
            Status = PaymentTransactionStatus.Success,
            FromDate = now.AddDays(-7),
            ToDate = now,
            SearchKeyword = "HD005",
            BillId = 12
        };

        var resultDto = new PaymentReconciliationResultDto
        {
            IsSuccess = true,
            Status = PaymentTransactionStatus.Success,
            TransactionId = "TXN_DTO_123",
            BillId = 12,
            BillCode = "HD005",
            Amount = 2500000m,
            Message = "Đối soát thành công"
        };

        var webhookDto = new WebhookPayloadDto
        {
            Gateway = "PayOS",
            TransactionId = "TXN_DTO_123",
            Amount = 2500000m,
            Description = "KTX HD005 NGUYEN VAN A",
            AccountNumber = "9988776655",
            BankBin = "970415",
            TransactionDate = now.AddDays(-1),
            Signature = "mock_signature_hmac",
            RawData = "{\"mock\":\"json\"}"
        };

        // Assert
        dto.Id.Should().Be(5);
        dto.TransactionId.Should().Be("TXN_DTO_123");
        dto.Status.Should().Be(PaymentTransactionStatus.Success);
        dto.Amount.Should().Be(2500000m);

        filter.Status.Should().Be(PaymentTransactionStatus.Success);
        filter.SearchKeyword.Should().Be("HD005");
        filter.BillId.Should().Be(12);

        resultDto.IsSuccess.Should().BeTrue();
        resultDto.Status.Should().Be(PaymentTransactionStatus.Success);
        resultDto.Message.Should().Be("Đối soát thành công");

        webhookDto.Gateway.Should().Be("PayOS");
        webhookDto.TransactionId.Should().Be("TXN_DTO_123");
        webhookDto.Signature.Should().Be("mock_signature_hmac");
        webhookDto.RawData.Should().Be("{\"mock\":\"json\"}");
    }
}
