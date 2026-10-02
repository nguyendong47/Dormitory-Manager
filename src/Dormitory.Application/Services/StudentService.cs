using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Services;

/// <summary>
/// Dịch vụ xử lý nghiệp vụ quản lý hồ sơ sinh viên
/// </summary>
public class StudentService : IStudentService
{
    private readonly IDormitoryDbContext _context;

    public StudentService(IDormitoryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách tất cả sinh viên với từ khóa tìm kiếm (họ tên, mã SV, CCCD) hoặc lọc theo phòng
    /// </summary>
    public async Task<List<StudentDto>> GetAllStudentsAsync(string? searchQuery = null, int? roomId = null)
    {
        var query = _context.Students
            .Include(s => s.CurrentRoom)
            .AsNoTracking()
            .AsQueryable();

        if (roomId.HasValue)
        {
            query = query.Where(s => s.CurrentRoomId == roomId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var search = searchQuery.Trim().ToLower();
            query = query.Where(s =>
                s.FullName.ToLower().Contains(search) ||
                s.StudentCode.ToLower().Contains(search) ||
                s.IdentityCard.Contains(search) ||
                s.ClassName.ToLower().Contains(search) ||
                s.HomeTown.ToLower().Contains(search));
        }

        return await query
            .OrderBy(s => s.FullName)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                StudentCode = s.StudentCode,
                FullName = s.FullName,
                DateOfBirth = s.DateOfBirth,
                Gender = s.Gender,
                IdentityCard = s.IdentityCard,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                HomeTown = s.HomeTown,
                ClassName = s.ClassName,
                Faculty = s.Faculty,
                ParentName = s.ParentName,
                ParentPhoneNumber = s.ParentPhoneNumber,
                Address = s.Address,
                CurrentRoomId = s.CurrentRoomId,
                CurrentRoomNumber = s.CurrentRoom != null ? s.CurrentRoom.RoomNumber : null
            })
            .ToListAsync();
    }

    /// <summary>
    /// Tìm sinh viên theo Id
    /// </summary>
    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var s = await _context.Students
            .Include(st => st.CurrentRoom)
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.Id == id);

        if (s == null) return null;

        return new StudentDto
        {
            Id = s.Id,
            StudentCode = s.StudentCode,
            FullName = s.FullName,
            DateOfBirth = s.DateOfBirth,
            Gender = s.Gender,
            IdentityCard = s.IdentityCard,
            PhoneNumber = s.PhoneNumber,
            Email = s.Email,
            HomeTown = s.HomeTown,
            ClassName = s.ClassName,
            Faculty = s.Faculty,
            ParentName = s.ParentName,
            ParentPhoneNumber = s.ParentPhoneNumber,
            Address = s.Address,
            CurrentRoomId = s.CurrentRoomId,
            CurrentRoomNumber = s.CurrentRoom?.RoomNumber
        };
    }

    /// <summary>
    /// Tìm sinh viên theo mã số sinh viên
    /// </summary>
    public async Task<StudentDto?> GetStudentByCodeAsync(string code)
    {
        var s = await _context.Students
            .Include(st => st.CurrentRoom)
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.StudentCode == code);

        if (s == null) return null;

        return new StudentDto
        {
            Id = s.Id,
            StudentCode = s.StudentCode,
            FullName = s.FullName,
            DateOfBirth = s.DateOfBirth,
            Gender = s.Gender,
            IdentityCard = s.IdentityCard,
            PhoneNumber = s.PhoneNumber,
            Email = s.Email,
            HomeTown = s.HomeTown,
            ClassName = s.ClassName,
            Faculty = s.Faculty,
            ParentName = s.ParentName,
            ParentPhoneNumber = s.ParentPhoneNumber,
            Address = s.Address,
            CurrentRoomId = s.CurrentRoomId,
            CurrentRoomNumber = s.CurrentRoom?.RoomNumber
        };
    }

    /// <summary>
    /// Thêm sinh viên mới
    /// </summary>
    public async Task<StudentDto> CreateStudentAsync(CreateOrUpdateStudentRequest request)
    {
        var codeExists = await _context.Students.AnyAsync(s => s.StudentCode == request.StudentCode);
        if (codeExists)
        {
            throw new InvalidOperationException($"Mã số sinh viên {request.StudentCode} đã tồn tại trong hệ thống.");
        }

        var idCardExists = await _context.Students.AnyAsync(s => s.IdentityCard == request.IdentityCard);
        if (idCardExists)
        {
            throw new InvalidOperationException($"Số CMND/CCCD {request.IdentityCard} đã được đăng ký.");
        }

        var student = new Student
        {
            StudentCode = request.StudentCode,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            IdentityCard = request.IdentityCard,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            HomeTown = request.HomeTown,
            ClassName = request.ClassName,
            Faculty = request.Faculty,
            ParentName = request.ParentName,
            ParentPhoneNumber = request.ParentPhoneNumber,
            Address = request.Address
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return new StudentDto
        {
            Id = student.Id,
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            IdentityCard = student.IdentityCard,
            PhoneNumber = student.PhoneNumber,
            Email = student.Email,
            HomeTown = student.HomeTown,
            ClassName = student.ClassName,
            Faculty = student.Faculty,
            ParentName = student.ParentName,
            ParentPhoneNumber = student.ParentPhoneNumber,
            Address = student.Address
        };
    }

    /// <summary>
    /// Cập nhật thông tin sinh viên
    /// </summary>
    public async Task<bool> UpdateStudentAsync(int id, CreateOrUpdateStudentRequest request)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return false;

        student.FullName = request.FullName;
        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.PhoneNumber = request.PhoneNumber;
        student.Email = request.Email;
        student.HomeTown = request.HomeTown;
        student.ClassName = request.ClassName;
        student.Faculty = request.Faculty;
        student.ParentName = request.ParentName;
        student.ParentPhoneNumber = request.ParentPhoneNumber;
        student.Address = request.Address;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Xóa hồ sơ sinh viên (chỉ được xóa nếu không có hợp đồng đang active)
    /// </summary>
    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return false;

        if (student.CurrentRoomId.HasValue)
        {
            throw new InvalidOperationException("Không thể xóa sinh viên đang ở trong phòng KTX. Cần thanh lý hợp đồng trước.");
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}
