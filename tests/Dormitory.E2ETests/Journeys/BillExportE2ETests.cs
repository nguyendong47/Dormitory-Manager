using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Xunit;

namespace Dormitory.E2ETests.Journeys;

/// <summary>
/// Kiểm thử luồng E2E cho quy trình xuất hóa đơn / phiếu thu ký túc xá ra định dạng tài liệu in ấn PDF
/// </summary>
public class BillExportE2ETests : IDisposable
{
    private readonly TestFixture _fixture = new();

    /// <summary>
    /// Kịch bản E2E: Tải danh sách hóa đơn -> Kích hoạt lệnh xuất PDF hóa đơn phòng -> Xác minh mảng byte PDF hợp lệ (%PDF-)
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Export_Bill_Receipt_Pdf_Successfully_With_Valid_Pdf_Signature()
    {
        // 1. Khởi tạo BillListViewModel từ TestFixture
        var vm = _fixture.CreateBillListViewModel();

        // 2. Tải danh sách hóa đơn (dữ liệu mẫu đã được seed)
        await vm.LoadBillsCommand.ExecuteAsync(null);

        vm.Bills.Should().NotBeEmpty("Danh sách hóa đơn phải có ít nhất một bản ghi mẫu");
        var testBill = vm.Bills.First();
        testBill.BillCode.Should().NotBeNullOrWhiteSpace();

        // 3. Thực thi lệnh xuất phiếu thu PDF với hóa đơn được chọn
        await vm.ExportBillPdfCommand.ExecuteAsync(testBill);

        // 4. Xác minh dịch vụ FileService nhận được yêu cầu lưu file với thông tin chuẩn
        _fixture.FileService.SaveCallCount.Should().Be(1, "Hệ thống phải gọi lưu tệp đúng 1 lần");
        _fixture.FileService.LastSavedFileName.Should().Contain(testBill.BillCode,
            "Tên tệp gợi ý phải bao gồm mã hóa đơn");
        _fixture.FileService.LastSavedExtension.Should().Be("pdf",
            "Định dạng tệp xuất ra phải là pdf");

        // 5. Kiểm tra tính hợp lệ của dữ liệu nhị phân PDF sinh bởi QuestPDF
        var pdfBytes = _fixture.FileService.LastSavedBytes;
        pdfBytes.Should().NotBeNullOrEmpty("Nội dung mảng byte PDF không được rỗng");
        pdfBytes!.Length.Should().BeGreaterThan(1000, "Tệp PDF phiếu thu hoàn chỉnh phải có dung lượng tối thiểu > 1KB");

        // Kiểm tra chữ ký Magic bytes chuẩn của tài liệu PDF: "%PDF-" (0x25 0x50 0x44 0x46 0x2D)
        var pdfHeader = Encoding.ASCII.GetString(pdfBytes.Take(5).ToArray());
        pdfHeader.Should().Be("%PDF-", "Tệp xuất ra phải bắt đầu bằng tiêu đề chuẩn %PDF-");

        // 6. Kiểm tra xuất PDF qua thuộc tính SelectedBill trên giao diện khi không truyền tham số
        vm.SelectedBill = testBill;
        await vm.ExportBillPdfCommand.ExecuteAsync(null);

        _fixture.FileService.SaveCallCount.Should().Be(2, "Lệnh xuất thông qua SelectedBill phải được kích hoạt thành công");
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
