using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Triển khai dịch vụ nghiệp vụ quản lý khách đến thăm KTX và kiểm soát ra vào
/// </summary>
public class VisitorService : IVisitorService
{
    private readonly IDormitoryDbContext _context;

    public VisitorService(IDormitoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Đăng ký thông tin và ghi nhận lượt khách check-in vào KTX
    /// </summary>
    public async Task<VisitorLogDto> CheckInAsync(CreateVisitorLogDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Dữ liệu đăng ký khách không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(dto.VisitorName))
        {
            throw new ArgumentException("Họ và tên khách đến thăm không được để trống.", nameof(dto.VisitorName));
        }

        if (string.IsNullOrWhiteSpace(dto.IdentificationNumber))
        {
            throw new ArgumentException("Số CCCD/CMND của khách không được để trống.", nameof(dto.IdentificationNumber));
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            throw new ArgumentException("Số điện thoại liên hệ của khách không được để trống.", nameof(dto.PhoneNumber));
        }

        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == dto.StudentId, cancellationToken);
        if (student == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy sinh viên có ID: {dto.StudentId}.");
        }

        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == dto.RoomId, cancellationToken);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng có ID: {dto.RoomId}.");
        }

        var visitorLog = new VisitorLog
        {
            VisitorName = dto.VisitorName.Trim(),
            IdentificationNumber = dto.IdentificationNumber.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            StudentId = dto.StudentId,
            RoomId = dto.RoomId,
            CheckInTime = DateTime.UtcNow,
            CheckOutTime = null,
            Status = VisitorStatus.InDormitory,
            Note = dto.Note?.Trim() ?? string.Empty
        };

        _context.VisitorLogs.Add(visitorLog);
        await _context.SaveChangesAsync(cancellationToken);

        return new VisitorLogDto
        {
            Id = visitorLog.Id,
            VisitorName = visitorLog.VisitorName,
            IdentificationNumber = visitorLog.IdentificationNumber,
            PhoneNumber = visitorLog.PhoneNumber,
            StudentId = visitorLog.StudentId,
            StudentName = student.FullName,
            RoomId = visitorLog.RoomId,
            RoomNumber = room.RoomNumber,
            CheckInTime = visitorLog.CheckInTime,
            CheckOutTime = visitorLog.CheckOutTime,
            Status = visitorLog.Status,
            Note = visitorLog.Note
        };
    }

    /// <summary>
    /// Ghi nhận khách check-out rời khỏi ký túc xá
    /// </summary>
    public async Task<VisitorLogDto> CheckOutAsync(int visitorLogId, string? note = null, CancellationToken cancellationToken = default)
    {
        var visitorLog = await _context.VisitorLogs
            .Include(v => v.Student)
            .Include(v => v.Room)
            .FirstOrDefaultAsync(v => v.Id == visitorLogId, cancellationToken);

        if (visitorLog == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy bản ghi khách với ID: {visitorLogId}.");
        }

        visitorLog.CheckOutTime = DateTime.UtcNow;
        visitorLog.Status = VisitorStatus.CheckedOut;

        if (!string.IsNullOrWhiteSpace(note))
        {
            var trimmedNote = note.Trim();
            visitorLog.Note = string.IsNullOrWhiteSpace(visitorLog.Note)
                ? trimmedNote
                : $"{visitorLog.Note} | {trimmedNote}";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(visitorLog);
    }

    /// <summary>
    /// Lấy danh sách lượt khách ra vào KTX theo bộ lọc tìm kiếm
    /// </summary>
    public async Task<IReadOnlyList<VisitorLogDto>> GetVisitorsAsync(VisitorFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _context.VisitorLogs
            .Include(v => v.Student)
            .Include(v => v.Room)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Status.HasValue)
            {
                query = query.Where(v => v.Status == filter.Status.Value);
            }

            if (filter.OnlyOverstayed)
            {
                query = query.Where(v => v.Status == VisitorStatus.Overstayed);
            }

            if (filter.FromDate.HasValue)
            {
                var fromDateUtc = filter.FromDate.Value.Date;
                query = query.Where(v => v.CheckInTime >= fromDateUtc);
            }

            if (filter.ToDate.HasValue)
            {
                var toDateUtc = filter.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(v => v.CheckInTime <= toDateUtc);
            }

            if (filter.RoomId.HasValue && filter.RoomId.Value > 0)
            {
                query = query.Where(v => v.RoomId == filter.RoomId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var kw = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(v =>
                    v.VisitorName.ToLower().Contains(kw) ||
                    v.IdentificationNumber.ToLower().Contains(kw) ||
                    v.PhoneNumber.ToLower().Contains(kw) ||
                    (v.Student != null && v.Student.FullName.ToLower().Contains(kw)) ||
                    (v.Room != null && v.Room.RoomNumber.ToLower().Contains(kw)));
            }
        }

        var results = await query
            .OrderByDescending(v => v.CheckInTime)
            .ToListAsync(cancellationToken);

        return results.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Lấy thông tin chi tiết một lượt khách theo mã định danh
    /// </summary>
    public async Task<VisitorLogDto?> GetVisitorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var log = await _context.VisitorLogs
            .Include(v => v.Student)
            .Include(v => v.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        return log == null ? null : MapToDto(log);
    }

    /// <summary>
    /// Quét và tự động đánh dấu các lượt khách đang ở KTX nhưng quá giờ quy định (sau 22:00) thành Overstayed
    /// </summary>
    public async Task<int> MarkOverstayedVisitorsAsync(DateTime? currentTime = null, CancellationToken cancellationToken = default)
    {
        var now = currentTime ?? DateTime.UtcNow;

        var activeVisitors = await _context.VisitorLogs
            .Where(v => v.Status == VisitorStatus.InDormitory)
            .ToListAsync(cancellationToken);

        var overstayed = activeVisitors
            .Where(v => now > v.CheckInTime.Date.AddHours(22))
            .ToList();

        if (overstayed.Count == 0)
        {
            return 0;
        }

        foreach (var visitor in overstayed)
        {
            visitor.Status = VisitorStatus.Overstayed;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return overstayed.Count;
    }

    /// <summary>
    /// Lấy số liệu tổng hợp thống kê lượt khách đến thăm trong ngày
    /// </summary>
    public async Task<VisitorSummaryDto> GetVisitorSummaryAsync(DateTime? date = null, CancellationToken cancellationToken = default)
    {
        var targetDate = (date ?? DateTime.UtcNow).Date;
        var dayStart = targetDate;
        var dayEnd = targetDate.AddDays(1).AddTicks(-1);

        var todayLogs = await _context.VisitorLogs
            .AsNoTracking()
            .Where(v => v.CheckInTime >= dayStart && v.CheckInTime <= dayEnd)
            .ToListAsync(cancellationToken);

        if (date.HasValue)
        {
            return new VisitorSummaryDto
            {
                TotalVisitorsToday = todayLogs.Count,
                ActiveVisitorsCount = todayLogs.Count(v => v.Status == VisitorStatus.InDormitory),
                OverstayedCount = todayLogs.Count(v => v.Status == VisitorStatus.Overstayed)
            };
        }

        var activeCount = await _context.VisitorLogs
            .AsNoTracking()
            .CountAsync(v => v.Status == VisitorStatus.InDormitory, cancellationToken);

        var overstayedCount = await _context.VisitorLogs
            .AsNoTracking()
            .CountAsync(v => v.Status == VisitorStatus.Overstayed, cancellationToken);

        return new VisitorSummaryDto
        {
            TotalVisitorsToday = todayLogs.Count,
            ActiveVisitorsCount = activeCount,
            OverstayedCount = overstayedCount
        };
    }

    private static VisitorLogDto MapToDto(VisitorLog log)
    {
        return new VisitorLogDto
        {
            Id = log.Id,
            VisitorName = log.VisitorName,
            IdentificationNumber = log.IdentificationNumber,
            PhoneNumber = log.PhoneNumber,
            StudentId = log.StudentId,
            StudentName = log.Student?.FullName ?? string.Empty,
            RoomId = log.RoomId,
            RoomNumber = log.Room?.RoomNumber ?? string.Empty,
            CheckInTime = log.CheckInTime,
            CheckOutTime = log.CheckOutTime,
            Status = log.Status,
            Note = log.Note
        };
    }
}
