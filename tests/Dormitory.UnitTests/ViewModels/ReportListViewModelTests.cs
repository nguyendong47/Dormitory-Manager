using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

public class ReportListViewModelTests
{
    private readonly Mock<IReportService> _mockReportService;
    private readonly Mock<IRoomService> _mockRoomService;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly Mock<IFileService> _mockFileService;

    public ReportListViewModelTests()
    {
        _mockReportService = new Mock<IReportService>();
        _mockRoomService = new Mock<IRoomService>();
        _mockDialogService = new Mock<IDialogService>();
        _mockFileService = new Mock<IFileService>();
    }

    private List<ReportHistoryDto> CreateSampleHistories()
    {
        return new List<ReportHistoryDto>
        {
            new ReportHistoryDto
            {
                Id = 1,
                Title = "Báo cáo vi phạm tháng 10/2026",
                ReportType = ReportType.Violations,
                Format = ReportFormat.Pdf,
                FileName = "violations_202610.pdf",
                FileSizeBytes = 102400,
                GeneratedBy = "Admin",
                GeneratedAt = new DateTime(2026, 10, 1, 8, 30, 0)
            },
            new ReportHistoryDto
            {
                Id = 2,
                Title = "Báo cáo tài chính quý 3/2026",
                ReportType = ReportType.Financial,
                Format = ReportFormat.Excel,
                FileName = "financial_q3.xlsx",
                FileSizeBytes = 204800,
                GeneratedBy = "Kế toán trưởng",
                GeneratedAt = new DateTime(2026, 10, 2, 9, 15, 0)
            },
            new ReportHistoryDto
            {
                Id = 3,
                Title = "Báo cáo lấp đầy phòng ký túc xá",
                ReportType = ReportType.Occupancy,
                Format = ReportFormat.Pdf,
                FileName = "occupancy_202610.pdf",
                FileSizeBytes = 51200,
                GeneratedBy = "Quản lý tòa",
                GeneratedAt = new DateTime(2026, 10, 3, 14, 0, 0)
            },
            new ReportHistoryDto
            {
                Id = 4,
                Title = "Báo cáo kiểm kê thiết bị năm 2026",
                ReportType = ReportType.Equipment,
                Format = ReportFormat.Excel,
                FileName = "equipment_inventory.xlsx",
                FileSizeBytes = 307200,
                GeneratedBy = "Kỹ thuật viên",
                GeneratedAt = new DateTime(2026, 10, 4, 16, 45, 0)
            }
        };
    }

    [Fact]
    public async Task LoadReportHistoriesAsync_ShouldPopulateListAndCalculateKPIs()
    {
        // Arrange
        var sampleData = CreateSampleHistories();
        _mockReportService
            .Setup(s => s.GetReportHistoriesAsync(null, null, null, null))
            .ReturnsAsync(sampleData);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object);

        // Act
        await viewModel.LoadReportHistoriesAsync();

        // Assert
        viewModel.ReportHistories.Should().HaveCount(4);
        viewModel.TotalReportsCount.Should().Be(4);
        viewModel.PdfReportsCount.Should().Be(2);
        viewModel.ExcelReportsCount.Should().Be(2);
        viewModel.LatestReportTitle.Should().Be("Báo cáo kiểm kê thiết bị năm 2026");
        viewModel.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task FilterByType_ShouldFilterCorrectRecords()
    {
        // Arrange
        var sampleData = CreateSampleHistories();
        _mockReportService
            .Setup(s => s.GetReportHistoriesAsync(null, null, null, null))
            .ReturnsAsync(sampleData);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object);

        // Act
        viewModel.SelectedTypeFilter = "Vi phạm";
        await viewModel.LoadReportHistoriesAsync();

        // Assert
        viewModel.ReportHistories.Should().HaveCount(1);
        viewModel.ReportHistories.First().ReportType.Should().Be(ReportType.Violations);
        viewModel.TotalReportsCount.Should().Be(4); // KPI phản ánh tổng số báo cáo
    }

    [Fact]
    public async Task FilterByFormat_ShouldFilterCorrectRecords()
    {
        // Arrange
        var sampleData = CreateSampleHistories();
        _mockReportService
            .Setup(s => s.GetReportHistoriesAsync(null, null, null, null))
            .ReturnsAsync(sampleData);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object);

        // Act
        viewModel.SelectedFormatFilter = "Excel";
        await viewModel.LoadReportHistoriesAsync();

        // Assert
        viewModel.ReportHistories.Should().HaveCount(2);
        viewModel.ReportHistories.All(r => r.Format == ReportFormat.Excel).Should().BeTrue();
    }

    [Fact]
    public async Task FilterBySearchText_ShouldMatchTitleOrAuthor()
    {
        // Arrange
        var sampleData = CreateSampleHistories();
        _mockReportService
            .Setup(s => s.GetReportHistoriesAsync(null, null, null, null))
            .ReturnsAsync(sampleData);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object);

        // Act
        viewModel.SearchText = "kế toán";
        await viewModel.LoadReportHistoriesAsync();

        // Assert
        viewModel.ReportHistories.Should().HaveCount(1);
        viewModel.ReportHistories.First().GeneratedBy.Should().Be("Kế toán trưởng");
    }

    [Fact]
    public async Task ClearFiltersAsync_ShouldResetFiltersAndReloadAll()
    {
        // Arrange
        var sampleData = CreateSampleHistories();
        _mockReportService
            .Setup(s => s.GetReportHistoriesAsync(null, null, null, null))
            .ReturnsAsync(sampleData);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object)
        {
            SearchText = "tìm kiếm mẫu",
            SelectedTypeFilter = "Tài chính",
            SelectedFormatFilter = "Excel"
        };

        // Act
        await viewModel.ClearFiltersAsync();

        // Assert
        viewModel.SearchText.Should().BeEmpty();
        viewModel.SelectedTypeFilter.Should().Be("Tất cả");
        viewModel.SelectedFormatFilter.Should().Be("Tất cả");
        viewModel.ReportHistories.Should().HaveCount(4);
    }

    [Fact]
    public async Task DeleteReportAsync_WhenConfirmed_ShouldCallServiceAndReload()
    {
        // Arrange
        var sampleData = CreateSampleHistories();
        var targetReport = sampleData.First();

        _mockReportService
            .Setup(s => s.GetReportHistoriesAsync(null, null, null, null))
            .ReturnsAsync(sampleData);

        _mockDialogService
            .Setup(d => d.ShowConfirmAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockReportService
            .Setup(s => s.DeleteReportHistoryAsync(targetReport.Id, true))
            .ReturnsAsync(true);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object);

        await viewModel.LoadReportHistoriesAsync();

        // Act
        await viewModel.DeleteReportAsync(targetReport);

        // Assert
        _mockReportService.Verify(s => s.DeleteReportHistoryAsync(targetReport.Id, true), Times.Once);
    }

    [Fact]
    public async Task DeleteReportAsync_WhenCancelled_ShouldNotCallService()
    {
        // Arrange
        var sampleData = CreateSampleHistories();
        var targetReport = sampleData.First();

        _mockDialogService
            .Setup(d => d.ShowConfirmAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object);

        // Act
        await viewModel.DeleteReportAsync(targetReport);

        // Assert
        _mockReportService.Verify(s => s.DeleteReportHistoryAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task DownloadReportAsync_ShouldCallGetFileAndSaveFile()
    {
        // Arrange
        var targetReport = new ReportHistoryDto
        {
            Id = 10,
            Title = "Báo cáo thử nghiệm",
            Format = ReportFormat.Pdf,
            FileName = "test.pdf"
        };
        var fileBytes = new byte[] { 1, 2, 3, 4, 5 };

        _mockReportService
            .Setup(s => s.GetReportFileAsync(10))
            .ReturnsAsync(fileBytes);

        _mockFileService
            .Setup(f => f.SaveFileAsync(It.IsAny<string>(), "pdf", It.IsAny<string>(), fileBytes))
            .ReturnsAsync(true);

        var viewModel = new ReportListViewModel(
            _mockReportService.Object,
            _mockRoomService.Object,
            _mockDialogService.Object,
            _mockFileService.Object);

        // Act
        await viewModel.DownloadReportAsync(targetReport);

        // Assert
        _mockReportService.Verify(s => s.GetReportFileAsync(10), Times.Once);
        _mockFileService.Verify(f => f.SaveFileAsync("test", "pdf", It.IsAny<string>(), fileBytes), Times.Once);
        _mockDialogService.Verify(d => d.ShowMessageAsync("Thành công", It.IsAny<string>()), Times.Once);
    }
}
