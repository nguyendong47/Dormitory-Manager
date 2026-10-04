using Dormitory.Core.Enums;

namespace Dormitory.Core.Entities;

/// <summary>
/// Thực thể đại diện cho biên bản vi phạm nội quy và kỷ luật Ký túc xá
/// </summary>
public class Violation
{
    /// <summary>
    /// Mã định danh biên bản vi phạm (Khóa chính)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Mã biên bản vi phạm (Ví dụ: "VP001", "VP-20261004-001")
    /// </summary>
    public string ViolationCode { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh sinh viên vi phạm (Khóa ngoại)
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Đối tượng sinh viên vi phạm (Navigation property)
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Mã định danh phòng xảy ra vi phạm (Khóa ngoại)
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Đối tượng phòng xảy ra vi phạm (Navigation property)
    /// </summary>
    public Room? Room { get; set; }

    /// <summary>
    /// Tiêu đề vi phạm (Ví dụ: "Sử dụng bếp điện nấu ăn trong phòng")
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả chi tiết hành vi và tình tiết vi phạm
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Mức độ nghiêm trọng của vi phạm
    /// </summary>
    public ViolationSeverity Severity { get; set; } = ViolationSeverity.Minor;

    /// <summary>
    /// Trạng thái giải quyết biên bản
    /// </summary>
    public ViolationStatus Status { get; set; } = ViolationStatus.Pending;

    /// <summary>
    /// Số tiền xử phạt tài chính (VND)
    /// </summary>
    public decimal FineAmount { get; set; }

    /// <summary>
    /// Số điểm rèn luyện KTX bị trừ
    /// </summary>
    public int DemeritPoints { get; set; }

    /// <summary>
    /// Thời điểm xảy ra vi phạm
    /// </summary>
    public DateTime ViolationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm lập biên bản trên hệ thống
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ghi chú giải quyết, khắc phục hoặc quyết định kỷ luật của BQL
    /// </summary>
    public string? ResolutionNotes { get; set; }

    /// <summary>
    /// Tên hoặc tài khoản cán bộ lập biên bản
    /// </summary>
    public string? RecordedBy { get; set; }
}
