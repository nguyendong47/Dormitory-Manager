using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện định nghĩa các nghiệp vụ quản lý hồ sơ nhân viên ký túc xá
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Lấy danh sách nhân viên với tùy chọn lọc theo từ khóa tìm kiếm và phòng ban
    /// </summary>
    Task<List<EmployeeDto>> GetAllEmployeesAsync(string? searchQuery = null, string? department = null);

    /// <summary>
    /// Lấy thông tin chi tiết nhân viên theo Id
    /// </summary>
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);

    /// <summary>
    /// Thêm mới một nhân viên vào hệ thống
    /// </summary>
    Task<EmployeeDto> CreateEmployeeAsync(CreateOrUpdateEmployeeRequest request);

    /// <summary>
    /// Cập nhật thông tin nhân viên theo Id
    /// </summary>
    Task<bool> UpdateEmployeeAsync(int id, CreateOrUpdateEmployeeRequest request);

    /// <summary>
    /// Xóa nhân viên khỏi hệ thống theo Id
    /// </summary>
    Task<bool> DeleteEmployeeAsync(int id);
}
