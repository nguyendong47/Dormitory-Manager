using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Services;

/// <summary>
/// Dịch vụ xử lý nghiệp vụ hợp đồng thuê phòng KTX
/// </summary>
public class ContractService : IContractService
{
    private readonly IDormitoryDbContext _context;

    public ContractService(IDormitoryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách hợp đồng kèm bộ lọc trạng thái, sinh viên hoặc phòng
    /// </summary>
    public async Task<List<ContractDto>> GetAllContractsAsync(ContractStatus? status = null, int? studentId = null, int? roomId = null)
    {
        var query = _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(c => c.StudentId == studentId.Value);
        }

        if (roomId.HasValue)
        {
            query = query.Where(c => c.RoomId == roomId.Value);
        }

        return await query
            .OrderByDescending(c => c.StartDate)
            .Select(c => new ContractDto
            {
                Id = c.Id,
                ContractNumber = c.ContractNumber,
                StudentId = c.StudentId,
                StudentName = c.Student != null ? c.Student.FullName : string.Empty,
                StudentCode = c.Student != null ? c.Student.StudentCode : string.Empty,
                RoomId = c.RoomId,
                RoomNumber = c.Room != null ? c.Room.RoomNumber : string.Empty,
                Building = c.Room != null ? c.Room.Building : string.Empty,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                DepositAmount = c.DepositAmount,
                MonthlyRate = c.MonthlyRate,
                Status = c.Status,
                Notes = c.Notes
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy chi tiết hợp đồng theo Id
    /// </summary>
    public async Task<ContractDto?> GetContractByIdAsync(int id)
    {
        var c = await _context.Contracts
            .Include(ct => ct.Student)
            .Include(ct => ct.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct => ct.Id == id);

        if (c == null) return null;

        return new ContractDto
        {
            Id = c.Id,
            ContractNumber = c.ContractNumber,
            StudentId = c.StudentId,
            StudentName = c.Student?.FullName ?? string.Empty,
            StudentCode = c.Student?.StudentCode ?? string.Empty,
            RoomId = c.RoomId,
            RoomNumber = c.Room?.RoomNumber ?? string.Empty,
            Building = c.Room?.Building ?? string.Empty,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            DepositAmount = c.DepositAmount,
            MonthlyRate = c.MonthlyRate,
            Status = c.Status,
            Notes = c.Notes
        };
    }

    /// <summary>
    /// Tạo hợp đồng thuê phòng mới
    /// </summary>
    public async Task<ContractDto> CreateContractAsync(CreateContractRequest request)
    {
        // 1. Kiểm tra ngày bắt đầu và kết thúc
        if (request.EndDate <= request.StartDate)
        {
            throw new ArgumentException("Ngày kết thúc hợp đồng phải sau ngày bắt đầu.");
        }

        // 2. Tìm sinh viên
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy sinh viên có ID {request.StudentId}.");
        }

        // 3. Kiểm tra xem sinh viên đã có hợp đồng Active nào chưa
        var hasActiveContract = await _context.Contracts.AnyAsync(c => c.StudentId == request.StudentId && c.Status == ContractStatus.Active);
        if (hasActiveContract)
        {
            throw new InvalidOperationException($"Sinh viên {student.FullName} ({student.StudentCode}) đang có một hợp đồng phòng còn hiệu lực.");
        }

        // 4. Tìm phòng
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng có ID {request.RoomId}.");
        }

        // 5. Kiểm tra trạng thái và sức chứa phòng
        if (room.Status == RoomStatus.Maintenance)
        {
            throw new InvalidOperationException($"Phòng {room.RoomNumber} đang trong thời gian bảo trì, không thể xếp phòng.");
        }

        if (room.CurrentOccupancy >= room.Capacity)
        {
            throw new InvalidOperationException($"Phòng {room.RoomNumber} đã đủ chỗ ({room.CurrentOccupancy}/{room.Capacity}).");
        }

        // 6. Kiểm tra giới tính
        if (room.AllowedGender != student.Gender)
        {
            var genderStr = room.AllowedGender == Gender.Male ? "Nam" : "Nữ";
            throw new InvalidOperationException($"Phòng {room.RoomNumber} chỉ dành cho sinh viên {genderStr}.");
        }

        // 7. Tạo mã hợp đồng tự động
        var count = await _context.Contracts.CountAsync();
        var contractNumber = $"HD-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D4}";

        var contract = new Contract
        {
            ContractNumber = contractNumber,
            StudentId = student.Id,
            RoomId = room.Id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DepositAmount = request.DepositAmount,
            MonthlyRate = request.MonthlyRate > 0 ? request.MonthlyRate : room.PricePerMonth,
            Status = ContractStatus.Active,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // 8. Cập nhật số người ở trong phòng và phòng hiện tại của SV
        room.CurrentOccupancy++;
        if (room.CurrentOccupancy >= room.Capacity)
        {
            room.Status = RoomStatus.Occupied;
        }
        student.CurrentRoomId = room.Id;

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        return new ContractDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            StudentId = student.Id,
            StudentName = student.FullName,
            StudentCode = student.StudentCode,
            RoomId = room.Id,
            RoomNumber = room.RoomNumber,
            Building = room.Building,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            DepositAmount = contract.DepositAmount,
            MonthlyRate = contract.MonthlyRate,
            Status = contract.Status,
            Notes = contract.Notes
        };
    }

    /// <summary>
    /// Chấm dứt / Thanh lý hợp đồng trước hoặc đúng hạn
    /// </summary>
    public async Task<bool> TerminateContractAsync(int id, string? reason = null)
    {
        var contract = await _context.Contracts
            .Include(c => c.Student)
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contract == null || contract.Status != ContractStatus.Active)
            return false;

        contract.Status = ContractStatus.Terminated;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            contract.Notes = string.IsNullOrWhiteSpace(contract.Notes)
                ? $"Lý do thanh lý: {reason}"
                : $"{contract.Notes} | Lý do thanh lý: {reason}";
        }

        // Giảm số người trong phòng
        if (contract.Room != null)
        {
            contract.Room.CurrentOccupancy = Math.Max(0, contract.Room.CurrentOccupancy - 1);
            if (contract.Room.CurrentOccupancy < contract.Room.Capacity && contract.Room.Status == RoomStatus.Occupied)
            {
                contract.Room.Status = RoomStatus.Available;
            }
        }

        // Hủy liên kết phòng của sinh viên
        if (contract.Student != null && contract.Student.CurrentRoomId == contract.RoomId)
        {
            contract.Student.CurrentRoomId = null;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Gia hạn hợp đồng đến ngày kết thúc mới
    /// </summary>
    public async Task<bool> RenewContractAsync(int id, DateTime newEndDate)
    {
        var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.Id == id);
        if (contract == null) return false;

        if (newEndDate <= contract.EndDate)
        {
            throw new ArgumentException("Ngày gia hạn mới phải sau ngày kết thúc hiện tại.");
        }

        contract.EndDate = newEndDate;
        contract.Status = ContractStatus.Active;
        await _context.SaveChangesAsync();
        return true;
    }
}
