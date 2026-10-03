using System.Globalization;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ hạ tầng quản trị cơ sở dữ liệu SQLite: sao lưu (backup), phục hồi (restore), kiểm tra toàn vẹn và thông tin CSDL
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly IDormitoryDbContext _context;
    private readonly IConfiguration? _configuration;
    private readonly string? _customDbPath;

    public DatabaseService(
        IDormitoryDbContext context,
        IConfiguration? configuration = null,
        string? customDbPath = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration;
        _customDbPath = customDbPath;
    }

    /// <summary>
    /// Hàm khởi tạo hỗ trợ truyền trực tiếp đường dẫn file CSDL vật lý (thường dùng trong Unit Test)
    /// </summary>
    public DatabaseService(IDormitoryDbContext context, string customDbPath)
        : this(context, null, customDbPath)
    {
    }

    /// <summary>
    /// Lấy đường dẫn tệp tin CSDL SQLite vật lý thực tế trên đĩa cứng
    /// </summary>
    public string GetDatabaseFilePath()
    {
        // 1. Ưu tiên đường dẫn tùy chỉnh được cấu hình trực tiếp
        if (!string.IsNullOrWhiteSpace(_customDbPath))
        {
            return Path.GetFullPath(_customDbPath);
        }

        // 2. Lấy chuỗi kết nối từ IConfiguration nếu có
        string? connStr = _configuration?.GetConnectionString("DefaultConnection")
            ?? _configuration?["ConnectionStrings:DefaultConnection"]
            ?? _configuration?["Database:ConnectionString"];

        // 3. Nếu chưa có, thử lấy từ DbContext
        if (string.IsNullOrWhiteSpace(connStr) && _context is DbContext dbContext)
        {
            connStr = dbContext.Database.GetConnectionString();
        }

        // 4. Giá trị mặc định nếu không có cấu hình
        if (string.IsNullOrWhiteSpace(connStr))
        {
            connStr = "Data Source=dormitory.db";
        }

        // Trích xuất DataSource từ connection string của SQLite
        try
        {
            var builder = new SqliteConnectionStringBuilder(connStr);
            if (!string.IsNullOrWhiteSpace(builder.DataSource))
            {
                return Path.GetFullPath(builder.DataSource);
            }
        }
        catch
        {
            // Bỏ qua lỗi cú pháp và fallback về dormitory.db
        }

        return Path.GetFullPath("dormitory.db");
    }

    /// <summary>
    /// Kiểm tra tính toàn vẹn (integrity check) của tệp tin CSDL SQLite
    /// </summary>
    public async Task<bool> VerifyDatabaseIntegrityAsync(string dbFilePath)
    {
        if (string.IsNullOrWhiteSpace(dbFilePath) || !File.Exists(dbFilePath))
        {
            return false;
        }

        try
        {
            var connStr = new SqliteConnectionStringBuilder
            {
                DataSource = dbFilePath,
                Mode = SqliteOpenMode.ReadOnly
            }.ToString();

            using var connection = new SqliteConnection(connStr);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA integrity_check;";
            var result = await command.ExecuteScalarAsync();

            return result != null && string.Equals(result.ToString()?.Trim(), "ok", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Tạo bản snapshot sao lưu an toàn của cơ sở dữ liệu SQLite thành mảng byte
    /// </summary>
    public async Task<byte[]> BackupDatabaseAsync()
    {
        var dbPath = GetDatabaseFilePath();

        // Tạo đường dẫn file snapshot tạm thời
        var tempSnapshotPath = Path.Combine(Path.GetTempPath(), $"dormitory_backup_{Guid.NewGuid():N}.db");

        try
        {
            // Nếu file DB vật lý tồn tại, ưu tiên dùng lệnh VACUUM INTO để tạo snapshot online nhất quán
            if (File.Exists(dbPath))
            {
                using (var connection = new SqliteConnection($"Data Source={dbPath};"))
                {
                    await connection.OpenAsync();

                    // Đồng bộ WAL log vào file chính trước khi snapshot
                    using (var walCmd = connection.CreateCommand())
                    {
                        walCmd.CommandText = "PRAGMA wal_checkpoint(TRUNCATE);";
                        try { await walCmd.ExecuteNonQueryAsync(); } catch { }
                    }

                    using var vacuumCmd = connection.CreateCommand();
                    var escapedPath = tempSnapshotPath.Replace("'", "''");
                    vacuumCmd.CommandText = $"VACUUM INTO '{escapedPath}';";
                    await vacuumCmd.ExecuteNonQueryAsync();
                }

                if (File.Exists(tempSnapshotPath))
                {
                    return await File.ReadAllBytesAsync(tempSnapshotPath);
                }
            }

            // Fallback: nếu VACUUM INTO không khả dụng hoặc file snapshot chưa được tạo, đọc file trực tiếp an toàn
            if (File.Exists(dbPath))
            {
                using var fs = new FileStream(dbPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var ms = new MemoryStream();
                await fs.CopyToAsync(ms);
                return ms.ToArray();
            }

            throw new FileNotFoundException($"Không tìm thấy tệp tin cơ sở dữ liệu tại '{dbPath}' để sao lưu.");
        }
        finally
        {
            // Dọn dẹp tệp snapshot tạm thời
            if (File.Exists(tempSnapshotPath))
            {
                try { File.Delete(tempSnapshotPath); } catch { }
            }
        }
    }

    /// <summary>
    /// Phục hồi cơ sở dữ liệu từ mảng byte đã sao lưu, có kiểm tra toàn vẹn trước khi ghi đè
    /// </summary>
    public async Task<bool> RestoreDatabaseAsync(byte[] backupBytes)
    {
        if (backupBytes == null || backupBytes.Length == 0)
        {
            return false;
        }

        var tempRestorePath = Path.Combine(Path.GetTempPath(), $"dormitory_restore_{Guid.NewGuid():N}.db");

        try
        {
            // 1. Ghi dữ liệu sao lưu ra file tạm
            await File.WriteAllBytesAsync(tempRestorePath, backupBytes);

            // 2. Kiểm tra tính toàn vẹn của file tạm trước khi phục hồi
            bool isIntegrityOk = await VerifyDatabaseIntegrityAsync(tempRestorePath);
            if (!isIntegrityOk)
            {
                return false;
            }

            // 3. Giải phóng mọi kết nối mở trong pool tới CSDL SQLite
            if (_context is DbContext dbContext)
            {
                try { dbContext.Database.CloseConnection(); } catch { }
            }
            SqliteConnection.ClearAllPools();

            var dbPath = GetDatabaseFilePath();

            // 4. Tạm thời sao lưu tệp CSDL hiện tại thành file .bak phòng hờ rủi ro
            var backupFilePath = $"{dbPath}.bak";
            if (File.Exists(dbPath))
            {
                File.Copy(dbPath, backupFilePath, overwrite: true);
            }

            // 5. Ghi đè file phục hồi vào vị trí CSDL chính
            var targetDir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            File.Copy(tempRestorePath, dbPath, overwrite: true);

            // 6. Xóa các tệp nhật ký WAL cũ nếu tồn tại để tránh xung đột dữ liệu
            var walFile = $"{dbPath}-wal";
            if (File.Exists(walFile))
            {
                try { File.Delete(walFile); } catch { }
            }

            var shmFile = $"{dbPath}-shm";
            if (File.Exists(shmFile))
            {
                try { File.Delete(shmFile); } catch { }
            }

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            // Dọn dẹp file tạm
            if (File.Exists(tempRestorePath))
            {
                try { File.Delete(tempRestorePath); } catch { }
            }
        }
    }

    /// <summary>
    /// Lấy thông tin thống kê tổng quan về tệp tin và dữ liệu CSDL hiện tại
    /// </summary>
    public async Task<DatabaseInfoDto> GetDatabaseInfoAsync()
    {
        var dbPath = GetDatabaseFilePath();
        var fileInfo = new FileInfo(dbPath);

        long fileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0;
        DateTime lastModified = fileInfo.Exists ? fileInfo.LastWriteTime : DateTime.MinValue;
        string formattedSize = FormatFileSize(fileSizeBytes);

        // Đếm tổng số bản ghi trong các bảng thực thể chính
        int totalRecords = await _context.Rooms.CountAsync()
            + await _context.Students.CountAsync()
            + await _context.Contracts.CountAsync()
            + await _context.Bills.CountAsync()
            + await _context.Users.CountAsync()
            + await _context.Employees.CountAsync();

        return new DatabaseInfoDto
        {
            DatabasePath = dbPath,
            FileSizeBytes = fileSizeBytes,
            FormattedFileSize = formattedSize,
            TotalRecords = totalRecords,
            LastModified = lastModified
        };
    }

    /// <summary>
    /// Định dạng kích thước bytes thành chuỗi dễ đọc (ví dụ: "128 KB", "2.4 MB")
    /// </summary>
    public static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        if (bytes < 1024 * 1024)
            return $"{(bytes / 1024.0).ToString("0.#", CultureInfo.InvariantCulture)} KB";
        if (bytes < 1024L * 1024L * 1024L)
            return $"{(bytes / (1024.0 * 1024.0)).ToString("0.#", CultureInfo.InvariantCulture)} MB";
        return $"{(bytes / (1024.0 * 1024.0 * 1024.0)).ToString("0.#", CultureInfo.InvariantCulture)} GB";
    }
}
