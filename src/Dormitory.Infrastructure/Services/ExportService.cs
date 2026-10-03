using ClosedXML.Excel;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ xuất dữ liệu báo cáo ra file Excel (.xlsx) sử dụng ClosedXML
/// </summary>
public class ExportService : IExportService
{
    private static readonly XLColor HeaderBackgroundColor = XLColor.FromArgb(0x00, 0x78, 0xD4); // #0078D4
    private static readonly XLColor DashboardHeaderColor = XLColor.FromArgb(0x10, 0x7C, 0x41); // #107C41
    private const string CurrencyFormat = "#,##0";
    private const string CurrencyWithSuffixFormat = "#,##0 đ";
    private const string DateFormat = "dd/MM/yyyy";

    /// <summary>
    /// Xuất danh sách phòng ở ra file Excel
    /// </summary>
    public Task<byte[]> ExportRoomsToExcelAsync(List<RoomDto> rooms)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Danh sách phòng");

        // Dòng tiêu đề
        var headers = new[]
        {
            "STT",
            "Số phòng",
            "Tòa nhà",
            "Tầng",
            "Loại phòng",
            "Giới tính",
            "Sức chứa",
            "Đang ở",
            "Đơn giá (VNĐ)",
            "Trạng thái",
            "Ghi chú"
        };

        for (var col = 0; col < headers.Length; col++)
        {
            worksheet.Cell(1, col + 1).Value = headers[col];
        }

        worksheet.Row(1).Height = 26;
        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        ApplyHeaderStyle(headerRange);

        // Ghi dữ liệu từng dòng
        var row = 2;
        for (var i = 0; i < rooms.Count; i++)
        {
            var room = rooms[i];
            worksheet.Row(row).Height = 20;

            worksheet.Cell(row, 1).SetValue(i + 1);
            worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 2).SetValue(room.RoomNumber);
            worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 3).SetValue(room.Building);
            worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 4).SetValue(room.Floor);
            worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 5).SetValue(GetRoomTypeName(room.Type));
            worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 6).SetValue(GetGenderName(room.AllowedGender));
            worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 7).SetValue(room.Capacity);
            worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 8).SetValue(room.CurrentOccupancy);
            worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 9).SetValue(room.PricePerMonth);
            worksheet.Cell(row, 9).Style.NumberFormat.Format = CurrencyFormat;
            worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 10).SetValue(GetRoomStatusName(room.Status));
            worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 11).SetValue(room.Description ?? string.Empty);

            row++;
        }

        // Định dạng viền dữ liệu nếu có
        if (rooms.Count > 0)
        {
            var dataRange = worksheet.Range(2, 1, 1 + rooms.Count, headers.Length);
            ApplyDataStyle(dataRange);
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    /// <summary>
    /// Xuất danh sách sinh viên nội trú ra file Excel
    /// </summary>
    public Task<byte[]> ExportStudentsToExcelAsync(List<StudentDto> students)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Danh sách sinh viên");

        // Dòng tiêu đề
        var headers = new[]
        {
            "STT",
            "Mã SV",
            "Họ và tên",
            "Ngày sinh",
            "Giới tính",
            "CCCD/CMND",
            "Số điện thoại",
            "Email",
            "Quê quán",
            "Lớp",
            "Khoa",
            "Phòng hiện tại",
            "Họ tên phụ huynh",
            "SĐT phụ huynh",
            "Địa chỉ thường trú"
        };

        for (var col = 0; col < headers.Length; col++)
        {
            worksheet.Cell(1, col + 1).Value = headers[col];
        }

        worksheet.Row(1).Height = 26;
        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        ApplyHeaderStyle(headerRange);

        // Ghi dữ liệu từng dòng
        var row = 2;
        for (var i = 0; i < students.Count; i++)
        {
            var s = students[i];
            worksheet.Row(row).Height = 20;

            worksheet.Cell(row, 1).SetValue(i + 1);
            worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 2).SetValue(s.StudentCode);
            worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 3).SetValue(s.FullName);

            worksheet.Cell(row, 4).SetValue(s.DateOfBirth.ToString(DateFormat));
            worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 5).SetValue(GetGenderName(s.Gender));
            worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 6).SetValue($"'{s.IdentityCard}");
            worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 7).SetValue($"'{s.PhoneNumber}");
            worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 8).SetValue(s.Email ?? string.Empty);

            worksheet.Cell(row, 9).SetValue(s.HomeTown);

            worksheet.Cell(row, 10).SetValue(s.ClassName);
            worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 11).SetValue(s.Faculty);

            worksheet.Cell(row, 12).SetValue(s.CurrentRoomNumber ?? "Chưa xếp");
            worksheet.Cell(row, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 13).SetValue(s.ParentName);

            worksheet.Cell(row, 14).SetValue($"'{s.ParentPhoneNumber}");
            worksheet.Cell(row, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 15).SetValue(s.Address ?? string.Empty);

            row++;
        }

        // Định dạng viền dữ liệu nếu có
        if (students.Count > 0)
        {
            var dataRange = worksheet.Range(2, 1, 1 + students.Count, headers.Length);
            ApplyDataStyle(dataRange);
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    /// <summary>
    /// Xuất danh sách hóa đơn điện nước ra file Excel
    /// </summary>
    public Task<byte[]> ExportBillsToExcelAsync(List<BillDto> bills)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Danh sách hóa đơn");

        // Dòng tiêu đề
        var headers = new[]
        {
            "STT",
            "Mã hóa đơn",
            "Số phòng",
            "Tòa nhà",
            "Tháng/Năm",
            "Tiền phòng (VNĐ)",
            "Chỉ số điện cũ",
            "Chỉ số điện mới",
            "Tiêu thụ điện (kWh)",
            "Tiền điện (VNĐ)",
            "Chỉ số nước cũ",
            "Chỉ số nước mới",
            "Tiêu thụ nước (m³)",
            "Tiền nước (VNĐ)",
            "Phí dịch vụ khác (VNĐ)",
            "Tổng tiền (VNĐ)",
            "Hạn thanh toán",
            "Ngày thanh toán",
            "Trạng thái",
            "Ghi chú"
        };

        for (var col = 0; col < headers.Length; col++)
        {
            worksheet.Cell(1, col + 1).Value = headers[col];
        }

        worksheet.Row(1).Height = 26;
        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        ApplyHeaderStyle(headerRange);

        // Ghi dữ liệu từng dòng
        var row = 2;
        for (var i = 0; i < bills.Count; i++)
        {
            var b = bills[i];
            worksheet.Row(row).Height = 20;

            worksheet.Cell(row, 1).SetValue(i + 1);
            worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 2).SetValue(b.BillCode);
            worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 3).SetValue(b.RoomNumber);
            worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 4).SetValue(b.Building);
            worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 5).SetValue($"{b.Month:D2}/{b.Year}");
            worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 6).SetValue(b.RoomFee);
            worksheet.Cell(row, 6).Style.NumberFormat.Format = CurrencyFormat;
            worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 7).SetValue(b.OldElectricIndex);
            worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 8).SetValue(b.NewElectricIndex);
            worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 9).SetValue(b.ElectricUsage);
            worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 10).SetValue(b.ElectricFee);
            worksheet.Cell(row, 10).Style.NumberFormat.Format = CurrencyFormat;
            worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 11).SetValue(b.OldWaterIndex);
            worksheet.Cell(row, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 12).SetValue(b.NewWaterIndex);
            worksheet.Cell(row, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 13).SetValue(b.WaterUsage);
            worksheet.Cell(row, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 14).SetValue(b.WaterFee);
            worksheet.Cell(row, 14).Style.NumberFormat.Format = CurrencyFormat;
            worksheet.Cell(row, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 15).SetValue(b.OtherServiceFee);
            worksheet.Cell(row, 15).Style.NumberFormat.Format = CurrencyFormat;
            worksheet.Cell(row, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 16).SetValue(b.TotalAmount);
            worksheet.Cell(row, 16).Style.NumberFormat.Format = CurrencyFormat;
            worksheet.Cell(row, 16).Style.Font.Bold = true;
            worksheet.Cell(row, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(row, 17).SetValue(b.DueDate.ToString(DateFormat));
            worksheet.Cell(row, 17).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 18).SetValue(b.PaidDate.HasValue ? b.PaidDate.Value.ToString(DateFormat) : "-");
            worksheet.Cell(row, 18).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 19).SetValue(GetBillStatusName(b.Status));
            worksheet.Cell(row, 19).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(row, 20).SetValue(b.Note ?? string.Empty);

            row++;
        }

        // Định dạng viền dữ liệu nếu có
        if (bills.Count > 0)
        {
            var dataRange = worksheet.Range(2, 1, 1 + bills.Count, headers.Length);
            ApplyDataStyle(dataRange);
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    /// <summary>
    /// Xuất báo cáo tổng hợp Dashboard (chỉ số KPI, tỷ lệ lấp đầy theo tòa nhà và xu hướng doanh thu) ra file Excel
    /// </summary>
    public Task<byte[]> ExportDashboardSummaryToExcelAsync(
        DashboardStatsDto stats,
        List<BuildingOccupancyDto> buildings,
        List<MonthlyRevenueTrendDto> trends)
    {
        using var workbook = new XLWorkbook();

        // 1. Worksheet 1: "Tổng quan KPI & Tòa nhà"
        var wsKpi = workbook.Worksheets.Add("Tổng quan KPI & Tòa nhà");

        // Khối KPI tổng quan (A1:B6)
        wsKpi.Cell(1, 1).SetValue("Tổng số phòng");
        wsKpi.Cell(1, 2).SetValue(stats.TotalRooms);

        wsKpi.Cell(2, 1).SetValue("Phòng còn trống");
        wsKpi.Cell(2, 2).SetValue(stats.AvailableRooms);

        wsKpi.Cell(3, 1).SetValue("Sinh viên nội trú");
        wsKpi.Cell(3, 2).SetValue(stats.TotalStudents);

        wsKpi.Cell(4, 1).SetValue("Hợp đồng hiệu lực");
        wsKpi.Cell(4, 2).SetValue(stats.ActiveContractsCount);

        wsKpi.Cell(5, 1).SetValue("Hóa đơn chưa thu");
        wsKpi.Cell(5, 2).SetValue(stats.UnpaidBillsCount);

        wsKpi.Cell(6, 1).SetValue("Tỷ lệ lấp đầy");
        wsKpi.Cell(6, 2).SetValue($"{stats.OccupancyRate:F1}%");

        for (var r = 1; r <= 6; r++)
        {
            wsKpi.Row(r).Height = 22;
            wsKpi.Cell(r, 1).Style.Font.Bold = true;
            wsKpi.Cell(r, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0xF2, 0xF4, 0xF7);
            wsKpi.Cell(r, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            wsKpi.Cell(r, 2).Style.Font.Bold = true;
            wsKpi.Cell(r, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            wsKpi.Cell(r, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        var kpiRange = wsKpi.Range(1, 1, 6, 2);
        kpiRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        kpiRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Khối Bảng số liệu từng tòa nhà (từ hàng 8)
        wsKpi.Cell(8, 1).SetValue("BÁO CÁO TỶ LỆ LẤP ĐẦY THEO TÒA NHÀ");
        wsKpi.Cell(8, 1).Style.Font.Bold = true;
        wsKpi.Cell(8, 1).Style.Font.FontSize = 13;
        wsKpi.Cell(8, 1).Style.Font.FontColor = DashboardHeaderColor;

        var buildingHeaders = new[]
        {
            "Tên tòa",
            "Tổng số phòng",
            "Phòng đã ở",
            "Phòng còn trống",
            "Tổng số chỗ",
            "Chỗ đã ở",
            "Tỷ lệ lấp đầy (%)"
        };

        wsKpi.Row(9).Height = 26;
        for (var col = 0; col < buildingHeaders.Length; col++)
        {
            wsKpi.Cell(9, col + 1).SetValue(buildingHeaders[col]);
        }
        var bHeaderRange = wsKpi.Range(9, 1, 9, buildingHeaders.Length);
        ApplyHeaderStyle(bHeaderRange, DashboardHeaderColor);

        var bRow = 10;
        for (var i = 0; i < buildings.Count; i++)
        {
            var b = buildings[i];
            wsKpi.Row(bRow).Height = 20;

            wsKpi.Cell(bRow, 1).SetValue(b.BuildingName);
            wsKpi.Cell(bRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wsKpi.Cell(bRow, 2).SetValue(b.TotalRooms);
            wsKpi.Cell(bRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            wsKpi.Cell(bRow, 3).SetValue(b.OccupiedRooms);
            wsKpi.Cell(bRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            wsKpi.Cell(bRow, 4).SetValue(b.AvailableRooms);
            wsKpi.Cell(bRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            wsKpi.Cell(bRow, 5).SetValue(b.TotalBeds);
            wsKpi.Cell(bRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            wsKpi.Cell(bRow, 6).SetValue(b.OccupiedBeds);
            wsKpi.Cell(bRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            wsKpi.Cell(bRow, 7).SetValue($"{b.OccupancyRate:F1}%");
            wsKpi.Cell(bRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            bRow++;
        }

        if (buildings.Count > 0)
        {
            var bDataRange = wsKpi.Range(10, 1, 9 + buildings.Count, buildingHeaders.Length);
            ApplyDataStyle(bDataRange);
        }

        wsKpi.Columns().AdjustToContents();

        // 2. Worksheet 2: "Xu hướng doanh thu"
        var wsTrend = workbook.Worksheets.Add("Xu hướng doanh thu");

        // Tiêu đề
        wsTrend.Cell(1, 1).SetValue("XU HƯỚNG DOANH THU & TIỆN ÍCH 6 THÁNG GẦN NHẤT");
        wsTrend.Cell(1, 1).Style.Font.Bold = true;
        wsTrend.Cell(1, 1).Style.Font.FontSize = 13;
        wsTrend.Cell(1, 1).Style.Font.FontColor = DashboardHeaderColor;

        var trendHeaders = new[]
        {
            "Tháng/Năm",
            "Doanh thu tiền phòng (VND)",
            "Doanh thu điện nước/dịch vụ (VND)",
            "Tổng doanh thu (VND)"
        };

        wsTrend.Row(3).Height = 26;
        for (var col = 0; col < trendHeaders.Length; col++)
        {
            wsTrend.Cell(3, col + 1).SetValue(trendHeaders[col]);
        }
        var tHeaderRange = wsTrend.Range(3, 1, 3, trendHeaders.Length);
        ApplyHeaderStyle(tHeaderRange, DashboardHeaderColor);

        var tRow = 4;
        for (var i = 0; i < trends.Count; i++)
        {
            var t = trends[i];
            wsTrend.Row(tRow).Height = 20;

            wsTrend.Cell(tRow, 1).SetValue(string.IsNullOrWhiteSpace(t.Label) ? $"{t.Month:D2}/{t.Year}" : t.Label);
            wsTrend.Cell(tRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wsTrend.Cell(tRow, 2).SetValue(t.RoomFeeRevenue);
            wsTrend.Cell(tRow, 2).Style.NumberFormat.Format = CurrencyWithSuffixFormat;
            wsTrend.Cell(tRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            wsTrend.Cell(tRow, 3).SetValue(t.UtilityFeeRevenue);
            wsTrend.Cell(tRow, 3).Style.NumberFormat.Format = CurrencyWithSuffixFormat;
            wsTrend.Cell(tRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            wsTrend.Cell(tRow, 4).SetValue(t.TotalRevenue);
            wsTrend.Cell(tRow, 4).Style.NumberFormat.Format = CurrencyWithSuffixFormat;
            wsTrend.Cell(tRow, 4).Style.Font.Bold = true;
            wsTrend.Cell(tRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            tRow++;
        }

        if (trends.Count > 0)
        {
            var tDataRange = wsTrend.Range(4, 1, 3 + trends.Count, trendHeaders.Length);
            ApplyDataStyle(tDataRange);
        }

        wsTrend.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    private static void ApplyHeaderStyle(IXLRange headerRange, XLColor? backgroundColor = null)
    {
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = backgroundColor ?? HeaderBackgroundColor;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }

    private static void ApplyDataStyle(IXLRange dataRange)
    {
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    private static string GetRoomTypeName(RoomType type) => type switch
    {
        RoomType.Standard => "Tiêu chuẩn",
        RoomType.Premium => "Chất lượng cao",
        RoomType.Vip => "VIP",
        _ => type.ToString()
    };

    private static string GetGenderName(Gender gender) => gender switch
    {
        Gender.Male => "Nam",
        Gender.Female => "Nữ",
        Gender.Other => "Khác",
        _ => gender.ToString()
    };

    private static string GetRoomStatusName(RoomStatus status) => status switch
    {
        RoomStatus.Available => "Còn trống",
        RoomStatus.Occupied => "Đã đầy",
        RoomStatus.Maintenance => "Bảo trì",
        _ => status.ToString()
    };

    private static string GetBillStatusName(BillStatus status) => status switch
    {
        BillStatus.Unpaid => "Chưa thanh toán",
        BillStatus.Paid => "Đã thanh toán",
        BillStatus.Overdue => "Quá hạn",
        _ => status.ToString()
    };
}
