using System.Threading.Tasks;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Dịch vụ kết xuất phiếu thu và hóa đơn ký túc xá định dạng PDF
/// </summary>
public interface IPdfExportService
{
    /// <summary>
    /// Sinh tệp PDF phiếu thu tiền phòng và điện nước cho một hóa đơn cụ thể
    /// </summary>
    /// <param name="billId">Mã định danh hóa đơn</param>
    /// <returns>Mảng byte tệp PDF hợp lệ</returns>
    Task<byte[]> GenerateBillReceiptPdfAsync(int billId);
}
