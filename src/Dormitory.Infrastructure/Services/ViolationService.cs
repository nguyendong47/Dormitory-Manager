using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Triển khai dịch vụ quản lý vi phạm nội quy và kỷ luật Ký túc xá
/// </summary>
public class ViolationService : IViolationService
{
    private readonly DormitoryDbContext _context;

    public ViolationService(DormitoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Lấy danh sách biên bản vi phạm theo các tiêu chí tìm kiếm và bộ lọc
    /// </summary>
    public async Task<List<ViolationDto>> GetAllViolationsAsync(
        string? searchTerm = null,
        ViolationSeverity? severity = null,
        ViolationStatus? status = null,
        int? studentId = null,
        int? roomId = null)
    {
        var query = _context.Violations
            .Include(v => v.Student)
            .Include(v => v.Room)
            .AsNoTracking()
            .AsQueryable();

        if (studentId.HasValue && studentId.Value > 0)
        {
            query = query.Where(v => v.StudentId == studentId.Value);
        }

        if (roomId.HasValue && roomId.Value > 0)
        {
            query = query.Where(v => v.RoomId == roomId.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(v => v.Severity == severity.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(v =>
                v.ViolationCode.ToLower().Contains(term) ||
                v.Title.ToLower().Contains(term) ||
                (v.Description != null && v.Description.ToLower().Contains(term)) ||
                (v.Student != null && (v.Student.FullName.ToLower().Contains(term) || v.Student.StudentCode.ToLower().Contains(term))) ||
                (v.Room != null && v.Room.RoomNumber.ToLower().Contains(term)));
        }

        var list = await query
            .OrderByDescending(v => v.ViolationDate)
            .ThenByDescending(v => v.Id)
            .ToListAsync();

        return list.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Lấy thông tin chi tiết biên bản vi phạm theo mã định danh
    /// </summary>
    public async Task<ViolationDto?> GetViolationByIdAsync(int id)
    {
        var violation = await _context.Violations
            .Include(v => v.Student)
            .Include(v => v.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        return violation == null ? null : MapToDto(violation);
    }

    /// <summary>
    /// Lấy danh sách các biên bản vi phạm của một sinh viên cụ thể
    /// </summary>
    public async Task<List<ViolationDto>> GetViolationsByStudentIdAsync(int studentId)
    {
        return await GetAllViolationsAsync(studentId: studentId);
    }

    /// <summary>
    /// Lập biên bản vi phạm mới
    /// </summary>
    public async Task<ViolationDto> CreateViolationAsync(CreateViolationDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        ValidateViolationData(dto.Title, dto.FineAmount, dto.DemeritPoints);

        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy sinh viên với mã ID {dto.StudentId}.");
        }

        var room = await _context.Rooms.FindAsync(dto.RoomId);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng với mã ID {dto.RoomId}.");
        }

        var violationCode = string.IsNullOrWhiteSpace(dto.ViolationCode)
            ? await GenerateViolationCodeAsync()
            : dto.ViolationCode.Trim();

        var entity = new Violation
        {
            ViolationCode = violationCode,
            StudentId = dto.StudentId,
            Student = student,
            RoomId = dto.RoomId,
            Room = room,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Severity = dto.Severity,
            Status = ViolationStatus.Pending,
            FineAmount = dto.FineAmount,
            DemeritPoints = dto.DemeritPoints,
            ViolationDate = dto.ViolationDate,
            CreatedAt = DateTime.UtcNow,
            RecordedBy = dto.RecordedBy?.Trim()
        };

        _context.Violations.Add(entity);
        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    /// <summary>
    /// Cập nhật thông tin biên bản vi phạm
    /// </summary>
    public async Task<ViolationDto> UpdateViolationAsync(int id, UpdateViolationDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        ValidateViolationData(dto.Title, dto.FineAmount, dto.DemeritPoints);

        var entity = await _context.Violations
            .Include(v => v.Student)
            .Include(v => v.Room)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy biên bản vi phạm với mã ID {id}.");
        }

        var student = await _context.Students.FindAsync(dto.StudentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy sinh viên với mã ID {dto.StudentId}.");
        }

        var room = await _context.Rooms.FindAsync(dto.RoomId);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng với mã ID {dto.RoomId}.");
        }

        entity.StudentId = dto.StudentId;
        entity.Student = student;
        entity.RoomId = dto.RoomId;
        entity.Room = room;
        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description?.Trim();
        entity.Severity = dto.Severity;
        entity.Status = dto.Status;
        entity.FineAmount = dto.FineAmount;
        entity.DemeritPoints = dto.DemeritPoints;
        entity.ViolationDate = dto.ViolationDate;
        entity.ResolutionNotes = dto.ResolutionNotes?.Trim();

        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    /// <summary>
    /// Xử lý / giải quyết biên bản vi phạm (đổi trạng thái, ghi chú kết luận)
    /// </summary>
    public async Task<bool> ResolveViolationAsync(int id, ResolveViolationDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var entity = await _context.Violations.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        entity.Status = dto.Status;
        entity.ResolutionNotes = dto.ResolutionNotes?.Trim();

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Xóa biên bản vi phạm khỏi hệ thống
    /// </summary>
    public async Task<bool> DeleteViolationAsync(int id)
    {
        var entity = await _context.Violations.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.Violations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Tự động sinh mã biên bản vi phạm theo định dạng VP-yyyyMMdd-XXX
    /// </summary>
    private async Task<string> GenerateViolationCodeAsync()
    {
        var todayStr = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"VP-{todayStr}-";

        var countToday = await _context.Violations
            .CountAsync(v => v.ViolationCode.StartsWith(prefix));

        return $"{prefix}{(countToday + 1):D3}";
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của dữ liệu biên bản vi phạm
    /// </summary>
    private static void ValidateViolationData(string title, decimal fineAmount, int demeritPoints)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Tiêu đề vi phạm không được để trống.", nameof(title));
        }

        if (fineAmount < 0)
        {
            throw new ArgumentException("Số tiền xử phạt không được là số âm.", nameof(fineAmount));
        }

        if (demeritPoints < 0)
        {
            throw new ArgumentException("Điểm rèn luyện bị trừ không được là số âm.", nameof(demeritPoints));
        }
    }

    /// <summary>
    /// Ánh xạ thực thể Violation sang DTO hiển thị
    /// </summary>
    public static ViolationDto MapToDto(Violation entity)
    {
        return new ViolationDto
        {
            Id = entity.Id,
            ViolationCode = entity.ViolationCode,
            StudentId = entity.StudentId,
            StudentName = entity.Student?.FullName ?? string.Empty,
            StudentCode = entity.Student?.StudentCode ?? string.Empty,
            RoomId = entity.RoomId,
            RoomNumber = entity.Room?.RoomNumber ?? string.Empty,
            BuildingName = entity.Room?.Building ?? string.Empty,
            Title = entity.Title,
            Description = entity.Description,
            Severity = entity.Severity,
            SeverityText = GetSeverityText(entity.Severity),
            Status = entity.Status,
            StatusText = GetStatusText(entity.Status),
            FineAmount = entity.FineAmount,
            DemeritPoints = entity.DemeritPoints,
            ViolationDate = entity.ViolationDate,
            CreatedAt = entity.CreatedAt,
            ResolutionNotes = entity.ResolutionNotes,
            RecordedBy = entity.RecordedBy
        };
    }

    /// <summary>
    /// Chuyển đổi Enum mức độ vi phạm sang tiếng Việt thân thiện
    /// </summary>
    private static string GetSeverityText(ViolationSeverity severity) => severity switch
    {
        ViolationSeverity.Minor => "Nhắc nhở",
        ViolationSeverity.Moderate => "Khiển trách",
        ViolationSeverity.Severe => "Cảnh cáo",
        ViolationSeverity.Critical => "Buộc rời KTX",
        _ => "Khác"
    };

    /// <summary>
    /// Chuyển đổi Enum trạng thái vi phạm sang tiếng Việt thân thiện
    /// </summary>
    private static string GetStatusText(ViolationStatus status) => status switch
    {
        ViolationStatus.Pending => "Chờ xử lý",
        ViolationStatus.Resolved => "Đã xử lý",
        ViolationStatus.Dismissed => "Hủy bỏ / Miễn trừ",
        _ => "Không xác định"
    };
}
