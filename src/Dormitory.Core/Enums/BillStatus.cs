namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái thanh toán hóa đơn
/// </summary>
public enum BillStatus
{
    /// <summary>
    /// Chưa thanh toán
    /// </summary>
    Unpaid = 1,

    /// <summary>
    /// Đã thanh toán đầy đủ
    /// </summary>
    Paid = 2,

    /// <summary>
    /// Đã quá hạn thanh toán
    /// </summary>
    Overdue = 3
}
