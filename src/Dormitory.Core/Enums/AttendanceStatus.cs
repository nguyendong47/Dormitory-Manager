namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái điểm danh chuyên cần sinh viên ban đêm tại phòng ở KTX
/// </summary>
public enum AttendanceStatus
{
    /// <summary>
    /// Có mặt tại phòng đúng giờ
    /// </summary>
    Present = 0,

    /// <summary>
    /// Vắng mặt không phép
    /// </summary>
    AbsentWithoutPermission = 1,

    /// <summary>
    /// Vắng mặt có phép / đã xin nghỉ
    /// </summary>
    AbsentWithPermission = 2,

    /// <summary>
    /// Về phòng muộn sau giờ giới nghiêm
    /// </summary>
    Late = 3
}
