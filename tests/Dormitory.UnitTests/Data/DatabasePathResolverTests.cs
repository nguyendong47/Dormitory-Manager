using Dormitory.Infrastructure.Data;
using FluentAssertions;
using Xunit;

namespace Dormitory.UnitTests.Data;

/// <summary>
/// Kiểm thử đơn vị cho cơ chế giải quyết đường dẫn CSDL SQLite (DatabasePathResolver)
/// </summary>
public class DatabasePathResolverTests : IDisposable
{
    private readonly string? _originalEnv;

    public DatabasePathResolverTests()
    {
        _originalEnv = Environment.GetEnvironmentVariable("DORMITORY_DB_PATH");
        Environment.SetEnvironmentVariable("DORMITORY_DB_PATH", null);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("DORMITORY_DB_PATH", _originalEnv);
    }

    [Fact]
    public void ResolveDatabasePath_WithNullOrEmpty_ShouldResolveToLocalApplicationDataPath()
    {
        // Act
        var resolvedPath = DatabasePathResolver.ResolveDatabasePath(null);

        // Assert
        resolvedPath.Should().NotBeNullOrWhiteSpace();
        Path.IsPathRooted(resolvedPath).Should().BeTrue();
        Path.GetFileName(resolvedPath).Should().Be("dormitory.db");
        resolvedPath.Should().Contain(DatabasePathResolver.AppFolderName);
    }

    [Fact]
    public void ResolveDatabasePath_WithRootedPath_ShouldPreserveSamePath()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), "custom_test_dormitory.db");

        // Act
        var resolvedPath = DatabasePathResolver.ResolveDatabasePath(tempFile);

        // Assert
        resolvedPath.Should().Be(Path.GetFullPath(tempFile));
    }

    [Fact]
    public void ResolveDatabasePath_WithCustomRelativeName_ShouldMapToAppDataFolder()
    {
        // Act
        var resolvedPath = DatabasePathResolver.ResolveDatabasePath("custom_backup.db");

        // Assert
        resolvedPath.Should().NotBeNullOrWhiteSpace();
        Path.IsPathRooted(resolvedPath).Should().BeTrue();
        Path.GetFileName(resolvedPath).Should().Be("custom_backup.db");
        resolvedPath.Should().Contain(DatabasePathResolver.AppFolderName);
    }

    [Fact]
    public void BuildConnectionString_WithRelativeDataSource_ShouldReturnConnectionStringWithAbsolutePath()
    {
        // Arrange
        var rawConnStr = "Data Source=dormitory.db";

        // Act
        var result = DatabasePathResolver.BuildConnectionString(rawConnStr);

        // Assert
        result.Should().StartWith("Data Source=");
        result.Should().Contain(DatabasePathResolver.AppFolderName);
        result.Should().Contain("dormitory.db");
        // Verify path inside is rooted
        var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(result);
        Path.IsPathRooted(builder.DataSource).Should().BeTrue();
    }

    [Fact]
    public void BuildConnectionString_WithNullOrEmpty_ShouldReturnDefaultConnectionString()
    {
        // Act
        var result = DatabasePathResolver.BuildConnectionString(null);

        // Assert
        result.Should().StartWith("Data Source=");
        result.Should().Contain(DatabasePathResolver.AppFolderName);
        result.Should().Contain("dormitory.db");
    }

    [Fact]
    public void ResolveDatabasePathFromConnectionString_WithNullOrInvalid_ShouldReturnSafeDefault()
    {
        // Act
        var pathNull = DatabasePathResolver.ResolveDatabasePathFromConnectionString(null);
        var pathEmpty = DatabasePathResolver.ResolveDatabasePathFromConnectionString("");

        // Assert
        pathNull.Should().Contain(DatabasePathResolver.AppFolderName);
        pathEmpty.Should().Contain(DatabasePathResolver.AppFolderName);
    }

    [Fact]
    public void ResolveDatabasePath_WithEnvironmentVariable_ShouldPrioritizeEnvironmentVariable()
    {
        // Arrange
        var customEnvPath = Path.Combine(Path.GetTempPath(), "env_override_dormitory.db");
        Environment.SetEnvironmentVariable("DORMITORY_DB_PATH", customEnvPath);

        try
        {
            // Act
            var resolved = DatabasePathResolver.ResolveDatabasePath("dormitory.db");

            // Assert
            resolved.Should().Be(Path.GetFullPath(customEnvPath));
        }
        finally
        {
            Environment.SetEnvironmentVariable("DORMITORY_DB_PATH", null);
        }
    }
}
