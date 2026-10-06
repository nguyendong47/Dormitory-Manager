using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.E2ETests.Journeys;

/// <summary>
/// Kiểm thử luồng E2E cho quy trình quản lý báo cáo và phân tích: Tải danh sách, lọc, sinh báo cáo qua Dialog, tải tệp và xóa
/// </summary>
public class ReportManagementE2ETests : IDisposable
{
    private readonly TestFixture _fixture = new();

    /// <summary>
    /// Kịch bản 1: Tải danh sách lịch sử báo cáo, kiểm tra chỉ số KPI và thực hiện lọc dữ liệu trên UI thành công
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Load_And_Filter_Report_Histories_In_UI_Successfully()
    {
        // 1. Chuẩn bị dữ liệu lịch sử báo cáo mẫu trong SQLite InMemory
        var now = DateTime.UtcNow;
        var sampleHistories = new List<ReportHistory>
        {
            new()
            {
                ReportType = ReportType.Violations,
                Title = "Báo cáo vi phạm kỷ luật tháng 10/2026",
                Format = ReportFormat.Excel,
                Status = ReportStatus.Completed,
                FileName = "BaoCao_ViPham_202610.xlsx",
                FilePath = "/tmp/test_reports/BaoCao_ViPham_202610.xlsx",
                FileSizeBytes = 15200,
                GeneratedBy = "admin",
                GeneratedAt = now.AddHours(-5)
            },
            new()
            {
                ReportType = ReportType.Violations,
                Title = "Báo cáo vi phạm nghiêm trọng quý 3",
                Format = ReportFormat.Pdf,
                Status = ReportStatus.Completed,
                FileName = "BaoCao_ViPham_Q3.pdf",
                FilePath = "/tmp/test_reports/BaoCao_ViPham_Q3.pdf",
                FileSizeBytes = 45000,
                GeneratedBy = "manager",
                GeneratedAt = now.AddHours(-4)
            },
            new()
            {
                ReportType = ReportType.Financial,
                Title = "Báo cáo tài chính & thu phí tháng 10/2026",
                Format = ReportFormat.Pdf,
                Status = ReportStatus.Completed,
                FileName = "BaoCao_TaiChinh_202610.pdf",
                FilePath = "/tmp/test_reports/BaoCao_TaiChinh_202610.pdf",
                FileSizeBytes = 32000,
                GeneratedBy = "admin",
                GeneratedAt = now.AddHours(-3)
            },
            new()
            {
                ReportType = ReportType.Occupancy,
                Title = "Báo cáo tỷ lệ lấp đầy KTX tháng 10/2026",
                Format = ReportFormat.Excel,
                Status = ReportStatus.Completed,
                FileName = "BaoCao_LapDay_202610.xlsx",
                FilePath = "/tmp/test_reports/BaoCao_LapDay_202610.xlsx",
                FileSizeBytes = 18000,
                GeneratedBy = "admin",
                GeneratedAt = now.AddHours(-2)
            },
            new()
            {
                ReportType = ReportType.Equipment,
                Title = "Báo cáo kiểm kê tài sản thiết bị năm 2026",
                Format = ReportFormat.Pdf,
                Status = ReportStatus.Completed,
                FileName = "BaoCao_KiemKe_2026.pdf",
                FilePath = "/tmp/test_reports/BaoCao_KiemKe_2026.pdf",
                FileSizeBytes = 28000,
                GeneratedBy = "admin",
                GeneratedAt = now.AddHours(-1)
            }
        };

        _fixture.Context.ReportHistories.AddRange(sampleHistories);
        await _fixture.Context.SaveChangesAsync();

        // 2. Khởi tạo ReportListViewModel từ fixture
        var vm = _fixture.CreateReportListViewModel();

        // Nạp danh sách ban đầu
        await vm.LoadReportHistoriesCommand.ExecuteAsync(null);

        // 3. Kiểm tra danh sách hiển thị và các chỉ số KPI
        vm.ReportHistories.Should().HaveCount(5, "ViewModel phải nạp đầy đủ 5 bản ghi báo cáo");
        vm.TotalReportsCount.Should().Be(5, "Tổng số báo cáo KPI phải bằng 5");
        vm.PdfReportsCount.Should().Be(3, "Tổng số báo cáo PDF phải bằng 3");
        vm.ExcelReportsCount.Should().Be(2, "Tổng số báo cáo Excel phải bằng 2");
        vm.LatestReportTitle.Should().Be("Báo cáo kiểm kê tài sản thiết bị năm 2026",
            "Báo cáo mới nhất phải là báo cáo sinh gần đây nhất");

        // 4. Kiểm tra lọc theo loại báo cáo: "Vi phạm"
        vm.SelectedTypeFilter = "Vi phạm";
        await vm.LoadReportHistoriesCommand.ExecuteAsync(null);

        vm.ReportHistories.Should().HaveCount(2, "Bộ lọc loại 'Vi phạm' phải trả về đúng 2 bản ghi");
        vm.ReportHistories.Should().OnlyContain(r => r.ReportType == ReportType.Violations);

        // Kiểm tra lọc theo loại báo cáo: "Tài chính"
        vm.SelectedTypeFilter = "Tài chính";
        await vm.LoadReportHistoriesCommand.ExecuteAsync(null);

        vm.ReportHistories.Should().HaveCount(1, "Bộ lọc loại 'Tài chính' phải trả về đúng 1 bản ghi");
        vm.ReportHistories.First().ReportType.Should().Be(ReportType.Financial);

        // 5. Kiểm tra lọc theo định dạng tệp: "PDF" và "Excel"
        vm.SelectedTypeFilter = "Tất cả";
        vm.SelectedFormatFilter = "PDF";
        await vm.LoadReportHistoriesCommand.ExecuteAsync(null);

        vm.ReportHistories.Should().HaveCount(3, "Bộ lọc định dạng 'PDF' phải trả về 3 bản ghi");
        vm.ReportHistories.Should().OnlyContain(r => r.Format == ReportFormat.Pdf);

        vm.SelectedFormatFilter = "Excel";
        await vm.LoadReportHistoriesCommand.ExecuteAsync(null);

        vm.ReportHistories.Should().HaveCount(2, "Bộ lọc định dạng 'Excel' phải trả về 2 bản ghi");
        vm.ReportHistories.Should().OnlyContain(r => r.Format == ReportFormat.Excel);

        // 6. Kiểm tra tìm kiếm theo từ khóa SearchText
        vm.SelectedFormatFilter = "Tất cả";
        vm.SearchText = "nghiêm trọng";
        await vm.LoadReportHistoriesCommand.ExecuteAsync(null);

        vm.ReportHistories.Should().HaveCount(1, "Tìm kiếm 'nghiêm trọng' chỉ khớp 1 báo cáo");
        vm.ReportHistories.First().Title.Should().Contain("nghiêm trọng");

        // 7. Kiểm tra nút xóa bộ lọc (ClearFiltersCommand)
        await vm.ClearFiltersCommand.ExecuteAsync(null);

        vm.SearchText.Should().BeEmpty("Từ khóa tìm kiếm phải được reset về rỗng");
        vm.SelectedTypeFilter.Should().Be("Tất cả", "Bộ lọc loại phải được reset về 'Tất cả'");
        vm.SelectedFormatFilter.Should().Be("Tất cả", "Bộ lọc định dạng phải được reset về 'Tất cả'");
        vm.ReportHistories.Should().HaveCount(5, "Danh sách phải khôi phục đầy đủ 5 bản ghi sau khi xóa lọc");
    }

    /// <summary>
    /// Kịch bản 2: Khởi tạo báo cáo qua Dialog trên 4 phân hệ (Vi phạm, Tài chính, Lấp đầy, Thiết bị)
    /// với cả hai định dạng (Excel, PDF) và xác minh dữ liệu được lưu vết vào cơ sở dữ liệu và đĩa
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Generate_Reports_Across_All_Categories_From_Dialog_And_Persist()
    {
        // 1. Khởi tạo ReportListViewModel từ fixture
        var listVm = _fixture.CreateReportListViewModel();
        await listVm.LoadReportHistoriesCommand.ExecuteAsync(null);
        var initialCount = listVm.ReportHistories.Count;

        // Định nghĩa 4 trường hợp sinh báo cáo qua dialog:
        // Case 1: Vi phạm - Excel (.xlsx)
        // Case 2: Tài chính - PDF (.pdf)
        // Case 3: Lấp đầy - Excel (.xlsx)
        // Case 4: Thiết bị - PDF (.pdf)
        var testCases = new[]
        {
            (Type: ReportType.Violations, Format: ReportFormat.Excel, Title: "Báo cáo E2E Vi phạm kỷ luật ClosedXML"),
            (Type: ReportType.Financial, Format: ReportFormat.Pdf, Title: "Báo cáo E2E Tài chính thu phí QuestPDF"),
            (Type: ReportType.Occupancy, Format: ReportFormat.Excel, Title: "Báo cáo E2E Tỷ lệ lấp đầy ClosedXML"),
            (Type: ReportType.Equipment, Format: ReportFormat.Pdf, Title: "Báo cáo E2E Kiểm kê thiết bị QuestPDF")
        };

        foreach (var tc in testCases)
        {
            _fixture.DialogService.OnShowDialogAsync = async dialog =>
            {
                if (dialog.DataContext is ReportGenerateDialogViewModel dialogVm)
                {
                    dialogVm.SelectedReportType = tc.Type;
                    dialogVm.SelectedFormat = tc.Format;
                    dialogVm.Title = tc.Title;

                    await dialogVm.GenerateCommand.ExecuteAsync(null);
                    return true;
                }
                return false;
            };

            await listVm.OpenGenerateDialogCommand.ExecuteAsync(tc.Type);
        }

        // 2. Xác minh trên ReportListViewModel sau khi sinh xong 4 báo cáo
        listVm.ReportHistories.Count.Should().Be(initialCount + 4,
            "Danh sách hiển thị trên ViewModel phải tăng thêm 4 báo cáo vừa tạo");

        // 3. Xác minh trong cơ sở dữ liệu SQLite
        var savedReports = await _fixture.Context.ReportHistories
            .Where(r => r.Title.StartsWith("Báo cáo E2E"))
            .ToListAsync();

        savedReports.Should().HaveCount(4, "Cơ sở dữ liệu phải lưu đúng 4 bản ghi báo cáo E2E");

        foreach (var report in savedReports)
        {
            report.Status.Should().Be(ReportStatus.Completed, "Trạng thái báo cáo phải là Completed");
            report.FileSizeBytes.Should().BeGreaterThan(0, "Dung lượng tệp báo cáo phải > 0 bytes");
            report.FileName.Should().NotBeNullOrWhiteSpace("Tên tệp phải được khởi tạo");
            report.FilePath.Should().NotBeNullOrWhiteSpace("Đường dẫn tệp phải được thiết lập");

            File.Exists(report.FilePath).Should().BeTrue($"Tệp báo cáo '{report.FilePath}' phải thực sự tồn tại trên đĩa");

            var fileBytes = await File.ReadAllBytesAsync(report.FilePath!);
            fileBytes.LongLength.Should().Be(report.FileSizeBytes, "Kích thước tệp trên đĩa phải khớp với FileSizeBytes trong DB");

            if (report.Format == ReportFormat.Pdf)
            {
                report.FileName.Should().EndWith(".pdf");
                var pdfHeader = Encoding.ASCII.GetString(fileBytes.Take(5).ToArray());
                pdfHeader.Should().Be("%PDF-", "Tệp PDF xuất ra phải có chữ ký hợp lệ %PDF-");
            }
            else
            {
                report.FileName.Should().EndWith(".xlsx");
                fileBytes.Length.Should().BeGreaterThan(500, "Tệp Excel xuất ra phải có dung lượng tối thiểu");
            }
        }
    }

    /// <summary>
    /// Kịch bản 3: Tải tệp báo cáo về máy (Download) và xóa báo cáo khỏi hệ thống (Delete)
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Download_And_Delete_Report_Successfully()
    {
        // 1. Sinh một báo cáo mẫu thực tế để kiểm tra tải và xóa
        var reportService = _fixture.GetService<IReportService>();
        var generatedReport = await reportService.GenerateAndSaveReportAsync(new GenerateReportRequestDto
        {
            ReportType = ReportType.Financial,
            Format = ReportFormat.Pdf,
            Title = "Báo cáo tải và xóa E2E Test"
        });

        generatedReport.Should().NotBeNull();
        var reportId = generatedReport.Id;
        var filePath = generatedReport.FilePath;

        // 2. Khởi tạo ViewModel và nạp dữ liệu
        var vm = _fixture.CreateReportListViewModel();
        await vm.LoadReportHistoriesCommand.ExecuteAsync(null);

        var reportItem = vm.ReportHistories.FirstOrDefault(r => r.Id == reportId);
        reportItem.Should().NotBeNull("Bản ghi báo cáo vừa tạo phải có mặt trong ViewModel");

        // 3. Thực hiện tải tệp báo cáo qua DownloadReportCommand
        var initialSaveCount = _fixture.FileService.SaveCallCount;
        await vm.DownloadReportCommand.ExecuteAsync(reportItem);

        _fixture.FileService.SaveCallCount.Should().Be(initialSaveCount + 1,
            "Lệnh DownloadReportCommand phải gọi IFileService.SaveFileAsync đúng 1 lần");
        _fixture.FileService.LastSavedExtension.Should().Be("pdf", "Định dạng tải về phải là pdf");
        _fixture.FileService.LastSavedBytes.Should().NotBeNullOrEmpty("Dữ liệu tải về không được rỗng");
        _fixture.FileService.LastSavedBytes!.Length.Should().BeGreaterThan(0);

        var pdfHeader = Encoding.ASCII.GetString(_fixture.FileService.LastSavedBytes.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-", "Dữ liệu nhị phân tải về phải là tài liệu PDF hợp lệ");

        // 4. Thực hiện xóa báo cáo qua DeleteReportCommand với xác nhận người dùng
        _fixture.DialogService.ConfirmResult = true; // Người dùng nhấn Đồng ý xóa
        await vm.DeleteReportCommand.ExecuteAsync(reportItem);

        // Xác minh bản ghi đã biến mất khỏi giao diện ViewModel
        vm.ReportHistories.Should().NotContain(r => r.Id == reportId,
            "Bản ghi đã xóa không được còn xuất hiện trong ViewModel.ReportHistories");

        // Xác minh bản ghi đã bị xóa khỏi cơ sở dữ liệu SQLite
        var dbEntity = await _fixture.Context.ReportHistories.FindAsync(reportId);
        dbEntity.Should().BeNull("Bản ghi phải bị xóa khỏi bảng ReportHistories trong cơ sở dữ liệu");

        // Xác minh tệp vật lý trên đĩa đã được dọn dẹp
        if (!string.IsNullOrEmpty(filePath))
        {
            File.Exists(filePath).Should().BeFalse("Tệp vật lý trên đĩa phải được xóa khi xóa lịch sử báo cáo");
        }
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
