using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

public class ReportGenerateDialogViewModelTests
{
    private readonly Mock<IReportService> _mockReportService;
    private readonly Mock<IRoomService> _mockRoomService;

    public ReportGenerateDialogViewModelTests()
    {
        _mockReportService = new Mock<IReportService>();
        _mockRoomService = new Mock<IRoomService>();
    }

    [Fact]
    public async Task InitializeAsync_WithDefaultType_ShouldSetProperDefaultsAndSuggestedTitle()
    {
        // Arrange
        var sampleRooms = new List<RoomDto>
        {
            new RoomDto { Id = 1, RoomNumber = "101", Building = "A" },
            new RoomDto { Id = 2, RoomNumber = "102", Building = "B" }
        };

        _mockRoomService
            .Setup(r => r.GetAllRoomsAsync(null, null))
            .ReturnsAsync(sampleRooms);

        var viewModel = new ReportGenerateDialogViewModel(
            _mockReportService.Object,
            _mockRoomService.Object);

        // Act
        await viewModel.InitializeAsync(ReportType.Violations);

        // Assert
        viewModel.SelectedReportType.Should().Be(ReportType.Violations);
        viewModel.IsViolationsReport.Should().BeTrue();
        viewModel.IsFinancialReport.Should().BeFalse();
        viewModel.IsOccupancyReport.Should().BeFalse();
        viewModel.IsEquipmentReport.Should().BeFalse();

        viewModel.Title.Should().Contain("Báo cáo vi phạm kỷ luật");
        viewModel.SelectedMonth.Should().Be(DateTime.Today.Month);
        viewModel.SelectedYear.Should().Be(DateTime.Today.Year);

        viewModel.Buildings.Should().HaveCount(3); // "-- Tất cả tòa nhà --", "A", "B"
        viewModel.Rooms.Should().NotBeEmpty();
    }

    [Fact]
    public void Presets_ShouldUpdateDateRangeAndTitle()
    {
        // Arrange
        var viewModel = new ReportGenerateDialogViewModel(_mockReportService.Object);

        // Act - Last Month
        viewModel.ApplyPresetLastMonth();

        // Assert
        var lastMonth = DateTime.Today.AddMonths(-1);
        viewModel.SelectedMonth.Should().Be(lastMonth.Month);
        viewModel.SelectedYear.Should().Be(lastMonth.Year);
        viewModel.FromDate.Should().NotBeNull();
        viewModel.ToDate.Should().NotBeNull();
        viewModel.FromDate!.Value.Day.Should().Be(1);

        // Act - This Year
        viewModel.ApplyPresetThisYear();

        // Assert
        viewModel.SelectedYear.Should().Be(DateTime.Today.Year);
        viewModel.FromDate!.Value.Month.Should().Be(1);
        viewModel.FromDate!.Value.Day.Should().Be(1);
    }

    [Fact]
    public void TypeChange_ShouldUpdateCategoryFlagsAndTitle()
    {
        // Arrange
        var viewModel = new ReportGenerateDialogViewModel(_mockReportService.Object);

        // Act - Financial
        viewModel.SelectedReportType = ReportType.Financial;

        // Assert
        viewModel.IsFinancialReport.Should().BeTrue();
        viewModel.IsViolationsReport.Should().BeFalse();
        viewModel.Title.Should().Contain("tài chính");

        // Act - Occupancy
        viewModel.SelectedReportType = ReportType.Occupancy;

        // Assert
        viewModel.IsOccupancyReport.Should().BeTrue();
        viewModel.Title.Should().Contain("lấp đầy");

        // Act - Equipment
        viewModel.SelectedReportType = ReportType.Equipment;

        // Assert
        viewModel.IsEquipmentReport.Should().BeTrue();
        viewModel.Title.Should().Contain("thiết bị");
    }

    [Fact]
    public async Task GenerateAsync_WithEmptyTitle_ShouldSetErrorMessage()
    {
        // Arrange
        var viewModel = new ReportGenerateDialogViewModel(_mockReportService.Object)
        {
            Title = "   "
        };

        // Act
        await viewModel.GenerateAsync();

        // Assert
        viewModel.ErrorMessage.Should().Contain("Tiêu đề báo cáo không được để trống");
        _mockReportService.Verify(s => s.GenerateAndSaveReportAsync(It.IsAny<GenerateReportRequestDto>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GenerateAsync_WithInvalidDateRange_ShouldSetErrorMessage()
    {
        // Arrange
        var viewModel = new ReportGenerateDialogViewModel(_mockReportService.Object)
        {
            Title = "Báo cáo hợp lệ",
            FromDate = DateTimeOffset.Now.AddDays(5),
            ToDate = DateTimeOffset.Now
        };

        // Act
        await viewModel.GenerateAsync();

        // Assert
        viewModel.ErrorMessage.Should().Contain("Từ ngày không được lớn hơn Đến ngày");
        _mockReportService.Verify(s => s.GenerateAndSaveReportAsync(It.IsAny<GenerateReportRequestDto>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GenerateAsync_WithValidInputs_ShouldBuildCorrectRequestAndClose()
    {
        // Arrange
        GenerateReportRequestDto? capturedRequest = null;
        _mockReportService
            .Setup(s => s.GenerateAndSaveReportAsync(It.IsAny<GenerateReportRequestDto>(), It.IsAny<string>()))
            .Callback<GenerateReportRequestDto, string?>((req, dir) => capturedRequest = req)
            .ReturnsAsync(new ReportHistoryDto { Id = 1, Title = "Đã lưu thành công" });

        var closedResult = false;
        var viewModel = new ReportGenerateDialogViewModel(_mockReportService.Object)
        {
            SelectedReportType = ReportType.Financial,
            SelectedFormat = ReportFormat.Excel,
            Title = "Báo cáo tài chính quý 4",
            SelectedMonth = 11,
            SelectedYear = 2026,
            OnlyOverdue = true,
            CloseAction = res => closedResult = res
        };

        // Act
        await viewModel.GenerateAsync();

        // Assert
        _mockReportService.Verify(s => s.GenerateAndSaveReportAsync(It.IsAny<GenerateReportRequestDto>(), null), Times.Once);
        capturedRequest.Should().NotBeNull();
        capturedRequest!.ReportType.Should().Be(ReportType.Financial);
        capturedRequest.Format.Should().Be(ReportFormat.Excel);
        capturedRequest.Title.Should().Be("Báo cáo tài chính quý 4");
        capturedRequest.Month.Should().Be(11);
        capturedRequest.Year.Should().Be(2026);
        capturedRequest.OnlyOverdue.Should().BeTrue();
        closedResult.Should().BeTrue();
    }
}
