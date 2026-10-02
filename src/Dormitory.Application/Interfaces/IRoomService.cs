using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý phòng ký túc xá
/// </summary>
public interface IRoomService
{
    Task<List<RoomDto>> GetAllRoomsAsync(string? building = null, RoomStatus? status = null);
    Task<RoomDto?> GetRoomByIdAsync(int id);
    Task<RoomDto> CreateRoomAsync(CreateOrUpdateRoomRequest request);
    Task<bool> UpdateRoomAsync(int id, CreateOrUpdateRoomRequest request);
    Task<bool> DeleteRoomAsync(int id);
    Task<bool> SetRoomStatusAsync(int id, RoomStatus status);
}
