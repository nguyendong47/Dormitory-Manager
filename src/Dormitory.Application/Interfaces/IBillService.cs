using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý và lập hóa đơn tiền điện nước, dịch vụ phòng
/// </summary>
public interface IBillService
{
    Task<List<BillDto>> GetAllBillsAsync(int? roomId = null, int? month = null, int? year = null, BillStatus? status = null);
    Task<BillDto?> GetBillByIdAsync(int id);
    Task<BillDto> CreateBillAsync(CreateBillRequest request);
    Task<bool> MarkAsPaidAsync(int id);
    Task<bool> DeleteBillAsync(int id);
}
