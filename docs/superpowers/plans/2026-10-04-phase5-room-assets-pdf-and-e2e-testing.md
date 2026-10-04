# Kế Hoạch Triển Khai Giai Đoạn 5: Quản Lý Tài Sản Phòng, Xuất Hóa Đơn PDF & Kiểm Thử E2E Giao Diện

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Mở rộng hệ thống quản lý Ký túc xá với:
1. **Phân hệ Quản lý Tài sản Phòng (`Equipment`)**: Quản lý trang thiết bị nội thất (điều hòa, giường, tủ, quạt, bàn ghế), tình trạng hỏng hóc, báo sửa chữa.
2. **Xuất hóa đơn & Phiếu thu sang PDF (`QuestPDF`)**: Tạo phiếu thu tiền phòng và điện nước định dạng PDF chuẩn in ấn A4/A5 chuyên nghiệp.
3. **Bộ kiểm thử tự động giao diện (UI E2E Testing)**: Xây dựng dự án `Dormitory.E2ETests` sử dụng `Avalonia.Headless.XUnit` giả lập thao tác người dùng (đăng nhập, điều hướng menu, thêm tài sản, xuất hóa đơn) chạy tự động cả trên local lẫn GitHub Actions CI/CD.

**Architecture & Tech Stack:**
- **Core / Domain**: Entity `Equipment`, Enum `EquipmentStatus` (Good, NeedsRepair, Broken).
- **Application**: `IEquipmentService`, `IPdfExportService`, DTOs.
- **Infrastructure**: EF Core 8 SQLite (`DbSet<Equipment>`), `QuestPDF` 2024.12.x.
- **Presentation**: Avalonia UI 11 + CommunityToolkit.Mvvm (`EquipmentListView`, `EquipmentDialogWindow`, `BillListView`).
- **Testing**: `tests/Dormitory.UnitTests` (xUnit + FluentAssertions), `tests/Dormitory.E2ETests` (`Avalonia.Headless.XUnit`).

---

## Danh Sách Tệp Sẽ Tạo & Chỉnh Sửa

| Tệp tin | Trách nhiệm |
|---|---|
| `src/Dormitory.Core/Enums/EquipmentStatus.cs` | Enum trạng thái tài sản (Good, NeedsRepair, Broken) |
| `src/Dormitory.Core/Entities/Equipment.cs` | Entity thiết bị phòng (Mã, tên, số lượng, đơn giá, phòng, ghi chú) |
| `src/Dormitory.Application/DTOs/EquipmentDto.cs` | DTOs truyền tải dữ liệu thiết bị và yêu cầu thêm/sửa |
| `src/Dormitory.Application/Interfaces/IEquipmentService.cs` | Giao diện dịch vụ nghiệp vụ tài sản phòng |
| `src/Dormitory.Application/Interfaces/IPdfExportService.cs` | Giao diện dịch vụ kết xuất phiếu thu PDF |
| `src/Dormitory.Infrastructure/Data/DormitoryDbContext.cs` | Cấu hình `DbSet<Equipment>` và quan hệ 1-N với `Room` |
| `src/Dormitory.Infrastructure/Services/EquipmentService.cs` | Triển khai CRUD tài sản, báo hỏng, lọc theo phòng |
| `src/Dormitory.Infrastructure/Services/PdfExportService.cs` | Thiết kế tài liệu QuestPDF phiếu thu tiền phòng/dịch vụ |
| `src/Dormitory.Desktop/ViewModels/EquipmentListViewModel.cs` | ViewModel danh sách tài sản phòng, lọc, tìm kiếm |
| `src/Dormitory.Desktop/ViewModels/EquipmentDialogViewModel.cs` | ViewModel dialog thêm/sửa tài sản |
| `src/Dormitory.Desktop/Views/EquipmentListView.axaml` | Giao diện danh sách tài sản (DataGrid, Badges trạng thái) |
| `src/Dormitory.Desktop/Views/EquipmentDialogWindow.axaml` | Cửa sổ modal nhập liệu tài sản |
| `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs` | Bổ sung lệnh `ExportBillPdfCommand` |
| `src/Dormitory.Desktop/Views/BillListView.axaml` | Thêm nút thao tác "📄 In phiếu thu (PDF)" |
| `tests/Dormitory.UnitTests/Services/EquipmentServiceTests.cs` | Unit tests kiểm thử nghiệp vụ tài sản (TDD) |
| `tests/Dormitory.UnitTests/Services/PdfExportServiceTests.cs` | Unit tests kiểm thử kết xuất file PDF |
| `tests/Dormitory.E2ETests/Dormitory.E2ETests.csproj` | Project kiểm thử giao diện tự động Avalonia Headless |
| `tests/Dormitory.E2ETests/Journeys/*.cs` | Các kịch bản E2E kiểm thử luồng người dùng thực tế |
| `.github/workflows/ci-cd.yml` | Cập nhật chạy cả UnitTests lẫn E2ETests trên CI/CD |

---

## Chi Tiết Các Task Triển Khai

### Task 1: Nghiệp Vụ Quản Lý Tài Sản Phòng (Room Assets Backend & TDD)

**Mục tiêu:** Xây dựng tầng Domain, Application, Database và Unit Tests cho quản lý tài sản và thiết bị phòng KTX.

- [ ] **Step 1.1:** Tạo Enum `EquipmentStatus.cs` (Good = 1, NeedsRepair = 2, Broken = 3) với comment tiếng Việt.
- [ ] **Step 1.2:** Tạo Entity `Equipment.cs` (Id, RoomId, Room, EquipmentCode, Name, Status, Quantity, Price, Notes, CreatedAt).
- [ ] **Step 1.3:** Cập nhật `DormitoryDbContext.cs` thêm `DbSet<Equipment> Equipments` và cấu hình quan hệ cascade với `Room`.
- [ ] **Step 1.4:** Định nghĩa `EquipmentDto.cs`, `CreateEquipmentDto`, `UpdateEquipmentDto` trong `Dormitory.Application`.
- [ ] **Step 1.5:** Định nghĩa interface `IEquipmentService.cs`:
  - `GetEquipmentsByRoomIdAsync(int roomId)`
  - `GetAllEquipmentsAsync(string? searchTerm, EquipmentStatus? status)`
  - `GetEquipmentByIdAsync(int id)`
  - `CreateEquipmentAsync(CreateEquipmentDto dto)`
  - `UpdateEquipmentAsync(int id, UpdateEquipmentDto dto)`
  - `DeleteEquipmentAsync(int id)`
  - `UpdateStatusAsync(int id, EquipmentStatus newStatus, string? notes)`
- [ ] **Step 1.6:** Viết Unit Tests TDD trong `tests/Dormitory.UnitTests/Services/EquipmentServiceTests.cs`.
- [ ] **Step 1.7:** Triển khai `EquipmentService.cs` trong `Dormitory.Infrastructure`.
- [ ] **Step 1.8:** Cập nhật `DataSeeder.cs` nạp dữ liệu tài sản mẫu cho các phòng (Điều hòa Daikin, Giường tầng sắt, Bàn học sinh, Bình nóng lạnh).
- [ ] **Step 1.9:** Chạy `dotnet test Dormitory.sln` để xác minh toàn bộ test backend mới pass.
- [ ] **Step 1.10:** Commit: `feat(equipment): implement room equipment and asset management backend with unit tests`.

---

### Task 2: Giao Diện Quản Lý Tài Sản Phòng Desktop (Avalonia MVVM UI)

**Mục tiêu:** Xây dựng màn hình quản lý tài sản, lọc theo phòng/trạng thái và hộp thoại thêm/sửa thiết bị.

- [ ] **Step 2.1:** Tạo `EquipmentDialogViewModel.cs` và `EquipmentDialogWindow.axaml` (hộp thoại thêm/chỉnh sửa thiết bị với validation).
- [ ] **Step 2.2:** Tạo `EquipmentListViewModel.cs`:
  - Danh sách `Equipments`, `Rooms` phục vụ combobox lọc theo phòng.
  - Các bộ lọc: Từ khóa tìm kiếm, Tòa/Phòng, Trạng thái (Tốt / Cần sửa / Hỏng).
  - Commands: `LoadEquipmentsCommand`, `AddEquipmentCommand`, `EditEquipmentCommand`, `DeleteEquipmentCommand`, `ReportIssueCommand`.
- [ ] **Step 2.3:** Tạo `EquipmentListView.axaml`:
  - Header: "Quản Lý Tài Sản & Trang Thiết Bị Phòng", phụ đề.
  - Toolbar: Ô tìm kiếm, Combobox chọn Phòng, Combobox chọn Trạng thái, Nút "➕ Thêm thiết bị", Nút "🔄 Làm mới".
  - DataGrid: Mã TB, Tên thiết bị, Phòng/Tòa, Số lượng, Đơn giá, Trạng thái (Badge màu: Xanh = Tốt, Vàng = Cần sửa, Đỏ = Hỏng), Ghi chú, Nút Sửa / Báo hỏng / Xóa.
- [ ] **Step 2.4:** Tích hợp Sidebar Navigation trong `MainWindow.axaml` và `MainWindowViewModel.cs`:
  - Thêm nút Sidebar: "🛋️  Quản lý tài sản".
  - Thêm DataTemplate cho `EquipmentListViewModel` -> `EquipmentListView`.
- [ ] **Step 2.5:** Đăng ký Dependency Injection trong `App.axaml.cs` (`IEquipmentService`, `EquipmentListViewModel`, `EquipmentDialogViewModel`).
- [ ] **Step 2.6:** Chạy build và test kiểm tra tính toàn vẹn.
- [ ] **Step 2.7:** Commit: `feat(desktop): add Room Equipment management desktop UI and dialogs`.

---

### Task 3: Xuất Hóa Đơn & Phiếu Thu Ra PDF (QuestPDF Integration & TDD)

**Mục tiêu:** Tích hợp QuestPDF để kết xuất phiếu thu tiền phòng/dịch vụ định dạng PDF chuẩn in ấn A4/A5.

- [ ] **Step 3.1:** Thêm package `QuestPDF` (2024.12.x) vào `Dormitory.Infrastructure.csproj`. Cấu hình license cộng đồng `QuestPDF.Settings.License = LicenseType.Community;`.
- [ ] **Step 3.2:** Tạo interface `IPdfExportService.cs` trong `Dormitory.Application`:
  - `Task<byte[]> GenerateBillReceiptPdfAsync(int billId);`
- [ ] **Step 3.3:** Viết unit tests TDD trong `tests/Dormitory.UnitTests/Services/PdfExportServiceTests.cs`:
  - `GenerateBillReceiptPdfAsync_WithValidBill_ShouldReturnValidPdfDocumentBytes`: Kiểm tra mảng byte bắt đầu bằng `%PDF-1.`.
  - `GenerateBillReceiptPdfAsync_WithNonExistentBill_ShouldThrowKeyNotFoundException`.
- [ ] **Step 3.4:** Triển khai `PdfExportService.cs` trong `Dormitory.Infrastructure`:
  - Thiết kế tài liệu QuestPDF:
    - Tiêu đề: "BAN QUẢN LÝ KÝ TÚC XÁ - PHIẾU THU TIỀN PHÒNG & TIỆN ÍCH".
    - Mã phiếu, ngày thu, trạng thái hóa đơn (Đã thu / Chưa thu).
    - Khối thông tin sinh viên & phòng: Họ tên, Mã SV, SĐT, Số phòng, Tòa nhà.
    - Bảng chi tiết dịch vụ: Tiền thuê phòng, Tiền điện (chỉ số cũ, chỉ số mới, số tiêu thụ, đơn giá, thành tiền), Tiền nước (chỉ số cũ/mới, số khối, đơn giá, thành tiền), Phụ phí dịch vụ khác.
    - Dòng tổng cộng tiền in đậm nổi bật (VND).
    - Khối chữ ký hai bên: "Người nộp tiền (Ký, ghi rõ họ tên)" và "Thủ quỹ / Đại diện BQL KTX (Ký, đóng dấu)".
- [ ] **Step 3.5:** Cập nhật `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs`:
  - Tiêm `IPdfExportService` và `IFileService`.
  - Thêm command `ExportBillPdfCommand(BillDto bill)`.
  - Gọi xuất PDF và lưu file: `await _fileService.SaveFileAsync($"PhieuThu_{bill.BillCode}", "pdf", "PDF Documents (*.pdf)|*.pdf", bytes)`.
- [ ] **Step 3.6:** Cập nhật `src/Dormitory.Desktop/Views/BillListView.axaml`:
  - Thêm nút "📄 In phiếu thu (PDF)" trên thanh công cụ và trong menu ngữ cảnh DataGrid.
- [ ] **Step 3.7:** Đăng ký DI trong `App.axaml.cs` và chạy unit tests xác minh.
- [ ] **Step 3.8:** Commit: `feat(export): integrate QuestPDF for professional dormitory bill receipt export`.

---

### Task 4: Xây Dựng Bộ Kiểm Thử Tự Động Giao Diện (Avalonia Headless UI E2E)

**Mục tiêu:** Tạo project kiểm thử giao diện E2E không cần màn hình vật lý bằng `Avalonia.Headless.XUnit` giả lập chính xác hành vi người dùng.

- [ ] **Step 4.1:** Khởi tạo project `tests/Dormitory.E2ETests/Dormitory.E2ETests.csproj` tham chiếu `Dormitory.Desktop`, `Avalonia.Headless.XUnit` (11.2.5), `FluentAssertions`, `xunit`.
- [ ] **Step 4.2:** Thêm project `Dormitory.E2ETests` vào solution `Dormitory.sln`.
- [ ] **Step 4.3:** Cấu hình `TestAppBuilder.cs` với thuộc tính `[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]`.
- [ ] **Step 4.4:** Viết kịch bản E2E 1: `AuthAndNavigationE2ETests.cs`:
  - Kiểm tra mở cửa sổ MainWindow.
  - Nhập sai mật khẩu -> hiển thị thông báo lỗi xác thực.
  - Nhập đúng `admin` / `Admin@123456` -> chuyển vào Dashboard thành công.
  - Chuyển đổi giữa các menu Sidebar (Phòng, Sinh viên, Hóa đơn, Cài đặt).
- [ ] **Step 4.5:** Viết kịch bản E2E 2: `EquipmentManagementE2ETests.cs`:
  - Đăng nhập -> Điều hướng vào Quản lý tài sản.
  - Mở dialog thêm thiết bị -> Nhập thông tin -> Lưu lại.
  - Xác minh thiết bị mới xuất hiện trên DataGrid.
- [ ] **Step 4.6:** Viết kịch bản E2E 3: `BillExportE2ETests.cs`:
  - Điều hướng vào Hóa đơn -> Chọn hóa đơn -> Bấm lệnh xuất PDF -> Xác minh luồng sinh byte PDF thành công.
- [ ] **Step 4.7:** Chạy `dotnet test tests/Dormitory.E2ETests` và xác minh toàn bộ các kịch bản E2E pass 100%.
- [ ] **Step 4.8:** Commit: `test(e2e): create Avalonia Headless UI end-to-end testing suite for critical user journeys`.

---

### Task 5: Tích Hợp CI/CD, Đồng Bộ GitNexus, Hoàn Thiện Tài Liệu & Phát Hành v2.1.0

**Mục tiêu:** Đưa E2E test vào GitHub Actions workflow, cập nhật tài liệu, đồng bộ đồ thị tri thức GitNexus và phát hành `v2.1.0`.

- [ ] **Step 5.1:** Cập nhật `.github/workflows/ci-cd.yml` để chạy cả kiểm thử Unit và E2E trong job `test-and-verify`.
- [ ] **Step 5.2:** Cập nhật `docs/user-guide.md`:
  - Hướng dẫn phân hệ Quản lý tài sản phòng KTX (Thêm, sửa, báo hỏng, kiểm kê thiết bị).
  - Hướng dẫn tính năng In/Xuất phiếu thu tiền phòng ra file PDF.
- [ ] **Step 5.3:** Cập nhật `README.md`:
  - Cập nhật số lượng bài test (Unit + E2E).
  - Cập nhật Feature Matrix thêm Phase 5 (Quản lý tài sản, PDF Receipt, Headless E2E).
- [ ] **Step 5.4:** Chạy toàn bộ giải pháp:
  - `dotnet build Dormitory.sln` -> 0 errors, 0 warnings.
  - `dotnet test Dormitory.sln` -> 100% pass (cả UnitTests và E2ETests).
- [ ] **Step 5.5:** Đồng bộ đồ thị tri thức GitNexus:
  - `node .gitnexus/run.cjs detect-changes --scope all --repo .`
  - `node .gitnexus/run.cjs analyze --index-only`
- [ ] **Step 5.6:** Commit: `docs: update documentation and CI/CD for Phase 5 features and E2E testing`.
- [ ] **Step 5.7:** Tạo tag `v2.1.0` và đẩy lên GitHub:
  - `git tag -a v2.1.0 -m "Release v2.1.0: Room Equipment Management, PDF Receipts, and Headless UI E2E Testing"`
  - `git push origin master --tags`

---

## Tiêu Chí Nghiệm Thu (Acceptance Criteria)

1. **Quản lý tài sản phòng**: Quản lý đầy đủ danh sách thiết bị theo từng phòng, báo hỏng, cập nhật trạng thái với validation chặt chẽ.
2. **Xuất PDF phiếu thu**: Kết xuất tệp PDF sắc nét, chuẩn định dạng biên lai thu tiền với chữ ký và chi tiết từng khoản mục tiền phòng, điện nước.
3. **Kiểm thử E2E Headless**: Tự động hóa kiểm thử trọn vẹn luồng người dùng thực tế trên Avalonia mà không cần GPU/màn hình vật lý, chạy mượt mà cả trên máy local và GitHub Actions CI.
4. **Chất lượng**: 100% tests pass (Unit Tests + E2E Tests), 0 cảnh báo, 0 lỗi biên dịch.
5. **Đồng bộ GitNexus**: Đồ thị tri thức bao phủ toàn bộ các entities, services, viewmodels và execution flows mới.
