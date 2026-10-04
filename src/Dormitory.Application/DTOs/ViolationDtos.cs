using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chứa thông tin chi tiết biên bản vi phạm nội quy và kỷ luật KTX
/// </summary>
public class ViolationDto
{
    public int Id { get; set; }
    public string ViolationCode { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ViolationSeverity Severity { get; set; }
    public string SeverityText { get; set; } = string.Empty;
    public ViolationStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public decimal FineAmount { get; set; }
    public int DemeritPoints { get; set; }
    public DateTime ViolationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ResolutionNotes { get; set; }
    public string? RecordedBy { get; set; }
}

/// <summary>
/// DTO yêu cầu lập biên bản vi phạm KTX mới
/// </summary>
public class CreateViolationDto
{
    public string? ViolationCode { get; set; }
    public int StudentId { get; set; }
    public int RoomId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ViolationSeverity Severity { get; set; } = ViolationSeverity.Minor;
    public decimal FineAmount { get; set; }
    public int DemeritPoints { get; set; }
    public DateTime ViolationDate { get; set; } = DateTime.UtcNow;
    public string? RecordedBy { get; set; }
}

/// <summary>
/// DTO yêu cầu cập nhật thông tin biên bản vi phạm KTX
/// </summary>
public class UpdateViolationDto
{
    public int StudentId { get; set; }
    public int RoomId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ViolationSeverity Severity { get; set; } = ViolationSeverity.Minor;
    public ViolationStatus Status { get; set; } = ViolationStatus.Pending;
    public decimal FineAmount { get; set; }
    public int DemeritPoints { get; set; }
    public DateTime ViolationDate { get; set; } = DateTime.UtcNow;
    public string? ResolutionNotes { get; set; }
}

/// <summary>
/// DTO yêu cầu xử lý / giải quyết biên bản vi phạm
/// </summary>
public class ResolveViolationDto
{
    public ViolationStatus Status { get; set; } = ViolationStatus.Resolved;
    public string? ResolutionNotes { get; set; }
}
