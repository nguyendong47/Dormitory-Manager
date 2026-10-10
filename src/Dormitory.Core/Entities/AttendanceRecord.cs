using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể ghi nhận kết quả điểm danh chuyên cần sinh viên ban đêm theo phòng
/// </summary>
public class AttendanceRecord
{
    /// <summary>
    /// Mã định danh bản ghi điểm danh (Khóa chính)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ngày điểm danh (dạng Date yyyy-MM-dd)
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// Mã định danh sinh viên được điểm danh (Khóa ngoại)
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Mã định danh phòng ở thời điểm điểm danh (Khóa ngoại)
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Trạng thái điểm danh chuyên cần
    /// </summary>
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    /// <summary>
    /// Lý do xin phép nghỉ hoặc ghi chú cụ thể
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Tên cán bộ / quản lý thực hiện điểm danh
    /// </summary>
    public string? RecordedBy { get; set; }

    /// <summary>
    /// Thời điểm tạo bản ghi điểm danh
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thông tin sinh viên
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Thông tin phòng ở
    /// </summary>
    public Room? Room { get; set; }
}
