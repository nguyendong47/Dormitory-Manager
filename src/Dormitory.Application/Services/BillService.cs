using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Services;

/// <summary>
/// Dịch vụ tính toán và quản lý hóa đơn tiền phòng, điện nước
/// </summary>
public class BillService : IBillService
{
    private readonly IDormitoryDbContext _context;

    public BillService(IDormitoryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách hóa đơn theo phòng, tháng, năm và trạng thái thanh toán
    /// </summary>
    public async Task<List<BillDto>> GetAllBillsAsync(int? roomId = null, int? month = null, int? year = null, BillStatus? status = null)
    {
        var query = _context.Bills
            .Include(b => b.Room)
            .AsNoTracking()
            .AsQueryable();

        if (roomId.HasValue)
        {
            query = query.Where(b => b.RoomId == roomId.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(b => b.Month == month.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(b => b.Year == year.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        return await query
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .Select(b => new BillDto
            {
                Id = b.Id,
                BillCode = b.BillCode,
                RoomId = b.RoomId,
                RoomNumber = b.Room != null ? b.Room.RoomNumber : string.Empty,
                Building = b.Room != null ? b.Room.Building : string.Empty,
                Month = b.Month,
                Year = b.Year,
                RoomFee = b.RoomFee,
                OldElectricIndex = b.OldElectricIndex,
                NewElectricIndex = b.NewElectricIndex,
                ElectricRate = b.ElectricRate,
                OldWaterIndex = b.OldWaterIndex,
                NewWaterIndex = b.NewWaterIndex,
                WaterRate = b.WaterRate,
                OtherServiceFee = b.OtherServiceFee,
                Status = b.Status,
                DueDate = b.DueDate,
                PaidDate = b.PaidDate,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy chi tiết hóa đơn theo Id
    /// </summary>
    public async Task<BillDto?> GetBillByIdAsync(int id)
    {
        var b = await _context.Bills
            .Include(bill => bill.Room)
            .AsNoTracking()
            .FirstOrDefaultAsync(bill => bill.Id == id);

        if (b == null) return null;

        return new BillDto
        {
            Id = b.Id,
            BillCode = b.BillCode,
            RoomId = b.RoomId,
            RoomNumber = b.Room?.RoomNumber ?? string.Empty,
            Building = b.Room?.Building ?? string.Empty,
            Month = b.Month,
            Year = b.Year,
            RoomFee = b.RoomFee,
            OldElectricIndex = b.OldElectricIndex,
            NewElectricIndex = b.NewElectricIndex,
            ElectricRate = b.ElectricRate,
            OldWaterIndex = b.OldWaterIndex,
            NewWaterIndex = b.NewWaterIndex,
            WaterRate = b.WaterRate,
            OtherServiceFee = b.OtherServiceFee,
            Status = b.Status,
            DueDate = b.DueDate,
            PaidDate = b.PaidDate,
            CreatedAt = b.CreatedAt
        };
    }

    /// <summary>
    /// Lập hóa đơn mới cho phòng
    /// </summary>
    public async Task<BillDto> CreateBillAsync(CreateBillRequest request)
    {
        if (request.NewElectricIndex < request.OldElectricIndex)
        {
            throw new ArgumentException("Chỉ số điện mới không được nhỏ hơn chỉ số cũ.");
        }

        if (request.NewWaterIndex < request.OldWaterIndex)
        {
            throw new ArgumentException("Chỉ số nước mới không được nhỏ hơn chỉ số cũ.");
        }

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy phòng có ID {request.RoomId}.");
        }

        // Kiểm tra xem đã lập hóa đơn tháng này cho phòng chưa
        var exists = await _context.Bills.AnyAsync(b => b.RoomId == request.RoomId && b.Month == request.Month && b.Year == request.Year);
        if (exists)
        {
            throw new InvalidOperationException($"Phòng {room.RoomNumber} đã được lập hóa đơn cho tháng {request.Month}/{request.Year}.");
        }

        var billCode = $"HD-{request.Year}{request.Month:D2}-{room.RoomNumber}";

        var bill = new Bill
        {
            BillCode = billCode,
            RoomId = room.Id,
            Month = request.Month,
            Year = request.Year,
            RoomFee = request.RoomFee > 0 ? request.RoomFee : room.PricePerMonth,
            OldElectricIndex = request.OldElectricIndex,
            NewElectricIndex = request.NewElectricIndex,
            ElectricRate = request.ElectricRate,
            OldWaterIndex = request.OldWaterIndex,
            NewWaterIndex = request.NewWaterIndex,
            WaterRate = request.WaterRate,
            OtherServiceFee = request.OtherServiceFee,
            Status = BillStatus.Unpaid,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bills.Add(bill);
        await _context.SaveChangesAsync();

        return new BillDto
        {
            Id = bill.Id,
            BillCode = bill.BillCode,
            RoomId = room.Id,
            RoomNumber = room.RoomNumber,
            Building = room.Building,
            Month = bill.Month,
            Year = bill.Year,
            RoomFee = bill.RoomFee,
            OldElectricIndex = bill.OldElectricIndex,
            NewElectricIndex = bill.NewElectricIndex,
            ElectricRate = bill.ElectricRate,
            OldWaterIndex = bill.OldWaterIndex,
            NewWaterIndex = bill.NewWaterIndex,
            WaterRate = bill.WaterRate,
            OtherServiceFee = bill.OtherServiceFee,
            Status = bill.Status,
            DueDate = bill.DueDate,
            CreatedAt = bill.CreatedAt
        };
    }

    /// <summary>
    /// Đánh dấu hóa đơn đã được thanh toán
    /// </summary>
    public async Task<bool> MarkAsPaidAsync(int id)
    {
        var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == id);
        if (bill == null) return false;

        bill.Status = BillStatus.Paid;
        bill.PaidDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Xóa hóa đơn
    /// </summary>
    public async Task<bool> DeleteBillAsync(int id)
    {
        var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == id);
        if (bill == null) return false;

        _context.Bills.Remove(bill);
        await _context.SaveChangesAsync();
        return true;
    }
}
