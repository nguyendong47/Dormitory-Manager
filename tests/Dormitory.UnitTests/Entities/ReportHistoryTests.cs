using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.UnitTests.Entities;

/// <summary>
/// Kiểm thử thực thể ReportHistory và cơ chế lưu trữ trên Entity Framework Core
/// </summary>
public class ReportHistoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;

    public ReportHistoryTests()
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
    public void ReportHistory_DefaultValues_ShouldBeInitializedProperly()
    {
        // Act
        var reportHistory = new ReportHistory();

        // Assert
        reportHistory.Id.Should().Be(0);
        reportHistory.Title.Should().BeEmpty();
        reportHistory.Status.Should().Be(ReportStatus.Completed);
        reportHistory.FileName.Should().BeNull();
        reportHistory.FilePath.Should().BeNull();
        reportHistory.FileSizeBytes.Should().Be(0);
        reportHistory.ParametersJson.Should().BeNull();
        reportHistory.ErrorMessage.Should().BeNull();
        reportHistory.GeneratedBy.Should().BeNull();
        reportHistory.GeneratedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ReportHistory_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var report = new ReportHistory
        {
            Id = 42,
            ReportType = ReportType.Financial,
            Title = "Báo Cáo Tài Chính Tháng 10/2026",
            Format = ReportFormat.Excel,
            Status = ReportStatus.Generating,
            FileName = "BaoCao_TaiChinh_10_2026.xlsx",
            FilePath = "/storage/reports/financial/BaoCao_TaiChinh_10_2026.xlsx",
            FileSizeBytes = 1048576,
            ParametersJson = "{\"Month\": 10, \"Year\": 2026}",
            ErrorMessage = null,
            GeneratedBy = "admin_ketoan",
            GeneratedAt = now
        };

        // Assert
        report.Id.Should().Be(42);
        report.ReportType.Should().Be(ReportType.Financial);
        report.Title.Should().Be("Báo Cáo Tài Chính Tháng 10/2026");
        report.Format.Should().Be(ReportFormat.Excel);
        report.Status.Should().Be(ReportStatus.Generating);
        report.FileName.Should().Be("BaoCao_TaiChinh_10_2026.xlsx");
        report.FilePath.Should().Be("/storage/reports/financial/BaoCao_TaiChinh_10_2026.xlsx");
        report.FileSizeBytes.Should().Be(1048576);
        report.ParametersJson.Should().Be("{\"Month\": 10, \"Year\": 2026}");
        report.ErrorMessage.Should().BeNull();
        report.GeneratedBy.Should().Be("admin_ketoan");
        report.GeneratedAt.Should().Be(now);
    }

    [Fact]
    public async Task DbContext_AddAndSaveReportHistory_ShouldPersistSuccessfully()
    {
        // Arrange
        var report = new ReportHistory
        {
            ReportType = ReportType.Violations,
            Title = "Báo Cáo Vi Phạm Kỷ Luật Học Kỳ 1",
            Format = ReportFormat.Pdf,
            Status = ReportStatus.Completed,
            FileName = "BaoCao_ViPham_HK1.pdf",
            FilePath = "reports/violations/BaoCao_ViPham_HK1.pdf",
            FileSizeBytes = 256000,
            ParametersJson = "{\"Building\": \"Tòa A\"}",
            GeneratedBy = "quanly_ktx",
            GeneratedAt = DateTime.UtcNow
        };

        // Act
        _context.ReportHistories.Add(report);
        await _context.SaveChangesAsync();

        // Assert
        report.Id.Should().BeGreaterThan(0);

        var saved = await _context.ReportHistories.FindAsync(report.Id);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be("Báo Cáo Vi Phạm Kỷ Luật Học Kỳ 1");
        saved.ReportType.Should().Be(ReportType.Violations);
        saved.Format.Should().Be(ReportFormat.Pdf);
        saved.Status.Should().Be(ReportStatus.Completed);
        saved.FileSizeBytes.Should().Be(256000);
        saved.GeneratedBy.Should().Be("quanly_ktx");
    }

    [Fact]
    public async Task DbContext_QueryByReportTypeAndSort_ShouldReturnExpectedRecords()
    {
        // Arrange
        var r1 = new ReportHistory
        {
            ReportType = ReportType.Occupancy,
            Title = "Báo Cáo Lấp Đầy Tuần 1",
            Format = ReportFormat.Excel,
            GeneratedAt = DateTime.UtcNow.AddHours(-2)
        };
        var r2 = new ReportHistory
        {
            ReportType = ReportType.Occupancy,
            Title = "Báo Cáo Lấp Đầy Tuần 2",
            Format = ReportFormat.Pdf,
            GeneratedAt = DateTime.UtcNow.AddHours(-1)
        };
        var r3 = new ReportHistory
        {
            ReportType = ReportType.Equipment,
            Title = "Báo Cáo Thiết Bị Hỏng",
            Format = ReportFormat.Excel,
            GeneratedAt = DateTime.UtcNow
        };

        _context.ReportHistories.AddRange(r1, r2, r3);
        await _context.SaveChangesAsync();

        // Act
        var occupancyReports = await _context.ReportHistories
            .Where(r => r.ReportType == ReportType.Occupancy)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync();

        // Assert
        occupancyReports.Should().HaveCount(2);
        occupancyReports[0].Title.Should().Be("Báo Cáo Lấp Đầy Tuần 2");
        occupancyReports[1].Title.Should().Be("Báo Cáo Lấp Đầy Tuần 1");
    }

    [Fact]
    public async Task DbContext_UpdateAndRemoveReportHistory_ShouldWorkCorrectly()
    {
        // Arrange
        var report = new ReportHistory
        {
            ReportType = ReportType.Equipment,
            Title = "Kiểm Kê Tài Sản Tháng 10",
            Format = ReportFormat.Pdf,
            Status = ReportStatus.Generating
        };

        _context.ReportHistories.Add(report);
        await _context.SaveChangesAsync();

        // Act 1: Update
        report.Status = ReportStatus.Failed;
        report.ErrorMessage = "Hết dung lượng ổ đĩa lưu trữ";
        await _context.SaveChangesAsync();

        var updated = await _context.ReportHistories.FindAsync(report.Id);
        updated!.Status.Should().Be(ReportStatus.Failed);
        updated.ErrorMessage.Should().Be("Hết dung lượng ổ đĩa lưu trữ");

        // Act 2: Remove
        _context.ReportHistories.Remove(updated);
        await _context.SaveChangesAsync();

        var deleted = await _context.ReportHistories.FindAsync(report.Id);
        deleted.Should().BeNull();
    }
}
