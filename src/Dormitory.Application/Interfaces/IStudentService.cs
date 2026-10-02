using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý hồ sơ sinh viên
/// </summary>
public interface IStudentService
{
    Task<List<StudentDto>> GetAllStudentsAsync(string? searchQuery = null, int? roomId = null);
    Task<StudentDto?> GetStudentByIdAsync(int id);
    Task<StudentDto?> GetStudentByCodeAsync(string code);
    Task<StudentDto> CreateStudentAsync(CreateOrUpdateStudentRequest request);
    Task<bool> UpdateStudentAsync(int id, CreateOrUpdateStudentRequest request);
    Task<bool> DeleteStudentAsync(int id);
}
