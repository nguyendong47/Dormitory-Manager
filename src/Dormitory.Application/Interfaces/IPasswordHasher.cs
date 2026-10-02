namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện băm và kiểm tra mật khẩu
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
