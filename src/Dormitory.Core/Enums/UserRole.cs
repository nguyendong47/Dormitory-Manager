namespace Dormitory.Core.Enums;

/// <summary>
/// Vai trò của người dùng trong hệ thống quản lý KTX
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Quản trị viên hệ thống (toàn quyền)
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Quản lý ký túc xá (quản lý phòng, sinh viên, hợp đồng, hóa đơn)
    /// </summary>
    Manager = 2,

    /// <summary>
    /// Nhân viên trực/hỗ trợ ký túc xá
    /// </summary>
    Staff = 3
}
