using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ xác thực người dùng và phân quyền
/// </summary>
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
}

/// <summary>
/// Giao diện dịch vụ thống kê và tổng hợp số liệu Dashboard
/// </summary>
public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
