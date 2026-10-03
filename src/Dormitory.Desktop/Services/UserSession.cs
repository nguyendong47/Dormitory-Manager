using System;
using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.Services;

/// <summary>
/// Triển khai dịch vụ quản lý phiên làm việc trong bộ nhớ (Singleton)
/// </summary>
public class UserSession : IUserSession
{
    public UserDto? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;

    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;

    public event Action? SessionChanged;

    public void SetUser(UserDto user)
    {
        CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
        SessionChanged?.Invoke();
    }

    public void ClearSession()
    {
        CurrentUser = null;
        SessionChanged?.Invoke();
    }
}
