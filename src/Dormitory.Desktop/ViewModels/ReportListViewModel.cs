using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.Views;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel quản lý màn hình danh sách báo cáo, bộ lọc và các thao tác tải/mở/xóa báo cáo KTX
/// </summary>
public partial class ReportListViewModel : ViewModelBase
{
    private readonly IReportService _reportService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IFileService? _fileService;
    private readonly IUserSession? _userSession;

    [ObservableProperty]
    private ObservableCollection<ReportHistoryDto> _reportHistories = new();

    [ObservableProperty]
    private ReportHistoryDto? _selectedReport;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedTypeFilter = "Tất cả";

    [ObservableProperty]
    private string _selectedFormatFilter = "Tất cả";

    [ObservableProperty]
    private bool _isLoading;

    // KPI Counters
    [ObservableProperty]
    private int _totalReportsCount;

    [ObservableProperty]
    private int _pdfReportsCount;

    [ObservableProperty]
    private int _excelReportsCount;

    [ObservableProperty]
    private string _latestReportTitle = "Chưa có báo cáo nào";

    public IReadOnlyList<string> TypeFilterOptions { get; } = new[]
    {
        "Tất cả",
        "Vi phạm",
        "Tài chính",
        "Lấp đầy",
        "Thiết bị"
    };

    public IReadOnlyList<string> FormatFilterOptions { get; } = new[]
    {
        "Tất cả",
        "PDF",
        "Excel"
    };

    public ReportListViewModel(
        IReportService reportService,
        IRoomService roomService,
        IDialogService dialogService,
        IFileService? fileService = null,
        IUserSession? userSession = null)
    {
        _reportService = reportService;
        _roomService = roomService;
        _dialogService = dialogService;
        _fileService = fileService;
        _userSession = userSession;
    }

    partial void OnSelectedTypeFilterChanged(string value)
    {
        _ = LoadReportHistoriesAsync();
    }

    partial void OnSelectedFormatFilterChanged(string value)
    {
        _ = LoadReportHistoriesAsync();
    }

    /// <summary>
    /// Nạp danh sách lịch sử báo cáo và tính toán chỉ số KPI
    /// </summary>
    [RelayCommand]
    public async Task LoadReportHistoriesAsync()
    {
        IsLoading = true;
        try
        {
            var allReports = await _reportService.GetReportHistoriesAsync();

            TotalReportsCount = allReports.Count;
            PdfReportsCount = allReports.Count(r => r.Format == ReportFormat.Pdf);
            ExcelReportsCount = allReports.Count(r => r.Format == ReportFormat.Excel);

            var latest = allReports.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
            LatestReportTitle = latest != null ? latest.Title : "Chưa có báo cáo nào";

            // Phân tích bộ lọc Loại báo cáo
            ReportType? typeFilter = SelectedTypeFilter switch
            {
                "Vi phạm" => ReportType.Violations,
                "Tài chính" => ReportType.Financial,
                "Lấp đầy" => ReportType.Occupancy,
                "Thiết bị" => ReportType.Equipment,
                _ => null
            };

            // Phân tích bộ lọc Định dạng
            ReportFormat? formatFilter = SelectedFormatFilter switch
            {
                "PDF" => ReportFormat.Pdf,
                "Excel" => ReportFormat.Excel,
                _ => null
            };

            IEnumerable<ReportHistoryDto> query = allReports;

            if (typeFilter.HasValue)
            {
                query = query.Where(r => r.ReportType == typeFilter.Value);
            }

            if (formatFilter.HasValue)
            {
                query = query.Where(r => r.Format == formatFilter.Value);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim().ToLowerInvariant();
                query = query.Where(r =>
                    (!string.IsNullOrEmpty(r.Title) && r.Title.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(r.GeneratedBy) && r.GeneratedBy.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(r.FileName) && r.FileName.ToLowerInvariant().Contains(term)));
            }

            ReportHistories = new ObservableCollection<ReportHistoryDto>(
                query.OrderByDescending(r => r.GeneratedAt));
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi nạp danh sách báo cáo", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Mở hộp thoại sinh báo cáo tùy chỉnh
    /// </summary>
    [RelayCommand]
    public async Task OpenGenerateDialogAsync(ReportType? initialType = null)
    {
        var dialogVm = new ReportGenerateDialogViewModel(_reportService, _roomService);
        await dialogVm.InitializeAsync(initialType ?? ReportType.Violations);
        var dialog = new ReportGenerateDialogWindow(dialogVm);
        var result = await _dialogService.ShowDialogAsync<bool>(dialog);
        if (result)
        {
            await LoadReportHistoriesAsync();
        }
    }

    /// <summary>
    /// Kích hoạt nhanh hộp thoại sinh báo cáo từ Quick Action Card
    /// </summary>
    [RelayCommand]
    public async Task QuickGenerateAsync(string typeStr)
    {
        var type = typeStr?.ToLowerInvariant() switch
        {
            "violations" or "viphams" or "vỉ_phạm" or "vi phạm" => ReportType.Violations,
            "financial" or "taichinh" or "tài chính" => ReportType.Financial,
            "occupancy" or "lapday" or "lấp đầy" => ReportType.Occupancy,
            "equipment" or "thietbi" or "thiết bị" => ReportType.Equipment,
            _ => ReportType.Violations
        };

        await OpenGenerateDialogAsync(type);
    }

    /// <summary>
    /// Tải tệp báo cáo về máy người dùng
    /// </summary>
    [RelayCommand]
    public async Task DownloadReportAsync(ReportHistoryDto? report)
    {
        if (report == null) return;

        if (_fileService == null)
        {
            await _dialogService.ShowMessageAsync("Thông báo", "Dịch vụ tệp tin không khả dụng.");
            return;
        }

        try
        {
            var fileBytes = await _reportService.GetReportFileAsync(report.Id);
            var extension = report.Format == ReportFormat.Pdf ? "pdf" : "xlsx";
            var filterName = report.Format == ReportFormat.Pdf ? "Tài liệu PDF (*.pdf)" : "Bảng tính Excel (*.xlsx)";
            var defaultFileName = !string.IsNullOrWhiteSpace(report.FileName)
                ? Path.GetFileNameWithoutExtension(report.FileName)
                : report.Title;

            var success = await _fileService.SaveFileAsync(defaultFileName, extension, filterName, fileBytes);
            if (success)
            {
                await _dialogService.ShowMessageAsync("Thành công", $"Đã lưu báo cáo '{report.Title}' thành công.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi tải tệp", ex.Message);
        }
    }

    /// <summary>
    /// Mở trực tiếp tệp báo cáo trên hệ điều hành
    /// </summary>
    [RelayCommand]
    public async Task OpenReportAsync(ReportHistoryDto? report)
    {
        if (report == null) return;

        try
        {
            string targetPath;

            if (!string.IsNullOrEmpty(report.FilePath) && File.Exists(report.FilePath))
            {
                targetPath = report.FilePath;
            }
            else
            {
                var fileBytes = await _reportService.GetReportFileAsync(report.Id);
                var ext = report.Format == ReportFormat.Pdf ? ".pdf" : ".xlsx";
                var safeName = string.Join("_", (report.FileName ?? $"report_{report.Id}{ext}").Split(Path.GetInvalidFileNameChars()));
                targetPath = Path.Combine(Path.GetTempPath(), safeName);
                await File.WriteAllBytesAsync(targetPath, fileBytes);
            }

            var psi = new ProcessStartInfo
            {
                FileName = targetPath,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessageAsync("Lỗi mở tệp", $"Không thể mở tệp báo cáo: {ex.Message}");
        }
    }

    /// <summary>
    /// Xác nhận và xóa một bản ghi báo cáo
    /// </summary>
    [RelayCommand]
    public async Task DeleteReportAsync(ReportHistoryDto? report)
    {
        if (report == null) return;

        var confirmed = await _dialogService.ShowConfirmAsync(
            "Xác nhận xóa báo cáo",
            $"Bạn có chắc chắn muốn xóa bản ghi báo cáo '{report.Title}' không?");

        if (confirmed)
        {
            try
            {
                var deleted = await _reportService.DeleteReportHistoryAsync(report.Id, deletePhysicalFile: true);
                if (deleted)
                {
                    await LoadReportHistoriesAsync();
                }
                else
                {
                    await _dialogService.ShowMessageAsync("Thông báo", "Không tìm thấy bản ghi để xóa.");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync("Lỗi xóa báo cáo", ex.Message);
            }
        }
    }

    /// <summary>
    /// Xóa bộ lọc và tải lại danh sách đầy đủ
    /// </summary>
    [RelayCommand]
    public async Task ClearFiltersAsync()
    {
        SearchText = string.Empty;
        SelectedTypeFilter = "Tất cả";
        SelectedFormatFilter = "Tất cả";
        await LoadReportHistoriesAsync();
    }
}
