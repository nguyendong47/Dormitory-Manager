using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể ghi nhận lịch sử các lượt xuất báo cáo nghiệp vụ trong hệ thống
/// </summary>
public class ReportHistory
{
    /// <summary>
    /// Khóa chính bản ghi lịch sử
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Loại báo cáo (Vi phạm, Tài chính, Lấp đầy, Thiết bị)
    /// </summary>
    public ReportType ReportType { get; set; }

    /// <summary>
    /// Tiêu đề báo cáo người dùng đặt hoặc hệ thống sinh tự động
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Định dạng tệp xuất bản (PDF hoặc Excel)
    /// </summary>
    public ReportFormat Format { get; set; }

    /// <summary>
    /// Trạng thái kết quả xuất báo cáo
    /// </summary>
    public ReportStatus Status { get; set; } = ReportStatus.Completed;

    /// <summary>
    /// Tên tệp lưu trữ trên hệ thống (Ví dụ: "BaoCao_TaiChinh_T10_2026.xlsx")
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Đường dẫn tương đối hoặc tuyệt đối tới tệp báo cáo đã lưu
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Dung lượng tệp tính bằng Bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Chuỗi JSON lưu trữ các tham số lọc áp dụng khi sinh báo cáo
    /// </summary>
    public string? ParametersJson { get; set; }

    /// <summary>
    /// Thông điệp lỗi chi tiết nếu quá trình xuất báo cáo gặp sự cố
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Tên hoặc tài khoản cán bộ thực hiện xuất báo cáo
    /// </summary>
    public string? GeneratedBy { get; set; }

    /// <summary>
    /// Thời điểm sinh báo cáo (mặc định UTC)
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
