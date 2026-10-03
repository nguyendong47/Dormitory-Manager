using Dormitory.Application.DTOs;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản trị hạ tầng cơ sở dữ liệu: sao lưu, phục hồi và kiểm tra toàn vẹn CSDL
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Lấy thông tin thống kê tổng quan về tệp tin và dữ liệu CSDL hiện tại
    /// </summary>
    /// <returns>Đối tượng DatabaseInfoDto chứa đường dẫn, dung lượng và tổng số bản ghi</returns>
    Task<DatabaseInfoDto> GetDatabaseInfoAsync();

    /// <summary>
    /// Tạo bản snapshot sao lưu an toàn của cơ sở dữ liệu SQLite thành mảng byte
    /// </summary>
    /// <returns>Mảng byte dữ liệu của file backup</returns>
    Task<byte[]> BackupDatabaseAsync();

    /// <summary>
    /// Phục hồi cơ sở dữ liệu từ mảng byte đã sao lưu, có kiểm tra toàn vẹn trước khi ghi đè
    /// </summary>
    /// <param name="backupBytes">Mảng byte dữ liệu file backup CSDL</param>
    /// <returns>True nếu phục hồi thành công; False nếu file hỏng hoặc phục hồi thất bại</returns>
    Task<bool> RestoreDatabaseAsync(byte[] backupBytes);

    /// <summary>
    /// Kiểm tra tính toàn vẹn (integrity check) của tệp tin CSDL SQLite
    /// </summary>
    /// <param name="dbFilePath">Đường dẫn tệp tin CSDL cần kiểm tra</param>
    /// <returns>True nếu CSDL hợp lệ và nguyên vẹn; False nếu tệp không tồn tại hoặc bị hỏng</returns>
    Task<bool> VerifyDatabaseIntegrityAsync(string dbFilePath);
}
