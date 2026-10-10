using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị cho WebhookListenerService, phân tích cú pháp webhook và bảo mật chữ ký HMAC
/// </summary>
public class WebhookListenerServiceTests : IDisposable
{
    private readonly string _tempSettingsFile;
    private readonly IServiceProvider _serviceProvider;
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;

    public WebhookListenerServiceTests()
    {
        _tempSettingsFile = Path.Combine(Path.GetTempPath(), $"webhook_test_{Guid.NewGuid()}.json");

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        var services = new ServiceCollection();
        services.AddSingleton<IDormitoryDbContext>(_context);
        services.AddScoped<IPaymentReconciliationService, PaymentReconciliationService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        if (File.Exists(_tempSettingsFile))
        {
            try { File.Delete(_tempSettingsFile); } catch { }
        }

        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetSettingsAsync_DefaultSettings_ShouldReturnDefaultValues()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        // Act
        var settings = await service.GetSettingsAsync();

        // Assert
        settings.Should().NotBeNull();
        settings.IsEnabled.Should().BeFalse();
        settings.Port.Should().Be(5005);
        settings.Path.Should().Be("/api/webhook/payment");
        settings.Provider.Should().Be("PayOS");
        settings.TransferPrefix.Should().Be("KTX");
    }

    [Fact]
    public async Task SaveSettingsAsync_And_GetSettingsAsync_ShouldPersistAndReload()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var newSettings = new WebhookSettingsDto
        {
            IsEnabled = true,
            Port = 6006,
            Path = "/webhook/custom",
            SecretKey = "my_secret_token_123",
            Provider = "Casso",
            TransferPrefix = "DORM"
        };

        // Act
        var saveResult = await service.SaveSettingsAsync(newSettings);
        var loadedSettings = await service.GetSettingsAsync();

        // Assert
        saveResult.Should().BeTrue();
        loadedSettings.IsEnabled.Should().BeTrue();
        loadedSettings.Port.Should().Be(6006);
        loadedSettings.Path.Should().Be("/webhook/custom");
        loadedSettings.SecretKey.Should().Be("my_secret_token_123");
        loadedSettings.Provider.Should().Be("Casso");
        loadedSettings.TransferPrefix.Should().Be("DORM");
    }

    [Fact]
    public void ValidateSignature_HMACSHA256_ShouldValidateCorrectly()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var secret = "secret_key_xyz";
        var payload = "{\"amount\":1000000,\"desc\":\"test\"}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var validSignature = Convert.ToHexString(hash);

        // Act & Assert
        // 1. Chữ ký hợp lệ
        service.ValidateSignature(payload, validSignature, secret).Should().BeTrue();

        // 2. Chữ ký viết thường vẫn hợp lệ (case-insensitive)
        service.ValidateSignature(payload, validSignature.ToLowerInvariant(), secret).Should().BeTrue();

        // 3. Chữ ký sai
        service.ValidateSignature(payload, "invalid_sig_123", secret).Should().BeFalse();

        // 4. Token trực tiếp khớp với secret
        service.ValidateSignature(payload, secret, secret).Should().BeTrue();

        // 5. SecretKey rỗng thì chấp nhận
        service.ValidateSignature(payload, "", "").Should().BeTrue();
        service.ValidateSignature(payload, "any_sig", "").Should().BeTrue();

        // 6. Có secretKey nhưng signature rỗng -> Từ chối
        service.ValidateSignature(payload, "", secret).Should().BeFalse();
    }

    [Fact]
    public void ParseIncomingPayload_PayOSWebhookJson_ShouldParseFieldsCorrectly()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var payosJson = """
        {
            "code": "00",
            "desc": "success",
            "data": {
                "orderCode": 998877,
                "amount": 1500000,
                "description": "KTX HD-2026-10-P101 NGUYEN VAN A",
                "accountNumber": "0123456789",
                "reference": "FT261010001",
                "transactionDateTime": "2026-10-10 10:30:00"
            },
            "signature": "mock_payos_signature"
        }
        """;

        // Act
        var result = service.ParseIncomingPayload(payosJson, "PayOS");

        // Assert
        result.Should().NotBeNull();
        result!.Gateway.Should().Be("PayOS");
        result.TransactionId.Should().Be("FT261010001");
        result.Amount.Should().Be(1500000m);
        result.Description.Should().Be("KTX HD-2026-10-P101 NGUYEN VAN A");
        result.AccountNumber.Should().Be("0123456789");
        result.Signature.Should().Be("mock_payos_signature");
        result.TransactionDate.Should().Be(DateTime.Parse("2026-10-10 10:30:00"));
    }

    [Fact]
    public void ParseIncomingPayload_CassoWebhookJson_ShouldParseFieldsCorrectly()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var cassoJson = """
        {
            "error": 0,
            "data": [
                {
                    "id": 12345,
                    "tid": "TID_CASSO_999",
                    "description": "KTX HD002",
                    "amount": 2500000,
                    "when": "2026-10-10 09:15:00",
                    "subAccId": "9876543210",
                    "bankName": "Vietcombank"
                }
            ]
        }
        """;

        // Act
        var result = service.ParseIncomingPayload(cassoJson, "Casso");

        // Assert
        result.Should().NotBeNull();
        result!.Gateway.Should().Be("Casso");
        result.TransactionId.Should().Be("TID_CASSO_999");
        result.Amount.Should().Be(2500000m);
        result.Description.Should().Be("KTX HD002");
        result.AccountNumber.Should().Be("9876543210");
        result.TransactionDate.Should().Be(DateTime.Parse("2026-10-10 09:15:00"));
    }

    [Fact]
    public void ParseIncomingPayload_GenericWebhookJson_ShouldParseFieldsCorrectly()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var genericJson = """
        {
            "gateway": "Generic",
            "transactionId": "GEN_TX_555",
            "amount": 800000,
            "description": "KTX HD003",
            "accountNumber": "1122334455",
            "bankBin": "970422",
            "transactionDate": "2026-10-10T08:00:00Z",
            "signature": "sig_gen"
        }
        """;

        // Act
        var result = service.ParseIncomingPayload(genericJson, "Generic");

        // Assert
        result.Should().NotBeNull();
        result!.Gateway.Should().Be("Generic");
        result.TransactionId.Should().Be("GEN_TX_555");
        result.Amount.Should().Be(800000m);
        result.Description.Should().Be("KTX HD003");
        result.AccountNumber.Should().Be("1122334455");
        result.BankBin.Should().Be("970422");
        result.Signature.Should().Be("sig_gen");
    }

    [Fact]
    public async Task StartAsync_And_StopAsync_ShouldToggleListenerState()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var randomPort = Random.Shared.Next(20000, 30000);
        await service.SaveSettingsAsync(new WebhookSettingsDto
        {
            IsEnabled = true,
            Port = randomPort,
            Path = "/api/test/webhook"
        });

        // Act & Assert 1: Start
        var startResult = await service.StartAsync();
        startResult.Should().BeTrue();
        service.IsRunning.Should().BeTrue();
        service.ActivePort.Should().Be(randomPort);

        // Act & Assert 2: Stop
        await service.StopAsync();
        service.IsRunning.Should().BeFalse();
        service.ActivePort.Should().Be(0);
    }

    [Fact]
    public async Task HttpListener_IncomingHttpRequest_ShouldReturn200AndTriggerReconciliation()
    {
        // Arrange
        var room = new Room { RoomNumber = "P501", Building = "Tòa A", PricePerMonth = 1500000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-HTTP-001",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1500000m,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var port = Random.Shared.Next(30001, 40000);
        await service.SaveSettingsAsync(new WebhookSettingsDto
        {
            IsEnabled = true,
            Port = port,
            Path = "/api/webhook/payment",
            Provider = "PayOS"
        });

        var started = await service.StartAsync();
        started.Should().BeTrue();

        try
        {
            var jsonPayload = $$"""
            {
                "code": "00",
                "desc": "success",
                "data": {
                    "orderCode": 12345,
                    "amount": 1500000,
                    "description": "KTX HD-HTTP-001 NGUYEN VAN A",
                    "accountNumber": "0123456789",
                    "reference": "TXN_HTTP_TEST_01",
                    "transactionDateTime": "2026-10-10 12:00:00"
                }
            }
            """;

            using var client = new HttpClient();
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync($"http://127.0.0.1:{port}/api/webhook/payment/", content);

            // Assert: Server phản hồi HTTP 200 OK ngay lập tức
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Webhook received successfully");

            // Chờ tác vụ background đối soát thực thi
            await Task.Delay(400);

            var savedTx = await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.TransactionId == "TXN_HTTP_TEST_01");
            savedTx.Should().NotBeNull();
            savedTx!.Status.Should().Be(PaymentTransactionStatus.Success);
            savedTx.BillId.Should().Be(bill.Id);

            var updatedBill = await _context.Bills.FindAsync(bill.Id);
            updatedBill!.Status.Should().Be(BillStatus.Paid);
        }
        finally
        {
            await service.StopAsync();
        }
    }

    [Fact]
    public async Task HttpListener_InvalidSignature_ShouldReturn401Unauthorized()
    {
        // Arrange
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var port = Random.Shared.Next(40001, 50000);
        await service.SaveSettingsAsync(new WebhookSettingsDto
        {
            IsEnabled = true,
            Port = port,
            Path = "/api/webhook/payment",
            SecretKey = "super_secret_key"
        });

        var started = await service.StartAsync();
        started.Should().BeTrue();

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Signature", "wrong_signature_here");
            var content = new StringContent("{\"test\": 1}", Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync($"http://127.0.0.1:{port}/api/webhook/payment/", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        finally
        {
            await service.StopAsync();
        }
    }

    [Fact]
    public async Task TestWebhookAsync_ShouldProcessDirectlyWithoutHttpConnection()
    {
        // Arrange
        var room = new Room { RoomNumber = "P502", Building = "Tòa A", PricePerMonth = 1000000m };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var bill = new Bill
        {
            BillCode = "HD-TEST-DIRECT",
            RoomId = room.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1000000m,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(7)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var service = new WebhookListenerService(scopeFactory, _tempSettingsFile);

        var payload = new WebhookPayloadDto
        {
            Gateway = "Generic",
            TransactionId = "DIRECT_TX_001",
            Amount = 1000000m,
            Description = "KTX HD-TEST-DIRECT",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await service.TestWebhookAsync(payload);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(PaymentTransactionStatus.Success);
        result.BillId.Should().Be(bill.Id);

        var updatedBill = await _context.Bills.FindAsync(bill.Id);
        updatedBill!.Status.Should().Be(BillStatus.Paid);
    }
}
