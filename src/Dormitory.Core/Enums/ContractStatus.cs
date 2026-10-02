namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái hợp đồng thuê phòng
/// </summary>
public enum ContractStatus
{
    /// <summary>
    /// Hợp đồng đang có hiệu lực
    /// </summary>
    Active = 1,

    /// <summary>
    /// Hợp đồng đã hết hạn
    /// </summary>
    Expired = 2,

    /// <summary>
    /// Hợp đồng đã bị chấm dứt hoặc thanh lý trước hạn
    /// </summary>
    Terminated = 3
}
