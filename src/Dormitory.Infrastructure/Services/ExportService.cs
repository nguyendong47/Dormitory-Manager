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
    private const string CurrencyFormat = "#,##0";
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

    private static void ApplyHeaderStyle(IXLRange headerRange)
    {
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = HeaderBackgroundColor;
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
