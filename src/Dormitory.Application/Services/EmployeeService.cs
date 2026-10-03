using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Services;

/// <summary>
/// Dịch vụ xử lý nghiệp vụ quản lý hồ sơ nhân viên ký túc xá
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IDormitoryDbContext _context;

    public EmployeeService(IDormitoryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách tất cả nhân viên với bộ lọc tìm kiếm (họ tên, mã NV, CCCD, SĐT) và phòng ban
    /// </summary>
    public async Task<List<EmployeeDto>> GetAllEmployeesAsync(string? searchQuery = null, string? department = null)
    {
        var query = _context.Employees
            .Include(e => e.User)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(e => e.Department == department);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var search = searchQuery.Trim().ToLower();
            query = query.Where(e =>
                e.FullName.ToLower().Contains(search) ||
                e.EmployeeCode.ToLower().Contains(search) ||
                e.IdentityCard.Contains(search) ||
                e.PhoneNumber.Contains(search));
        }

        return await query
            .OrderBy(e => e.EmployeeCode)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FullName,
                DateOfBirth = e.DateOfBirth,
                Gender = e.Gender,
                PhoneNumber = e.PhoneNumber,
                IdentityCard = e.IdentityCard,
                Position = e.Position,
                Department = e.Department,
                Address = e.Address,
                UserId = e.UserId,
                Username = e.User != null ? e.User.Username : null
            })
            .ToListAsync();
    }

    /// <summary>
    /// Tìm thông tin nhân viên theo Id
    /// </summary>
    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var e = await _context.Employees
            .Include(emp => emp.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(emp => emp.Id == id);

        if (e == null) return null;

        return new EmployeeDto
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            FullName = e.FullName,
            DateOfBirth = e.DateOfBirth,
            Gender = e.Gender,
            PhoneNumber = e.PhoneNumber,
            IdentityCard = e.IdentityCard,
            Position = e.Position,
            Department = e.Department,
            Address = e.Address,
            UserId = e.UserId,
            Username = e.User?.Username
        };
    }

    /// <summary>
    /// Thêm nhân viên mới vào hệ thống
    /// </summary>
    public async Task<EmployeeDto> CreateEmployeeAsync(CreateOrUpdateEmployeeRequest request)
    {
        var codeExists = await _context.Employees.AnyAsync(e => e.EmployeeCode == request.EmployeeCode);
        if (codeExists)
        {
            throw new InvalidOperationException($"Mã số nhân viên {request.EmployeeCode} đã tồn tại trong hệ thống.");
        }

        var idCardExists = await _context.Employees.AnyAsync(e => e.IdentityCard == request.IdentityCard);
        if (idCardExists)
        {
            throw new InvalidOperationException($"Số CMND/CCCD {request.IdentityCard} đã được đăng ký.");
        }

        var employee = new Employee
        {
            EmployeeCode = request.EmployeeCode,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            IdentityCard = request.IdentityCard,
            Position = request.Position,
            Department = request.Department,
            Address = request.Address,
            UserId = request.UserId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return new EmployeeDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            DateOfBirth = employee.DateOfBirth,
            Gender = employee.Gender,
            PhoneNumber = employee.PhoneNumber,
            IdentityCard = employee.IdentityCard,
            Position = employee.Position,
            Department = employee.Department,
            Address = employee.Address,
            UserId = employee.UserId
        };
    }

    /// <summary>
    /// Cập nhật thông tin nhân viên theo Id
    /// </summary>
    public async Task<bool> UpdateEmployeeAsync(int id, CreateOrUpdateEmployeeRequest request)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return false;

        var codeExists = await _context.Employees.AnyAsync(e => e.EmployeeCode == request.EmployeeCode && e.Id != id);
        if (codeExists)
        {
            throw new InvalidOperationException($"Mã số nhân viên {request.EmployeeCode} đã tồn tại trong hệ thống.");
        }

        var idCardExists = await _context.Employees.AnyAsync(e => e.IdentityCard == request.IdentityCard && e.Id != id);
        if (idCardExists)
        {
            throw new InvalidOperationException($"Số CMND/CCCD {request.IdentityCard} đã được đăng ký.");
        }

        employee.EmployeeCode = request.EmployeeCode;
        employee.FullName = request.FullName;
        employee.DateOfBirth = request.DateOfBirth;
        employee.Gender = request.Gender;
        employee.PhoneNumber = request.PhoneNumber;
        employee.IdentityCard = request.IdentityCard;
        employee.Position = request.Position;
        employee.Department = request.Department;
        employee.Address = request.Address;
        employee.UserId = request.UserId;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Xóa thông tin nhân viên theo Id
    /// </summary>
    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }
}
