namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO thống kê tỷ lệ lấp đầy phòng và giường theo từng tòa nhà
/// </summary>
public class BuildingOccupancyDto
{
    /// <summary>
    /// Tên tòa nhà (Ví dụ: "Tòa A", "Tòa B", "Chưa phân tòa")
    /// </summary>
    public string BuildingName { get; set; } = string.Empty;

    /// <summary>
    /// Tổng số phòng trong tòa nhà
    /// </summary>
    public int TotalRooms { get; set; }

    /// <summary>
    /// Số phòng đang có người ở (Occupied)
    /// </summary>
    public int OccupiedRooms { get; set; }

    /// <summary>
    /// Số phòng còn trống hoàn toàn (Available)
    /// </summary>
    public int AvailableRooms { get; set; }

    /// <summary>
    /// Tổng số giường trong toàn bộ tòa nhà
    /// </summary>
    public int TotalBeds { get; set; }

    /// <summary>
    /// Số lượng giường hiện tại đang có sinh viên cư trú
    /// </summary>
    public int OccupiedBeds { get; set; }

    /// <summary>
    /// Tỷ lệ lấp đầy giường tính theo phần trăm (0 - 100%)
    /// </summary>
    public decimal OccupancyRate => TotalBeds > 0 ? Math.Round((decimal)OccupiedBeds / TotalBeds * 100, 2) : 0;
}

/// <summary>
/// DTO thống kê xu hướng doanh thu theo tháng (tiền phòng và dịch vụ điện nước)
/// </summary>
public class MonthlyRevenueTrendDto
{
    /// <summary>
    /// Tháng thống kê (1 - 12)
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Năm thống kê (Ví dụ: 2024)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Nhãn hiển thị trên trục hoành biểu đồ (Ví dụ: "T05/2024", "T10/2024")
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Doanh thu từ tiền thuê phòng (VND)
    /// </summary>
    public decimal RoomFeeRevenue { get; set; }

    /// <summary>
    /// Doanh thu từ tiền dịch vụ (điện, nước, dịch vụ khác) (VND)
    /// </summary>
    public decimal UtilityFeeRevenue { get; set; }

    /// <summary>
    /// Tổng doanh thu thu được trong tháng (VND)
    /// </summary>
    public decimal TotalRevenue => RoomFeeRevenue + UtilityFeeRevenue;
}
