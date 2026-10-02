using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Services;

/// <summary>
/// Dịch vụ xử lý nghiệp vụ quản lý phòng ký túc xá
/// </summary>
public class RoomService : IRoomService
{
    private readonly IDormitoryDbContext _context;

    public RoomService(IDormitoryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách tất cả các phòng với bộ lọc tòa nhà và trạng thái
    /// </summary>
    public async Task<List<RoomDto>> GetAllRoomsAsync(string? building = null, RoomStatus? status = null)
    {
        var query = _context.Rooms.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(building))
        {
            query = query.Where(r => r.Building.Contains(building));
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query
            .OrderBy(r => r.Building)
            .ThenBy(r => r.RoomNumber)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Building = r.Building,
                Floor = r.Floor,
                Capacity = r.Capacity,
                CurrentOccupancy = r.CurrentOccupancy,
                PricePerMonth = r.PricePerMonth,
                Status = r.Status,
                Type = r.Type,
                AllowedGender = r.AllowedGender,
                Description = r.Description
            })
            .ToListAsync();
    }

    /// <summary>
    /// Tìm phòng theo mã định danh Id
    /// </summary>
    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        var room = await _context.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return null;

        return new RoomDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Building = room.Building,
            Floor = room.Floor,
            Capacity = room.Capacity,
            CurrentOccupancy = room.CurrentOccupancy,
            PricePerMonth = room.PricePerMonth,
            Status = room.Status,
            Type = room.Type,
            AllowedGender = room.AllowedGender,
            Description = room.Description
        };
    }

    /// <summary>
    /// Thêm phòng mới vào hệ thống
    /// </summary>
    public async Task<RoomDto> CreateRoomAsync(CreateOrUpdateRoomRequest request)
    {
        // Kiểm tra trùng số phòng trong cùng tòa nhà
        var exists = await _context.Rooms.AnyAsync(r => r.Building == request.Building && r.RoomNumber == request.RoomNumber);
        if (exists)
        {
            throw new InvalidOperationException($"Phòng {request.RoomNumber} thuộc {request.Building} đã tồn tại trong hệ thống.");
        }

        var room = new Room
        {
            RoomNumber = request.RoomNumber,
            Building = request.Building,
            Floor = request.Floor,
            Capacity = request.Capacity,
            CurrentOccupancy = 0,
            PricePerMonth = request.PricePerMonth,
            Status = RoomStatus.Available,
            Type = request.Type,
            AllowedGender = request.AllowedGender,
            Description = request.Description
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Building = room.Building,
            Floor = room.Floor,
            Capacity = room.Capacity,
            CurrentOccupancy = room.CurrentOccupancy,
            PricePerMonth = room.PricePerMonth,
            Status = room.Status,
            Type = room.Type,
            AllowedGender = room.AllowedGender,
            Description = room.Description
        };
    }

    /// <summary>
    /// Cập nhật thông tin phòng
    /// </summary>
    public async Task<bool> UpdateRoomAsync(int id, CreateOrUpdateRoomRequest request)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return false;

        room.RoomNumber = request.RoomNumber;
        room.Building = request.Building;
        room.Floor = request.Floor;
        room.Capacity = request.Capacity;
        room.PricePerMonth = request.PricePerMonth;
        room.Type = request.Type;
        room.AllowedGender = request.AllowedGender;
        room.Description = request.Description;

        // Cập nhật lại trạng thái dựa trên sức chứa
        if (room.CurrentOccupancy >= room.Capacity && room.Status == RoomStatus.Available)
        {
            room.Status = RoomStatus.Occupied;
        }
        else if (room.CurrentOccupancy < room.Capacity && room.Status == RoomStatus.Occupied)
        {
            room.Status = RoomStatus.Available;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Xóa phòng (chỉ cho phép nếu không có người đang ở)
    /// </summary>
    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return false;

        if (room.CurrentOccupancy > 0)
        {
            throw new InvalidOperationException("Không thể xóa phòng đang có sinh viên cư trú.");
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Thay đổi trạng thái phòng (Bảo trì / Mở lại hoạt động)
    /// </summary>
    public async Task<bool> SetRoomStatusAsync(int id, RoomStatus status)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return false;

        room.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
