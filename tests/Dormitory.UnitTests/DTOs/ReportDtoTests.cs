using Dormitory.Application.DTOs;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using FluentAssertions;
using Xunit;

namespace Dormitory.UnitTests.DTOs;

/// <summary>
/// Kiểm thử các DTOs phân hệ Báo cáo & Phân tích, đặc biệt là các thuộc tính tính toán tự động
/// </summary>
public class ReportDtoTests
{
    // =========================================================================
    // 1. KIỂM THỬ ReportHistoryDto
    // =========================================================================

    [Theory]
    [InlineData(ReportType.Violations, "Vi phạm kỷ luật")]
    [InlineData(ReportType.Financial, "Tài chính & Thu phí")]
    [InlineData(ReportType.Occupancy, "Tỷ lệ lấp đầy phòng")]
    [InlineData(ReportType.Equipment, "Kiểm kê tài sản thiết bị")]
    [InlineData((ReportType)999, "Khác")]
    public void ReportHistoryDto_ReportTypeText_ShouldReturnExpectedDescription(ReportType type, string expectedText)
    {
        var dto = new ReportHistoryDto { ReportType = type };

        dto.ReportTypeText.Should().Be(expectedText);
        dto.ReportTypeName.Should().Be(expectedText);
    }

    [Theory]
    [InlineData(ReportFormat.Pdf, "PDF")]
    [InlineData(ReportFormat.Excel, "Excel")]
    public void ReportHistoryDto_FormatText_ShouldReturnExpectedDescription(ReportFormat format, string expectedText)
    {
        var dto = new ReportHistoryDto { Format = format };

        dto.FormatText.Should().Be(expectedText);
        dto.FormatName.Should().Be(expectedText);
    }

    [Theory]
    [InlineData(ReportStatus.Generating, "Đang tạo")]
    [InlineData(ReportStatus.Completed, "Hoàn thành")]
    [InlineData(ReportStatus.Failed, "Thất bại")]
    [InlineData((ReportStatus)999, "Không rõ")]
    public void ReportHistoryDto_StatusText_ShouldReturnExpectedDescription(ReportStatus status, string expectedText)
    {
        var dto = new ReportHistoryDto { Status = status };

        dto.StatusText.Should().Be(expectedText);
        dto.StatusName.Should().Be(expectedText);
    }

    [Theory]
    [InlineData(0, "0 B")]
    [InlineData(512, "512 B")]
    [InlineData(1023, "1023 B")]
    [InlineData(1024, "1.0 KB")]
    [InlineData(2048, "2.0 KB")]
    [InlineData(1536, "1.5 KB")]
    [InlineData(1048576, "1.00 MB")]
    [InlineData(2621440, "2.50 MB")]
    public void ReportHistoryDto_FormattedFileSize_ShouldFormatCorrectly(long bytes, string expectedFormatted)
    {
        var dto = new ReportHistoryDto { FileSizeBytes = bytes };

        dto.FormattedFileSize.Should().Be(expectedFormatted);
        dto.FileSizeFormatted.Should().Be(expectedFormatted);
    }

    [Fact]
    public void ReportHistoryDto_FromEntity_ShouldMapAllFieldsAccurately()
    {
        // Arrange
        var entity = new ReportHistory
        {
            Id = 15,
            ReportType = ReportType.Financial,
            Title = "Báo Cáo Thu Chi Tháng 9/2026",
            Format = ReportFormat.Excel,
            Status = ReportStatus.Completed,
            FileName = "BaoCao_TaiChinh_09_2026.xlsx",
            FilePath = "reports/Financial/BaoCao_TaiChinh_09_2026.xlsx",
            FileSizeBytes = 20480,
            ParametersJson = "{\"Month\":9}",
            ErrorMessage = null,
            GeneratedBy = "ketoan01",
            GeneratedAt = new DateTime(2026, 9, 30, 8, 30, 0, DateTimeKind.Utc)
        };

        // Act
        var dto = ReportHistoryDto.FromEntity(entity);

        // Assert
        dto.Id.Should().Be(15);
        dto.ReportType.Should().Be(ReportType.Financial);
        dto.Title.Should().Be("Báo Cáo Thu Chi Tháng 9/2026");
        dto.Format.Should().Be(ReportFormat.Excel);
        dto.Status.Should().Be(ReportStatus.Completed);
        dto.FileName.Should().Be("BaoCao_TaiChinh_09_2026.xlsx");
        dto.FilePath.Should().Be("reports/Financial/BaoCao_TaiChinh_09_2026.xlsx");
        dto.FileSizeBytes.Should().Be(20480);
        dto.ParametersJson.Should().Be("{\"Month\":9}");
        dto.ErrorMessage.Should().BeNull();
        dto.GeneratedBy.Should().Be("ketoan01");
        dto.GeneratedAt.Should().Be(new DateTime(2026, 9, 30, 8, 30, 0, DateTimeKind.Utc));
    }

    // =========================================================================
    // 2. KIỂM THỬ GenerateReportRequestDto
    // =========================================================================

    [Fact]
    public void GenerateReportRequestDto_TypeAlias_ShouldSyncWithReportType()
    {
        var request = new GenerateReportRequestDto();

        request.ReportType = ReportType.Occupancy;
        request.Type.Should().Be(ReportType.Occupancy);

        request.Type = ReportType.Equipment;
        request.ReportType.Should().Be(ReportType.Equipment);
    }

    [Fact]
    public void GenerateReportRequestDto_DefaultValues_ShouldBeExcel()
    {
        var request = new GenerateReportRequestDto();

        request.Format.Should().Be(ReportFormat.Excel);
        request.Title.Should().BeEmpty();
    }

    // =========================================================================
    // 3. KIỂM THỬ ViolationReportSummaryDto
    // =========================================================================

    [Fact]
    public void ViolationReportSummaryDto_ResolutionRate_WhenTotalIsZero_ShouldBeZero()
    {
        var dto = new ViolationReportSummaryDto
        {
            TotalViolations = 0,
            ResolvedCount = 0,
            DismissedCount = 0
        };

        dto.ResolutionRate.Should().Be(0);
    }

    [Fact]
    public void ViolationReportSummaryDto_ResolutionRate_WhenTotalPositive_ShouldCalculateCorrectPercentage()
    {
        var dto = new ViolationReportSummaryDto
        {
            TotalViolations = 10,
            ResolvedCount = 6,
            DismissedCount = 2 // Total resolved/dismissed = 8
        };

        dto.ResolutionRate.Should().Be(80.00m);
    }

    [Fact]
    public void ViolationReportSummaryDto_CriticalSeverityRate_ShouldCalculateCorrectly()
    {
        var dto = new ViolationReportSummaryDto
        {
            TotalViolations = 12,
            SevereCount = 2,
            CriticalCount = 1
        };

        // 3 / 12 * 100 = 25.00%
        dto.CriticalSeverityRate.Should().Be(25.00m);
    }

    // =========================================================================
    // 4. KIỂM THỬ FinancialReportSummaryDto
    // =========================================================================

    [Fact]
    public void FinancialReportSummaryDto_CollectionRate_WhenTotalExpectedZero_ShouldBeZero()
    {
        var dto = new FinancialReportSummaryDto
        {
            TotalExpectedRevenue = 0,
            TotalCollectedRevenue = 0
        };

        dto.CollectionRate.Should().Be(0);
    }

    [Fact]
    public void FinancialReportSummaryDto_CollectionRate_WhenExpectedPositive_ShouldCalculateAccurately()
    {
        var dto = new FinancialReportSummaryDto
        {
            TotalExpectedRevenue = 10000000,
            TotalCollectedRevenue = 8500000
        };

        dto.CollectionRate.Should().Be(85.00m);
    }

    [Fact]
    public void FinancialReportSummaryDto_TotalOutstandingDebt_ShouldNeverBeNegative()
    {
        var dtoNormal = new FinancialReportSummaryDto
        {
            TotalExpectedRevenue = 10000000,
            TotalCollectedRevenue = 7000000
        };
        dtoNormal.TotalOutstandingDebt.Should().Be(3000000);

        var dtoOverpaid = new FinancialReportSummaryDto
        {
            TotalExpectedRevenue = 10000000,
            TotalCollectedRevenue = 12000000
        };
        dtoOverpaid.TotalOutstandingDebt.Should().Be(0);
    }

    [Fact]
    public void FinancialReportSummaryDto_PaidBillRate_ShouldCalculateCorrectly()
    {
        var dtoZero = new FinancialReportSummaryDto { TotalBillsCount = 0, PaidBillsCount = 0 };
        dtoZero.PaidBillRate.Should().Be(0);

        var dto = new FinancialReportSummaryDto { TotalBillsCount = 50, PaidBillsCount = 45 };
        dto.PaidBillRate.Should().Be(90.00m);
    }

    [Fact]
    public void RevenueBreakdownDto_CalculationProperties_ShouldBeAccurate()
    {
        var breakdown = new RevenueBreakdownDto
        {
            BuildingName = "Tòa A",
            ExpectedRevenue = 50000000,
            CollectedRevenue = 40000000
        };

        breakdown.OutstandingDebt.Should().Be(10000000);
        breakdown.CollectionRate.Should().Be(80.00m);
    }

    // =========================================================================
    // 5. KIỂM THỬ OccupancyReportSummaryDto
    // =========================================================================

    [Fact]
    public void OccupancyReportSummaryDto_OccupancyRateAndAvailableBeds_ShouldCalculateProperly()
    {
        var dtoZero = new OccupancyReportSummaryDto { TotalBedsCapacity = 0, OccupiedBeds = 0 };
        dtoZero.OccupancyRate.Should().Be(0);
        dtoZero.AvailableBeds.Should().Be(0);

        var dto = new OccupancyReportSummaryDto
        {
            TotalRooms = 100,
            TotalBedsCapacity = 400,
            OccupiedBeds = 320,
            AvailableRoomsCount = 20
        };

        dto.AvailableBeds.Should().Be(80);
        dto.OccupancyRate.Should().Be(80.00m);
        dto.VacantRoomRate.Should().Be(20.00m);
    }

    [Fact]
    public void OccupancyBuildingSummaryDto_Calculations_ShouldBeAccurate()
    {
        var dto = new OccupancyBuildingSummaryDto
        {
            BuildingName = "Tòa B",
            TotalRooms = 50,
            TotalBedsCapacity = 200,
            OccupiedBeds = 150
        };

        dto.AvailableBeds.Should().Be(50);
        dto.OccupancyRate.Should().Be(75.00m);
    }

    [Fact]
    public void VacantRoomDto_VacantBeds_ShouldCalculateCapacityMinusOccupancy()
    {
        var room = new VacantRoomDto
        {
            RoomNumber = "A204",
            Capacity = 6,
            CurrentOccupancy = 2
        };

        room.VacantBeds.Should().Be(4);
    }

    // =========================================================================
    // 6. KIỂM THỬ EquipmentReportSummaryDto
    // =========================================================================

    [Fact]
    public void EquipmentReportSummaryDto_HealthRateAndFaultyRate_ShouldCalculateAccurately()
    {
        var dtoZero = new EquipmentReportSummaryDto { TotalEquipmentCount = 0 };
        dtoZero.HealthRate.Should().Be(0);
        dtoZero.FaultyRate.Should().Be(0);

        var dto = new EquipmentReportSummaryDto
        {
            TotalEquipmentCount = 100,
            GoodConditionCount = 85,
            NeedsRepairCount = 10,
            BrokenCount = 5
        };

        dto.HealthRate.Should().Be(85.00m);
        dto.FaultyRate.Should().Be(15.00m);
    }

    [Fact]
    public void EquipmentReportItemDto_TotalValue_ShouldBeQuantityTimesPrice()
    {
        var item = new EquipmentReportItemDto
        {
            Name = "Điều hòa Daikin Inverter 12000BTU",
            Quantity = 3,
            Price = 11500000
        };

        item.TotalValue.Should().Be(34500000);
    }
}
