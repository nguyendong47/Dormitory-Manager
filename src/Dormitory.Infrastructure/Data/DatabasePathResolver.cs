using Microsoft.Data.Sqlite;

namespace Dormitory.Infrastructure.Data;

/// <summary>
/// Trợ giúp giải quyết đường dẫn tệp tin cơ sở dữ liệu SQLite cho các môi trường chạy khác nhau (Desktop, Dev, Test).
/// Đảm bảo trên các nền tảng máy tính (macOS App Bundle, DMG, Windows, Linux), SQLite database luôn được đặt tại
/// thư mục dữ liệu người dùng có đầy đủ quyền đọc/ghi (LocalApplicationData), tránh lỗi SQLite Error 14 (unable to open database file).
/// </summary>
public static class DatabasePathResolver
{
    public const string AppFolderName = "DormitoryManager";
    public const string DefaultDbFileName = "dormitory.db";

    /// <summary>
    /// Giải quyết đường dẫn tuyệt đối cho tệp tin CSDL SQLite.
    /// - Nếu đường dẫn đã là đường dẫn tuyệt đối (rooted): giữ nguyên và đảm bảo thư mục cha tồn tại.
    /// - Nếu là đường dẫn tương đối (ví dụ "dormitory.db"): ánh xạ vào thư mục LocalApplicationData của hệ điều hành.
    /// </summary>
    /// <param name="rawPathOrFileName">Đường dẫn tương đối, tên file hoặc đường dẫn tuyệt đối</param>
    /// <returns>Đường dẫn tuyệt đối hợp lệ và an toàn cho SQLite</returns>
    public static string ResolveDatabasePath(string? rawPathOrFileName = null)
    {
        // 1. Kiểm tra biến môi trường ghi đè đường dẫn (dành cho kiểm thử hoặc DevOps cấu hình)
        var envOverride = Environment.GetEnvironmentVariable("DORMITORY_DB_PATH");
        if (!string.IsNullOrWhiteSpace(envOverride))
        {
            var envDir = Path.GetDirectoryName(envOverride);
            if (!string.IsNullOrEmpty(envDir) && !Directory.Exists(envDir))
            {
                Directory.CreateDirectory(envDir);
            }
            return Path.GetFullPath(envOverride);
        }

        var fileName = string.IsNullOrWhiteSpace(rawPathOrFileName)
            ? DefaultDbFileName
            : rawPathOrFileName.Trim();

        // 2. Nếu đã là đường dẫn tuyệt đối (ví dụ unit test cấp đường dẫn tạm hoặc cấu hình chỉ định thư mục cụ thể)
        if (Path.IsPathRooted(fileName))
        {
            var parentDir = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
            {
                Directory.CreateDirectory(parentDir);
            }
            return Path.GetFullPath(fileName);
        }

        // 3. Đường dẫn tương đối: Ánh xạ vào thư mục dữ liệu ứng dụng của người dùng (LocalApplicationData)
        // macOS: ~/Library/Application Support/DormitoryManager/dormitory.db
        // Windows: %LOCALAPPDATA%\DormitoryManager\dormitory.db
        // Linux: ~/.local/share/DormitoryManager/dormitory.db
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(appDataFolder))
        {
            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        var targetDirectory = Path.Combine(appDataFolder, AppFolderName);
        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        var targetDbPath = Path.Combine(targetDirectory, Path.GetFileName(fileName));

        // 4. Nếu database chưa tồn tại trong thư mục người dùng, kiểm tra xem có database mẫu kèm theo ứng dụng không
        if (!File.Exists(targetDbPath))
        {
            try
            {
                var bundledDbPath = Path.Combine(AppContext.BaseDirectory, Path.GetFileName(fileName));
                if (File.Exists(bundledDbPath) && !string.Equals(bundledDbPath, targetDbPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(bundledDbPath, targetDbPath, overwrite: false);
                }
            }
            catch
            {
                // Bỏ qua lỗi copy nếu có (để EF Core tự động tạo và seed dữ liệu mới)
            }
        }

        return targetDbPath;
    }

    /// <summary>
    /// Trích xuất và giải quyết đường dẫn file SQLite từ chuỗi kết nối bất kỳ.
    /// </summary>
    /// <param name="connectionString">Chuỗi kết nối SQLite (ví dụ: "Data Source=dormitory.db")</param>
    /// <returns>Đường dẫn tuyệt đối tới tệp tin CSDL SQLite</returns>
    public static string ResolveDatabasePathFromConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return ResolveDatabasePath(DefaultDbFileName);
        }

        try
        {
            var builder = new SqliteConnectionStringBuilder(connectionString);
            if (!string.IsNullOrWhiteSpace(builder.DataSource))
            {
                return ResolveDatabasePath(builder.DataSource);
            }
        }
        catch
        {
            // Bỏ qua lỗi phân tích cú pháp connection string không chuẩn
        }

        return ResolveDatabasePath(DefaultDbFileName);
    }

    /// <summary>
    /// Chuẩn hóa chuỗi kết nối SQLite đảm bảo Data Source luôn trỏ tới đường dẫn tuyệt đối an toàn.
    /// </summary>
    /// <param name="rawConnectionString">Chuỗi kết nối ban đầu từ appsettings.json hoặc fallback</param>
    /// <returns>Chuỗi kết nối SQLite hoàn chỉnh với DataSource tuyệt đối</returns>
    public static string BuildConnectionString(string? rawConnectionString = null)
    {
        if (string.IsNullOrWhiteSpace(rawConnectionString))
        {
            return $"Data Source={ResolveDatabasePath(DefaultDbFileName)}";
        }

        try
        {
            var builder = new SqliteConnectionStringBuilder(rawConnectionString);
            builder.DataSource = ResolveDatabasePath(builder.DataSource);
            return builder.ToString();
        }
        catch
        {
            return $"Data Source={ResolveDatabasePath(DefaultDbFileName)}";
        }
    }
}
