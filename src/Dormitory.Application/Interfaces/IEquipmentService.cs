using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý trang thiết bị, tài sản phòng ký túc xá
/// </summary>
public interface IEquipmentService
{
    /// <summary>
    /// Lấy danh sách trang thiết bị kèm bộ lọc từ khóa, phòng và trạng thái
    /// </summary>
    Task<List<EquipmentDto>> GetAllEquipmentsAsync(string? searchTerm = null, int? roomId = null, EquipmentStatus? status = null);

    /// <summary>
    /// Lấy thông tin chi tiết trang thiết bị theo mã định danh
    /// </summary>
    Task<EquipmentDto?> GetEquipmentByIdAsync(int id);

    /// <summary>
    /// Lấy danh sách trang thiết bị gắn liền với một phòng cụ thể
    /// </summary>
    Task<List<EquipmentDto>> GetEquipmentsByRoomIdAsync(int roomId);

    /// <summary>
    /// Thêm mới trang thiết bị vào phòng
    /// </summary>
    Task<EquipmentDto> CreateEquipmentAsync(CreateEquipmentDto dto);

    /// <summary>
    /// Cập nhật thông tin trang thiết bị
    /// </summary>
    Task<EquipmentDto> UpdateEquipmentAsync(int id, UpdateEquipmentDto dto);

    /// <summary>
    /// Xóa trang thiết bị khỏi hệ thống
    /// </summary>
    Task<bool> DeleteEquipmentAsync(int id);

    /// <summary>
    /// Cập nhật trạng thái hoạt động của thiết bị và ghi nhận lịch sử bảo trì
    /// </summary>
    Task<bool> UpdateStatusAsync(int id, EquipmentStatus newStatus, string? notes = null);
}
