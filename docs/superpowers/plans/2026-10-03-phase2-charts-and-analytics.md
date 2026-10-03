# Giai Đoạn 2: Biểu Đồ & Thống Kê Trực Quan (LiveCharts2 & Báo Cáo KTX) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tích hợp thư viện biểu đồ hiện đại LiveCharts2 vào ứng dụng Avalonia Desktop (.NET 8), trực quan hóa số liệu tỷ lệ lấp đầy theo tòa nhà, xu hướng doanh thu tiền phòng và điện nước 6 tháng gần nhất, đồng thời bổ sung tính năng xuất báo cáo quản trị tổng hợp ra file Excel.

**Architecture:** Mở rộng tầng Application (`IDashboardService`, `IExportService`) để tổng hợp số liệu theo nhóm tòa nhà và dòng thời gian tháng; tích hợp `LiveChartsCore.SkiaSharpView.Avalonia` vào tầng Desktop theo chuẩn MVVM (`ISeries[]`, `Axis[]`); đóng gói xuất báo cáo Excel đa bảng với `ClosedXML`.

**Tech Stack:** .NET 8 LTS, Avalonia UI 11.2.5, LiveChartsCore.SkiaSharpView.Avalonia 2.0.5, ClosedXML 0.104.2, CommunityToolkit.Mvvm 8.4.0, xUnit.

**Spec:** `docs/spec-modernization.md`

## Global Constraints

- **Language Conventions**: Tên class, method, variable, interface 100% bằng Tiếng Anh PascalCase/camelCase; Comment trong code và giao diện UI 100% bằng Tiếng Việt chuẩn mực.
- **Naming Conventions**: Prefix project và namespace là `Dormitory.*`.
- **Avalonia DataGrid & Chart Bindings**: Mọi binding hiển thị có format phải dùng `Mode=OneWay`.
- **Thread Safety**: Mọi thao tác cập nhật biểu đồ và nạp dữ liệu nền phải thông qua ViewModel properties hoặc `Dispatcher.UIThread`.
- **Pre-commit Gate**: Luôn chạy `node .gitnexus/run.cjs detect-changes --scope all --repo .` trước khi commit.

## Review Focus

1. **Dữ liệu trống hoặc chưa có hóa đơn nào**: Biểu đồ doanh thu 6 tháng và biểu đồ tròn tòa nhà phải hiển thị an toàn, không bị văng Exception chia cho 0 (`DivideByZeroException`) hoặc trục rỗng.
2. **Tòa nhà chưa có tên hoặc giá trị rỗng**: Xử lý fallback gán tên hiển thị là "Khác" hoặc "Chưa phân tòa".
3. **Màu sắc và Theme tương thích**: LiveCharts series dùng palette màu Fluent hài hòa (#0078D4, #107C41, #D83B01, #5C2D91, #FFB900) hiển thị rõ trên cả nền sáng (Light) và tối (Dark).
4. **Hiệu năng Render UI**: Tránh re-create toàn bộ Series array khi không cần thiết, chỉ cập nhật giá trị hoặc khởi tạo lại khi dữ liệu reload.
5. **Độ chính xác phép tính doanh thu**: Doanh thu 6 tháng chỉ tính các hóa đơn ở trạng thái `BillStatus.Paid`, phân tách rõ `RoomFee` và phí dịch vụ điện nước (`ElectricFee + WaterFee + OtherServiceFee`).

---

### Task 1: Core Analytics Service & DTOs (TDD)

**Files:**
- Create: `src/Dormitory.Application/DTOs/AnalyticsDtos.cs`
- Modify: `src/Dormitory.Application/Interfaces/IAuthAndDashboardService.cs`
- Modify: `src/Dormitory.Application/Services/DashboardService.cs`
- Create: `tests/Dormitory.UnitTests/Services/DashboardServiceTests.cs`

**Interfaces:**
- Consumes: `IDormitoryDbContext`, `Room`, `Bill`, `Contract`
- Produces:
  - `BuildingOccupancyDto`: `string BuildingName`, `int TotalRooms`, `int OccupiedRooms`, `int AvailableRooms`, `int TotalBeds`, `int OccupiedBeds`, `decimal OccupancyRate`
  - `MonthlyRevenueTrendDto`: `int Month`, `int Year`, `string Label`, `decimal RoomFeeRevenue`, `decimal UtilityFeeRevenue`, `decimal TotalRevenue`
  - `IDashboardService`: `Task<List<BuildingOccupancyDto>> GetBuildingOccupancyAsync();`, `Task<List<MonthlyRevenueTrendDto>> GetRevenueTrendsAsync(int months = 6);`

- [ ] **Step 1: Write failing tests in `tests/Dormitory.UnitTests/Services/DashboardServiceTests.cs`**

```csharp
[Fact]
public async Task GetBuildingOccupancyAsync_ShouldGroupRoomsByBuildingCorrectly()
{
    // Arrange: db context with rooms in "Tòa A" (2 rooms, 1 occupied) and "Tòa B" (1 room, available)
    // Act: var result = await service.GetBuildingOccupancyAsync();
    // Assert: result.Count == 2; "Tòa A" has OccupiedRooms = 1, AvailableRooms = 1;
}

[Fact]
public async Task GetRevenueTrendsAsync_ShouldCalculateLastNMonthsRevenueCorrectly()
{
    // Arrange: db context with paid bills in current month and previous month
    // Act: var trends = await service.GetRevenueTrendsAsync(6);
    // Assert: trends.Count == 6; correctly separates RoomFee and UtilityFee for paid bills only
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/Dormitory.UnitTests/ --filter FullyQualifiedName~DashboardServiceTests`
Expected: FAIL (types and methods not defined yet).

- [ ] **Step 3: Define DTOs in `src/Dormitory.Application/DTOs/AnalyticsDtos.cs`**

Tạo `BuildingOccupancyDto` và `MonthlyRevenueTrendDto` với đầy đủ các thuộc tính thống kê.

- [ ] **Step 4: Update `IAuthAndDashboardService.cs` & implement in `DashboardService.cs`**

Triển khai `GetBuildingOccupancyAsync` và `GetRevenueTrendsAsync(int months = 6)`:
- `GetBuildingOccupancyAsync`: nhóm `_context.Rooms` theo `r.Building`, tính tổng số phòng, số phòng `Occupied`, `Available`, tổng giường `Capacity`, số giường đã ở `CurrentOccupancy`.
- `GetRevenueTrendsAsync`: lấy mốc 6 tháng từ tháng hiện tại lùi về trước. Lấy các hóa đơn `Status == BillStatus.Paid` của từng tháng, tính tổng `RoomFee` và `ElectricFee + WaterFee + OtherServiceFee`.

- [ ] **Step 5: Run tests to verify they pass**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Expected: PASS (All tests pass, including new DashboardServiceTests).

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Application/ tests/Dormitory.UnitTests/
git commit -m "feat(application): add analytics DTOs and dashboard occupancy & revenue trend services"
```

---

### Task 2: LiveCharts2 Integration & Building Occupancy Pie/Donut Chart

**Files:**
- Modify: `src/Dormitory.Desktop/Dormitory.Desktop.csproj`
- Modify: `src/Dormitory.Desktop/ViewModels/DashboardViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/DashboardView.axaml`

**Interfaces:**
- Consumes: `IDashboardService.GetBuildingOccupancyAsync()`, `BuildingOccupancyDto`
- Produces:
  - `DashboardViewModel.BuildingOccupancySeries`: `ISeries[]`
  - `DashboardViewModel.BuildingOccupancyList`: `ObservableCollection<BuildingOccupancyDto>`
  - `LiveChartsCore.SkiaSharpView.Avalonia.PieChart` trong `DashboardView.axaml`

- [ ] **Step 1: Add package `LiveChartsCore.SkiaSharpView.Avalonia` 2.0.5**

Thêm `<PackageReference Include="LiveChartsCore.SkiaSharpView.Avalonia" Version="2.0.5" />` vào `src/Dormitory.Desktop/Dormitory.Desktop.csproj`.

- [ ] **Step 2: Update `DashboardViewModel.cs` for Pie Chart**

- Khai báo thuộc tính:
  `[ObservableProperty] private ISeries[] _buildingOccupancySeries = Array.Empty<ISeries>();`
  `[ObservableProperty] private ObservableCollection<BuildingOccupancyDto> _buildingStats = new();`
- Trong `LoadStatsAsync`:
  Gọi `var buildings = await _dashboardService.GetBuildingOccupancyAsync();`
  Chuyển đổi từng tòa nhà thành `PieSeries<int>`:
  - Name = `b.BuildingName`
  - Values = `new int[] { b.OccupiedBeds }` (hoặc `b.OccupiedRooms`)
  - Tooltip: `{Name}: {PrimaryValue} giường đã ở / {b.TotalBeds} ({b.OccupancyRate:F1}%)`
  Gán `BuildingOccupancySeries` và `BuildingStats`.

- [ ] **Step 3: Update `DashboardView.axaml` to embed Pie Chart**

- Khai báo namespace: `xmlns:lvc="using:LiveChartsCore.SkiaSharpView.Avalonia"`
- Thiết kế Card: "Tỷ lệ lấp đầy theo Tòa nhà" (Border bo tròn, đổ bóng nhẹ):
  - Bên trái: `lvc:PieChart` kích thước min-height 260px, LegendPosition="Right".
  - Bên phải (hoặc bên dưới): Bảng con tóm tắt số liệu từng tòa (Tên tòa, Số phòng, Chỗ ở, Tỷ lệ %).

- [ ] **Step 4: Verify build and compile**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln`
Expected: Build succeeded with 0 warnings, 0 errors.

- [ ] **Step 5: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): integrate LiveCharts2 and building occupancy pie chart"
```

---

### Task 3: Revenue & Utility Trend Column Chart (6 Months)

**Files:**
- Modify: `src/Dormitory.Desktop/ViewModels/DashboardViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/DashboardView.axaml`

**Interfaces:**
- Consumes: `IDashboardService.GetRevenueTrendsAsync(6)`, `MonthlyRevenueTrendDto`
- Produces:
  - `DashboardViewModel.RevenueTrendSeries`: `ISeries[]` (ColumnSeries Tiền phòng & ColumnSeries Tiền điện nước/dịch vụ)
  - `DashboardViewModel.RevenueXAxes`: `Axis[]`
  - `DashboardViewModel.RevenueYAxes`: `Axis[]`
  - `LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart` trong `DashboardView.axaml`

- [ ] **Step 1: Update `DashboardViewModel.cs` for Cartesian Column Chart**

- Khai báo thuộc tính:
  `[ObservableProperty] private ISeries[] _revenueTrendSeries = Array.Empty<ISeries>();`
  `[ObservableProperty] private Axis[] _revenueXAxes = Array.Empty<Axis>();`
  `[ObservableProperty] private Axis[] _revenueYAxes = Array.Empty<Axis>();`
- Trong `LoadStatsAsync`:
  Gọi `var trends = await _dashboardService.GetRevenueTrendsAsync(6);`
  Tạo 2 `ColumnSeries<decimal>`:
  - Series 1: Name = "Tiền phòng", Values = `trends.Select(t => t.RoomFeeRevenue).ToArray()`, Fill = `#0078D4`
  - Series 2: Name = "Điện nước & Dịch vụ", Values = `trends.Select(t => t.UtilityFeeRevenue).ToArray()`, Fill = `#107C41`
  Cấu hình `RevenueXAxes` với Labels = `trends.Select(t => t.Label).ToArray()`.
  Cấu hình `RevenueYAxes` với Labeler = `value => $"{value:N0} đ"`.

- [ ] **Step 2: Update `DashboardView.axaml` with Cartesian Chart**

- Bổ sung Card biểu đồ cột: "Biểu đồ Doanh thu KTX 6 tháng gần nhất" bên cạnh hoặc bên dưới biểu đồ tròn:
  - `lvc:CartesianChart` Series="{Binding RevenueTrendSeries}" XAxes="{Binding RevenueXAxes}" YAxes="{Binding RevenueYAxes}" LegendPosition="Top" Height="280".
  - Chú thích rõ ràng hai nguồn thu (Tiền phòng vs Phí dịch vụ tiện ích).

- [ ] **Step 3: Verify build and compile**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln`
Expected: Build succeeded with 0 warnings, 0 errors.

- [ ] **Step 4: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): add 6-month revenue & utility fee trend column chart"
```

---

### Task 4: Excel Dashboard Summary Report Export

**Files:**
- Modify: `src/Dormitory.Application/Interfaces/IExportService.cs`
- Modify: `src/Dormitory.Infrastructure/Services/ExportService.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/DashboardViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/DashboardView.axaml`
- Modify: `tests/Dormitory.UnitTests/Services/ExportServiceTests.cs`

**Interfaces:**
- Consumes: `DashboardStatsDto`, `List<BuildingOccupancyDto>`, `List<MonthlyRevenueTrendDto>`
- Produces:
  - `IExportService.ExportDashboardSummaryToExcelAsync(...)`: `Task<byte[]>`
  - `DashboardViewModel.ExportDashboardReportCommand`: `IAsyncRelayCommand`

- [ ] **Step 1: Write failing test in `tests/Dormitory.UnitTests/Services/ExportServiceTests.cs`**

```csharp
[Fact]
public async Task ExportDashboardSummaryToExcelAsync_ShouldGenerateValidExcelWorkbook()
{
    // Arrange: dummy stats, building list, revenue trends
    // Act: byte[] data = await service.ExportDashboardSummaryToExcelAsync(stats, buildings, trends);
    // Assert: data is not null; data.Length > 0; workbook has sheets/tables for KPI and Trends
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/Dormitory.UnitTests/ --filter FullyQualifiedName~ExportDashboardSummaryToExcelAsync`
Expected: FAIL (method not defined).

- [ ] **Step 3: Implement `ExportDashboardSummaryToExcelAsync` in `ExportService.cs`**

- Định nghĩa trong `IExportService.cs`.
- Triển khai trong `ExportService.cs`:
  - Sheet 1: "Tổng quan KPI & Tòa nhà" (Chỉ số số phòng, tỷ lệ lấp đầy, bảng phân tích từng tòa nhà).
  - Sheet 2: "Xu hướng doanh thu" (Bảng doanh thu 6 tháng gồm tiền phòng, điện nước và tổng cộng).
  - Định dạng header xanh Excel (#107C41), border mỏng, tự động co giãn độ rộng cột.

- [ ] **Step 4: Connect command in `DashboardViewModel.cs` & `DashboardView.axaml`**

- Tiêm `IExportService` và `IFileService` vào `DashboardViewModel`.
- Thêm `ExportDashboardReportCommand`:
  Gọi `_exportService.ExportDashboardSummaryToExcelAsync(...)` rồi gọi `_fileService.SaveFileAsync("BaoCao_TongQuan_KTX", "xlsx", "Excel Files (*.xlsx)|*.xlsx", bytes)`.
- Thêm nút "📊 Xuất báo cáo tổng hợp" trên header của `DashboardView.axaml`.

- [ ] **Step 5: Run tests and verify build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Expected: PASS (All tests pass).

- [ ] **Step 6: Commit**

```bash
git add src/ tests/
git commit -m "feat(export): implement executive dashboard summary Excel report export"
```

---

### Task 5: Full Verification, Documentation & GitNexus Sync

**Files:**
- Modify: `docs/user-guide.md`
- Modify: `README.md`
- Graph: `.gitnexus/`

**Interfaces:**
- Consumes: toàn bộ tính năng hoàn thành ở Tasks 1-4
- Produces:
  - 100% test pass
  - Đồng bộ knowledge graph GitNexus
  - Tài liệu hướng dẫn sử dụng và cập nhật README

- [ ] **Step 1: Run full test suite and build**

Run:
```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet test Dormitory.sln
dotnet build Dormitory.sln
```
Expected: 100% tests pass (tối thiểu 27+ tests), 0 build errors.

- [ ] **Step 2: Detect changes and re-index GitNexus knowledge graph**

Run:
```bash
node .gitnexus/run.cjs detect-changes --scope all --repo .
node .gitnexus/run.cjs analyze --index-only
```
Expected: Index cập nhật thành công, sạch sẽ không lỗi.

- [ ] **Step 3: Update documentation in `docs/user-guide.md` & `README.md`**

- `docs/user-guide.md`: Thêm mục "Màn hình Dashboard & Báo cáo trực quan", hướng dẫn cách xem biểu đồ tỷ lệ lấp đầy theo tòa, biểu đồ cột xu hướng doanh thu 6 tháng và cách xuất file Excel báo cáo tổng hợp.
- `README.md`: Cập nhật bảng tiến độ hoàn thành Phase 2 (LiveCharts2 & Executive Analytics).

- [ ] **Step 4: Commit**

```bash
git add docs/ README.md .gitnexus/
git commit -m "docs: update documentation and README for Phase 2 visual charts and analytics"
```
