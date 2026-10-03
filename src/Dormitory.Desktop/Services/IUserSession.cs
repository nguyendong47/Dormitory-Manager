using System;
using Dormitory.Application.DTOs;

namespace Dormitory.Desktop.Services;

/// <summary>
/// Quản lý thông tin phiên làm việc và phân quyền của người dùng hiện tại trên ứng dụng Desktop
/// </summary>
public interface IUserSession
{
    /// <summary>
    /// Thông tin tài khoản người dùng đang đăng nhập
    /// </summary>
    UserDto? CurrentUser { get; }

    /// <summary>
    /// Kiểm tra người dùng đã đăng nhập thành công hay chưa
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Kiểm tra người dùng hiện tại có vai trò Quản trị viên (Admin) hay không
    /// </summary>
    bool IsAdmin { get; }

    /// <summary>
    /// Thiết lập thông tin người dùng khi đăng nhập thành công
    /// </summary>
    void SetUser(UserDto user);

    /// <summary>
    /// Xóa thông tin phiên làm việc khi đăng xuất
    /// </summary>
    void ClearSession();

    /// <summary>
    /// Sự kiện thông báo khi trạng thái phiên thay đổi (đăng nhập hoặc đăng xuất)
    /// </summary>
    event Action? SessionChanged;
}
