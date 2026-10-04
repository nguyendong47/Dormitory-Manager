using System;
using System.IO;
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
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị cho dịch vụ gửi email hóa đơn tự động kèm PDF (EmailService)
/// </summary>
public class EmailServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly string _tempSettingsPath;
    private readonly IEmailService _service;

    public EmailServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _tempSettingsPath = Path.Combine(Path.GetTempPath(), $"test_emailsettings_{Guid.NewGuid():N}.json");

        _service = new EmailService(_context, _tempSettingsPath);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();

        if (File.Exists(_tempSettingsPath))
        {
            try { File.Delete(_tempSettingsPath); } catch { }
        }
    }

    private async Task<Bill> SeedSampleBillAsync()
    {
        var room = new Room
        {
            Id = 1,
            RoomNumber = "P101",
            Building = "Tòa A",
            Floor = 1,
            Capacity = 4,
            CurrentOccupancy = 1,
            PricePerMonth = 1500000m,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);

        var bill = new Bill
        {
            Id = 1,
            BillCode = "HD-202410-A101",
            RoomId = 1,
            Month = 10,
            Year = 2024,
            RoomFee = 1500000m,
            OldElectricIndex = 100m,
            NewElectricIndex = 150m,
            ElectricRate = 3500m,
            OldWaterIndex = 10m,
            NewWaterIndex = 15m,
            WaterRate = 15000m,
            OtherServiceFee = 50000m,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.Today.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();
        return bill;
    }

    [Fact]
    public async Task SendBillInvoiceEmailAsync_InMockMode_ShouldReturnSuccess()
    {
        // Sắp đặt
        await SeedSampleBillAsync();
        var fakePdfBytes = Encoding.UTF8.GetBytes("%PDF-1.4 Mock PDF Content");

        // Khi cấu hình mặc định (IsEnabled = false) hoặc địa chỉ kết thúc bằng @example.com
        var result = await _service.SendBillInvoiceEmailAsync(1, "student@example.com", "Nguyễn Văn A", fakePdfBytes);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task SendBillInvoiceEmailAsync_WithEmptyRecipientEmail_ShouldReturnFail()
    {
        // Sắp đặt
        await SeedSampleBillAsync();
        var fakePdfBytes = Encoding.UTF8.GetBytes("%PDF-1.4 Mock PDF Content");

        // Thực hiện
        var result = await _service.SendBillInvoiceEmailAsync(1, "", "Nguyễn Văn A", fakePdfBytes);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Địa chỉ email");
    }

    [Fact]
    public async Task SendBillInvoiceEmailAsync_WithInvalidEmailFormat_ShouldReturnFail()
    {
        // Sắp đặt
        await SeedSampleBillAsync();
        var fakePdfBytes = Encoding.UTF8.GetBytes("%PDF-1.4 Mock PDF Content");

        // Thực hiện với email không có ký tự @
        var result = await _service.SendBillInvoiceEmailAsync(1, "invalid-email-no-at-sign", "Nguyễn Văn A", fakePdfBytes);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("không hợp lệ");
    }

    [Fact]
    public async Task SendBillInvoiceEmailAsync_WithInvalidBillId_ShouldReturnFail()
    {
        // Sắp đặt
        var fakePdfBytes = Encoding.UTF8.GetBytes("%PDF-1.4 Mock PDF Content");

        // Thực hiện với ID không tồn tại
        var result = await _service.SendBillInvoiceEmailAsync(9999, "student@example.com", "Nguyễn Văn A", fakePdfBytes);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Không tìm thấy hóa đơn");
    }

    [Fact]
    public async Task SendBillInvoiceEmailAsync_WithEmptyPdfBytes_ShouldReturnFail()
    {
        // Sắp đặt
        await SeedSampleBillAsync();

        // Thực hiện với mảng byte rỗng
        var result = await _service.SendBillInvoiceEmailAsync(1, "student@example.com", "Nguyễn Văn A", Array.Empty<byte>());

        // Kiểm tra
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("PDF");
    }

    [Fact]
    public async Task TestSmtpConnectionAsync_InMockMode_ShouldReturnSuccess()
    {
        // Sắp đặt
        var settings = new EmailSettingsDto
        {
            SmtpServer = "mock",
            IsEnabled = false
        };

        // Thực hiện
        var result = await _service.TestSmtpConnectionAsync(settings);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task TestSmtpConnectionAsync_WithEmptyServer_ShouldReturnFail()
    {
        // Sắp đặt: Server rỗng và đã IsEnabled
        var settings = new EmailSettingsDto
        {
            SmtpServer = "",
            IsEnabled = true
        };

        // Thực hiện
        var result = await _service.TestSmtpConnectionAsync(settings);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("máy chủ SMTP");
    }

    [Fact]
    public async Task SaveAndGetEmailSettingsAsync_ShouldPersistAndRetrieveCorrectly()
    {
        // Sắp đặt
        var newSettings = new EmailSettingsDto
        {
            SmtpServer = "smtp.office365.com",
            SmtpPort = 587,
            SenderEmail = "ktx@university.edu.vn",
            SenderName = "KTX Đại Học Bách Khoa",
            Username = "ktx@university.edu.vn",
            Password = "SecretPassword123",
            EnableSsl = true,
            IsEnabled = true
        };

        // Thực hiện Lưu
        var saveResult = await _service.SaveEmailSettingsAsync(newSettings);
        saveResult.Should().BeTrue();

        // Thực hiện Lấy
        var retrieved = await _service.GetEmailSettingsAsync();

        // Kiểm tra
        retrieved.Should().NotBeNull();
        retrieved.SmtpServer.Should().Be("smtp.office365.com");
        retrieved.SmtpPort.Should().Be(587);
        retrieved.SenderEmail.Should().Be("ktx@university.edu.vn");
        retrieved.SenderName.Should().Be("KTX Đại Học Bách Khoa");
        retrieved.Username.Should().Be("ktx@university.edu.vn");
        retrieved.Password.Should().Be("SecretPassword123");
        retrieved.EnableSsl.Should().BeTrue();
        retrieved.IsEnabled.Should().BeTrue();
    }
}
