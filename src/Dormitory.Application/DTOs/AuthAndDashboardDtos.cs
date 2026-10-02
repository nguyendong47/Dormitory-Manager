using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO yêu cầu đăng nhập
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO kết quả đăng nhập thành công
/// </summary>
public class LoginResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public UserDto? User { get; set; }
}

/// <summary>
/// DTO thông tin tài khoản người dùng
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

/// <summary>
/// DTO số liệu tổng quan trên màn hình Dashboard
/// </summary>
public class DashboardStatsDto
{
    public int TotalRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int TotalStudents { get; set; }
    public int ActiveContractsCount { get; set; }
    public int UnpaidBillsCount { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal OccupancyRate => TotalRooms > 0 ? (decimal)OccupiedRooms / TotalRooms * 100 : 0;
}
