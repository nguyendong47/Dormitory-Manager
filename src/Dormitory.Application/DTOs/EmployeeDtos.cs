using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO thông tin nhân viên ký túc xá
/// </summary>
public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.Male;
    public string PhoneNumber { get; set; } = string.Empty;
    public string IdentityCard { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
}

/// <summary>
/// DTO yêu cầu thêm mới hoặc cập nhật thông tin nhân viên
/// </summary>
public class CreateOrUpdateEmployeeRequest
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.Male;
    public string PhoneNumber { get; set; } = string.Empty;
    public string IdentityCard { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int? UserId { get; set; }
}
