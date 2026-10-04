using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Triển khai dịch vụ quản lý trang thiết bị, tài sản phòng KTX
/// </summary>
public class EquipmentService : IEquipmentService
{
    private readonly DormitoryDbContext _context;

    public EquipmentService(DormitoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Lấy danh sách toàn bộ trang thiết bị theo bộ lọc tìm kiếm, phòng và trạng thái
    /// </summary>
    public async Task<List<EquipmentDto>> GetAllEquipmentsAsync(string? searchTerm = null, int? roomId = null, EquipmentStatus? status = null)
    {
        var query = _context.Equipments
            .Include(e => e.Room)
            .AsNoTracking()
            .AsQueryable();

        if (roomId.HasValue && roomId.Value > 0)
        {
            query = query.Where(e => e.RoomId == roomId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(e => e.EquipmentCode.ToLower().Contains(term) || e.Name.ToLower().Contains(term));
        }

        var list = await query
            .OrderBy(e => e.RoomId)
            .ThenBy(e => e.EquipmentCode)
            .ToListAsync();

        return list.Select(e => MapToDto(e)).ToList();
    }

    /// <summary>
    /// Lấy thông tin thiết bị chi tiết theo ID
    /// </summary>
    public async Task<EquipmentDto?> GetEquipmentByIdAsync(int id)
    {
        var equipment = await _context.Equipments
            .Include(e => e.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return equipment == null ? null : MapToDto(equipment);
    }

    /// <summary>
    /// Lấy danh sách trang thiết bị gắn với phòng cụ thể
    /// </summary>
    public async Task<List<EquipmentDto>> GetEquipmentsByRoomIdAsync(int roomId)
    {
        return await GetAllEquipmentsAsync(roomId: roomId);
    }

    /// <summary>
    /// Tạo mới và lưu trang thiết bị vào phòng
    /// </summary>
    public async Task<EquipmentDto> CreateEquipmentAsync(CreateEquipmentDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        ValidateEquipmentData(dto.EquipmentCode, dto.Name, dto.Quantity, dto.Price);

        var room = await _context.Rooms.FindAsync(dto.RoomId);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng với mã ID {dto.RoomId}.");
        }

        var entity = new Equipment
        {
            RoomId = dto.RoomId,
            EquipmentCode = dto.EquipmentCode.Trim(),
            Name = dto.Name.Trim(),
            Status = dto.Status,
            Quantity = dto.Quantity,
            Price = dto.Price,
            Notes = dto.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Equipments.Add(entity);
        await _context.SaveChangesAsync();

        return MapToDto(entity, room);
    }

    /// <summary>
    /// Cập nhật thông tin trang thiết bị
    /// </summary>
    public async Task<EquipmentDto> UpdateEquipmentAsync(int id, UpdateEquipmentDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        ValidateEquipmentData(dto.EquipmentCode, dto.Name, dto.Quantity, dto.Price);

        var entity = await _context.Equipments
            .Include(e => e.Room)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy thiết bị với mã ID {id}.");
        }

        var room = await _context.Rooms.FindAsync(dto.RoomId);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng với mã ID {dto.RoomId}.");
        }

        entity.RoomId = dto.RoomId;
        entity.EquipmentCode = dto.EquipmentCode.Trim();
        entity.Name = dto.Name.Trim();
        entity.Status = dto.Status;
        entity.Quantity = dto.Quantity;
        entity.Price = dto.Price;
        entity.Notes = dto.Notes?.Trim();

        await _context.SaveChangesAsync();

        return MapToDto(entity, room);
    }

    /// <summary>
    /// Xóa thiết bị khỏi hệ thống
    /// </summary>
    public async Task<bool> DeleteEquipmentAsync(int id)
    {
        var entity = await _context.Equipments.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.Equipments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Cập nhật trạng thái và ghi nhận bảo trì nếu đưa về trạng thái hoạt động tốt
    /// </summary>
    public async Task<bool> UpdateStatusAsync(int id, EquipmentStatus newStatus, string? notes = null)
    {
        var entity = await _context.Equipments.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        entity.Status = newStatus;
        if (newStatus == EquipmentStatus.Good)
        {
            entity.LastMaintainedAt = DateTime.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(notes))
        {
            entity.Notes = notes.Trim();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu đầu vào cho trang thiết bị
    /// </summary>
    private static void ValidateEquipmentData(string equipmentCode, string name, int quantity, decimal price)
    {
        if (string.IsNullOrWhiteSpace(equipmentCode))
        {
            throw new ArgumentException("Mã tài sản không được để trống.", nameof(equipmentCode));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tên thiết bị không được để trống.", nameof(name));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Số lượng thiết bị phải lớn hơn 0.", nameof(quantity));
        }

        if (price < 0)
        {
            throw new ArgumentException("Đơn giá thiết bị không được âm.", nameof(price));
        }
    }

    /// <summary>
    /// Ánh xạ đối tượng Entity sang DTO kèm thông tin phòng và văn bản trạng thái
    /// </summary>
    private static EquipmentDto MapToDto(Equipment entity, Room? room = null)
    {
        var r = room ?? entity.Room;
        return new EquipmentDto
        {
            Id = entity.Id,
            RoomId = entity.RoomId,
            RoomNumber = r?.RoomNumber ?? string.Empty,
            BuildingName = r?.Building ?? string.Empty,
            EquipmentCode = entity.EquipmentCode,
            Name = entity.Name,
            Status = entity.Status,
            StatusText = GetStatusText(entity.Status),
            Quantity = entity.Quantity,
            Price = entity.Price,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            LastMaintainedAt = entity.LastMaintainedAt
        };
    }

    /// <summary>
    /// Chuyển đổi mã trạng thái thiết bị sang văn bản hiển thị tiếng Việt
    /// </summary>
    private static string GetStatusText(EquipmentStatus status) => status switch
    {
        EquipmentStatus.Good => "Hoạt động tốt",
        EquipmentStatus.NeedsRepair => "Cần bảo trì",
        EquipmentStatus.Broken => "Hỏng hóc",
        _ => "Không xác định"
    };
}
