using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Services;

/// <summary>
/// Dịch vụ xử lý đăng nhập, đổi mật khẩu và xác thực người dùng
/// </summary>
public class AuthService : IAuthService
{
    private readonly IDormitoryDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IDormitoryDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Đăng nhập hệ thống với tên đăng nhập và mật khẩu
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = "Tên đăng nhập và mật khẩu không được để trống."
            };
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = "Tài khoản hoặc mật khẩu không chính xác."
            };
        }

        if (!user.IsActive)
        {
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = "Tài khoản đã bị tạm khóa. Vui lòng liên hệ quản trị viên."
            };
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = "Tài khoản hoặc mật khẩu không chính xác."
            };
        }

        // Cập nhật thời điểm đăng nhập gần nhất
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            Success = true,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    /// <summary>
    /// Đổi mật khẩu tài khoản
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        if (!_passwordHasher.VerifyPassword(oldPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Mật khẩu hiện tại không chính xác.");
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        {
            throw new ArgumentException("Mật khẩu mới phải có tối thiểu 6 ký tự.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }
}
