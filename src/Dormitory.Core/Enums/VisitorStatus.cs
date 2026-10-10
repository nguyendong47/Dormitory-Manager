namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái lưu trú của khách đến thăm khu nội trú ký túc xá
/// </summary>
public enum VisitorStatus
{
    /// <summary>
    /// Khách đang trong khu nội trú KTX
    /// </summary>
    InDormitory = 0,

    /// <summary>
    /// Khách đã làm thủ tục rời KTX
    /// </summary>
    CheckedOut = 1,

    /// <summary>
    /// Khách lưu trú quá giờ quy định / quá giờ giới nghiêm 22:00
    /// </summary>
    Overstayed = 2
}
