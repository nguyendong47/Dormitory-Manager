using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Triển khai dịch vụ điểm danh và theo dõi chuyên cần sinh viên nội trú ban đêm
/// </summary>
public class AttendanceService : IAttendanceService
{
    private readonly IDormitoryDbContext _context;

    public AttendanceService(IDormitoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Ghi nhận hoặc cập nhật kết quả điểm danh cho một sinh viên cụ thể trong ngày
    /// </summary>
    public async Task<AttendanceRecordDto> RecordSingleAttendanceAsync(
        int studentId,
        int roomId,
        DateTime date,
        AttendanceStatus status,
        string? reason = null,
        string? recordedBy = null,
        CancellationToken cancellationToken = default)
    {
        if (studentId <= 0)
        {
            throw new ArgumentException("Mã sinh viên không hợp lệ.", nameof(studentId));
        }

        if (roomId <= 0)
        {
            throw new ArgumentException("Mã phòng không hợp lệ.", nameof(roomId));
        }

        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        if (student == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy sinh viên có ID: {studentId}.");
        }

        var room = await _context.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng có ID: {roomId}.");
        }

        var attendanceDate = date.Date;

        var existing = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.StudentId == studentId && a.AttendanceDate == attendanceDate, cancellationToken);

        if (existing != null)
        {
            existing.Status = status;
            existing.Reason = reason?.Trim();
            existing.RecordedBy = recordedBy?.Trim();
            existing.RoomId = roomId;

            await _context.SaveChangesAsync(cancellationToken);

            return new AttendanceRecordDto
            {
                Id = existing.Id,
                AttendanceDate = existing.AttendanceDate,
                StudentId = student.Id,
                StudentName = student.FullName,
                StudentCode = student.StudentCode,
                RoomId = room.Id,
                RoomNumber = room.RoomNumber,
                Status = existing.Status,
                Reason = existing.Reason,
                RecordedBy = existing.RecordedBy,
                CreatedAt = existing.CreatedAt
            };
        }

        var newRecord = new AttendanceRecord
        {
            StudentId = studentId,
            RoomId = roomId,
            AttendanceDate = attendanceDate,
            Status = status,
            Reason = reason?.Trim(),
            RecordedBy = recordedBy?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.AttendanceRecords.Add(newRecord);
        await _context.SaveChangesAsync(cancellationToken);

        return new AttendanceRecordDto
        {
            Id = newRecord.Id,
            AttendanceDate = newRecord.AttendanceDate,
            StudentId = student.Id,
            StudentName = student.FullName,
            StudentCode = student.StudentCode,
            RoomId = room.Id,
            RoomNumber = room.RoomNumber,
            Status = newRecord.Status,
            Reason = newRecord.Reason,
            RecordedBy = newRecord.RecordedBy,
            CreatedAt = newRecord.CreatedAt
        };
    }

    /// <summary>
    /// Ghi nhận hoặc cập nhật kết quả điểm danh theo lô cho cả phòng sinh viên
    /// </summary>
    public async Task<IReadOnlyList<AttendanceRecordDto>> RecordBatchRoomAttendanceAsync(
        BatchRoomAttendanceDto batchDto,
        CancellationToken cancellationToken = default)
    {
        if (batchDto == null)
        {
            throw new ArgumentNullException(nameof(batchDto), "Dữ liệu điểm danh theo phòng không được để trống.");
        }

        if (batchDto.RoomId <= 0)
        {
            throw new ArgumentException("Mã phòng không hợp lệ.", nameof(batchDto.RoomId));
        }

        var roomExists = await _context.Rooms
            .AnyAsync(r => r.Id == batchDto.RoomId, cancellationToken);
        if (!roomExists)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng có ID: {batchDto.RoomId}.");
        }

        var attendanceDate = batchDto.AttendanceDate.Date;

        if (batchDto.StudentAttendances != null && batchDto.StudentAttendances.Count > 0)
        {
            var studentIds = batchDto.StudentAttendances
                .Where(x => x.StudentId > 0)
                .Select(x => x.StudentId)
                .Distinct()
                .ToList();

            var existingMap = await _context.AttendanceRecords
                .Where(a => studentIds.Contains(a.StudentId) && a.AttendanceDate == attendanceDate)
                .ToDictionaryAsync(a => a.StudentId, cancellationToken);

            foreach (var item in batchDto.StudentAttendances)
            {
                if (item.StudentId <= 0)
                {
                    continue;
                }

                existingMap.TryGetValue(item.StudentId, out var existing);

                var recorder = !string.IsNullOrWhiteSpace(item.RecordedBy)
                    ? item.RecordedBy.Trim()
                    : batchDto.RecordedBy?.Trim();

                if (existing != null)
                {
                    existing.Status = item.Status;
                    existing.Reason = item.Reason?.Trim();
                    existing.RecordedBy = recorder;
                    existing.RoomId = batchDto.RoomId;
                }
                else
                {
                    var newRecord = new AttendanceRecord
                    {
                        StudentId = item.StudentId,
                        RoomId = batchDto.RoomId,
                        AttendanceDate = attendanceDate,
                        Status = item.Status,
                        Reason = item.Reason?.Trim(),
                        RecordedBy = recorder,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.AttendanceRecords.Add(newRecord);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return await GetRoomAttendanceAsync(batchDto.RoomId, attendanceDate, cancellationToken);
    }

    /// <summary>
    /// Lấy danh sách kết quả điểm danh của một phòng trong ngày chỉ định
    /// </summary>
    public async Task<IReadOnlyList<AttendanceRecordDto>> GetRoomAttendanceAsync(
        int roomId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        var attendanceDate = date.Date;

        var records = await _context.AttendanceRecords
            .Include(a => a.Student)
            .Include(a => a.Room)
            .AsNoTracking()
            .Where(a => a.RoomId == roomId && a.AttendanceDate == attendanceDate)
            .OrderBy(a => a.Student != null ? a.Student.FullName : string.Empty)
            .ToListAsync(cancellationToken);

        return records.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Lấy danh sách bản ghi điểm danh theo bộ lọc tìm kiếm
    /// </summary>
    public async Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(
        AttendanceFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AttendanceRecords
            .Include(a => a.Student)
            .Include(a => a.Room)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Date.HasValue)
            {
                var d = filter.Date.Value.Date;
                query = query.Where(a => a.AttendanceDate == d);
            }

            if (filter.FromDate.HasValue)
            {
                var fromD = filter.FromDate.Value.Date;
                query = query.Where(a => a.AttendanceDate >= fromD);
            }

            if (filter.ToDate.HasValue)
            {
                var toD = filter.ToDate.Value.Date;
                query = query.Where(a => a.AttendanceDate <= toD);
            }

            if (filter.RoomId.HasValue && filter.RoomId.Value > 0)
            {
                query = query.Where(a => a.RoomId == filter.RoomId.Value);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(a => a.Status == filter.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var kw = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(a =>
                    (a.Student != null && (a.Student.FullName.ToLower().Contains(kw) || a.Student.StudentCode.ToLower().Contains(kw))) ||
                    (a.Room != null && a.Room.RoomNumber.ToLower().Contains(kw)));
            }
        }

        var results = await query
            .OrderByDescending(a => a.AttendanceDate)
            .ThenBy(a => a.RoomId)
            .ThenBy(a => a.Student != null ? a.Student.FullName : string.Empty)
            .ToListAsync(cancellationToken);

        return results.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Thống kê kết quả và tính tỷ lệ chuyên cần theo khoảng ngày hoặc phòng
    /// </summary>
    public async Task<AttendanceStatsDto> GetAttendanceStatsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? roomId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AttendanceRecords
            .AsNoTracking()
            .AsQueryable();

        if (fromDate.HasValue)
        {
            var fromD = fromDate.Value.Date;
            query = query.Where(a => a.AttendanceDate >= fromD);
        }

        if (toDate.HasValue)
        {
            var toD = toDate.Value.Date;
            query = query.Where(a => a.AttendanceDate <= toD);
        }

        if (roomId.HasValue && roomId.Value > 0)
        {
            query = query.Where(a => a.RoomId == roomId.Value);
        }

        var records = await query.ToListAsync(cancellationToken);

        var total = records.Count;
        var present = records.Count(r => r.Status == AttendanceStatus.Present);
        var absentWithoutPermission = records.Count(r => r.Status == AttendanceStatus.AbsentWithoutPermission);
        var absentWithPermission = records.Count(r => r.Status == AttendanceStatus.AbsentWithPermission);
        var late = records.Count(r => r.Status == AttendanceStatus.Late);
        var rate = total > 0 ? Math.Round(present * 100.0 / total, 2) : 0.0;

        return new AttendanceStatsDto
        {
            TotalStudents = total,
            PresentCount = present,
            AbsentWithoutPermissionCount = absentWithoutPermission,
            AbsentWithPermissionCount = absentWithPermission,
            LateCount = late,
            AttendanceRate = rate
        };
    }

    private static AttendanceRecordDto MapToDto(AttendanceRecord r)
    {
        return new AttendanceRecordDto
        {
            Id = r.Id,
            AttendanceDate = r.AttendanceDate,
            StudentId = r.StudentId,
            StudentName = r.Student?.FullName ?? string.Empty,
            StudentCode = r.Student?.StudentCode ?? string.Empty,
            RoomId = r.RoomId,
            RoomNumber = r.Room?.RoomNumber ?? string.Empty,
            Status = r.Status,
            Reason = r.Reason,
            RecordedBy = r.RecordedBy,
            CreatedAt = r.CreatedAt
        };
    }
}
