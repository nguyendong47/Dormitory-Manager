using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
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
/// Bộ kiểm thử đơn vị cho dịch vụ Báo cáo &amp; Phân tích (ReportService)
/// </summary>
public class ReportServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly ReportService _service;
    private readonly string _testTempDir;

    private readonly Room _roomA101;
    private readonly Room _roomA102;
    private readonly Room _roomB201;

    private readonly Student _student1;
    private readonly Student _student2;

    public ReportServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _testTempDir = Path.Combine(Path.GetTempPath(), "DormitoryReportTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testTempDir);

        _service = new ReportService(_context);

        // 1. Seed Rooms
        _roomA101 = new Room
        {
            RoomNumber = "A101",
            Building = "Tòa A",
            Floor = 1,
            Capacity = 4,
            CurrentOccupancy = 2,
            PricePerMonth = 1500000,
            Status = RoomStatus.Available,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male
        };

        _roomA102 = new Room
        {
            RoomNumber = "A102",
            Building = "Tòa A",
            Floor = 1,
            Capacity = 4,
            CurrentOccupancy = 4,
            PricePerMonth = 1500000,
            Status = RoomStatus.Occupied,
            Type = RoomType.Standard,
            AllowedGender = Gender.Male
        };

        _roomB201 = new Room
        {
            RoomNumber = "B201",
            Building = "Tòa B",
            Floor = 2,
            Capacity = 2,
            CurrentOccupancy = 1,
            PricePerMonth = 2500000,
            Status = RoomStatus.Available,
            Type = RoomType.Premium,
            AllowedGender = Gender.Female
        };

        _context.Rooms.AddRange(_roomA101, _roomA102, _roomB201);
        _context.SaveChanges();

        // 2. Seed Students
        _student1 = new Student
        {
            StudentCode = "SV001",
            FullName = "Nguyễn Văn An",
            Gender = Gender.Male,
            IdentityCard = "001201000001",
            Email = "an.nguyen@example.com",
            PhoneNumber = "0912345678",
            CurrentRoomId = _roomA101.Id
        };

        _student2 = new Student
        {
            StudentCode = "SV002",
            FullName = "Trần Thị Bình",
            Gender = Gender.Female,
            IdentityCard = "001201000002",
            Email = "binh.tran@example.com",
            PhoneNumber = "0987654321",
            CurrentRoomId = _roomB201.Id
        };

        _context.Students.AddRange(_student1, _student2);
        _context.SaveChanges();

        // 3. Seed Contracts
        var contract1 = new Contract
        {
            ContractNumber = "HD-2026-001",
            StudentId = _student1.Id,
            RoomId = _roomA101.Id,
            StartDate = DateTime.UtcNow.AddMonths(-3),
            EndDate = DateTime.UtcNow.AddMonths(9),
            MonthlyRate = 1500000,
            Status = ContractStatus.Active
        };

        var contract2 = new Contract
        {
            ContractNumber = "HD-2026-002",
            StudentId = _student2.Id,
            RoomId = _roomB201.Id,
            StartDate = DateTime.UtcNow.AddMonths(-2),
            EndDate = DateTime.UtcNow.AddMonths(10),
            MonthlyRate = 2500000,
            Status = ContractStatus.Active
        };

        _context.Contracts.AddRange(contract1, contract2);
        _context.SaveChanges();

        // 4. Seed Bills
        var bill1 = new Bill
        {
            BillCode = "BILL-A101-1026",
            RoomId = _roomA101.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 1500000,
            OldElectricIndex = 100,
            NewElectricIndex = 150,
            ElectricRate = 3500,
            OldWaterIndex = 20,
            NewWaterIndex = 30,
            WaterRate = 15000,
            OtherServiceFee = 50000,
            Status = BillStatus.Paid,
            DueDate = DateTime.UtcNow.AddDays(5),
            PaidDate = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var bill2 = new Bill
        {
            BillCode = "BILL-B201-1026",
            RoomId = _roomB201.Id,
            Month = 10,
            Year = 2026,
            RoomFee = 2500000,
            OldElectricIndex = 200,
            NewElectricIndex = 280,
            ElectricRate = 3500,
            OldWaterIndex = 40,
            NewWaterIndex = 55,
            WaterRate = 15000,
            OtherServiceFee = 70000,
            Status = BillStatus.Unpaid,
            DueDate = DateTime.UtcNow.AddDays(-2), // Quá hạn
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        };

        _context.Bills.AddRange(bill1, bill2);
        _context.SaveChanges();

        // 5. Seed Violations
        var violation1 = new Violation
        {
            ViolationCode = "VP001",
            StudentId = _student1.Id,
            RoomId = _roomA101.Id,
            Title = "Nấu ăn trong phòng sai quy định",
            Severity = ViolationSeverity.Minor,
            Status = ViolationStatus.Resolved,
            DemeritPoints = 5,
            FineAmount = 100000,
            ViolationDate = DateTime.UtcNow.AddDays(-5),
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            RecordedBy = "Quản lý tầng 1"
        };

        var violation2 = new Violation
        {
            ViolationCode = "VP002",
            StudentId = _student1.Id,
            RoomId = _roomA101.Id,
            Title = "Gây mất trật tự sau 23h",
            Severity = ViolationSeverity.Moderate,
            Status = ViolationStatus.Pending,
            DemeritPoints = 10,
            FineAmount = 200000,
            ViolationDate = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            RecordedBy = "Bảo vệ ca đêm"
        };

        _context.Violations.AddRange(violation1, violation2);
        _context.SaveChanges();

        // 6. Seed Equipments
        var equip1 = new Equipment
        {
            EquipmentCode = "TB001",
            Name = "Điều hòa Daikin 12000BTU",
            RoomId = _roomA101.Id,
            Quantity = 1,
            Price = 12000000,
            Status = EquipmentStatus.Good,
            CreatedAt = DateTime.UtcNow.AddMonths(-6),
            LastMaintainedAt = DateTime.UtcNow.AddMonths(-1)
        };

        var equip2 = new Equipment
        {
            EquipmentCode = "TB002",
            Name = "Bình nóng lạnh Ariston",
            RoomId = _roomA101.Id,
            Quantity = 1,
            Price = 3500000,
            Status = EquipmentStatus.NeedsRepair,
            CreatedAt = DateTime.UtcNow.AddMonths(-6),
            LastMaintainedAt = DateTime.UtcNow.AddMonths(-8) // Quá hạn 180 ngày
        };

        var equip3 = new Equipment
        {
            EquipmentCode = "TB003",
            Name = "Quạt trần Panasonic",
            RoomId = _roomB201.Id,
            Quantity = 2,
            Price = 1500000,
            Status = EquipmentStatus.Broken,
            CreatedAt = DateTime.UtcNow.AddMonths(-4)
        };

        _context.Equipments.AddRange(equip1, equip2, equip3);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();

        try
        {
            if (Directory.Exists(_testTempDir))
            {
                Directory.Delete(_testTempDir, true);
            }
        }
        catch
        {
            // Bỏ qua lỗi xóa thư mục tạm
        }
    }

    #region 1. Unit Tests Tổng Hợp Dữ Liệu (Data Aggregation)

    [Fact]
    public async Task GetViolationReportDataAsync_ShouldReturnCorrectSummaryAndKpis()
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = ReportType.Violations
        };
        var result = await _service.GetViolationReportDataAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalViolations.Should().Be(2);
        result.MinorCount.Should().Be(1);
        result.ModerateCount.Should().Be(1);
        result.SevereCount.Should().Be(0);
        result.CriticalCount.Should().Be(0);
        result.ResolvedCount.Should().Be(1);
        result.PendingCount.Should().Be(1);
        result.TotalDemeritPoints.Should().Be(15);
        result.TotalFineAmount.Should().Be(300000);
        result.ResolutionRate.Should().Be(50.0m);
        result.TopStudents.Should().NotBeEmpty();
        result.TopStudents.First().StudentCode.Should().Be("SV001");
        result.TopStudents.First().ViolationCount.Should().Be(2);
        result.TopRooms.Should().NotBeEmpty();
        result.TopRooms.First().RoomNumber.Should().Be("A101");
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetViolationReportDataAsync_WithDateFilter_ShouldFilterCorrectly()
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = ReportType.Violations,
            FromDate = DateTime.UtcNow.AddDays(-3),
            ToDate = DateTime.UtcNow
        };
        var result = await _service.GetViolationReportDataAsync(request);

        // Assert
        result.TotalViolations.Should().Be(1);
        result.Items.First().ViolationCode.Should().Be("VP002");
    }

    [Fact]
    public async Task GetFinancialReportDataAsync_ShouldReturnCorrectSummaryAndKpis()
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = ReportType.Financial,
            Month = 10,
            Year = 2026
        };
        var result = await _service.GetFinancialReportDataAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalBillsCount.Should().Be(2);
        result.PaidBillsCount.Should().Be(1);
        result.UnpaidBillsCount.Should().Be(1);
        result.OverdueBillsCount.Should().Be(1);

        // Bill 1: 1500000 + (50*3500=175000) + (10*15000=150000) + 50000 = 1,875,000 đ
        // Bill 2: 2500000 + (80*3500=280000) + (15*15000=225000) + 70000 = 3,075,000 đ
        result.TotalExpectedRevenue.Should().Be(1875000m + 3075000m);
        result.TotalCollectedRevenue.Should().Be(1875000m);
        result.TotalOutstandingDebt.Should().Be(3075000m);
        result.OverdueDebt.Should().Be(3075000m);

        result.RoomFeeCollected.Should().Be(1500000m);
        result.ElectricFeeCollected.Should().Be(175000m);
        result.WaterFeeCollected.Should().Be(150000m);
        result.OtherFeeCollected.Should().Be(50000m);

        result.TotalElectricUsageKwh.Should().Be(50 + 80);
        result.TotalWaterUsageM3.Should().Be(10 + 15);
        result.BuildingRevenues.Should().HaveCount(2);
        result.OverdueBills.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetOccupancyReportDataAsync_ShouldReturnCorrectSummaryAndKpis()
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = ReportType.Occupancy
        };
        var result = await _service.GetOccupancyReportDataAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalRooms.Should().Be(3);
        result.TotalBedsCapacity.Should().Be(4 + 4 + 2); // 10
        result.OccupiedBeds.Should().Be(2 + 4 + 1); // 7
        result.AvailableBeds.Should().Be(3);
        result.OccupancyRate.Should().Be(70.0m);

        result.OccupiedRoomsCount.Should().Be(1); // A102 is fully occupied
        result.AvailableRoomsCount.Should().Be(2); // A101, B201 have vacancies
        result.EmptyRoomsCount.Should().Be(0);

        result.MaleBedsCapacity.Should().Be(8);
        result.MaleBedsOccupied.Should().Be(6);
        result.FemaleBedsCapacity.Should().Be(2);
        result.FemaleBedsOccupied.Should().Be(1);

        result.Buildings.Should().HaveCount(2);
        result.VacantRooms.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetEquipmentReportDataAsync_ShouldReturnCorrectSummaryAndKpis()
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = ReportType.Equipment
        };
        var result = await _service.GetEquipmentReportDataAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalEquipmentCount.Should().Be(1 + 1 + 2); // 4
        result.GoodConditionCount.Should().Be(1);
        result.NeedsRepairCount.Should().Be(1);
        result.BrokenCount.Should().Be(2);

        // Giá trị tài sản: 1*12tr + 1*3.5tr + 2*1.5tr = 18.5tr
        result.TotalAssetValue.Should().Be(12000000m + 3500000m + 3000000m);
        result.DamagedAssetValue.Should().Be(3000000m); // 2 broken fans
        result.HealthRate.Should().Be(25.0m); // 1/4 = 25%
        result.FaultyRate.Should().Be(75.0m); // 3/4 = 75%
        result.OverdueMaintenanceCount.Should().Be(2); // equip2 (> 180 days) + equip3 (never maintained)

        result.Items.Should().HaveCount(3);
        result.FaultyEquipments.Should().HaveCount(2);
        result.StatusBreakdowns.Should().HaveCount(3);
        result.BuildingEquipments.Should().HaveCount(2);
    }

    #endregion

    #region 2. Unit Tests Xuất Excel (GenerateExcelReportAsync)

    [Theory]
    [InlineData(ReportType.Violations)]
    [InlineData(ReportType.Financial)]
    [InlineData(ReportType.Occupancy)]
    [InlineData(ReportType.Equipment)]
    public async Task GenerateExcelReportAsync_ForAllReportTypes_ShouldReturnValidBytes(ReportType reportType)
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = reportType,
            Format = ReportFormat.Excel,
            Month = 10,
            Year = 2026
        };
        var bytes = await _service.GenerateExcelReportAsync(request);

        // Assert
        bytes.Should().NotBeNull();
        bytes.Length.Should().BeGreaterThan(500, "tệp Excel sinh ra phải có kích thước tối thiểu lớn hơn 500 bytes");

        // Kiểm tra chữ ký file Zip/OpenXML: PK\x03\x04
        bytes[0].Should().Be(0x50);
        bytes[1].Should().Be(0x4B);
    }

    #endregion

    #region 3. Unit Tests Xuất PDF (GeneratePdfReportAsync)

    [Theory]
    [InlineData(ReportType.Violations)]
    [InlineData(ReportType.Financial)]
    [InlineData(ReportType.Occupancy)]
    [InlineData(ReportType.Equipment)]
    public async Task GeneratePdfReportAsync_ForAllReportTypes_ShouldReturnValidPdfHeader(ReportType reportType)
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = reportType,
            Format = ReportFormat.Pdf,
            Month = 10,
            Year = 2026
        };
        var bytes = await _service.GeneratePdfReportAsync(request);

        // Assert
        bytes.Should().NotBeNull();
        bytes.Length.Should().BeGreaterThan(500, "tệp PDF sinh ra phải có kích thước tối thiểu lớn hơn 500 bytes");

        // Kiểm tra chữ ký file PDF: %PDF-
        var header = Encoding.ASCII.GetString(bytes.Take(5).ToArray());
        header.Should().Be("%PDF-");
    }

    #endregion

    #region 4. Unit Tests Quản Lý Lịch Sử & Lưu Trữ Tệp (History & Storage)

    [Fact]
    public async Task GenerateAndSaveReportAsync_ShouldSaveFileToDiskAndCreateReportHistoryRecord()
    {
        // Act
        var request = new GenerateReportRequestDto
        {
            ReportType = ReportType.Financial,
            Format = ReportFormat.Excel,
            Title = "Báo cáo tài chính tháng 10/2026",
            Month = 10,
            Year = 2026,
            GeneratedBy = "Kế toán viên"
        };
        var historyDto = await _service.GenerateAndSaveReportAsync(request, _testTempDir);

        // Assert
        historyDto.Should().NotBeNull();
        historyDto.Id.Should().BeGreaterThan(0);
        historyDto.Title.Should().Be("Báo cáo tài chính tháng 10/2026");
        historyDto.ReportType.Should().Be(ReportType.Financial);
        historyDto.Format.Should().Be(ReportFormat.Excel);
        historyDto.Status.Should().Be(ReportStatus.Completed);
        historyDto.FileSizeBytes.Should().BeGreaterThan(500);
        historyDto.GeneratedBy.Should().Be("Kế toán viên");

        // Kiểm tra file trên đĩa
        File.Exists(historyDto.FilePath).Should().BeTrue();
        var diskBytes = await File.ReadAllBytesAsync(historyDto.FilePath!);
        diskBytes.Length.Should().Be((int)historyDto.FileSizeBytes);

        // Kiểm tra trong DB
        var dbEntity = await _context.ReportHistories.FindAsync(historyDto.Id);
        dbEntity.Should().NotBeNull();
        dbEntity!.FileName.Should().Be(historyDto.FileName);
    }

    [Fact]
    public async Task GetReportHistoriesAsync_ShouldFilterByTypeAndFormat()
    {
        // Arrange: Tạo 2 bản ghi lịch sử
        var req1 = new GenerateReportRequestDto
        {
            ReportType = ReportType.Violations,
            Format = ReportFormat.Pdf,
            GeneratedBy = "Admin"
        };
        var req2 = new GenerateReportRequestDto
        {
            ReportType = ReportType.Occupancy,
            Format = ReportFormat.Excel,
            GeneratedBy = "Admin"
        };
        await _service.GenerateAndSaveReportAsync(req1, _testTempDir);
        await _service.GenerateAndSaveReportAsync(req2, _testTempDir);

        // Act & Assert
        var all = await _service.GetReportHistoriesAsync();
        all.Should().HaveCount(2);

        var violationOnly = await _service.GetReportHistoriesAsync(type: ReportType.Violations);
        violationOnly.Should().HaveCount(1);
        violationOnly.First().ReportType.Should().Be(ReportType.Violations);

        var excelOnly = await _service.GetReportHistoriesAsync(format: ReportFormat.Excel);
        excelOnly.Should().HaveCount(1);
        excelOnly.First().Format.Should().Be(ReportFormat.Excel);
    }

    [Fact]
    public async Task GetReportHistoryByIdAsync_ShouldReturnCorrectRecord()
    {
        // Arrange
        var req = new GenerateReportRequestDto
        {
            ReportType = ReportType.Equipment,
            Format = ReportFormat.Pdf,
            Title = "Kiểm kê tháng 10"
        };
        var created = await _service.GenerateAndSaveReportAsync(req, _testTempDir);

        // Act
        var fetched = await _service.GetReportHistoryByIdAsync(created.Id);

        // Assert
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Title.Should().Be("Kiểm kê tháng 10");

        var notFound = await _service.GetReportHistoryByIdAsync(9999);
        notFound.Should().BeNull();
    }

    [Fact]
    public async Task GetReportFileAsync_ShouldReturnFileBytesWhenFound_AndThrowWhenNotFound()
    {
        // Arrange
        var req = new GenerateReportRequestDto
        {
            ReportType = ReportType.Occupancy,
            Format = ReportFormat.Excel
        };
        var created = await _service.GenerateAndSaveReportAsync(req, _testTempDir);

        // Act
        var fileBytes = await _service.GetReportFileAsync(created.Id);

        // Assert
        fileBytes.Should().NotBeNull();
        fileBytes.Length.Should().Be((int)created.FileSizeBytes);

        // Non-existent ID throws KeyNotFoundException
        Func<Task> actNonExistent = async () => await _service.GetReportFileAsync(9999);
        await actNonExistent.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteReportHistoryAsync_ShouldRemoveDbRecordAndPhysicalFile()
    {
        // Arrange
        var req = new GenerateReportRequestDto
        {
            ReportType = ReportType.Financial,
            Format = ReportFormat.Pdf
        };
        var created = await _service.GenerateAndSaveReportAsync(req, _testTempDir);
        File.Exists(created.FilePath).Should().BeTrue();

        // Act
        var deleteResult = await _service.DeleteReportHistoryAsync(created.Id, deletePhysicalFile: true);

        // Assert
        deleteResult.Should().BeTrue();

        // Kiểm tra DB đã bị xóa
        var dbRecord = await _context.ReportHistories.FindAsync(created.Id);
        dbRecord.Should().BeNull();

        // Kiểm tra file trên đĩa đã bị xóa
        File.Exists(created.FilePath).Should().BeFalse();

        // Xóa lại ID không tồn tại trả về false
        var deleteAgain = await _service.DeleteReportHistoryAsync(created.Id);
        deleteAgain.Should().BeFalse();
    }

    #endregion
}
