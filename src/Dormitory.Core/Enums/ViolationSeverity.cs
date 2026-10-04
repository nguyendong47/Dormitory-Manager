namespace Dormitory.Core.Enums;

/// <summary>
/// Mức độ nghiêm trọng của hành vi vi phạm nội quy KTX
/// </summary>
public enum ViolationSeverity
{
    /// <summary>
    /// Nhắc nhở / Mức nhẹ (vệ sinh bẩn, phơi đồ sai quy định...)
    /// </summary>
    Minor = 1,

    /// <summary>
    /// Khiển trách / Mức vừa (gây ồn sau 23h, vi phạm giờ giới nghiêm...)
    /// </summary>
    Moderate = 2,

    /// <summary>
    /// Cảnh cáo / Mức nghiêm trọng (nấu ăn bằng bếp điện trái phép, dẫn người ngoài qua đêm...)
    /// </summary>
    Severe = 3,

    /// <summary>
    /// Buộc rời KTX / Rất nghiêm trọng (đánh bạc, ẩu đả, tàng trữ chất cấm...)
    /// </summary>
    Critical = 4
}
