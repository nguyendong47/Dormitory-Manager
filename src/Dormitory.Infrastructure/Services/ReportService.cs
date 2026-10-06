using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Dormitory.Infrastructure.Services;

/// <summary>
/// Dịch vụ tổng hợp dữ liệu, xuất bản báo cáo (Excel &amp; PDF) và quản lý lịch sử báo cáo
/// </summary>
public class ReportService : IReportService
{
    private readonly IDormitoryDbContext _context;

    // Bảng màu nhận diện thương hiệu cho từng phân hệ báo cáo
    private static readonly XLColor ViolationColor = XLColor.FromArgb(0xC4, 0x2B, 0x1C);   // Đỏ cam
    private static readonly XLColor FinancialColor = XLColor.FromArgb(0x10, 0x7C, 0x41);   // Xanh lá đậm
    private static readonly XLColor OccupancyColor = XLColor.FromArgb(0x00, 0x78, 0xD4);   // Xanh dương Office
    private static readonly XLColor EquipmentColor = XLColor.FromArgb(0x00, 0x82, 0x72);   // Xanh ngọc Teal

    private const string CurrencyFormat = "#,##0 \"đ\"";
    private const string PercentFormat = "0.0%";
    private const string IntegerFormat = "#,##0";
    private const string DateTimeFormat = "dd/MM/yyyy HH:mm";
    private const string DateFormat = "dd/MM/yyyy";

    public ReportService(IDormitoryDbContext context)
    {
        _context = context;
        // Thiết lập giấy phép cộng đồng (Community License) cho QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region 1. TỔNG HỢP DỮ LIỆU BÁO CÁO (AGGREGATION METHODS)

    /// <summary>
    /// Tổng hợp số liệu báo cáo vi phạm nội quy &amp; kỷ luật sinh viên
    /// </summary>
    public async Task<ViolationReportSummaryDto> GetViolationReportDataAsync(GenerateReportRequestDto request)
    {
        var query = _context.Violations
            .Include(v => v.Student)
            .Include(v => v.Room)
            .AsNoTracking()
            .AsQueryable();

        if (request.FromDate.HasValue)
        {
            query = query.Where(v => v.ViolationDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            var endOfDay = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            var toCompare = request.ToDate.Value.TimeOfDay == TimeSpan.Zero ? endOfDay : request.ToDate.Value;
            query = query.Where(v => v.ViolationDate <= toCompare);
        }

        if (request.Severity.HasValue)
        {
            query = query.Where(v => v.Severity == request.Severity.Value);
        }

        if (request.ViolationStatus.HasValue)
        {
            query = query.Where(v => v.Status == request.ViolationStatus.Value);
        }

        if (request.RoomId.HasValue && request.RoomId.Value > 0)
        {
            query = query.Where(v => v.RoomId == request.RoomId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Building))
        {
            var buildingLower = request.Building.Trim().ToLower();
            query = query.Where(v => v.Room != null && v.Room.Building.ToLower().Contains(buildingLower));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(v => (v.ViolationCode != null && v.ViolationCode.ToLower().Contains(term))
                || (v.Title != null && v.Title.ToLower().Contains(term))
                || (v.Student != null && (v.Student.FullName.ToLower().Contains(term) || v.Student.StudentCode.ToLower().Contains(term)))
                || (v.Room != null && v.Room.RoomNumber.ToLower().Contains(term)));
        }

        var list = await query.OrderByDescending(v => v.ViolationDate).ToListAsync();

        var summary = new ViolationReportSummaryDto
        {
            TotalViolations = list.Count,
            MinorCount = list.Count(v => v.Severity == ViolationSeverity.Minor),
            ModerateCount = list.Count(v => v.Severity == ViolationSeverity.Moderate),
            SevereCount = list.Count(v => v.Severity == ViolationSeverity.Severe),
            CriticalCount = list.Count(v => v.Severity == ViolationSeverity.Critical),
            PendingCount = list.Count(v => v.Status == ViolationStatus.Pending),
            ResolvedCount = list.Count(v => v.Status == ViolationStatus.Resolved),
            DismissedCount = list.Count(v => v.Status == ViolationStatus.Dismissed),
            TotalDemeritPoints = list.Sum(v => v.DemeritPoints),
            TotalFineAmount = list.Sum(v => v.FineAmount)
        };

        summary.Items = list.Select(v => new ViolationReportItemDto
        {
            Id = v.Id,
            ViolationCode = v.ViolationCode,
            StudentCode = v.Student?.StudentCode ?? string.Empty,
            StudentName = v.Student?.FullName ?? string.Empty,
            RoomNumber = v.Room?.RoomNumber ?? string.Empty,
            BuildingName = v.Room?.Building ?? string.Empty,
            Title = v.Title,
            Severity = v.Severity,
            SeverityText = GetSeverityText(v.Severity),
            Status = v.Status,
            StatusText = GetViolationStatusText(v.Status),
            DemeritPoints = v.DemeritPoints,
            FineAmount = v.FineAmount,
            ViolationDate = v.ViolationDate,
            RecordedBy = v.RecordedBy,
            ResolutionNotes = v.ResolutionNotes
        }).ToList();

        summary.Violations = list.Select(v => new ViolationDto
        {
            Id = v.Id,
            ViolationCode = v.ViolationCode,
            StudentId = v.StudentId,
            StudentName = v.Student?.FullName ?? string.Empty,
            StudentCode = v.Student?.StudentCode ?? string.Empty,
            RoomId = v.RoomId,
            RoomNumber = v.Room?.RoomNumber ?? string.Empty,
            BuildingName = v.Room?.Building ?? string.Empty,
            Title = v.Title,
            Description = v.Description,
            Severity = v.Severity,
            SeverityText = GetSeverityText(v.Severity),
            Status = v.Status,
            StatusText = GetViolationStatusText(v.Status),
            FineAmount = v.FineAmount,
            DemeritPoints = v.DemeritPoints,
            ViolationDate = v.ViolationDate,
            CreatedAt = v.CreatedAt,
            ResolutionNotes = v.ResolutionNotes,
            RecordedBy = v.RecordedBy
        }).ToList();

        // Top 5 sinh viên vi phạm
        var topStudents = list
            .Where(v => v.Student != null)
            .GroupBy(v => new
            {
                v.StudentId,
                v.Student!.StudentCode,
                v.Student.FullName,
                RoomNumber = v.Room?.RoomNumber ?? string.Empty,
                BuildingName = v.Room?.Building ?? string.Empty
            })
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Sum(x => x.DemeritPoints))
            .Take(5)
            .Select(g => new TopViolatorDto
            {
                StudentId = g.Key.StudentId,
                StudentCode = g.Key.StudentCode,
                FullName = g.Key.FullName,
                RoomNumber = g.Key.RoomNumber,
                BuildingName = g.Key.BuildingName,
                ViolationCount = g.Count(),
                DemeritPoints = g.Sum(x => x.DemeritPoints),
                TotalFines = g.Sum(x => x.FineAmount)
            }).ToList();

        summary.TopViolators = topStudents;
        summary.TopStudents = topStudents.Select(s => new TopViolatingStudentDto
        {
            StudentId = s.StudentId,
            StudentCode = s.StudentCode,
            FullName = s.FullName,
            RoomNumber = s.RoomNumber,
            BuildingName = s.BuildingName,
            ViolationCount = s.ViolationCount,
            DemeritPoints = s.DemeritPoints,
            TotalFines = s.TotalFines
        }).ToList();

        // Top 5 phòng xảy ra vi phạm nhiều nhất
        summary.TopRooms = list
            .Where(v => v.Room != null)
            .GroupBy(v => new { v.RoomId, RoomNumber = v.Room!.RoomNumber, Building = v.Room.Building })
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new TopViolatingRoomDto
            {
                RoomId = g.Key.RoomId,
                RoomNumber = g.Key.RoomNumber,
                Building = g.Key.Building,
                ViolationCount = g.Count()
            }).ToList();

        return summary;
    }

    /// <summary>
    /// Tổng hợp số liệu báo cáo tài chính &amp; thu phí ký túc xá
    /// </summary>
    public async Task<FinancialReportSummaryDto> GetFinancialReportDataAsync(GenerateReportRequestDto request)
    {
        var query = _context.Bills
            .Include(b => b.Room)
            .AsNoTracking()
            .AsQueryable();

        if (request.Month.HasValue && request.Month.Value > 0)
        {
            query = query.Where(b => b.Month == request.Month.Value);
        }

        if (request.Year.HasValue && request.Year.Value > 0)
        {
            query = query.Where(b => b.Year == request.Year.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(b => b.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            var endOfDay = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            var toCompare = request.ToDate.Value.TimeOfDay == TimeSpan.Zero ? endOfDay : request.ToDate.Value;
            query = query.Where(b => b.CreatedAt <= toCompare);
        }

        if (request.RoomId.HasValue && request.RoomId.Value > 0)
        {
            query = query.Where(b => b.RoomId == request.RoomId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Building))
        {
            var buildingLower = request.Building.Trim().ToLower();
            query = query.Where(b => b.Room != null && b.Room.Building.ToLower().Contains(buildingLower));
        }

        var now = DateTime.UtcNow;

        if (request.OnlyOverdue == true)
        {
            query = query.Where(b => b.Status == BillStatus.Overdue || (b.Status == BillStatus.Unpaid && b.DueDate < now));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(b => (b.BillCode != null && b.BillCode.ToLower().Contains(term))
                || (b.Room != null && (b.Room.RoomNumber.ToLower().Contains(term) || b.Room.Building.ToLower().Contains(term))));
        }

        var list = await query.OrderByDescending(b => b.Year).ThenByDescending(b => b.Month).ToListAsync();

        var paidBills = list.Where(b => b.Status == BillStatus.Paid).ToList();
        var unpaidBills = list.Where(b => b.Status == BillStatus.Unpaid).ToList();
        var overdueBills = list.Where(b => b.Status == BillStatus.Overdue || (b.Status == BillStatus.Unpaid && b.DueDate < now)).ToList();
        var pendingDebtBills = list.Where(b => b.Status == BillStatus.Unpaid && b.DueDate >= now).ToList();

        var summary = new FinancialReportSummaryDto
        {
            TotalBillsCount = list.Count,
            PaidBillsCount = paidBills.Count,
            UnpaidBillsCount = unpaidBills.Count,
            OverdueBillsCount = overdueBills.Count,
            TotalExpectedRevenue = list.Sum(b => b.TotalAmount),
            TotalCollectedRevenue = paidBills.Sum(b => b.TotalAmount),
            RoomFeeCollected = paidBills.Sum(b => b.RoomFee),
            ElectricFeeCollected = paidBills.Sum(b => b.ElectricFee),
            WaterFeeCollected = paidBills.Sum(b => b.WaterFee),
            OtherFeeCollected = paidBills.Sum(b => b.OtherServiceFee),
            PendingDebt = pendingDebtBills.Sum(b => b.TotalAmount),
            OverdueDebt = overdueBills.Sum(b => b.TotalAmount),
            TotalElectricUsageKwh = list.Sum(b => b.ElectricUsage),
            TotalWaterUsageM3 = list.Sum(b => b.WaterUsage)
        };

        summary.Items = list.Select(b => new FinancialReportItemDto
        {
            BillId = b.Id,
            BillCode = b.BillCode,
            RoomNumber = b.Room?.RoomNumber ?? string.Empty,
            BuildingName = b.Room?.Building ?? string.Empty,
            Month = b.Month,
            Year = b.Year,
            RoomFee = b.RoomFee,
            OldElectricIndex = b.OldElectricIndex,
            NewElectricIndex = b.NewElectricIndex,
            ElectricUsage = b.ElectricUsage,
            ElectricFee = b.ElectricFee,
            OldWaterIndex = b.OldWaterIndex,
            NewWaterIndex = b.NewWaterIndex,
            WaterUsage = b.WaterUsage,
            WaterFee = b.WaterFee,
            OtherServiceFee = b.OtherServiceFee,
            TotalAmount = b.TotalAmount,
            Status = b.Status,
            StatusText = GetBillStatusText(b.Status),
            DueDate = b.DueDate,
            PaidAt = b.PaidDate,
            OverdueDays = (b.Status == BillStatus.Overdue || (b.Status == BillStatus.Unpaid && b.DueDate < now))
                ? Math.Max(0, (int)(now - b.DueDate).TotalDays)
                : 0
        }).ToList();

        summary.Bills = list.Select(b => new BillDto
        {
            Id = b.Id,
            BillCode = b.BillCode,
            RoomId = b.RoomId,
            RoomNumber = b.Room?.RoomNumber ?? string.Empty,
            Building = b.Room?.Building ?? string.Empty,
            Month = b.Month,
            Year = b.Year,
            RoomFee = b.RoomFee,
            OldElectricIndex = b.OldElectricIndex,
            NewElectricIndex = b.NewElectricIndex,
            ElectricRate = b.ElectricRate,
            OldWaterIndex = b.OldWaterIndex,
            NewWaterIndex = b.NewWaterIndex,
            WaterRate = b.WaterRate,
            OtherServiceFee = b.OtherServiceFee,
            Status = b.Status,
            Note = b.Note,
            DueDate = b.DueDate,
            PaidDate = b.PaidDate,
            CreatedAt = b.CreatedAt
        }).ToList();

        summary.OverdueBills = summary.Items
            .Where(i => i.Status == BillStatus.Overdue || (i.Status == BillStatus.Unpaid && i.DueDate < now))
            .ToList();

        var buildingGroups = list
            .GroupBy(b => b.Room?.Building ?? "Chưa phân tòa")
            .Select(g => new BuildingRevenueSummaryDto
            {
                BuildingName = g.Key,
                BillsCount = g.Count(),
                ExpectedRevenue = g.Sum(x => x.TotalAmount),
                CollectedRevenue = g.Where(x => x.Status == BillStatus.Paid).Sum(x => x.TotalAmount)
            }).ToList();

        summary.BuildingRevenues = buildingGroups;
        summary.RevenueBreakdowns = buildingGroups.Select(bg => new RevenueBreakdownDto
        {
            BuildingName = bg.BuildingName,
            BillsCount = bg.BillsCount,
            ExpectedRevenue = bg.ExpectedRevenue,
            CollectedRevenue = bg.CollectedRevenue
        }).ToList();

        return summary;
    }

    /// <summary>
    /// Tổng hợp số liệu báo cáo tỷ lệ lấp đầy và tình trạng phòng ở
    /// </summary>
    public async Task<OccupancyReportSummaryDto> GetOccupancyReportDataAsync(GenerateReportRequestDto request)
    {
        var query = _context.Rooms
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Building))
        {
            var buildingLower = request.Building.Trim().ToLower();
            query = query.Where(r => r.Building.ToLower().Contains(buildingLower));
        }

        if (request.RoomId.HasValue && request.RoomId.Value > 0)
        {
            query = query.Where(r => r.Id == request.RoomId.Value);
        }

        if (request.RoomType.HasValue)
        {
            query = query.Where(r => r.Type == request.RoomType.Value);
        }

        if (request.OnlyVacant == true)
        {
            query = query.Where(r => r.Status == RoomStatus.Available && r.CurrentOccupancy < r.Capacity);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(r => r.RoomNumber.ToLower().Contains(term) || r.Building.ToLower().Contains(term));
        }

        var rooms = await query.OrderBy(r => r.Building).ThenBy(r => r.RoomNumber).ToListAsync();

        var summary = new OccupancyReportSummaryDto
        {
            TotalRooms = rooms.Count,
            TotalBedsCapacity = rooms.Sum(r => r.Capacity),
            OccupiedBeds = rooms.Sum(r => r.CurrentOccupancy),
            OccupiedRoomsCount = rooms.Count(r => r.Status == RoomStatus.Occupied || r.CurrentOccupancy >= r.Capacity),
            AvailableRoomsCount = rooms.Count(r => r.Status == RoomStatus.Available && r.CurrentOccupancy < r.Capacity),
            MaintenanceRoomsCount = rooms.Count(r => r.Status == RoomStatus.Maintenance),
            EmptyRoomsCount = rooms.Count(r => r.CurrentOccupancy == 0 && r.Status == RoomStatus.Available),
            MaleBedsCapacity = rooms.Where(r => r.AllowedGender == Gender.Male).Sum(r => r.Capacity),
            MaleBedsOccupied = rooms.Where(r => r.AllowedGender == Gender.Male).Sum(r => r.CurrentOccupancy),
            FemaleBedsCapacity = rooms.Where(r => r.AllowedGender == Gender.Female).Sum(r => r.Capacity),
            FemaleBedsOccupied = rooms.Where(r => r.AllowedGender == Gender.Female).Sum(r => r.CurrentOccupancy)
        };

        summary.Buildings = rooms
            .GroupBy(r => r.Building)
            .Select(g => new OccupancyBuildingSummaryDto
            {
                BuildingName = g.Key,
                TotalRooms = g.Count(),
                TotalBedsCapacity = g.Sum(x => x.Capacity),
                OccupiedBeds = g.Sum(x => x.CurrentOccupancy),
                MaintenanceRoomsCount = g.Count(x => x.Status == RoomStatus.Maintenance)
            }).ToList();

        summary.VacantRooms = rooms
            .Where(r => r.Status == RoomStatus.Available && r.CurrentOccupancy < r.Capacity)
            .Select(r => new VacantRoomDto
            {
                RoomId = r.Id,
                RoomNumber = r.RoomNumber,
                BuildingName = r.Building,
                Floor = r.Floor,
                RoomType = r.Type,
                RoomTypeText = GetRoomTypeText(r.Type),
                AllowedGender = r.AllowedGender,
                AllowedGenderText = r.AllowedGender == Gender.Male ? "Nam" : "Nữ",
                Capacity = r.Capacity,
                CurrentOccupancy = r.CurrentOccupancy,
                PricePerMonth = r.PricePerMonth
            }).ToList();

        return summary;
    }

    /// <summary>
    /// Tổng hợp số liệu báo cáo kiểm kê tài sản &amp; thiết bị
    /// </summary>
    public async Task<EquipmentReportSummaryDto> GetEquipmentReportDataAsync(GenerateReportRequestDto request)
    {
        var query = _context.Equipments
            .Include(e => e.Room)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Building))
        {
            var buildingLower = request.Building.Trim().ToLower();
            query = query.Where(e => e.Room != null && e.Room.Building.ToLower().Contains(buildingLower));
        }

        if (request.RoomId.HasValue && request.RoomId.Value > 0)
        {
            query = query.Where(e => e.RoomId == request.RoomId.Value);
        }

        if (request.EquipmentStatus.HasValue)
        {
            query = query.Where(e => e.Status == request.EquipmentStatus.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(e => (e.Name != null && e.Name.ToLower().Contains(term))
                || (e.EquipmentCode != null && e.EquipmentCode.ToLower().Contains(term))
                || (e.Room != null && (e.Room.RoomNumber.ToLower().Contains(term) || e.Room.Building.ToLower().Contains(term))));
        }

        var equipments = await query.OrderBy(e => e.EquipmentCode).ToListAsync();

        var totalCount = equipments.Sum(e => e.Quantity);
        var goodCount = equipments.Where(e => e.Status == EquipmentStatus.Good).Sum(e => e.Quantity);
        var needsRepairCount = equipments.Where(e => e.Status == EquipmentStatus.NeedsRepair).Sum(e => e.Quantity);
        var brokenCount = equipments.Where(e => e.Status == EquipmentStatus.Broken).Sum(e => e.Quantity);
        var totalValue = equipments.Sum(e => e.Quantity * e.Price);
        var damagedValue = equipments.Where(e => e.Status == EquipmentStatus.Broken).Sum(e => e.Quantity * e.Price);
        var estimatedRepair = equipments.Where(e => e.Status == EquipmentStatus.NeedsRepair).Sum(e => e.Quantity * e.Price * 0.2m)
            + equipments.Where(e => e.Status == EquipmentStatus.Broken).Sum(e => e.Quantity * e.Price * 0.5m);

        var now = DateTime.UtcNow;
        var overdueThreshold = now.AddDays(-180);
        var overdueCount = equipments.Count(e => e.LastMaintainedAt == null || e.LastMaintainedAt < overdueThreshold);

        var summary = new EquipmentReportSummaryDto
        {
            TotalEquipmentCount = totalCount,
            TotalAssetValue = totalValue,
            GoodConditionCount = goodCount,
            NeedsRepairCount = needsRepairCount,
            BrokenCount = brokenCount,
            DamagedAssetValue = damagedValue,
            EstimatedRepairCost = estimatedRepair,
            OverdueMaintenanceCount = overdueCount
        };

        summary.Items = equipments.Select(e => new EquipmentReportItemDto
        {
            Id = e.Id,
            EquipmentCode = e.EquipmentCode,
            Name = e.Name,
            RoomId = e.RoomId,
            RoomNumber = e.Room?.RoomNumber ?? string.Empty,
            BuildingName = e.Room?.Building ?? string.Empty,
            Quantity = e.Quantity,
            Price = e.Price,
            Status = e.Status,
            StatusText = GetEquipmentStatusText(e.Status),
            AssignedDate = e.CreatedAt,
            LastMaintainedAt = e.LastMaintainedAt,
            Notes = e.Notes
        }).ToList();

        summary.Equipments = equipments.Select(e => new EquipmentDto
        {
            Id = e.Id,
            RoomId = e.RoomId,
            RoomNumber = e.Room?.RoomNumber ?? string.Empty,
            BuildingName = e.Room?.Building ?? string.Empty,
            EquipmentCode = e.EquipmentCode,
            Name = e.Name,
            Status = e.Status,
            StatusText = GetEquipmentStatusText(e.Status),
            Quantity = e.Quantity,
            Price = e.Price,
            Notes = e.Notes,
            CreatedAt = e.CreatedAt,
            LastMaintainedAt = e.LastMaintainedAt
        }).ToList();

        summary.FaultyEquipments = summary.Items
            .Where(i => i.Status == EquipmentStatus.NeedsRepair || i.Status == EquipmentStatus.Broken)
            .ToList();

        var statusList = new[] { EquipmentStatus.Good, EquipmentStatus.NeedsRepair, EquipmentStatus.Broken };
        summary.StatusBreakdowns = statusList.Select(st =>
        {
            var q = equipments.Where(e => e.Status == st).Sum(e => e.Quantity);
            var v = equipments.Where(e => e.Status == st).Sum(e => e.Quantity * e.Price);
            return new EquipmentStatusBreakdownDto
            {
                Status = st,
                StatusText = GetEquipmentStatusText(st),
                Quantity = q,
                TotalValue = v,
                Percentage = totalCount > 0 ? Math.Round((decimal)q / totalCount * 100, 2) : 0
            };
        }).ToList();

        summary.BuildingEquipments = equipments
            .GroupBy(e => e.Room?.Building ?? "Chưa phân phòng")
            .Select(g => new BuildingEquipmentSummaryDto
            {
                BuildingName = g.Key,
                TotalQuantity = g.Sum(x => x.Quantity),
                GoodQuantity = g.Where(x => x.Status == EquipmentStatus.Good).Sum(x => x.Quantity),
                NeedsRepairQuantity = g.Where(x => x.Status == EquipmentStatus.NeedsRepair).Sum(x => x.Quantity),
                BrokenQuantity = g.Where(x => x.Status == EquipmentStatus.Broken).Sum(x => x.Quantity),
                TotalValue = g.Sum(x => x.Quantity * x.Price)
            }).ToList();

        return summary;
    }

    #endregion

    #region 2. XUẤT BẢN EXCEL (CLOSEDXML)

    /// <summary>
    /// Xuất báo cáo định dạng Microsoft Excel (.xlsx) qua ClosedXML
    /// </summary>
    public async Task<byte[]> GenerateExcelReportAsync(GenerateReportRequestDto request)
    {
        NormalizeReportType(request);

        return request.ReportType switch
        {
            ReportType.Violations => await GenerateViolationExcelAsync(request),
            ReportType.Financial => await GenerateFinancialExcelAsync(request),
            ReportType.Occupancy => await GenerateOccupancyExcelAsync(request),
            ReportType.Equipment => await GenerateEquipmentExcelAsync(request),
            _ => await GenerateViolationExcelAsync(request)
        };
    }

    private async Task<byte[]> GenerateViolationExcelAsync(GenerateReportRequestDto request)
    {
        var data = await GetViolationReportDataAsync(request);
        using var workbook = new XLWorkbook();

        // Sheet 1: Tổng quan KPI
        var wsOverview = workbook.Worksheets.Add("Tổng quan KPI");
        wsOverview.ShowGridLines = true;

        CreateSheetTitle(wsOverview, "BÁO CÁO TỔNG HỢP VI PHẠM NỘI QUY KÝ TÚC XÁ", ViolationColor, 8);
        CreateMetadataBlock(wsOverview, 3, request.GeneratedBy, request.FromDate, request.ToDate, request.Building);

        // Khối thẻ KPI
        var kpiRow = 6;
        wsOverview.Cell(kpiRow, 1).Value = "CHỈ SỐ TỔNG HỢP";
        wsOverview.Range(kpiRow, 1, kpiRow, 8).Merge();
        StyleSectionHeader(wsOverview.Range(kpiRow, 1, kpiRow, 8), ViolationColor);

        var kpis = new (string Label, object Value)[]
        {
            ("Tổng số vi phạm", data.TotalViolations),
            ("Nhắc nhở", data.MinorCount),
            ("Khiển trách", data.ModerateCount),
            ("Cảnh cáo", data.SevereCount),
            ("Buộc rời KTX", data.CriticalCount),
            ("Đã giải quyết", data.ResolvedCount),
            ("Đang xử lý", data.PendingCount),
            ("Tổng điểm trừ", data.TotalDemeritPoints),
            ("Tổng tiền phạt", data.TotalFineAmount),
            ("Tỷ lệ giải quyết", (double)(data.ResolutionRate / 100)),
            ("Tỷ lệ nghiêm trọng", (double)(data.CriticalSeverityRate / 100))
        };

        var curRow = kpiRow + 1;
        foreach (var kpi in kpis)
        {
            wsOverview.Cell(curRow, 1).Value = kpi.Label;
            wsOverview.Cell(curRow, 1).Style.Font.Bold = true;
            wsOverview.Range(curRow, 1, curRow, 3).Merge();

            var valCell = wsOverview.Cell(curRow, 4);
            valCell.Value = XLCellValue.FromObject(kpi.Value);
            if (kpi.Label.Contains("tiền"))
            {
                valCell.Style.NumberFormat.Format = CurrencyFormat;
            }
            else if (kpi.Label.Contains("Tỷ lệ"))
            {
                valCell.Style.NumberFormat.Format = PercentFormat;
            }
            else
            {
                valCell.Style.NumberFormat.Format = IntegerFormat;
            }
            wsOverview.Range(curRow, 4, curRow, 5).Merge();
            curRow++;
        }
        ApplyThinBorders(wsOverview.Range(kpiRow + 1, 1, curRow - 1, 5));

        // Top 5 Sinh viên vi phạm nhiều nhất
        curRow += 2;
        wsOverview.Cell(curRow, 1).Value = "TOP 5 SINH VIÊN VI PHẠM NHIỀU NHẤT";
        wsOverview.Range(curRow, 1, curRow, 8).Merge();
        StyleSectionHeader(wsOverview.Range(curRow, 1, curRow, 8), ViolationColor);

        curRow++;
        var topStudentHeaders = new[] { "STT", "Mã SV", "Họ và tên", "Phòng", "Tòa nhà", "Số vi phạm", "Điểm trừ", "Tiền phạt" };
        CreateTableHeader(wsOverview, curRow, topStudentHeaders, ViolationColor);

        var stt = 1;
        var startStudentRow = curRow + 1;
        foreach (var st in data.TopStudents)
        {
            curRow++;
            wsOverview.Cell(curRow, 1).Value = stt++;
            wsOverview.Cell(curRow, 2).Value = st.StudentCode;
            wsOverview.Cell(curRow, 3).Value = st.FullName;
            wsOverview.Cell(curRow, 4).Value = st.RoomNumber;
            wsOverview.Cell(curRow, 5).Value = st.BuildingName;
            wsOverview.Cell(curRow, 6).Value = st.ViolationCount;
            wsOverview.Cell(curRow, 7).Value = st.DemeritPoints;
            wsOverview.Cell(curRow, 8).Value = st.TotalFines;
            wsOverview.Cell(curRow, 8).Style.NumberFormat.Format = CurrencyFormat;
        }
        if (data.TopStudents.Count > 0)
        {
            ApplyThinBorders(wsOverview.Range(startStudentRow, 1, curRow, 8));
        }

        // Top 5 Phòng xảy ra vi phạm
        curRow += 2;
        wsOverview.Cell(curRow, 1).Value = "TOP 5 PHÒNG XẢY RA VI PHẠM NHIỀU NHẤT";
        wsOverview.Range(curRow, 1, curRow, 4).Merge();
        StyleSectionHeader(wsOverview.Range(curRow, 1, curRow, 4), ViolationColor);

        curRow++;
        var topRoomHeaders = new[] { "STT", "Số phòng", "Tòa nhà", "Số vụ vi phạm" };
        CreateTableHeader(wsOverview, curRow, topRoomHeaders, ViolationColor);

        stt = 1;
        var startRoomRow = curRow + 1;
        foreach (var rm in data.TopRooms)
        {
            curRow++;
            wsOverview.Cell(curRow, 1).Value = stt++;
            wsOverview.Cell(curRow, 2).Value = rm.RoomNumber;
            wsOverview.Cell(curRow, 3).Value = rm.Building;
            wsOverview.Cell(curRow, 4).Value = rm.ViolationCount;
        }
        if (data.TopRooms.Count > 0)
        {
            ApplyThinBorders(wsOverview.Range(startRoomRow, 1, curRow, 4));
        }

        wsOverview.Columns().AdjustToContents();

        // Sheet 2: Bảng kê chi tiết
        var wsDetail = workbook.Worksheets.Add("Chi tiết vi phạm");
        wsDetail.ShowGridLines = true;

        CreateSheetTitle(wsDetail, "BẢNG KÊ CHI TIẾT BIÊN BẢN VI PHẠM KỶ LUẬT", ViolationColor, 14);
        var detailHeaders = new[]
        {
            "STT", "Mã biên bản", "Thời gian", "Mã SV", "Họ và tên", "Phòng", "Tòa nhà",
            "Hành vi vi phạm", "Mức độ", "Điểm trừ", "Tiền phạt", "Trạng thái", "Người lập", "Ghi chú xử lý"
        };
        CreateTableHeader(wsDetail, 3, detailHeaders, ViolationColor);

        var rIndex = 4;
        stt = 1;
        foreach (var it in data.Items)
        {
            wsDetail.Cell(rIndex, 1).Value = stt++;
            wsDetail.Cell(rIndex, 2).Value = it.ViolationCode;
            wsDetail.Cell(rIndex, 3).Value = it.ViolationDate.ToString(DateTimeFormat);
            wsDetail.Cell(rIndex, 4).Value = it.StudentCode;
            wsDetail.Cell(rIndex, 5).Value = it.StudentName;
            wsDetail.Cell(rIndex, 6).Value = it.RoomNumber;
            wsDetail.Cell(rIndex, 7).Value = it.BuildingName;
            wsDetail.Cell(rIndex, 8).Value = it.Title;
            wsDetail.Cell(rIndex, 9).Value = it.SeverityText;
            wsDetail.Cell(rIndex, 10).Value = it.DemeritPoints;
            wsDetail.Cell(rIndex, 11).Value = it.FineAmount;
            wsDetail.Cell(rIndex, 11).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 12).Value = it.StatusText;
            wsDetail.Cell(rIndex, 13).Value = it.RecordedBy ?? string.Empty;
            wsDetail.Cell(rIndex, 14).Value = it.ResolutionNotes ?? string.Empty;
            rIndex++;
        }
        if (data.Items.Count > 0)
        {
            ApplyThinBorders(wsDetail.Range(4, 1, rIndex - 1, 14));
        }
        wsDetail.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    private async Task<byte[]> GenerateFinancialExcelAsync(GenerateReportRequestDto request)
    {
        var data = await GetFinancialReportDataAsync(request);
        using var workbook = new XLWorkbook();

        // Sheet 1: Tổng hợp thu chi
        var wsOverview = workbook.Worksheets.Add("Tổng hợp thu chi");
        wsOverview.ShowGridLines = true;

        CreateSheetTitle(wsOverview, "BÁO CÁO DOANH THU & THU PHÍ KÝ TÚC XÁ", FinancialColor, 8);
        CreateMetadataBlock(wsOverview, 3, request.GeneratedBy, request.FromDate, request.ToDate, request.Building);

        // KPI Section
        var kpiRow = 6;
        wsOverview.Cell(kpiRow, 1).Value = "CHỈ SỐ TÀI CHÍNH TỔNG HỢP";
        wsOverview.Range(kpiRow, 1, kpiRow, 8).Merge();
        StyleSectionHeader(wsOverview.Range(kpiRow, 1, kpiRow, 8), FinancialColor);

        var kpis = new (string Label, object Value)[]
        {
            ("Tổng số hóa đơn", data.TotalBillsCount),
            ("Đã thanh toán", data.PaidBillsCount),
            ("Chưa thanh toán", data.UnpaidBillsCount),
            ("Quá hạn thanh toán", data.OverdueBillsCount),
            ("Tổng doanh thu dự kiến (Phải thu)", data.TotalExpectedRevenue),
            ("Tổng doanh thu thực thu", data.TotalCollectedRevenue),
            ("Doanh thu tiền phòng", data.RoomFeeCollected),
            ("Doanh thu tiền điện", data.ElectricFeeCollected),
            ("Doanh thu tiền nước", data.WaterFeeCollected),
            ("Doanh thu phụ phí dịch vụ", data.OtherFeeCollected),
            ("Công nợ tồn đọng", data.TotalOutstandingDebt),
            ("Công nợ quá hạn", data.OverdueDebt),
            ("Tỷ lệ thu hồi công nợ", (double)(data.CollectionRate / 100)),
            ("Tỷ lệ hóa đơn hoàn thành", (double)(data.PaidBillRate / 100)),
            ("Tổng điện tiêu thụ (kWh)", data.TotalElectricUsageKwh),
            ("Tổng nước tiêu thụ (m3)", data.TotalWaterUsageM3)
        };

        var curRow = kpiRow + 1;
        foreach (var kpi in kpis)
        {
            wsOverview.Cell(curRow, 1).Value = kpi.Label;
            wsOverview.Cell(curRow, 1).Style.Font.Bold = true;
            wsOverview.Range(curRow, 1, curRow, 3).Merge();

            var valCell = wsOverview.Cell(curRow, 4);
            valCell.Value = XLCellValue.FromObject(kpi.Value);
            if (kpi.Label.Contains("thu") || kpi.Label.Contains("Công nợ") || kpi.Label.Contains("phí") || kpi.Label.Contains("tiền"))
            {
                valCell.Style.NumberFormat.Format = CurrencyFormat;
            }
            else if (kpi.Label.Contains("Tỷ lệ"))
            {
                valCell.Style.NumberFormat.Format = PercentFormat;
            }
            else
            {
                valCell.Style.NumberFormat.Format = IntegerFormat;
            }
            wsOverview.Range(curRow, 4, curRow, 5).Merge();
            curRow++;
        }
        ApplyThinBorders(wsOverview.Range(kpiRow + 1, 1, curRow - 1, 5));

        // Thống kê theo Tòa nhà
        curRow += 2;
        wsOverview.Cell(curRow, 1).Value = "DOANH THU THEO TÒA NHÀ";
        wsOverview.Range(curRow, 1, curRow, 6).Merge();
        StyleSectionHeader(wsOverview.Range(curRow, 1, curRow, 6), FinancialColor);

        curRow++;
        var bldHeaders = new[] { "STT", "Tòa nhà", "Số hóa đơn", "Dự kiến thu", "Thực thu", "Công nợ tồn" };
        CreateTableHeader(wsOverview, curRow, bldHeaders, FinancialColor);

        var stt = 1;
        var startBldRow = curRow + 1;
        foreach (var bld in data.BuildingRevenues)
        {
            curRow++;
            wsOverview.Cell(curRow, 1).Value = stt++;
            wsOverview.Cell(curRow, 2).Value = bld.BuildingName;
            wsOverview.Cell(curRow, 3).Value = bld.BillsCount;
            wsOverview.Cell(curRow, 4).Value = bld.ExpectedRevenue;
            wsOverview.Cell(curRow, 4).Style.NumberFormat.Format = CurrencyFormat;
            wsOverview.Cell(curRow, 5).Value = bld.CollectedRevenue;
            wsOverview.Cell(curRow, 5).Style.NumberFormat.Format = CurrencyFormat;
            wsOverview.Cell(curRow, 6).Value = bld.OutstandingDebt;
            wsOverview.Cell(curRow, 6).Style.NumberFormat.Format = CurrencyFormat;
        }
        if (data.BuildingRevenues.Count > 0)
        {
            ApplyThinBorders(wsOverview.Range(startBldRow, 1, curRow, 6));
        }
        wsOverview.Columns().AdjustToContents();

        // Sheet 2: Bảng kê hóa đơn chi tiết
        var wsDetail = workbook.Worksheets.Add("Bảng kê hóa đơn");
        wsDetail.ShowGridLines = true;
        CreateSheetTitle(wsDetail, "BẢNG KÊ CHI TIẾT HÓA ĐƠN TIỀN PHÒNG & ĐIỆN NƯỚC", FinancialColor, 18);

        var billHeaders = new[]
        {
            "STT", "Mã HĐ", "Phòng", "Tòa nhà", "Tháng/Năm", "Tiền phòng",
            "Điện cũ", "Điện mới", "Điện SD (kWh)", "Tiền điện",
            "Nước cũ", "Nước mới", "Nước SD (m3)", "Tiền nước",
            "Phụ phí", "Tổng cộng", "Hạn nộp", "Ngày đóng", "Trạng thái"
        };
        CreateTableHeader(wsDetail, 3, billHeaders, FinancialColor);

        var rIndex = 4;
        stt = 1;
        foreach (var it in data.Items)
        {
            wsDetail.Cell(rIndex, 1).Value = stt++;
            wsDetail.Cell(rIndex, 2).Value = it.BillCode;
            wsDetail.Cell(rIndex, 3).Value = it.RoomNumber;
            wsDetail.Cell(rIndex, 4).Value = it.BuildingName;
            wsDetail.Cell(rIndex, 5).Value = $"{it.Month:D2}/{it.Year}";
            wsDetail.Cell(rIndex, 6).Value = it.RoomFee;
            wsDetail.Cell(rIndex, 6).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 7).Value = it.OldElectricIndex;
            wsDetail.Cell(rIndex, 8).Value = it.NewElectricIndex;
            wsDetail.Cell(rIndex, 9).Value = it.ElectricUsage;
            wsDetail.Cell(rIndex, 10).Value = it.ElectricFee;
            wsDetail.Cell(rIndex, 10).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 11).Value = it.OldWaterIndex;
            wsDetail.Cell(rIndex, 12).Value = it.NewWaterIndex;
            wsDetail.Cell(rIndex, 13).Value = it.WaterUsage;
            wsDetail.Cell(rIndex, 14).Value = it.WaterFee;
            wsDetail.Cell(rIndex, 14).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 15).Value = it.OtherServiceFee;
            wsDetail.Cell(rIndex, 15).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 16).Value = it.TotalAmount;
            wsDetail.Cell(rIndex, 16).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 17).Value = it.DueDate.ToString(DateFormat);
            wsDetail.Cell(rIndex, 18).Value = it.PaidAt?.ToString(DateFormat) ?? "-";
            wsDetail.Cell(rIndex, 19).Value = it.StatusText;
            rIndex++;
        }
        if (data.Items.Count > 0)
        {
            ApplyThinBorders(wsDetail.Range(4, 1, rIndex - 1, 19));
        }
        wsDetail.Columns().AdjustToContents();

        // Sheet 3: Công nợ quá hạn
        var wsOverdue = workbook.Worksheets.Add("Công nợ quá hạn");
        wsOverdue.ShowGridLines = true;
        CreateSheetTitle(wsOverdue, "DANH SÁCH HÓA ĐƠN NỢ ĐỌNG QUÁ HẠN", FinancialColor, 8);

        var overdueHeaders = new[] { "STT", "Mã HĐ", "Phòng", "Tòa nhà", "Kỳ hóa đơn", "Tổng nợ", "Hạn thanh toán", "Số ngày trễ", "Trạng thái" };
        CreateTableHeader(wsOverdue, 3, overdueHeaders, FinancialColor);

        rIndex = 4;
        stt = 1;
        foreach (var it in data.OverdueBills)
        {
            wsOverdue.Cell(rIndex, 1).Value = stt++;
            wsOverdue.Cell(rIndex, 2).Value = it.BillCode;
            wsOverdue.Cell(rIndex, 3).Value = it.RoomNumber;
            wsOverdue.Cell(rIndex, 4).Value = it.BuildingName;
            wsOverdue.Cell(rIndex, 5).Value = $"{it.Month:D2}/{it.Year}";
            wsOverdue.Cell(rIndex, 6).Value = it.TotalAmount;
            wsOverdue.Cell(rIndex, 6).Style.NumberFormat.Format = CurrencyFormat;
            wsOverdue.Cell(rIndex, 7).Value = it.DueDate.ToString(DateFormat);
            wsOverdue.Cell(rIndex, 8).Value = it.OverdueDays;
            wsOverdue.Cell(rIndex, 9).Value = it.StatusText;
            rIndex++;
        }
        if (data.OverdueBills.Count > 0)
        {
            ApplyThinBorders(wsOverdue.Range(4, 1, rIndex - 1, 9));
        }
        wsOverdue.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    private async Task<byte[]> GenerateOccupancyExcelAsync(GenerateReportRequestDto request)
    {
        var data = await GetOccupancyReportDataAsync(request);
        using var workbook = new XLWorkbook();

        // Sheet 1: Tổng quan lấp đầy
        var wsOverview = workbook.Worksheets.Add("Tổng quan lấp đầy");
        wsOverview.ShowGridLines = true;

        CreateSheetTitle(wsOverview, "BÁO CÁO CÔNG SUẤT & TỶ LỆ LẤP ĐẦY PHÒNG KÝ TÚC XÁ", OccupancyColor, 8);
        CreateMetadataBlock(wsOverview, 3, request.GeneratedBy, request.FromDate, request.ToDate, request.Building);

        var kpiRow = 6;
        wsOverview.Cell(kpiRow, 1).Value = "CHỈ SỐ CÔNG SUẤT PHÒNG";
        wsOverview.Range(kpiRow, 1, kpiRow, 8).Merge();
        StyleSectionHeader(wsOverview.Range(kpiRow, 1, kpiRow, 8), OccupancyColor);

        var kpis = new (string Label, object Value)[]
        {
            ("Tổng số phòng quản lý", data.TotalRooms),
            ("Tổng sức chứa (giường)", data.TotalBedsCapacity),
            ("Giường đang ở thực tế", data.OccupiedBeds),
            ("Giường còn trống khả dụng", data.AvailableBeds),
            ("Tỷ lệ lấp đầy toàn KTX", (double)(data.OccupancyRate / 100)),
            ("Số phòng đã kín chỗ", data.OccupiedRoomsCount),
            ("Số phòng còn chỗ", data.AvailableRoomsCount),
            ("Số phòng đang bảo trì", data.MaintenanceRoomsCount),
            ("Số phòng trống hoàn toàn", data.EmptyRoomsCount),
            ("Sức chứa giường Nam", data.MaleBedsCapacity),
            ("Giường Nam đang ở", data.MaleBedsOccupied),
            ("Sức chứa giường Nữ", data.FemaleBedsCapacity),
            ("Giường Nữ đang ở", data.FemaleBedsOccupied)
        };

        var curRow = kpiRow + 1;
        foreach (var kpi in kpis)
        {
            wsOverview.Cell(curRow, 1).Value = kpi.Label;
            wsOverview.Cell(curRow, 1).Style.Font.Bold = true;
            wsOverview.Range(curRow, 1, curRow, 3).Merge();

            var valCell = wsOverview.Cell(curRow, 4);
            valCell.Value = XLCellValue.FromObject(kpi.Value);
            if (kpi.Label.Contains("Tỷ lệ"))
            {
                valCell.Style.NumberFormat.Format = PercentFormat;
            }
            else
            {
                valCell.Style.NumberFormat.Format = IntegerFormat;
            }
            wsOverview.Range(curRow, 4, curRow, 5).Merge();
            curRow++;
        }
        ApplyThinBorders(wsOverview.Range(kpiRow + 1, 1, curRow - 1, 5));

        // Thống kê theo Tòa nhà
        curRow += 2;
        wsOverview.Cell(curRow, 1).Value = "TỶ LỆ LẤP ĐẦY THEO TÒA NHÀ";
        wsOverview.Range(curRow, 1, curRow, 7).Merge();
        StyleSectionHeader(wsOverview.Range(curRow, 1, curRow, 7), OccupancyColor);

        curRow++;
        var bldHeaders = new[] { "STT", "Tòa nhà", "Tổng phòng", "Sức chứa", "Đang ở", "Còn trống", "Tỷ lệ lấp đầy" };
        CreateTableHeader(wsOverview, curRow, bldHeaders, OccupancyColor);

        var stt = 1;
        var startBldRow = curRow + 1;
        foreach (var bld in data.Buildings)
        {
            curRow++;
            wsOverview.Cell(curRow, 1).Value = stt++;
            wsOverview.Cell(curRow, 2).Value = bld.BuildingName;
            wsOverview.Cell(curRow, 3).Value = bld.TotalRooms;
            wsOverview.Cell(curRow, 4).Value = bld.TotalBedsCapacity;
            wsOverview.Cell(curRow, 5).Value = bld.OccupiedBeds;
            wsOverview.Cell(curRow, 6).Value = bld.AvailableBeds;
            wsOverview.Cell(curRow, 7).Value = (double)(bld.OccupancyRate / 100);
            wsOverview.Cell(curRow, 7).Style.NumberFormat.Format = PercentFormat;
        }
        if (data.Buildings.Count > 0)
        {
            ApplyThinBorders(wsOverview.Range(startBldRow, 1, curRow, 7));
        }
        wsOverview.Columns().AdjustToContents();

        // Sheet 2: Danh sách phòng chi tiết
        var wsDetail = workbook.Worksheets.Add("Danh sách phòng");
        wsDetail.ShowGridLines = true;
        CreateSheetTitle(wsDetail, "DANH SÁCH CHI TIẾT TỪNG PHÒNG Ở", OccupancyColor, 11);

        var roomHeaders = new[] { "STT", "Số phòng", "Tòa nhà", "Tầng", "Loại phòng", "Giới tính", "Sức chứa", "Đang ở", "Còn trống", "Đơn giá", "Trạng thái" };
        CreateTableHeader(wsDetail, 3, roomHeaders, OccupancyColor);

        // Lấy danh sách toàn bộ phòng
        var allRooms = await _context.Rooms.AsNoTracking().OrderBy(r => r.Building).ThenBy(r => r.RoomNumber).ToListAsync();
        var rIndex = 4;
        stt = 1;
        foreach (var r in allRooms)
        {
            wsDetail.Cell(rIndex, 1).Value = stt++;
            wsDetail.Cell(rIndex, 2).Value = r.RoomNumber;
            wsDetail.Cell(rIndex, 3).Value = r.Building;
            wsDetail.Cell(rIndex, 4).Value = r.Floor;
            wsDetail.Cell(rIndex, 5).Value = GetRoomTypeText(r.Type);
            wsDetail.Cell(rIndex, 6).Value = r.AllowedGender == Gender.Male ? "Nam" : "Nữ";
            wsDetail.Cell(rIndex, 7).Value = r.Capacity;
            wsDetail.Cell(rIndex, 8).Value = r.CurrentOccupancy;
            wsDetail.Cell(rIndex, 9).Value = Math.Max(0, r.Capacity - r.CurrentOccupancy);
            wsDetail.Cell(rIndex, 10).Value = r.PricePerMonth;
            wsDetail.Cell(rIndex, 10).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 11).Value = GetRoomStatusText(r.Status);
            rIndex++;
        }
        if (allRooms.Count > 0)
        {
            ApplyThinBorders(wsDetail.Range(4, 1, rIndex - 1, 11));
        }
        wsDetail.Columns().AdjustToContents();

        // Sheet 3: Phòng còn chỗ khả dụng
        var wsVacant = workbook.Worksheets.Add("Phòng còn chỗ trống");
        wsVacant.ShowGridLines = true;
        CreateSheetTitle(wsVacant, "DANH SÁCH PHÒNG CÒN CHỖ KHẢ DỤNG ĐIỀU PHỐI", OccupancyColor, 10);

        var vacantHeaders = new[] { "STT", "Số phòng", "Tòa nhà", "Tầng", "Loại phòng", "Giới tính", "Sức chứa", "Đang ở", "Chỗ trống", "Đơn giá" };
        CreateTableHeader(wsVacant, 3, vacantHeaders, OccupancyColor);

        rIndex = 4;
        stt = 1;
        foreach (var vr in data.VacantRooms)
        {
            wsVacant.Cell(rIndex, 1).Value = stt++;
            wsVacant.Cell(rIndex, 2).Value = vr.RoomNumber;
            wsVacant.Cell(rIndex, 3).Value = vr.BuildingName;
            wsVacant.Cell(rIndex, 4).Value = vr.Floor;
            wsVacant.Cell(rIndex, 5).Value = vr.RoomTypeText;
            wsVacant.Cell(rIndex, 6).Value = vr.AllowedGenderText;
            wsVacant.Cell(rIndex, 7).Value = vr.Capacity;
            wsVacant.Cell(rIndex, 8).Value = vr.CurrentOccupancy;
            wsVacant.Cell(rIndex, 9).Value = vr.VacantBeds;
            wsVacant.Cell(rIndex, 10).Value = vr.PricePerMonth;
            wsVacant.Cell(rIndex, 10).Style.NumberFormat.Format = CurrencyFormat;
            rIndex++;
        }
        if (data.VacantRooms.Count > 0)
        {
            ApplyThinBorders(wsVacant.Range(4, 1, rIndex - 1, 10));
        }
        wsVacant.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    private async Task<byte[]> GenerateEquipmentExcelAsync(GenerateReportRequestDto request)
    {
        var data = await GetEquipmentReportDataAsync(request);
        using var workbook = new XLWorkbook();

        // Sheet 1: Tổng hợp kiểm kê
        var wsOverview = workbook.Worksheets.Add("Tổng hợp kiểm kê");
        wsOverview.ShowGridLines = true;

        CreateSheetTitle(wsOverview, "BÁO CÁO KIỂM KÊ TÀI SẢN & TRANG THIẾT BỊ", EquipmentColor, 8);
        CreateMetadataBlock(wsOverview, 3, request.GeneratedBy, request.FromDate, request.ToDate, request.Building);

        var kpiRow = 6;
        wsOverview.Cell(kpiRow, 1).Value = "CHỈ SỐ KIỂM KÊ TÀI SẢN";
        wsOverview.Range(kpiRow, 1, kpiRow, 8).Merge();
        StyleSectionHeader(wsOverview.Range(kpiRow, 1, kpiRow, 8), EquipmentColor);

        var kpis = new (string Label, object Value)[]
        {
            ("Tổng số lượng thiết bị", data.TotalEquipmentCount),
            ("Tổng giá trị tài sản", data.TotalAssetValue),
            ("Số lượng hoạt động tốt", data.GoodConditionCount),
            ("Số lượng cần sửa chữa", data.NeedsRepairCount),
            ("Số lượng hỏng hóc", data.BrokenCount),
            ("Tỷ lệ thiết bị tốt", (double)(data.HealthRate / 100)),
            ("Tỷ lệ thiết bị lỗi / hỏng", (double)(data.FaultyRate / 100)),
            ("Giá trị tài sản hư hại", data.DamagedAssetValue),
            ("Ước tính kinh phí sửa chữa", data.EstimatedRepairCost),
            ("Quá hạn kiểm tra bảo trì", data.OverdueMaintenanceCount)
        };

        var curRow = kpiRow + 1;
        foreach (var kpi in kpis)
        {
            wsOverview.Cell(curRow, 1).Value = kpi.Label;
            wsOverview.Cell(curRow, 1).Style.Font.Bold = true;
            wsOverview.Range(curRow, 1, curRow, 3).Merge();

            var valCell = wsOverview.Cell(curRow, 4);
            valCell.Value = XLCellValue.FromObject(kpi.Value);
            if (kpi.Label.Contains("giá trị") || kpi.Label.Contains("kinh phí") || kpi.Label.Contains("Giá trị"))
            {
                valCell.Style.NumberFormat.Format = CurrencyFormat;
            }
            else if (kpi.Label.Contains("Tỷ lệ"))
            {
                valCell.Style.NumberFormat.Format = PercentFormat;
            }
            else
            {
                valCell.Style.NumberFormat.Format = IntegerFormat;
            }
            wsOverview.Range(curRow, 4, curRow, 5).Merge();
            curRow++;
        }
        ApplyThinBorders(wsOverview.Range(kpiRow + 1, 1, curRow - 1, 5));

        // Phân loại trạng thái
        curRow += 2;
        wsOverview.Cell(curRow, 1).Value = "PHÂN LOẠI THEO HIỆN TRẠNG SỬ DỤNG";
        wsOverview.Range(curRow, 1, curRow, 5).Merge();
        StyleSectionHeader(wsOverview.Range(curRow, 1, curRow, 5), EquipmentColor);

        curRow++;
        var statusHeaders = new[] { "STT", "Hiện trạng", "Số lượng", "Tổng giá trị", "Tỷ lệ (%)" };
        CreateTableHeader(wsOverview, curRow, statusHeaders, EquipmentColor);

        var stt = 1;
        var startStatusRow = curRow + 1;
        foreach (var st in data.StatusBreakdowns)
        {
            curRow++;
            wsOverview.Cell(curRow, 1).Value = stt++;
            wsOverview.Cell(curRow, 2).Value = st.StatusText;
            wsOverview.Cell(curRow, 3).Value = st.Quantity;
            wsOverview.Cell(curRow, 4).Value = st.TotalValue;
            wsOverview.Cell(curRow, 4).Style.NumberFormat.Format = CurrencyFormat;
            wsOverview.Cell(curRow, 5).Value = (double)(st.Percentage / 100);
            wsOverview.Cell(curRow, 5).Style.NumberFormat.Format = PercentFormat;
        }
        if (data.StatusBreakdowns.Count > 0)
        {
            ApplyThinBorders(wsOverview.Range(startStatusRow, 1, curRow, 5));
        }

        // Bảng theo Tòa nhà
        curRow += 2;
        wsOverview.Cell(curRow, 1).Value = "TRANG THIẾT BỊ THEO TÒA NHÀ";
        wsOverview.Range(curRow, 1, curRow, 7).Merge();
        StyleSectionHeader(wsOverview.Range(curRow, 1, curRow, 7), EquipmentColor);

        curRow++;
        var bldHeaders = new[] { "STT", "Tòa nhà", "Tổng số lượng", "Tốt", "Cần sửa", "Hỏng", "Tổng giá trị" };
        CreateTableHeader(wsOverview, curRow, bldHeaders, EquipmentColor);

        stt = 1;
        var startBldRow = curRow + 1;
        foreach (var bld in data.BuildingEquipments)
        {
            curRow++;
            wsOverview.Cell(curRow, 1).Value = stt++;
            wsOverview.Cell(curRow, 2).Value = bld.BuildingName;
            wsOverview.Cell(curRow, 3).Value = bld.TotalQuantity;
            wsOverview.Cell(curRow, 4).Value = bld.GoodQuantity;
            wsOverview.Cell(curRow, 5).Value = bld.NeedsRepairQuantity;
            wsOverview.Cell(curRow, 6).Value = bld.BrokenQuantity;
            wsOverview.Cell(curRow, 7).Value = bld.TotalValue;
            wsOverview.Cell(curRow, 7).Style.NumberFormat.Format = CurrencyFormat;
        }
        if (data.BuildingEquipments.Count > 0)
        {
            ApplyThinBorders(wsOverview.Range(startBldRow, 1, curRow, 7));
        }
        wsOverview.Columns().AdjustToContents();

        // Sheet 2: Danh mục thiết bị chi tiết
        var wsDetail = workbook.Worksheets.Add("Danh mục thiết bị");
        wsDetail.ShowGridLines = true;
        CreateSheetTitle(wsDetail, "DANH MỤC KIỂM KÊ TRANG THIẾT BỊ CHI TIẾT", EquipmentColor, 12);

        var equipHeaders = new[]
        {
            "STT", "Mã thiết bị", "Tên thiết bị", "Phòng", "Tòa nhà",
            "Số lượng", "Đơn giá", "Thành tiền", "Hiện trạng", "Ngày trang bị", "Bảo trì cuối", "Ghi chú"
        };
        CreateTableHeader(wsDetail, 3, equipHeaders, EquipmentColor);

        var rIndex = 4;
        stt = 1;
        foreach (var it in data.Items)
        {
            wsDetail.Cell(rIndex, 1).Value = stt++;
            wsDetail.Cell(rIndex, 2).Value = it.EquipmentCode;
            wsDetail.Cell(rIndex, 3).Value = it.Name;
            wsDetail.Cell(rIndex, 4).Value = it.RoomNumber;
            wsDetail.Cell(rIndex, 5).Value = it.BuildingName;
            wsDetail.Cell(rIndex, 6).Value = it.Quantity;
            wsDetail.Cell(rIndex, 7).Value = it.Price;
            wsDetail.Cell(rIndex, 7).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 8).Value = it.TotalValue;
            wsDetail.Cell(rIndex, 8).Style.NumberFormat.Format = CurrencyFormat;
            wsDetail.Cell(rIndex, 9).Value = it.StatusText;
            wsDetail.Cell(rIndex, 10).Value = it.AssignedDate?.ToString(DateFormat) ?? "-";
            wsDetail.Cell(rIndex, 11).Value = it.LastMaintainedAt?.ToString(DateFormat) ?? "-";
            wsDetail.Cell(rIndex, 12).Value = it.Notes ?? string.Empty;
            rIndex++;
        }
        if (data.Items.Count > 0)
        {
            ApplyThinBorders(wsDetail.Range(4, 1, rIndex - 1, 12));
        }
        wsDetail.Columns().AdjustToContents();

        // Sheet 3: Thiết bị cần sửa chữa / hỏng
        var wsFaulty = workbook.Worksheets.Add("Đề xuất sửa chữa");
        wsFaulty.ShowGridLines = true;
        CreateSheetTitle(wsFaulty, "DANH SÁCH THIẾT BỊ HƯ HỎNG & ĐỀ XUẤT SỬA CHỮA", EquipmentColor, 9);

        var faultyHeaders = new[] { "STT", "Mã TB", "Tên thiết bị", "Phòng", "Tòa nhà", "Số lượng", "Đơn giá", "Hiện trạng", "Ghi chú lỗi" };
        CreateTableHeader(wsFaulty, 3, faultyHeaders, EquipmentColor);

        rIndex = 4;
        stt = 1;
        foreach (var it in data.FaultyEquipments)
        {
            wsFaulty.Cell(rIndex, 1).Value = stt++;
            wsFaulty.Cell(rIndex, 2).Value = it.EquipmentCode;
            wsFaulty.Cell(rIndex, 3).Value = it.Name;
            wsFaulty.Cell(rIndex, 4).Value = it.RoomNumber;
            wsFaulty.Cell(rIndex, 5).Value = it.BuildingName;
            wsFaulty.Cell(rIndex, 6).Value = it.Quantity;
            wsFaulty.Cell(rIndex, 7).Value = it.Price;
            wsFaulty.Cell(rIndex, 7).Style.NumberFormat.Format = CurrencyFormat;
            wsFaulty.Cell(rIndex, 8).Value = it.StatusText;
            wsFaulty.Cell(rIndex, 9).Value = it.Notes ?? string.Empty;
            rIndex++;
        }
        if (data.FaultyEquipments.Count > 0)
        {
            ApplyThinBorders(wsFaulty.Range(4, 1, rIndex - 1, 9));
        }
        wsFaulty.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    #region Helper Định Dạng Excel

    private static void CreateSheetTitle(IXLWorksheet ws, string title, XLColor bg, int colSpan)
    {
        ws.Row(1).Height = 34;
        var cell = ws.Cell(1, 1);
        cell.Value = title;
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = 14;
        cell.Style.Font.FontColor = XLColor.White;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        var range = ws.Range(1, 1, 1, colSpan);
        range.Merge();
        range.Style.Fill.BackgroundColor = bg;
    }

    private static void CreateMetadataBlock(IXLWorksheet ws, int startRow, string? generatedBy, DateTime? fromDate, DateTime? toDate, string? building)
    {
        var now = DateTime.Now;
        var info = $"Ngày xuất: {now:dd/MM/yyyy HH:mm}   |   Người lập: {(string.IsNullOrWhiteSpace(generatedBy) ? "Hệ thống" : generatedBy)}";
        if (fromDate.HasValue || toDate.HasValue)
        {
            info += $"   |   Thời kỳ: {(fromDate.HasValue ? fromDate.Value.ToString(DateFormat) : "Từ trước")} - {(toDate.HasValue ? toDate.Value.ToString(DateFormat) : "Hiện tại")}";
        }
        if (!string.IsNullOrWhiteSpace(building))
        {
            info += $"   |   Phạm vi: {building}";
        }

        ws.Cell(startRow, 1).Value = info;
        ws.Cell(startRow, 1).Style.Font.Italic = true;
        ws.Cell(startRow, 1).Style.Font.FontSize = 9.5;
        ws.Cell(startRow, 1).Style.Font.FontColor = XLColor.FromHtml("#4B5563");
    }

    private static void StyleSectionHeader(IXLRange range, XLColor bg)
    {
        range.Style.Fill.BackgroundColor = XLColor.FromArgb(0xF3, 0xF4, 0xF6);
        range.Style.Font.Bold = true;
        range.Style.Font.FontColor = bg;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    private static void CreateTableHeader(IXLWorksheet ws, int row, string[] headers, XLColor bg)
    {
        ws.Row(row).Height = 24;
        for (var i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(row, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = bg;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
    }

    private static void ApplyThinBorders(IXLRange range)
    {
        var border = range.Style.Border;
        border.OutsideBorder = XLBorderStyleValues.Thin;
        border.InsideBorder = XLBorderStyleValues.Thin;
        var color = XLColor.FromArgb(0xDC, 0xDF, 0xE4);
        border.OutsideBorderColor = color;
        border.InsideBorderColor = color;
    }

    #endregion

    #endregion

    #region 3. XUẤT BẢN PDF (QUESTPDF)

    /// <summary>
    /// Xuất báo cáo định dạng Adobe PDF (.pdf) qua QuestPDF
    /// </summary>
    public async Task<byte[]> GeneratePdfReportAsync(GenerateReportRequestDto request)
    {
        NormalizeReportType(request);

        return request.ReportType switch
        {
            ReportType.Violations => await GenerateViolationPdfAsync(request),
            ReportType.Financial => await GenerateFinancialPdfAsync(request),
            ReportType.Occupancy => await GenerateOccupancyPdfAsync(request),
            ReportType.Equipment => await GenerateEquipmentPdfAsync(request),
            _ => await GenerateViolationPdfAsync(request)
        };
    }

    private async Task<byte[]> GenerateViolationPdfAsync(GenerateReportRequestDto request)
    {
        var data = await GetViolationReportDataAsync(request);
        const string primaryColor = "#C42B1C";

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(header => ComposePdfHeader(header, "BÁO CÁO VI PHẠM NỘI QUY & KỶ LUẬT", primaryColor, request));

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Spacing(12);

                        // KPI Cards Grid
                        col.Item().Row(r =>
                        {
                            r.Spacing(8);
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TỔNG VI PHẠM", $"{data.TotalViolations} vụ", primaryColor));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "ĐÃ XỬ LÝ", $"{data.ResolvedCount} vụ", "#107C41"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "ĐIỂM TRỪ", $"{data.TotalDemeritPoints} điểm", "#D97706"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TIỀN PHẠT", $"{data.TotalFineAmount:N0} đ", primaryColor));
                        });

                        // Bảng tóm tắt theo mức độ
                        col.Item().Text("1. Cơ cấu mức độ vi phạm").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(5).Text("Mức độ").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(5).AlignCenter().Text("Số lượng").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(5).AlignRight().Text("Tỷ lệ").Bold().FontColor("#FFFFFF");
                            });

                            var sevList = new (string Label, int Count)[]
                            {
                                ("Nhắc nhở (Minor)", data.MinorCount),
                                ("Khiển trách (Moderate)", data.ModerateCount),
                                ("Cảnh cáo (Severe)", data.SevereCount),
                                ("Buộc rời KTX (Critical)", data.CriticalCount)
                            };

                            foreach (var s in sevList)
                            {
                                var pct = data.TotalViolations > 0 ? (double)s.Count / data.TotalViolations * 100 : 0;
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.Label);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text(s.Count.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{pct:F1}%");
                            }
                        });

                        // Bảng danh sách vi phạm tiêu biểu
                        col.Item().PaddingTop(6).Text("2. Danh sách các biên bản vi phạm").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(25);
                                cols.ConstantColumn(65);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.ConstantColumn(55);
                                cols.ConstantColumn(60);
                                cols.ConstantColumn(55);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("STT").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Mã BB").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Sinh viên").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Hành vi").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Mức độ").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignRight().Text("Tiền phạt").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Trạng thái").Bold().FontColor("#FFFFFF");
                            });

                            var stt = 1;
                            foreach (var it in data.Items.Take(15))
                            {
                                var bg = stt % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(stt++.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(it.ViolationCode);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text($"{it.StudentName} ({it.StudentCode})");
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(it.Title);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(it.SeverityText);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text($"{it.FineAmount:N0} đ");
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(it.StatusText);
                            }
                        });

                        if (data.Items.Count > 15)
                        {
                            col.Item().Text($"*(Hiển thị 15/{data.Items.Count} bản ghi tiêu biểu. Xem chi tiết đầy đủ trong tệp Excel)*")
                                .Italic().FontSize(8).FontColor(Colors.Grey.Darken1);
                        }

                        // Khối chữ ký xác nhận
                        col.Item().Element(ComposeSignatureBlock);
                    });
                });

                page.Footer().Element(ComposePdfFooter);
            });
        });

        return doc.GeneratePdf();
    }

    private async Task<byte[]> GenerateFinancialPdfAsync(GenerateReportRequestDto request)
    {
        var data = await GetFinancialReportDataAsync(request);
        const string primaryColor = "#107C41";

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(header => ComposePdfHeader(header, "BÁO CÁO DOANH THU & THU PHÍ KÝ TÚC XÁ", primaryColor, request));

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Spacing(12);

                        // KPI Cards
                        col.Item().Row(r =>
                        {
                            r.Spacing(8);
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "DỰ KIẾN THU", $"{data.TotalExpectedRevenue:N0} đ", primaryColor));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "THỰC THU", $"{data.TotalCollectedRevenue:N0} đ", "#0078D4"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "CÔNG NỢ TỒN", $"{data.TotalOutstandingDebt:N0} đ", "#D97706"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TỶ LỆ THU", $"{data.CollectionRate:F1}%", primaryColor));
                        });

                        // Bảng cơ cấu nguồn thu
                        col.Item().Text("1. Cơ cấu doanh thu thực thu").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(5).Text("Khoản mục").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(5).AlignRight().Text("Số tiền thực thu").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(5).AlignRight().Text("Tỷ lệ").Bold().FontColor("#FFFFFF");
                            });

                            var feeList = new (string Label, decimal Amount)[]
                            {
                                ("Tiền phòng lưu trú", data.RoomFeeCollected),
                                ("Tiền điện sinh hoạt", data.ElectricFeeCollected),
                                ("Tiền nước sinh hoạt", data.WaterFeeCollected),
                                ("Phụ phí dịch vụ khác", data.OtherFeeCollected)
                            };

                            foreach (var f in feeList)
                            {
                                var pct = data.TotalCollectedRevenue > 0 ? (double)(f.Amount / data.TotalCollectedRevenue) * 100 : 0;
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(f.Label);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{f.Amount:N0} đ");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{pct:F1}%");
                            }
                        });

                        // Bảng thống kê theo Tòa nhà
                        col.Item().PaddingTop(6).Text("2. Doanh thu theo từng tòa nhà").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.ConstantColumn(50);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(4).Text("Tòa nhà").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Số HĐ").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignRight().Text("Dự kiến").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignRight().Text("Thực thu").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignRight().Text("Công nợ").Bold().FontColor("#FFFFFF");
                            });

                            var stt = 1;
                            foreach (var b in data.BuildingRevenues)
                            {
                                var bg = stt++ % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(b.BuildingName);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(b.BillsCount.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text($"{b.ExpectedRevenue:N0} đ");
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text($"{b.CollectedRevenue:N0} đ");
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text($"{b.OutstandingDebt:N0} đ");
                            }
                        });

                        col.Item().Element(ComposeSignatureBlock);
                    });
                });

                page.Footer().Element(ComposePdfFooter);
            });
        });

        return doc.GeneratePdf();
    }

    private async Task<byte[]> GenerateOccupancyPdfAsync(GenerateReportRequestDto request)
    {
        var data = await GetOccupancyReportDataAsync(request);
        const string primaryColor = "#0078D4";

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(header => ComposePdfHeader(header, "BÁO CÁO CÔNG SUẤT & TỶ LỆ LẤP ĐẦY PHÒNG", primaryColor, request));

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Spacing(12);

                        // KPI Cards
                        col.Item().Row(r =>
                        {
                            r.Spacing(8);
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TỔNG PHÒNG", $"{data.TotalRooms} phòng", primaryColor));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "SỨC CHỨA", $"{data.TotalBedsCapacity} giường", "#107C41"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "CHỖ TRỐNG", $"{data.AvailableBeds} chỗ", "#D97706"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TỶ LỆ LẤP ĐẦY", $"{data.OccupancyRate:F1}%", primaryColor));
                        });

                        // Bảng lấp đầy theo tòa nhà
                        col.Item().Text("1. Tỷ lệ lấp đầy theo từng tòa nhà").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.ConstantColumn(60);
                                cols.ConstantColumn(65);
                                cols.ConstantColumn(60);
                                cols.ConstantColumn(60);
                                cols.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(4).Text("Tòa nhà").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Tổng phòng").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Sức chứa").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Đang ở").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Còn trống").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignRight().Text("Lấp đầy (%)").Bold().FontColor("#FFFFFF");
                            });

                            var stt = 1;
                            foreach (var b in data.Buildings)
                            {
                                var bg = stt++ % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(b.BuildingName);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(b.TotalRooms.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(b.TotalBedsCapacity.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(b.OccupiedBeds.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(b.AvailableBeds.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text($"{b.OccupancyRate:F1}%");
                            }
                        });

                        // Danh sách phòng còn chỗ trống điều phối
                        col.Item().PaddingTop(6).Text("2. Danh sách phòng còn chỗ khả dụng điều phối").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(25);
                                cols.ConstantColumn(55);
                                cols.ConstantColumn(55);
                                cols.RelativeColumn(2);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(50);
                                cols.ConstantColumn(55);
                                cols.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("STT").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Số phòng").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Tòa nhà").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Loại phòng").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Giới tính").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Sức chứa").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Chỗ trống").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignRight().Text("Đơn giá").Bold().FontColor("#FFFFFF");
                            });

                            var stt = 1;
                            foreach (var vr in data.VacantRooms.Take(12))
                            {
                                var bg = stt % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(stt++.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(vr.RoomNumber);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(vr.BuildingName);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(vr.RoomTypeText);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(vr.AllowedGenderText);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(vr.Capacity.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(vr.VacantBeds.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text($"{vr.PricePerMonth:N0} đ");
                            }
                        });

                        col.Item().Element(ComposeSignatureBlock);
                    });
                });

                page.Footer().Element(ComposePdfFooter);
            });
        });

        return doc.GeneratePdf();
    }

    private async Task<byte[]> GenerateEquipmentPdfAsync(GenerateReportRequestDto request)
    {
        var data = await GetEquipmentReportDataAsync(request);
        const string primaryColor = "#008272";

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Header().Element(header => ComposePdfHeader(header, "BÁO CÁO KIỂM KÊ TÀI SẢN & THIẾT BỊ", primaryColor, request));

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Spacing(12);

                        // KPI Cards
                        col.Item().Row(r =>
                        {
                            r.Spacing(8);
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TỔNG THIẾT BỊ", $"{data.TotalEquipmentCount} món", primaryColor));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TỔNG GIÁ TRỊ", $"{data.TotalAssetValue:N0} đ", "#107C41"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "CẦN SỬA/HỎNG", $"{data.NeedsRepairCount + data.BrokenCount} món", "#D97706"));
                            r.RelativeItem().Element(c => ComposeKpiCard(c, "TỶ LỆ TỐT", $"{data.HealthRate:F1}%", primaryColor));
                        });

                        // Phân bố hiện trạng
                        col.Item().Text("1. Phân loại theo hiện trạng sử dụng").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(5).Text("Hiện trạng").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(5).AlignCenter().Text("Số lượng").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(5).AlignRight().Text("Tổng giá trị").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(5).AlignRight().Text("Tỷ lệ").Bold().FontColor("#FFFFFF");
                            });

                            foreach (var s in data.StatusBreakdowns)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.StatusText);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text(s.Quantity.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{s.TotalValue:N0} đ");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{s.Percentage:F1}%");
                            }
                        });

                        // Danh sách thiết bị hỏng cần sửa chữa
                        col.Item().PaddingTop(6).Text("2. Danh sách thiết bị sự cố & hỏng hóc").Bold().FontSize(10.5f).FontColor(primaryColor);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(25);
                                cols.ConstantColumn(60);
                                cols.RelativeColumn(3);
                                cols.ConstantColumn(55);
                                cols.ConstantColumn(55);
                                cols.ConstantColumn(50);
                                cols.RelativeColumn(2);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("STT").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Mã TB").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Tên thiết bị").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Phòng").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).Text("Tòa").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("SL").Bold().FontColor("#FFFFFF");
                                h.Cell().Background(primaryColor).Padding(4).AlignCenter().Text("Trạng thái").Bold().FontColor("#FFFFFF");
                            });

                            var stt = 1;
                            foreach (var it in data.FaultyEquipments.Take(12))
                            {
                                var bg = stt % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(stt++.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(it.EquipmentCode);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(it.Name);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(it.RoomNumber);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(it.BuildingName);
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(it.Quantity.ToString());
                                table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(it.StatusText);
                            }
                        });

                        col.Item().Element(ComposeSignatureBlock);
                    });
                });

                page.Footer().Element(ComposePdfFooter);
            });
        });

        return doc.GeneratePdf();
    }

    #region Helper QuestPDF

    private static void ComposePdfHeader(IContainer container, string title, string color, GenerateReportRequestDto request)
    {
        container.Column(col =>
        {
            col.Item().Row(r =>
            {
                r.RelativeItem().Column(c =>
                {
                    c.Item().Text("TRƯỜNG ĐẠI HỌC ...").FontSize(9).Bold();
                    c.Item().Text("BAN QUẢN LÝ KÝ TÚC XÁ").FontSize(9.5f).Bold().FontColor(color);
                });

                r.RelativeItem().AlignRight().Column(c =>
                {
                    c.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").FontSize(9).Bold();
                    c.Item().AlignCenter().Text("Độc lập - Tự do - Hạnh phúc").FontSize(8.5f).Italic();
                    c.Item().AlignCenter().Text("------------------").FontSize(7).FontColor(Colors.Grey.Lighten1);
                });
            });

            col.Item().PaddingTop(12).AlignCenter().Column(c =>
            {
                c.Item().AlignCenter().Text(title).FontSize(14).Bold().FontColor(color);

                var now = DateTime.Now;
                var info = $"Ngày trích xuất: {now:dd/MM/yyyy HH:mm}   •   Người lập: {(string.IsNullOrWhiteSpace(request.GeneratedBy) ? "Hệ thống" : request.GeneratedBy)}";
                if (request.FromDate.HasValue || request.ToDate.HasValue)
                {
                    info += $"   •   Giai đoạn: {(request.FromDate.HasValue ? request.FromDate.Value.ToString(DateFormat) : "Tất cả")} - {(request.ToDate.HasValue ? request.ToDate.Value.ToString(DateFormat) : "Hiện tại")}";
                }
                c.Item().AlignCenter().PaddingTop(2).Text(info).FontSize(8.5f).FontColor(Colors.Grey.Darken2);
            });

            col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private static void ComposeKpiCard(IContainer container, string title, string value, string color)
    {
        container.Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.Grey.Lighten4)
            .Padding(8)
            .Column(col =>
            {
                col.Item().Text(title).FontSize(7.5f).Bold().FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(4).Text(value).FontSize(11).Bold().FontColor(color);
            });
    }

    private static void ComposeSignatureBlock(IContainer container)
    {
        container.PaddingTop(15).Row(r =>
        {
            r.RelativeItem().AlignCenter().Column(c =>
            {
                c.Item().Text("NGƯỜI LẬP BÁO CÁO").Bold().FontSize(9);
                c.Item().Text("(Ký và ghi rõ họ tên)").Italic().FontSize(8).FontColor(Colors.Grey.Darken1);
                c.Item().PaddingTop(40).Text("........................................").FontColor(Colors.Grey.Lighten1);
            });

            r.RelativeItem().AlignCenter().Column(c =>
            {
                var now = DateTime.Now;
                c.Item().Text($"Ngày {now.Day:D2} tháng {now.Month:D2} năm {now.Year}").Italic().FontSize(8.5f);
                c.Item().Text("TRƯỞNG BAN QUẢN LÝ KTX").Bold().FontSize(9);
                c.Item().Text("(Ký, đóng dấu và ghi rõ họ tên)").Italic().FontSize(8).FontColor(Colors.Grey.Darken1);
                c.Item().PaddingTop(32).Text("........................................").FontColor(Colors.Grey.Lighten1);
            });
        });
    }

    private static void ComposePdfFooter(IContainer container)
    {
        container.PaddingTop(8).Row(r =>
        {
            r.RelativeItem().Text("Hệ thống Quản lý Ký túc xá - Trích xuất tự động").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
            r.RelativeItem().AlignRight().Text(text =>
            {
                text.Span("Trang ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                text.CurrentPageNumber().FontSize(7.5f).Bold().FontColor(Colors.Grey.Darken1);
                text.Span(" / ").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                text.TotalPages().FontSize(7.5f).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    #endregion

    #endregion

    #region 4. QUẢN LÝ LỊCH SỬ & LƯU TRỮ BÁO CÁO (HISTORY & STORAGE)

    /// <summary>
    /// Thực hiện trọn gói: Sinh báo cáo, lưu tệp vào thư mục đích và ghi bản ghi ReportHistory
    /// </summary>
    public async Task<ReportHistoryDto> GenerateAndSaveReportAsync(GenerateReportRequestDto request, string? targetDirectory = null)
    {
        NormalizeReportType(request);

        byte[] fileBytes;
        string extension;

        if (request.Format == ReportFormat.Excel)
        {
            fileBytes = await GenerateExcelReportAsync(request);
            extension = ".xlsx";
        }
        else
        {
            fileBytes = await GeneratePdfReportAsync(request);
            extension = ".pdf";
        }

        string baseDir;
        if (!string.IsNullOrWhiteSpace(targetDirectory))
        {
            baseDir = targetDirectory;
        }
        else
        {
            var reportsRoot = DatabasePathResolver.ResolveDatabasePath("reports");
            baseDir = Path.Combine(reportsRoot, request.ReportType.ToString(), DateTime.UtcNow.ToString("yyyy-MM"));
        }

        if (!Directory.Exists(baseDir))
        {
            Directory.CreateDirectory(baseDir);
        }

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var randomSuffix = Guid.NewGuid().ToString("N")[..6];
        var fileName = $"BaoCao_{request.ReportType}_{timestamp}_{randomSuffix}{extension}";
        var fullPath = Path.Combine(baseDir, fileName);

        await File.WriteAllBytesAsync(fullPath, fileBytes);

        var title = !string.IsNullOrWhiteSpace(request.Title)
            ? request.Title
            : GetDefaultReportTitle(request.ReportType);

        var history = new ReportHistory
        {
            ReportType = request.ReportType,
            Title = title,
            Format = request.Format,
            Status = ReportStatus.Completed,
            FileName = fileName,
            FilePath = fullPath,
            FileSizeBytes = fileBytes.LongLength,
            ParametersJson = request.AdditionalFiltersJson,
            GeneratedBy = string.IsNullOrWhiteSpace(request.GeneratedBy) ? "Hệ thống" : request.GeneratedBy,
            GeneratedAt = DateTime.UtcNow
        };

        _context.ReportHistories.Add(history);
        await _context.SaveChangesAsync();

        return ReportHistoryDto.FromEntity(history);
    }

    /// <summary>
    /// Lấy danh sách lịch sử các lượt xuất báo cáo có lọc theo tiêu chí
    /// </summary>
    public async Task<List<ReportHistoryDto>> GetReportHistoriesAsync(
        ReportType? type = null,
        ReportFormat? format = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _context.ReportHistories.AsNoTracking().AsQueryable();

        if (type.HasValue)
        {
            query = query.Where(h => h.ReportType == type.Value);
        }

        if (format.HasValue)
        {
            query = query.Where(h => h.Format == format.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(h => h.GeneratedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
            var toCompare = toDate.Value.TimeOfDay == TimeSpan.Zero ? endOfDay : toDate.Value;
            query = query.Where(h => h.GeneratedAt <= toCompare);
        }

        var list = await query.OrderByDescending(h => h.GeneratedAt).ToListAsync();
        return list.Select(ReportHistoryDto.FromEntity).ToList();
    }

    /// <summary>
    /// Lấy chi tiết một bản ghi lịch sử báo cáo theo ID
    /// </summary>
    public async Task<ReportHistoryDto?> GetReportHistoryByIdAsync(int id)
    {
        var history = await _context.ReportHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id);

        return history != null ? ReportHistoryDto.FromEntity(history) : null;
    }

    /// <summary>
    /// Đọc nội dung file nhị phân của báo cáo đã lưu theo ID lịch sử
    /// </summary>
    public async Task<byte[]> GetReportFileAsync(int id)
    {
        var history = await _context.ReportHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id);

        if (history == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy bản ghi lịch sử báo cáo với ID: {id}");
        }

        if (string.IsNullOrWhiteSpace(history.FilePath) || !File.Exists(history.FilePath))
        {
            throw new FileNotFoundException($"Không tìm thấy tệp báo cáo trên đĩa: {history.FilePath}");
        }

        return await File.ReadAllBytesAsync(history.FilePath);
    }

    /// <summary>
    /// Xóa một bản ghi lịch sử báo cáo và tùy chọn xóa tệp đính kèm trên đĩa
    /// </summary>
    public async Task<bool> DeleteReportHistoryAsync(int id, bool deletePhysicalFile = true)
    {
        var history = await _context.ReportHistories.FirstOrDefaultAsync(h => h.Id == id);
        if (history == null)
        {
            return false;
        }

        if (deletePhysicalFile && !string.IsNullOrWhiteSpace(history.FilePath) && File.Exists(history.FilePath))
        {
            try
            {
                File.Delete(history.FilePath);
            }
            catch
            {
                // Bỏ qua lỗi xóa tệp vật lý nếu bị khóa hoặc quyền truy cập
            }
        }

        _context.ReportHistories.Remove(history);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Helper Chuyển Đổi & Nhãn Text

    private static void NormalizeReportType(GenerateReportRequestDto request)
    {
        if (request.ReportType == 0 && request.Type != 0)
        {
            request.ReportType = request.Type;
        }
        else if (request.ReportType == 0)
        {
            request.ReportType = ReportType.Violations;
        }
    }

    private static string GetDefaultReportTitle(ReportType type) => type switch
    {
        ReportType.Violations => "Báo cáo vi phạm nội quy & kỷ luật sinh viên",
        ReportType.Financial => "Báo cáo doanh thu & thu phí ký túc xá",
        ReportType.Occupancy => "Báo cáo công suất & tỷ lệ lấp đầy phòng ký túc xá",
        ReportType.Equipment => "Báo cáo kiểm kê tài sản & trang thiết bị phòng ở",
        _ => "Báo cáo thống kê ký túc xá"
    };

    private static string GetSeverityText(ViolationSeverity severity) => severity switch
    {
        ViolationSeverity.Minor => "Nhắc nhở",
        ViolationSeverity.Moderate => "Khiển trách",
        ViolationSeverity.Severe => "Cảnh cáo",
        ViolationSeverity.Critical => "Buộc rời KTX",
        _ => "Khác"
    };

    private static string GetViolationStatusText(ViolationStatus status) => status switch
    {
        ViolationStatus.Pending => "Chờ xử lý",
        ViolationStatus.Resolved => "Đã xử lý",
        ViolationStatus.Dismissed => "Miễn trừ",
        _ => "Khác"
    };

    private static string GetBillStatusText(BillStatus status) => status switch
    {
        BillStatus.Paid => "Đã thanh toán",
        BillStatus.Unpaid => "Chưa thanh toán",
        BillStatus.Overdue => "Quá hạn",
        _ => "Khác"
    };

    private static string GetRoomTypeText(RoomType type) => type switch
    {
        RoomType.Standard => "Tiêu chuẩn",
        RoomType.Premium => "Chất lượng cao",
        RoomType.Vip => "VIP",
        _ => "Khác"
    };

    private static string GetRoomStatusText(RoomStatus status) => status switch
    {
        RoomStatus.Available => "Còn chỗ",
        RoomStatus.Occupied => "Đã đầy",
        RoomStatus.Maintenance => "Bảo trì",
        _ => "Khác"
    };

    private static string GetEquipmentStatusText(EquipmentStatus status) => status switch
    {
        EquipmentStatus.Good => "Hoạt động tốt",
        EquipmentStatus.NeedsRepair => "Cần sửa chữa",
        EquipmentStatus.Broken => "Hỏng hóc",
        _ => "Khác"
    };

    #endregion
}
