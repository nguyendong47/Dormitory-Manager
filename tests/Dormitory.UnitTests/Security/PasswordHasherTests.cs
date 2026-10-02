using Dormitory.Infrastructure.Security;
using FluentAssertions;
using Xunit;

namespace Dormitory.UnitTests.Security;

/// <summary>
/// Kiểm thử tự động cho tính năng mã hóa và xác minh mật khẩu BCrypt
/// </summary>
public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_ShouldReturnValidBcryptHash()
    {
        // Sắp đặt
        var plainPassword = "MySecurePassword@123";

        // Thực hiện
        var hash = _hasher.HashPassword(plainPassword);

        // Kiểm tra
        hash.Should().NotBeNullOrWhiteSpace();
        hash.Should().StartWith("$2"); // Tiền tố tiêu chuẩn của định dạng hash BCrypt
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Sắp đặt
        var plainPassword = "SecretPassword@2024";
        var hash = _hasher.HashPassword(plainPassword);

        // Thực hiện
        var isValid = _hasher.VerifyPassword(plainPassword, hash);

        // Kiểm tra
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        // Sắp đặt
        var plainPassword = "SecretPassword@2024";
        var wrongPassword = "WrongPassword@2024";
        var hash = _hasher.HashPassword(plainPassword);

        // Thực hiện
        var isValid = _hasher.VerifyPassword(wrongPassword, hash);

        // Kiểm tra
        isValid.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_WithEmptyString_ShouldThrowArgumentException()
    {
        // Thực hiện & Kiểm tra
        var act = () => _hasher.HashPassword("");
        act.Should().Throw<ArgumentException>();
    }
}
