namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chứa thông tin tổng quan về tệp tin và dữ liệu cơ sở dữ liệu SQLite
/// </summary>
public class DatabaseInfoDto
{
    /// <summary>
    /// Đường dẫn tuyệt đối tới tệp tin cơ sở dữ liệu SQLite
    /// </summary>
    public string DatabasePath { get; set; } = string.Empty;

    /// <summary>
    /// Kích thước tệp tin tính bằng bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Kích thước tệp tin đã được định dạng hiển thị (ví dụ: "128 KB", "2.4 MB")
    /// </summary>
    public string FormattedFileSize { get; set; } = string.Empty;

    /// <summary>
    /// Tổng số bản ghi trong các bảng chính (Rooms, Students, Contracts, Bills, Users, Employees)
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Thời điểm chỉnh sửa tệp tin gần nhất
    /// </summary>
    public DateTime LastModified { get; set; }
}

/// <summary>
/// Kết quả thực hiện thao tác phục hồi cơ sở dữ liệu
/// </summary>
public class DatabaseRestoreResult
{
    /// <summary>
    /// Trạng thái phục hồi thành công hay thất bại
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Thông báo lỗi chi tiết khi phục hồi thất bại
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Tạo kết quả thành công
    /// </summary>
    public static DatabaseRestoreResult Ok() => new() { Success = true };

    /// <summary>
    /// Tạo kết quả thất bại với thông điệp lỗi
    /// </summary>
    public static DatabaseRestoreResult Fail(string errorMessage) => new() { Success = false, ErrorMessage = errorMessage };
}

/// <summary>
/// Alias DTO cho kết quả phục hồi CSDL
/// </summary>
public class DatabaseRestoreResultDto : DatabaseRestoreResult
{
}
