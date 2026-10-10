using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chứa thông tin chi tiết bản ghi điểm danh chuyên cần sinh viên ban đêm
/// </summary>
public class AttendanceRecordDto
{
    /// <summary>
    /// Mã định danh bản ghi điểm danh
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ngày điểm danh (Date yyyy-MM-dd)
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// Mã định danh sinh viên
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// Họ và tên sinh viên
    /// </summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>
    /// Mã số sinh viên
    /// </summary>
    public string StudentCode { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh phòng ở
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Số phòng ký túc xá
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái điểm danh
    /// </summary>
    public AttendanceStatus Status { get; set; }

    /// <summary>
    /// Lý do xin phép hoặc ghi chú cụ thể
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Người thực hiện điểm danh
    /// </summary>
    public string? RecordedBy { get; set; }

    /// <summary>
    /// Thời điểm tạo bản ghi điểm danh
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Nhãn hiển thị tiếng Việt của trạng thái điểm danh
    /// </summary>
    public string StatusText => Status switch
    {
        AttendanceStatus.Present => "Có mặt",
        AttendanceStatus.AbsentWithoutPermission => "Vắng không phép",
        AttendanceStatus.AbsentWithPermission => "Vắng có phép",
        AttendanceStatus.Late => "Về muộn",
        _ => "Không rõ"
    };

    /// <summary>
    /// Màu chữ badge trạng thái
    /// </summary>
    public string StatusBadgeColor => Status switch
    {
        AttendanceStatus.Present => "#107C41",
        AttendanceStatus.AbsentWithoutPermission => "#A80000",
        AttendanceStatus.AbsentWithPermission => "#0078D4",
        AttendanceStatus.Late => "#C67D00",
        _ => "#605E5C"
    };

    /// <summary>
    /// Màu nền badge trạng thái
    /// </summary>
    public string StatusBadgeBackground => Status switch
    {
        AttendanceStatus.Present => "#DFF6DD",
        AttendanceStatus.AbsentWithoutPermission => "#FDE7E9",
        AttendanceStatus.AbsentWithPermission => "#DEECF9",
        AttendanceStatus.Late => "#FFF4CE",
        _ => "#F3F2F1"
    };
}

/// <summary>
/// DTO ghi nhận kết quả điểm danh theo lô cho toàn bộ sinh viên trong phòng
/// </summary>
public class BatchRoomAttendanceDto
{
    /// <summary>
    /// Mã định danh phòng điểm danh
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Ngày điểm danh
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// Tên cán bộ / quản lý thực hiện điểm danh
    /// </summary>
    public string? RecordedBy { get; set; }

    /// <summary>
    /// Danh sách kết quả điểm danh của từng sinh viên trong phòng
    /// </summary>
    public List<AttendanceRecordDto> StudentAttendances { get; set; } = new();
}

/// <summary>
/// DTO bộ lọc tìm kiếm và truy vấn dữ liệu điểm danh
/// </summary>
public class AttendanceFilterDto
{
    /// <summary>
    /// Lọc theo một ngày điểm danh cụ thể
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Lọc từ ngày
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Lọc đến ngày
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Lọc theo phòng cụ thể
    /// </summary>
    public int? RoomId { get; set; }

    /// <summary>
    /// Lọc theo trạng thái điểm danh
    /// </summary>
    public AttendanceStatus? Status { get; set; }

    /// <summary>
    /// Từ khóa tìm kiếm (tên sinh viên, MSSV, số phòng)
    /// </summary>
    public string? SearchKeyword { get; set; }
}

/// <summary>
/// DTO thống kê kết quả điểm danh chuyên cần
/// </summary>
public class AttendanceStatsDto
{
    /// <summary>
    /// Tổng số sinh viên cần điểm danh
    /// </summary>
    public int TotalStudents { get; set; }

    /// <summary>
    /// Số lượng sinh viên có mặt
    /// </summary>
    public int PresentCount { get; set; }

    /// <summary>
    /// Số lượng sinh viên vắng không phép
    /// </summary>
    public int AbsentWithoutPermissionCount { get; set; }

    /// <summary>
    /// Số lượng sinh viên vắng có phép
    /// </summary>
    public int AbsentWithPermissionCount { get; set; }

    /// <summary>
    /// Số lượng sinh viên về muộn
    /// </summary>
    public int LateCount { get; set; }

    /// <summary>
    /// Tỷ lệ có mặt / chuyên cần (%)
    /// </summary>
    public double AttendanceRate { get; set; }
}
