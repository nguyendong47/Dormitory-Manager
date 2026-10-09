using System;
using System.IO;
using System.Linq;
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
using Moq;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị cho dịch vụ xuất phiếu thu tiền phòng ra file PDF (PdfExportService)
/// </summary>
public class PdfExportServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly IPdfExportService _service;

    public PdfExportServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _service = new PdfExportService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GenerateBillReceiptPdfAsync_WithValidBill_ShouldReturnValidPdfDocumentBytes()
    {
        // Sắp đặt dữ liệu mẫu: Phòng, Sinh viên, Hợp đồng, Hóa đơn
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

        var student = new Student
        {
            Id = 1,
            StudentCode = "SV202401",
            FullName = "Nguyễn Văn An",
            DateOfBirth = new DateTime(2003, 5, 20),
            Gender = Gender.Male,
            IdentityCard = "001203001234",
            PhoneNumber = "0987654321",
            ClassName = "CNTT-K18",
            Faculty = "Công nghệ thông tin",
            HomeTown = "Hà Nội"
        };
        _context.Students.Add(student);

        var contract = new Contract
        {
            Id = 1,
            ContractNumber = "HD-2024-001",
            RoomId = 1,
            StudentId = 1,
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 12, 31),
            DepositAmount = 1500000m,
            MonthlyRate = 1500000m,
            Status = ContractStatus.Active
        };
        _context.Contracts.Add(contract);

        var bill = new Bill
        {
            Id = 1,
            BillCode = "HD-202410-A101",
            RoomId = 1,
            Month = 10,
            Year = 2024,
            RoomFee = 1500000m,
            OldElectricIndex = 120m,
            NewElectricIndex = 170m, // 50 kWh
            ElectricRate = 3500m,
            OldWaterIndex = 10m,
            NewWaterIndex = 16m, // 6 m3
            WaterRate = 15000m,
            OtherServiceFee = 50000m,
            Status = BillStatus.Paid,
            Note = "Thanh toán đúng hạn",
            DueDate = new DateTime(2024, 10, 25),
            PaidDate = new DateTime(2024, 10, 20)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        // Thực hiện
        var pdfBytes = await _service.GenerateBillReceiptPdfAsync(1);

        // Kiểm tra
        pdfBytes.Should().NotBeNull();
        pdfBytes.Length.Should().BeGreaterThan(100);

        // Header bytes của PDF luôn bắt đầu bằng %PDF-
        var header = Encoding.ASCII.GetString(pdfBytes.Take(5).ToArray());
        header.Should().Be("%PDF-");
    }

    [Fact]
    public async Task GenerateBillReceiptPdfAsync_WithNonExistentBill_ShouldThrowKeyNotFoundException()
    {
        // Thực hiện với ID không tồn tại
        var act = async () => await _service.GenerateBillReceiptPdfAsync(9999);

        // Kiểm tra ném KeyNotFoundException
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GenerateBillReceiptPdfAsync_WithUnpaidBillAndNoContract_ShouldReturnValidPdfDocumentBytes()
    {
        // Sắp đặt: Phòng và hóa đơn chưa thanh toán, chưa có hợp đồng sinh viên
        var room = new Room
        {
            Id = 2,
            RoomNumber = "P202",
            Building = "Tòa B",
            Floor = 2,
            Capacity = 2,
            CurrentOccupancy = 0,
            PricePerMonth = 2000000m,
            Type = RoomType.Vip,
            AllowedGender = Gender.Female,
            Status = RoomStatus.Available
        };
        _context.Rooms.Add(room);

        var bill = new Bill
        {
            Id = 2,
            BillCode = "HD-202410-B202",
            RoomId = 2,
            Month = 10,
            Year = 2024,
            RoomFee = 2000000m,
            OldElectricIndex = 50m,
            NewElectricIndex = 100m,
            ElectricRate = 3500m,
            OldWaterIndex = 5m,
            NewWaterIndex = 10m,
            WaterRate = 15000m,
            OtherServiceFee = 30000m,
            Status = BillStatus.Unpaid,
            Note = null,
            DueDate = new DateTime(2024, 10, 30),
            PaidDate = null
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        // Thực hiện
        var pdfBytes = await _service.GenerateBillReceiptPdfAsync(2);

        // Kiểm tra
        pdfBytes.Should().NotBeNull();
        pdfBytes.Length.Should().BeGreaterThan(100);
        var header = Encoding.ASCII.GetString(pdfBytes.Take(5).ToArray());
        header.Should().Be("%PDF-");
    }

    [Fact]
    public async Task GenerateBillReceiptPdfAsync_WithVietQrEnabled_ShouldIncludeVietQrInReceipt()
    {
        // Sắp đặt: Dữ liệu phòng, sinh viên, hợp đồng và hóa đơn
        var room = new Room
        {
            Id = 10,
            RoomNumber = "P301",
            Building = "Tòa C",
            Floor = 3,
            Capacity = 4,
            CurrentOccupancy = 1,
            PricePerMonth = 1600000m,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);

        var student = new Student
        {
            Id = 10,
            StudentCode = "SV202410",
            FullName = "Trần Minh Đức",
            DateOfBirth = new DateTime(2003, 3, 15),
            Gender = Gender.Male,
            IdentityCard = "001203009999",
            PhoneNumber = "0912345678",
            ClassName = "CNTT-K18A",
            Faculty = "CNTT",
            HomeTown = "Hải Phòng"
        };
        _context.Students.Add(student);

        var contract = new Contract
        {
            Id = 10,
            ContractNumber = "HD-2024-010",
            RoomId = 10,
            StudentId = 10,
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 12, 31),
            DepositAmount = 1600000m,
            MonthlyRate = 1600000m,
            Status = ContractStatus.Active
        };
        _context.Contracts.Add(contract);

        var bill = new Bill
        {
            Id = 10,
            BillCode = "HD-202410-C301",
            RoomId = 10,
            Month = 10,
            Year = 2024,
            RoomFee = 1600000m,
            OldElectricIndex = 100m,
            NewElectricIndex = 160m,
            ElectricRate = 3500m,
            OldWaterIndex = 10m,
            NewWaterIndex = 15m,
            WaterRate = 15000m,
            OtherServiceFee = 50000m,
            Status = BillStatus.Unpaid,
            Note = "Tiền điện nước tháng 10",
            DueDate = new DateTime(2024, 10, 25),
            PaidDate = null
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var bankSettings = new BankSettingsDto
        {
            IsEnabled = true,
            BankBin = "970422",
            BankName = "Ngân hàng TMCP Quân Đội",
            BankShortName = "MBBank",
            AccountNumber = "0987654321",
            AccountHolder = "BQL KTX",
            TransferPrefix = "KTX"
        };

        var mockBankSettingsService = new Mock<IBankSettingsService>();
        mockBankSettingsService.Setup(s => s.GetBankSettingsAsync()).ReturnsAsync(bankSettings);

        var vietQrService = new VietQrService();
        var serviceWithQr = new PdfExportService(_context, mockBankSettingsService.Object, vietQrService);

        // Thực hiện
        var pdfBytes = await serviceWithQr.GenerateBillReceiptPdfAsync(10);

        // Kiểm tra
        pdfBytes.Should().NotBeNull();
        pdfBytes.Length.Should().BeGreaterThan(100);
        var header = Encoding.ASCII.GetString(pdfBytes.Take(5).ToArray());
        header.Should().Be("%PDF-");
    }

    [Fact]
    public async Task GenerateBillReceiptPdfAsync_WithVietQrDisabled_ShouldGenerateReceiptWithoutVietQr()
    {
        // Sắp đặt: Hóa đơn chưa thanh toán
        var room = new Room
        {
            Id = 20,
            RoomNumber = "P302",
            Building = "Tòa C",
            Floor = 3,
            Capacity = 4,
            CurrentOccupancy = 1,
            PricePerMonth = 1600000m,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male,
            Status = RoomStatus.Occupied
        };
        _context.Rooms.Add(room);

        var bill = new Bill
        {
            Id = 20,
            BillCode = "HD-202410-C302",
            RoomId = 20,
            Month = 10,
            Year = 2024,
            RoomFee = 1600000m,
            OldElectricIndex = 50m,
            NewElectricIndex = 100m,
            ElectricRate = 3500m,
            OldWaterIndex = 5m,
            NewWaterIndex = 10m,
            WaterRate = 15000m,
            OtherServiceFee = 50000m,
            Status = BillStatus.Unpaid,
            DueDate = new DateTime(2024, 10, 25)
        };
        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        var bankSettings = new BankSettingsDto
        {
            IsEnabled = false
        };

        var mockBankSettingsService = new Mock<IBankSettingsService>();
        mockBankSettingsService.Setup(s => s.GetBankSettingsAsync()).ReturnsAsync(bankSettings);

        var vietQrService = new VietQrService();
        var serviceDisabledQr = new PdfExportService(_context, mockBankSettingsService.Object, vietQrService);

        // Thực hiện
        var pdfBytes = await serviceDisabledQr.GenerateBillReceiptPdfAsync(20);

        // Kiểm tra
        pdfBytes.Should().NotBeNull();
        pdfBytes.Length.Should().BeGreaterThan(100);
        var header = Encoding.ASCII.GetString(pdfBytes.Take(5).ToArray());
        header.Should().Be("%PDF-");
    }
}
