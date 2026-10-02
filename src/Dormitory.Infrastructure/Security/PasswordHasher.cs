using Dormitory.Application.Interfaces;

namespace Dormitory.Infrastructure.Security;

/// <summary>
/// Dịch vụ mã hóa và kiểm tra mật khẩu sử dụng giải thuật BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Băm mật khẩu người dùng trước khi lưu vào cơ sở dữ liệu (Static method)
    /// </summary>
    public static string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Mật khẩu không được để trống.", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    }

    /// <summary>
    /// Xác thực mật khẩu nhập vào so với chuỗi băm trong cơ sở dữ liệu (Static method)
    /// </summary>
    public static bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Băm mật khẩu (Instance method triển khai IPasswordHasher)
    /// </summary>
    public string HashPassword(string password) => Hash(password);

    /// <summary>
    /// Xác thực mật khẩu (Instance method triển khai IPasswordHasher)
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword) => Verify(password, hashedPassword);
}
