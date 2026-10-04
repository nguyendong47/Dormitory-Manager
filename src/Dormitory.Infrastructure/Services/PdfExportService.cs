using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ kết xuất phiếu thu và hóa đơn ký túc xá định dạng PDF sử dụng QuestPDF
/// </summary>
public class PdfExportService : IPdfExportService
{
    private readonly IDormitoryDbContext _context;

    public PdfExportService(IDormitoryDbContext context)
    {
        _context = context;
        // Thiết lập giấy phép cộng đồng (Community License) cho QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Sinh mảng byte PDF phiếu thu tiền phòng và tiện ích điện nước cho hóa đơn được chỉ định
    /// </summary>
    /// <param name="billId">Mã định danh hóa đơn</param>
    /// <returns>Mảng byte tệp PDF hợp lệ</returns>
    public async Task<byte[]> GenerateBillReceiptPdfAsync(int billId)
    {
        // 1. Truy vấn thông tin hóa đơn kèm phòng
        var bill = await _context.Bills
            .Include(b => b.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == billId);

        if (bill == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy hóa đơn với ID: {billId}");
        }

        // 2. Truy vấn hợp đồng đang áp dụng cho phòng này (ưu tiên hợp đồng Active hoặc mới nhất)
        var contract = await _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .AsNoTracking()
            .Where(c => c.RoomId == bill.RoomId)
            .OrderByDescending(c => c.Status == ContractStatus.Active)
            .ThenByDescending(c => c.StartDate)
            .FirstOrDefaultAsync();

        // 3. Khởi tạo và thiết kế tài liệu QuestPDF
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // Header trang
                page.Header().Element(header => ComposeHeader(header, bill));

                // Thân trang: thông tin sinh viên & phòng, bảng chi phí
                page.Content().Element(content => ComposeContent(content, bill, contract));

                // Footer trang: chữ ký 2 bên và ghi chú chân trang
                page.Footer().Element(footer => ComposeFooter(footer));
            });
        });

        // 4. Sinh file PDF ra mảng byte
        return document.GeneratePdf();
    }

    /// <summary>
    /// Thiết kế phần đầu phiếu thu (Logo/Tên BQL, Hotline, Tiêu đề chính, Mã hóa đơn)
    /// </summary>
    private static void ComposeHeader(IContainer container, Bill bill)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // Thông tin Ban Quản Lý KTX bên trái
                row.RelativeItem().Column(bql =>
                {
                    bql.Item().Text("BAN QUẢN LÝ KÝ TÚC XÁ SINH VIÊN")
                        .FontSize(12).Bold().FontColor("#0078D4");
                    bql.Item().Text("Hệ thống Quản lý Ký túc xá Hiện đại & Chuyên nghiệp")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                    bql.Item().Text("Hotline: 1900 6868  |  Email: bqlktx@dormitory.edu.vn")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                });

                // Huy hiệu trạng thái thanh toán bên phải
                row.ConstantItem(150).AlignRight().AlignMiddle().Column(badgeCol =>
                {
                    var isPaid = bill.Status == BillStatus.Paid;
                    var badgeBg = isPaid ? "#DFF6DD" : "#FFF4CE";
                    var badgeFg = isPaid ? "#107C41" : "#A80000";
                    var statusText = isPaid ? "ĐÃ THANH TOÁN" : "CHƯA THANH TOÁN";

                    badgeCol.Item().Background(badgeBg)
                        .PaddingVertical(6)
                        .PaddingHorizontal(12)
                        .AlignCenter()
                        .Text(statusText)
                        .FontSize(10).Bold().FontColor(badgeFg);
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Tiêu đề phiếu thu chính giữa
            col.Item().PaddingTop(12).AlignCenter().Column(titleCol =>
            {
                titleCol.Item().AlignCenter().Text("PHIẾU THU TIỀN PHÒNG & TIỆN ÍCH DỊCH VỤ")
                    .FontSize(15).Bold().FontColor("#242424");

                titleCol.Item().AlignCenter().PaddingTop(3).Text(
                    $"Kỳ thu: Tháng {bill.Month:D2}/{bill.Year}   •   Mã phiếu: {bill.BillCode}   •   Ngày lập: {bill.CreatedAt:dd/MM/yyyy}")
                    .FontSize(9.5f).FontColor(Colors.Grey.Darken2);
            });

            col.Item().PaddingBottom(12);
        });
    }

    /// <summary>
    /// Thiết kế thân phiếu: khối thông tin sinh viên/phòng và bảng chi tiết các khoản thu
    /// </summary>
    private static void ComposeContent(IContainer container, Bill bill, Contract? contract)
    {
        container.Column(col =>
        {
            // Khối thông tin sinh viên và phòng ở (2 cột viền bo góc)
            col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
            {
                // Cột 1: Thông tin sinh viên
                row.RelativeItem().Column(stdCol =>
                {
                    stdCol.Item().Text("THÔNG TIN SINH VIÊN").FontSize(9.5f).Bold().FontColor("#0078D4");
                    stdCol.Item().PaddingTop(4).Text(text =>
                    {
                        text.Span("Họ và tên: ");
                        text.Span(contract?.Student?.FullName ?? "Chưa gán sinh viên").SemiBold();
                    });
                    stdCol.Item().Text($"Mã sinh viên: {contract?.Student?.StudentCode ?? "N/A"}");
                    stdCol.Item().Text($"Số điện thoại: {contract?.Student?.PhoneNumber ?? "N/A"}");
                    stdCol.Item().Text($"Lớp / Khoa: {contract?.Student?.ClassName ?? "N/A"}");
                });

                // Cột 2: Thông tin phòng & hợp đồng
                row.RelativeItem().Column(roomCol =>
                {
                    roomCol.Item().Text("THÔNG TIN PHÒNG & HỢP ĐỒNG").FontSize(9.5f).Bold().FontColor("#0078D4");
                    roomCol.Item().PaddingTop(4).Text(text =>
                    {
                        text.Span("Số phòng: ");
                        text.Span($"{bill.Room?.RoomNumber ?? "N/A"} ({bill.Room?.Building ?? "N/A"})").SemiBold();
                    });
                    roomCol.Item().Text($"Tòa nhà: {bill.Room?.Building ?? "N/A"}");
                    roomCol.Item().Text($"Mã hợp đồng: {contract?.ContractNumber ?? "N/A"}");
                    roomCol.Item().Text($"Hạn nộp: {bill.DueDate:dd/MM/yyyy}");
                });
            });

            col.Item().PaddingTop(12);

            // Bảng danh mục các khoản thu chi tiết
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);   // STT
                    columns.RelativeColumn(3f);   // Khoản thu
                    columns.RelativeColumn(2.2f); // Chỉ số (cũ -> mới)
                    columns.RelativeColumn(1.6f); // Tiêu thụ
                    columns.RelativeColumn(1.8f); // Đơn giá (đ)
                    columns.RelativeColumn(2.4f); // Thành tiền (đ)
                });

                // Header bảng
                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("STT");
                    header.Cell().Element(HeaderStyle).Text("Khoản thu / Dịch vụ");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("Chỉ số (Cũ - Mới)");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("Sử dụng");
                    header.Cell().Element(HeaderStyle).AlignRight().Text("Đơn giá (đ)");
                    header.Cell().Element(HeaderStyle).AlignRight().Text("Thành tiền (đ)");

                    static IContainer HeaderStyle(IContainer cell) =>
                        cell.Background("#0078D4").PaddingVertical(6).PaddingHorizontal(5)
                            .DefaultTextStyle(x => x.FontColor(Colors.White).Bold().FontSize(9));
                });

                // 1. Tiền thuê phòng
                table.Cell().Element(RowStyle).AlignCenter().Text("1");
                table.Cell().Element(RowStyle).Text("Tiền thuê phòng lưu trú");
                table.Cell().Element(RowStyle).AlignCenter().Text("-");
                table.Cell().Element(RowStyle).AlignCenter().Text("1 tháng");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.RoomFee:N0}");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.RoomFee:N0}").Bold();

                // 2. Tiền điện sinh hoạt
                table.Cell().Element(RowStyle).AlignCenter().Text("2");
                table.Cell().Element(RowStyle).Text("Điện sinh hoạt tiêu thụ");
                table.Cell().Element(RowStyle).AlignCenter().Text($"{bill.OldElectricIndex:N0} → {bill.NewElectricIndex:N0}");
                table.Cell().Element(RowStyle).AlignCenter().Text($"{bill.ElectricUsage:N0} kWh");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.ElectricRate:N0}");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.ElectricFee:N0}").Bold();

                // 3. Tiền nước sinh hoạt
                table.Cell().Element(RowStyle).AlignCenter().Text("3");
                table.Cell().Element(RowStyle).Text("Nước sinh hoạt tiêu thụ");
                table.Cell().Element(RowStyle).AlignCenter().Text($"{bill.OldWaterIndex:N0} → {bill.NewWaterIndex:N0}");
                table.Cell().Element(RowStyle).AlignCenter().Text($"{bill.WaterUsage:N0} m³");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.WaterRate:N0}");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.WaterFee:N0}").Bold();

                // 4. Phụ phí dịch vụ khác
                table.Cell().Element(RowStyle).AlignCenter().Text("4");
                table.Cell().Element(RowStyle).Text("Phụ phí tiện ích khác (vệ sinh, an ninh, wifi)");
                table.Cell().Element(RowStyle).AlignCenter().Text("-");
                table.Cell().Element(RowStyle).AlignCenter().Text("Trọn gói");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.OtherServiceFee:N0}");
                table.Cell().Element(RowStyle).AlignRight().Text($"{bill.OtherServiceFee:N0}").Bold();

                static IContainer RowStyle(IContainer cell) =>
                    cell.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(6).PaddingHorizontal(5)
                        .DefaultTextStyle(x => x.FontSize(9));
            });

            // Tổng cộng thanh toán
            col.Item().PaddingTop(6).AlignRight().Row(row =>
            {
                row.ConstantItem(260).Background("#F0F6FC").Border(1).BorderColor("#0078D4").Padding(8).Column(totCol =>
                {
                    totCol.Item().Row(r =>
                    {
                        r.RelativeItem().Text("TỔNG THANH TOÁN:").Bold().FontSize(10.5f).FontColor("#0078D4");
                        r.RelativeItem().AlignRight().Text($"{bill.TotalAmount:N0} VNĐ").Bold().FontSize(11.5f).FontColor("#0078D4");
                    });

                    if (bill.PaidDate.HasValue)
                    {
                        totCol.Item().PaddingTop(2).Text($"Đã nộp ngày: {bill.PaidDate:dd/MM/yyyy HH:mm}")
                            .FontSize(8.5f).FontColor("#107C41");
                    }
                });
            });

            // Ghi chú nếu có
            if (!string.IsNullOrWhiteSpace(bill.Note))
            {
                col.Item().PaddingTop(6).Text($"* Ghi chú: {bill.Note}")
                    .FontSize(8.5f).Italic().FontColor(Colors.Grey.Darken1);
            }
        });
    }

    /// <summary>
    /// Thiết kế chân trang: Khối chữ ký 2 bên và lưu ý hiệu lực phiếu thu
    /// </summary>
    private static void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(15).Row(row =>
            {
                // Cột ký: Người nộp tiền
                row.RelativeItem().AlignCenter().Column(sigCol =>
                {
                    sigCol.Item().Text("Người nộp tiền").Bold().FontSize(9.5f);
                    sigCol.Item().Text("(Ký và ghi rõ họ tên)").FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                    sigCol.Item().PaddingTop(50).Text("........................................").FontColor(Colors.Grey.Lighten1);
                });

                // Cột ký: Đại diện BQL KTX / Thủ quỹ
                row.RelativeItem().AlignCenter().Column(sigCol =>
                {
                    var now = DateTime.Now;
                    sigCol.Item().Text($"Ngày {now.Day:D2} tháng {now.Month:D2} năm {now.Year}").FontSize(8.5f).Italic();
                    sigCol.Item().Text("Đại diện BQL KTX / Thủ quỹ").Bold().FontSize(9.5f);
                    sigCol.Item().Text("(Ký, ghi rõ họ tên & đóng dấu)").FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                    sigCol.Item().PaddingTop(42).Text("........................................").FontColor(Colors.Grey.Lighten1);
                });
            });

            // Ghi chú pháp lý phiếu thu
            col.Item().PaddingTop(16).AlignCenter().Text(
                "Phiếu thu này được phát hành bởi Hệ thống Quản trị Ký túc xá Sinh viên. Vui lòng giữ phiếu để đối soát khi cần thiết.")
                .FontSize(7.5f).FontColor(Colors.Grey.Darken1);
        });
    }
}
