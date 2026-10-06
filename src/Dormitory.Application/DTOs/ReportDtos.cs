using Dormitory.Core.Entities;
using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO chứa thông tin chi tiết một bản ghi lịch sử xuất báo cáo
/// </summary>
public class ReportHistoryDto
{
    public int Id { get; set; }

    public ReportType ReportType { get; set; }

    /// <summary>
    /// Tên tiếng Việt hiển thị của loại báo cáo
    /// </summary>
    public string ReportTypeText => ReportType switch
    {
        ReportType.Violations => "Vi phạm kỷ luật",
        ReportType.Financial => "Tài chính & Thu phí",
        ReportType.Occupancy => "Tỷ lệ lấp đầy phòng",
        ReportType.Equipment => "Kiểm kê tài sản thiết bị",
        _ => "Khác"
    };

    public string ReportTypeName => ReportTypeText;

    public string Title { get; set; } = string.Empty;

    public ReportFormat Format { get; set; }

    /// <summary>
    /// Tên hiển thị của định dạng tệp xuất bản
    /// </summary>
    public string FormatText => Format == ReportFormat.Pdf ? "PDF" : "Excel";

    public string FormatName => FormatText;

    public ReportStatus Status { get; set; }

    /// <summary>
    /// Tên tiếng Việt hiển thị của trạng thái báo cáo
    /// </summary>
    public string StatusText => Status switch
    {
        ReportStatus.Generating => "Đang tạo",
        ReportStatus.Completed => "Hoàn thành",
        ReportStatus.Failed => "Thất bại",
        _ => "Không rõ"
    };

    public string StatusName => StatusText;

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Định dạng dung lượng tệp tin dễ đọc (B, KB, MB)
    /// </summary>
    public string FormattedFileSize => FileSizeBytes < 1024
        ? $"{FileSizeBytes} B"
        : FileSizeBytes < 1024 * 1024
            ? $"{FileSizeBytes / 1024.0:F1} KB"
            : $"{FileSizeBytes / (1024.0 * 1024.0):F2} MB";

    public string FileSizeFormatted => FormattedFileSize;

    public string? ParametersJson { get; set; }

    public string? ErrorMessage { get; set; }

    public string? GeneratedBy { get; set; }

    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Ánh xạ từ thực thể ReportHistory sang DTO
    /// </summary>
    public static ReportHistoryDto FromEntity(ReportHistory entity)
    {
        return new ReportHistoryDto
        {
            Id = entity.Id,
            ReportType = entity.ReportType,
            Title = entity.Title,
            Format = entity.Format,
            Status = entity.Status,
            FileName = entity.FileName,
            FilePath = entity.FilePath,
            FileSizeBytes = entity.FileSizeBytes,
            ParametersJson = entity.ParametersJson,
            ErrorMessage = entity.ErrorMessage,
            GeneratedBy = entity.GeneratedBy,
            GeneratedAt = entity.GeneratedAt
        };
    }
}

/// <summary>
/// DTO chứa tham số yêu cầu sinh báo cáo với các bộ lọc nghiệp vụ
/// </summary>
public class GenerateReportRequestDto
{
    public ReportType ReportType { get; set; }

    /// <summary>
    /// Bí danh cho ReportType
    /// </summary>
    public ReportType Type
    {
        get => ReportType;
        set => ReportType = value;
    }

    public ReportFormat Format { get; set; } = ReportFormat.Excel;

    public string Title { get; set; } = string.Empty;

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? BuildingId { get; set; }

    public string? Building { get; set; }

    public int? RoomId { get; set; }

    public string? SearchTerm { get; set; }

    public ViolationSeverity? Severity { get; set; }

    public ViolationStatus? ViolationStatus { get; set; }

    public RoomType? RoomType { get; set; }

    public bool? OnlyVacant { get; set; }

    public EquipmentStatus? EquipmentStatus { get; set; }

    public bool? OnlyOverdue { get; set; }

    public string? AdditionalFiltersJson { get; set; }

    public string? GeneratedBy { get; set; }
}

// =========================================================================
// 1. DTOs BÁO CÁO VI PHẠM NỘI QUY & KỶ LUẬT (VIOLATIONS)
// =========================================================================

/// <summary>
/// DTO tổng hợp số liệu báo cáo vi phạm kỷ luật sinh viên
/// </summary>
public class ViolationReportSummaryDto
{
    public int TotalViolations { get; set; }

    public int MinorCount { get; set; }

    public int ModerateCount { get; set; }

    public int SevereCount { get; set; }

    public int CriticalCount { get; set; }

    public int PendingCount { get; set; }

    public int ResolvedCount { get; set; }

    public int DismissedCount { get; set; }

    public int TotalDemeritPoints { get; set; }

    public decimal TotalFineAmount { get; set; }

    /// <summary>
    /// Tỷ lệ xử lý dứt điểm vi phạm (%) = ((Resolved + Dismissed) / Total) * 100
    /// </summary>
    public decimal ResolutionRate => TotalViolations > 0
        ? Math.Round((decimal)(ResolvedCount + DismissedCount) / TotalViolations * 100, 2)
        : 0;

    /// <summary>
    /// Tỷ lệ vi phạm mức độ nghiêm trọng (%) = ((Severe + Critical) / Total) * 100
    /// </summary>
    public decimal CriticalSeverityRate => TotalViolations > 0
        ? Math.Round((decimal)(SevereCount + CriticalCount) / TotalViolations * 100, 2)
        : 0;

    public List<ViolationReportItemDto> Items { get; set; } = new();

    public List<ViolationDto> Violations { get; set; } = new();

    public List<TopViolatorDto> TopViolators { get; set; } = new();

    public List<TopViolatingStudentDto> TopStudents { get; set; } = new();

    public List<TopViolatingRoomDto> TopRooms { get; set; } = new();
}

/// <summary>
/// DTO chi tiết từng biên bản vi phạm xuất trong bảng kê
/// </summary>
public class ViolationReportItemDto
{
    public int Id { get; set; }

    public string ViolationCode { get; set; } = string.Empty;

    public string StudentCode { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public string BuildingName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public ViolationSeverity Severity { get; set; }

    public string SeverityText { get; set; } = string.Empty;

    public ViolationStatus Status { get; set; }

    public string StatusText { get; set; } = string.Empty;

    public int DemeritPoints { get; set; }

    public decimal FineAmount { get; set; }

    public DateTime ViolationDate { get; set; }

    public string? RecordedBy { get; set; }

    public string? ResolutionNotes { get; set; }
}

/// <summary>
/// DTO thống kê sinh viên có nhiều vi phạm nhất
/// </summary>
public class TopViolatorDto
{
    public int StudentId { get; set; }

    public string StudentCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string StudentName
    {
        get => FullName;
        set => FullName = value;
    }

    public string RoomNumber { get; set; } = string.Empty;

    public string BuildingName { get; set; } = string.Empty;

    public int ViolationCount { get; set; }

    public int DemeritPoints { get; set; }

    public decimal TotalFines { get; set; }

    public decimal TotalFineAmount
    {
        get => TotalFines;
        set => TotalFines = value;
    }
}

/// <summary>
/// Bí danh tương thích cho TopViolatorDto
/// </summary>
public class TopViolatingStudentDto : TopViolatorDto
{
}

/// <summary>
/// DTO thống kê phòng xảy ra nhiều vi phạm nhất
/// </summary>
public class TopViolatingRoomDto
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public string Building { get; set; } = string.Empty;

    public int ViolationCount { get; set; }
}

// =========================================================================
// 2. DTOs BÁO CÁO TÀI CHÍNH & THU PHÍ (FINANCIAL)
// =========================================================================

/// <summary>
/// DTO tổng hợp chỉ số báo cáo tài chính và thu phí KTX
/// </summary>
public class FinancialReportSummaryDto
{
    public decimal TotalExpectedRevenue { get; set; }

    public decimal TotalCollectedRevenue { get; set; }

    public decimal RoomFeeCollected { get; set; }

    public decimal ElectricFeeCollected { get; set; }

    public decimal WaterFeeCollected { get; set; }

    public decimal OtherFeeCollected { get; set; }

    /// <summary>
    /// Tổng công nợ tồn đọng = Max(0, Phải thu - Thực thu)
    /// </summary>
    public decimal TotalOutstandingDebt => Math.Max(0, TotalExpectedRevenue - TotalCollectedRevenue);

    public decimal PendingDebt { get; set; }

    public decimal OverdueDebt { get; set; }

    /// <summary>
    /// Tỷ lệ thu hồi công nợ (%) = (Thực thu / Phải thu) * 100
    /// </summary>
    public decimal CollectionRate => TotalExpectedRevenue > 0
        ? Math.Round(TotalCollectedRevenue / TotalExpectedRevenue * 100, 2)
        : 0;

    public int TotalBillsCount { get; set; }

    public int PaidBillsCount { get; set; }

    public int UnpaidBillsCount { get; set; }

    public int OverdueBillsCount { get; set; }

    /// <summary>
    /// Tỷ lệ hóa đơn hoàn thành (%) = (Đã thanh toán / Tổng số HĐ) * 100
    /// </summary>
    public decimal PaidBillRate => TotalBillsCount > 0
        ? Math.Round((decimal)PaidBillsCount / TotalBillsCount * 100, 2)
        : 0;

    public decimal TotalElectricUsageKwh { get; set; }

    public decimal TotalWaterUsageM3 { get; set; }

    public List<FinancialReportItemDto> Items { get; set; } = new();

    public List<BillDto> Bills { get; set; } = new();

    public List<FinancialReportItemDto> OverdueBills { get; set; } = new();

    public List<RevenueBreakdownDto> RevenueBreakdowns { get; set; } = new();

    public List<BuildingRevenueSummaryDto> BuildingRevenues { get; set; } = new();
}

/// <summary>
/// DTO chi tiết hóa đơn trong bảng kê tài chính
/// </summary>
public class FinancialReportItemDto
{
    public int BillId { get; set; }

    public string BillCode { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public string BuildingName { get; set; } = string.Empty;

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal RoomFee { get; set; }

    public decimal OldElectricIndex { get; set; }

    public decimal NewElectricIndex { get; set; }

    public decimal ElectricUsage { get; set; }

    public decimal ElectricFee { get; set; }

    public decimal OldWaterIndex { get; set; }

    public decimal NewWaterIndex { get; set; }

    public decimal WaterUsage { get; set; }

    public decimal WaterFee { get; set; }

    public decimal OtherServiceFee { get; set; }

    public decimal TotalAmount { get; set; }

    public BillStatus Status { get; set; }

    public string StatusText { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public DateTime? PaidAt { get; set; }

    public int OverdueDays { get; set; }
}

/// <summary>
/// DTO cơ cấu doanh thu theo tòa nhà hoặc danh mục
/// </summary>
public class RevenueBreakdownDto
{
    public string BuildingName { get; set; } = string.Empty;

    public int BillsCount { get; set; }

    public decimal ExpectedRevenue { get; set; }

    public decimal CollectedRevenue { get; set; }

    public decimal OutstandingDebt => Math.Max(0, ExpectedRevenue - CollectedRevenue);

    public decimal CollectionRate => ExpectedRevenue > 0
        ? Math.Round(CollectedRevenue / ExpectedRevenue * 100, 2)
        : 0;
}

/// <summary>
/// Bí danh tương thích cho RevenueBreakdownDto
/// </summary>
public class BuildingRevenueSummaryDto : RevenueBreakdownDto
{
}

// =========================================================================
// 3. DTOs BÁO CÁO TỶ LỆ LẤP ĐẦY & PHÒNG (OCCUPANCY)
// =========================================================================

/// <summary>
/// DTO tổng hợp tỷ lệ lấp đầy và tình trạng phòng ở KTX
/// </summary>
public class OccupancyReportSummaryDto
{
    public int TotalRooms { get; set; }

    public int TotalBedsCapacity { get; set; }

    public int OccupiedBeds { get; set; }

    /// <summary>
    /// Số giường còn trống khả dụng = Max(0, Tổng sức chứa - Đang ở)
    /// </summary>
    public int AvailableBeds => Math.Max(0, TotalBedsCapacity - OccupiedBeds);

    /// <summary>
    /// Tỷ lệ lấp đầy toàn bộ KTX (%) = (Đang ở / Tổng sức chứa) * 100
    /// </summary>
    public decimal OccupancyRate => TotalBedsCapacity > 0
        ? Math.Round((decimal)OccupiedBeds / TotalBedsCapacity * 100, 2)
        : 0;

    public int OccupiedRoomsCount { get; set; }

    public int AvailableRoomsCount { get; set; }

    public int MaintenanceRoomsCount { get; set; }

    public int EmptyRoomsCount { get; set; }

    public int MaleBedsCapacity { get; set; }

    public int MaleBedsOccupied { get; set; }

    public int FemaleBedsCapacity { get; set; }

    public int FemaleBedsOccupied { get; set; }

    /// <summary>
    /// Tỷ lệ phòng sẵn sàng đón tiếp (%) = (Phòng khả dụng / Tổng phòng) * 100
    /// </summary>
    public decimal VacantRoomRate => TotalRooms > 0
        ? Math.Round((decimal)AvailableRoomsCount / TotalRooms * 100, 2)
        : 0;

    public List<OccupancyBuildingSummaryDto> Buildings { get; set; } = new();

    public List<VacantRoomDto> VacantRooms { get; set; } = new();
}

/// <summary>
/// DTO tổng hợp lấp đầy theo từng tòa nhà
/// </summary>
public class OccupancyBuildingSummaryDto
{
    public string BuildingName { get; set; } = string.Empty;

    public int TotalRooms { get; set; }

    public int TotalBedsCapacity { get; set; }

    public int OccupiedBeds { get; set; }

    public int AvailableBeds => Math.Max(0, TotalBedsCapacity - OccupiedBeds);

    public decimal OccupancyRate => TotalBedsCapacity > 0
        ? Math.Round((decimal)OccupiedBeds / TotalBedsCapacity * 100, 2)
        : 0;

    public int MaintenanceRoomsCount { get; set; }
}

/// <summary>
/// DTO danh sách phòng còn chỗ trống để điều phối sinh viên
/// </summary>
public class VacantRoomDto
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public string BuildingName { get; set; } = string.Empty;

    public int Floor { get; set; }

    public RoomType RoomType { get; set; }

    public string RoomTypeText { get; set; } = string.Empty;

    public Gender AllowedGender { get; set; }

    public string AllowedGenderText { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int CurrentOccupancy { get; set; }

    /// <summary>
    /// Số chỗ trống khả dụng của phòng = Max(0, Sức chứa - Đang ở)
    /// </summary>
    public int VacantBeds => Math.Max(0, Capacity - CurrentOccupancy);

    public decimal PricePerMonth { get; set; }
}

// =========================================================================
// 4. DTOs BÁO CÁO KIỂM KÊ TÀI SẢN & THIẾT BỊ (EQUIPMENT)
// =========================================================================

/// <summary>
/// DTO tổng hợp kiểm kê tài sản và trang thiết bị phòng ở
/// </summary>
public class EquipmentReportSummaryDto
{
    public int TotalEquipmentCount { get; set; }

    public decimal TotalAssetValue { get; set; }

    public int GoodConditionCount { get; set; }

    public int NeedsRepairCount { get; set; }

    public int BrokenCount { get; set; }

    /// <summary>
    /// Tỷ lệ thiết bị hoạt động tốt / sức khỏe (%) = (Good / Total) * 100
    /// </summary>
    public decimal HealthRate => TotalEquipmentCount > 0
        ? Math.Round((decimal)GoodConditionCount / TotalEquipmentCount * 100, 2)
        : 0;

    /// <summary>
    /// Tỷ lệ thiết bị hư hỏng / cần sửa (%) = ((NeedsRepair + Broken) / Total) * 100
    /// </summary>
    public decimal FaultyRate => TotalEquipmentCount > 0
        ? Math.Round((decimal)(NeedsRepairCount + BrokenCount) / TotalEquipmentCount * 100, 2)
        : 0;

    public decimal DamagedAssetValue { get; set; }

    public decimal EstimatedRepairCost { get; set; }

    public int OverdueMaintenanceCount { get; set; }

    public List<EquipmentReportItemDto> Items { get; set; } = new();

    public List<EquipmentDto> Equipments { get; set; } = new();

    public List<EquipmentReportItemDto> FaultyEquipments { get; set; } = new();

    public List<EquipmentStatusBreakdownDto> StatusBreakdowns { get; set; } = new();

    public List<BuildingEquipmentSummaryDto> BuildingEquipments { get; set; } = new();
}

/// <summary>
/// DTO chi tiết từng trang thiết bị trong bảng kiểm kê
/// </summary>
public class EquipmentReportItemDto
{
    public int Id { get; set; }

    public string EquipmentCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public string BuildingName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    /// <summary>
    /// Tổng giá trị = Số lượng * Đơn giá
    /// </summary>
    public decimal TotalValue => Quantity * Price;

    public EquipmentStatus Status { get; set; }

    public string StatusText { get; set; } = string.Empty;

    public DateTime? AssignedDate { get; set; }

    public DateTime? LastMaintainedAt { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// DTO thống kê theo phân loại trạng thái thiết bị
/// </summary>
public class EquipmentStatusBreakdownDto
{
    public EquipmentStatus Status { get; set; }

    public string StatusText { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal TotalValue { get; set; }

    public decimal Percentage { get; set; }
}

/// <summary>
/// DTO tổng hợp tài sản thiết bị theo từng tòa nhà
/// </summary>
public class BuildingEquipmentSummaryDto
{
    public string BuildingName { get; set; } = string.Empty;

    public int TotalQuantity { get; set; }

    public int GoodQuantity { get; set; }

    public int NeedsRepairQuantity { get; set; }

    public int BrokenQuantity { get; set; }

    public decimal TotalValue { get; set; }
}
