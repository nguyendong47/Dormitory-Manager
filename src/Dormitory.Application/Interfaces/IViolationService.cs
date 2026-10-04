using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý vi phạm nội quy và kỷ luật Ký túc xá
/// </summary>
public interface IViolationService
{
    /// <summary>
    /// Lấy danh sách biên bản vi phạm theo các tiêu chí tìm kiếm và bộ lọc
    /// </summary>
    Task<List<ViolationDto>> GetAllViolationsAsync(string? searchTerm = null, ViolationSeverity? severity = null, ViolationStatus? status = null, int? studentId = null, int? roomId = null);

    /// <summary>
    /// Lấy thông tin chi tiết biên bản vi phạm theo mã định danh
    /// </summary>
    Task<ViolationDto?> GetViolationByIdAsync(int id);

    /// <summary>
    /// Lấy danh sách các biên bản vi phạm của một sinh viên cụ thể
    /// </summary>
    Task<List<ViolationDto>> GetViolationsByStudentIdAsync(int studentId);

    /// <summary>
    /// Lập biên bản vi phạm mới
    /// </summary>
    Task<ViolationDto> CreateViolationAsync(CreateViolationDto dto);

    /// <summary>
    /// Cập nhật thông tin biên bản vi phạm
    /// </summary>
    Task<ViolationDto> UpdateViolationAsync(int id, UpdateViolationDto dto);

    /// <summary>
    /// Xử lý / giải quyết biên bản vi phạm (đổi trạng thái, ghi chú kết luận)
    /// </summary>
    Task<bool> ResolveViolationAsync(int id, ResolveViolationDto dto);

    /// <summary>
    /// Xóa biên bản vi phạm khỏi hệ thống
    /// </summary>
    Task<bool> DeleteViolationAsync(int id);
}
