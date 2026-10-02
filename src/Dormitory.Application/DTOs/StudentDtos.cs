using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO thông tin sinh viên nội trú
/// </summary>
public class StudentDto
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string IdentityCard { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string HomeTown { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentPhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int? CurrentRoomId { get; set; }
    public string? CurrentRoomNumber { get; set; }
}

/// <summary>
/// DTO yêu cầu thêm mới hoặc cập nhật sinh viên
/// </summary>
public class CreateOrUpdateStudentRequest
{
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; } = Gender.Male;
    public string IdentityCard { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string HomeTown { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentPhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
}
