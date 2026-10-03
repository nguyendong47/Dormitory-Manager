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

    /// <summary>
    /// Thống kê tỷ lệ lấp đầy phòng và giường theo từng tòa nhà
    /// </summary>
    Task<List<BuildingOccupancyDto>> GetBuildingOccupancyAsync();

    /// <summary>
    /// Thống kê xu hướng doanh thu theo N tháng gần nhất
    /// </summary>
    Task<List<MonthlyRevenueTrendDto>> GetRevenueTrendsAsync(int months = 6);
}

