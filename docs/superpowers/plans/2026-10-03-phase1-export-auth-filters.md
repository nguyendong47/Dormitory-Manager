# Phase 1: Excel Export, Authentication Flow & Advanced Filters Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Bổ sung tính năng Xuất báo cáo Excel (ClosedXML), luồng Đăng nhập/Đăng xuất và phân quyền (RBAC), cùng các bộ lọc nâng cao và cảnh báo hợp đồng sắp hết hạn trên Avalonia Desktop.

**Architecture:** Mở rộng tầng Application với `IExportService`, `IUserSession`; triển khai `ExportService` (ClosedXML) trong Infrastructure; tích hợp màn hình `LoginView`, nút Đăng xuất và phân quyền giao diện (Admin vs Manager) trong Desktop; bổ sung cảnh báo hợp đồng/hóa đơn trên Dashboard.

**Tech Stack:** C# 12, .NET 8 LTS, Avalonia UI 11.2, ClosedXML 0.104, CommunityToolkit.Mvvm 8.4, Entity Framework Core 8, xUnit, FluentAssertions.

**Spec:** [docs/spec-modernization.md](file:///Volumes/AI_Models/source/Dormitory-Manager/docs/spec-modernization.md)

## Global Constraints

- **Language Conventions**: Tên class, method, variable 100% bằng Tiếng Anh; Comment trong code và giao diện UI 100% bằng Tiếng Việt.
- **Naming Conventions**: Prefix project và namespace là `Dormitory.*`.
- **DataGrid Binding Rule**: Mọi cột DataGrid hiển thị có format luôn sử dụng `Mode=OneWay`.
- **Git Commit Gate**: Phải chạy `gitnexus detect-changes --scope all --repo .` trước khi commit.

## Review Focus

1. **ClosedXML Memory Stream & Disposal**: Tạo file Excel phải giải phóng tài nguyên `XLWorkbook` và `MemoryStream` đúng cách, trả về mảng `byte[]` an toàn.
2. **Avalonia StorageProvider File Picker**: Lưu file trên macOS/Windows/Linux phải thông qua `TopLevel.GetTopLevel(visual).StorageProvider.SaveFilePickerAsync` an toàn, xử lý khi người dùng ấn Hủy (Cancel).
3. **Session State Isolation**: Đăng xuất phải xóa sạch thông tin người dùng hiện tại và đưa ứng dụng về màn hình đăng nhập, không lưu vết thông tin phiên cũ.
4. **RBAC UI Protection**: Người dùng vai trò `Manager` không được phép thực hiện thao tác xóa Phòng hoặc xóa Nhân viên (Ẩn hoặc disable nút Xóa).
5. **Contract Expiry Date Range**: Bộ lọc hợp đồng sắp hết hạn phải tính chính xác khoảng ngày từ `Today` đến `Today.AddDays(30)` và chỉ áp dụng cho hợp đồng `Status == ContractStatus.Active`.

---

### Task 1: Excel Export Infrastructure & Unit Tests

**Files:**
- Modify: `src/Dormitory.Infrastructure/Dormitory.Infrastructure.csproj` (Thêm PackageReference ClosedXML 0.104.2)
- Create: `src/Dormitory.Application/Interfaces/IExportService.cs`
- Create: `src/Dormitory.Infrastructure/Services/ExportService.cs`
- Test: `tests/Dormitory.UnitTests/Services/ExportServiceTests.cs`

**Interfaces:**
- Consumes: `RoomDto`, `StudentDto`, `BillDto`
- Produces: `IExportService`:
  - `Task<byte[]> ExportRoomsToExcelAsync(List<RoomDto> rooms);`
  - `Task<byte[]> ExportStudentsToExcelAsync(List<StudentDto> students);`
  - `Task<byte[]> ExportBillsToExcelAsync(List<BillDto> bills);`

- [ ] **Step 1: Add ClosedXML to `Dormitory.Infrastructure.csproj`**

Thêm `<PackageReference Include="ClosedXML" Version="0.104.2" />`.

- [ ] **Step 2: Write failing unit tests in `tests/Dormitory.UnitTests/Services/ExportServiceTests.cs`**

Kiểm tra:
1. `ExportRoomsToExcelAsync` trả về byte array hợp lệ và không rỗng.
2. `ExportStudentsToExcelAsync` tạo đúng các tiêu đề cột (Mã SV, Họ tên, Giới tính, Lớp, v.v.).
3. `ExportBillsToExcelAsync` tạo đúng số dòng tương ứng danh sách hóa đơn.

- [ ] **Step 3: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln --filter ExportServiceTests`
Expected: FAIL do interface và class chưa tồn tại.

- [ ] **Step 4: Implement `IExportService.cs` and `ExportService.cs`**

Triển khai xuất Excel định dạng chuyên nghiệp: Header có màu nền xanh dương, in đậm, border viền, căn lề và định dạng số tiền VND.

- [ ] **Step 5: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln --filter ExportServiceTests`
Expected: PASS toàn bộ tests.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Infrastructure/ src/Dormitory.Application/ tests/Dormitory.UnitTests/
git commit -m "feat(export): implement Excel export service with ClosedXML and unit tests"
```

---

### Task 2: Desktop Excel Export UI Integration

**Files:**
- Create: `src/Dormitory.Desktop/Services/IFileService.cs`
- Create: `src/Dormitory.Desktop/Services/FileService.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/RoomListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/RoomListView.axaml`
- Modify: `src/Dormitory.Desktop/ViewModels/StudentListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/StudentListView.axaml`
- Modify: `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/BillListView.axaml`
- Modify: `src/Dormitory.Desktop/App.axaml.cs` (Đăng ký `IExportService`, `IFileService`)

**Interfaces:**
- Consumes: `IExportService`, `IFileService`
- Produces: Nút bấm "📊 Xuất Excel" trên các màn hình danh sách, cho phép người dùng chọn đường dẫn lưu file `.xlsx`.

- [ ] **Step 1: Implement `IFileService` and `FileService`**

Cung cấp phương thức:
`Task<bool> SaveFileAsync(string defaultFileName, string extension, string fileTypeFilter, byte[] content);`
Sử dụng Avalonia `StorageProvider.SaveFilePickerAsync`.

- [ ] **Step 2: Add Export commands to ViewModels**

- `RoomListViewModel.ExportToExcelCommand`: lấy danh sách phòng hiện tại -> gọi `_exportService.ExportRoomsToExcelAsync` -> gọi `_fileService.SaveFileAsync("Danh_Sach_Phong", "xlsx", "Excel Files", bytes)`.
- `StudentListViewModel.ExportToExcelCommand`: tương tự cho danh sách sinh viên.
- `BillListViewModel.ExportToExcelCommand`: tương tự cho danh sách hóa đơn.

- [ ] **Step 3: Update Views with Export buttons**

Thêm nút "📊 Xuất Excel" trên thanh Toolbar của `RoomListView`, `StudentListView`, `BillListView`.

- [ ] **Step 4: Verify build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln`
Expected: Build thành công 0 error.

- [ ] **Step 5: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): integrate Excel export buttons in Room, Student, and Bill views"
```

---

### Task 3: Authentication & Login Flow in Avalonia Desktop

**Files:**
- Create: `src/Dormitory.Desktop/Services/IUserSession.cs`
- Create: `src/Dormitory.Desktop/Services/UserSession.cs`
- Create: `src/Dormitory.Desktop/ViewModels/LoginViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/LoginView.axaml`
- Create: `src/Dormitory.Desktop/Views/LoginView.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/MainWindowViewModel.cs`
- Modify: `src/Dormitory.Desktop/MainWindow.axaml`
- Modify: `src/Dormitory.Desktop/App.axaml.cs`

**Interfaces:**
- Consumes: `IAuthService`, `IUserSession`, `LoginRequest`
- Produces: Màn hình đăng nhập khi mở app, điều hướng vào Dashboard khi thành công, nút đăng xuất, và hiển thị thông tin tài khoản + vai trò.

- [ ] **Step 1: Implement `IUserSession.cs` and `UserSession.cs`**

Lưu trữ `CurrentUser` (UserDto?), `IsAuthenticated` (bool), `IsAdmin` (bool). Hỗ trợ đăng xuất (`ClearSession`).

- [ ] **Step 2: Implement `LoginViewModel.cs` and `LoginView.axaml`**

- Form đăng nhập đẹp chuẩn Fluent: Logo KTX, Ô nhập Username, Password (`PasswordChar="*"`, phím Enter để đăng nhập), Nút "Đăng nhập" (Accent), Thông báo lỗi nếu sai tài khoản/mật khẩu.
- Tài khoản mẫu gợi ý hiển thị: `admin` / `admin123` hoặc `manager` / `manager123`.

- [ ] **Step 3: Integrate into `MainWindowViewModel.cs` & `MainWindow.axaml`**

- Thêm trạng thái `IsLoggedIn` vào `MainWindowViewModel`.
- Nếu chưa đăng nhập: hiển thị `LoginView`.
- Nếu đã đăng nhập: hiển thị giao diện chính (Sidebar + CurrentView).
- Cập nhật Sidebar footer: hiển thị Tên người dùng và Vai trò thực tế (`👤 Admin: Nguyễn Văn A (Quản trị viên)`).
- Thêm nút "🚪 Đăng xuất": gọi `LogoutCommand`, xóa session, quay về `LoginView`.
- RBAC: Ràng buộc quyền Admin trên các thao tác xóa phòng/nhân viên.

- [ ] **Step 4: Register in `App.axaml.cs`**

Đăng ký `IUserSession` dạng Singleton, `LoginViewModel` dạng Transient.

- [ ] **Step 5: Verify build & tests**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln`
Expected: Build và test 100% pass.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): implement Login view, session management, and RBAC logout flow"
```

---

### Task 4: Advanced Filters & Dashboard Expiry Alerts

**Files:**
- Modify: `src/Dormitory.Desktop/ViewModels/ContractListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/ContractListView.axaml`
- Modify: `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/BillListView.axaml`
- Modify: `src/Dormitory.Desktop/ViewModels/DashboardViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/DashboardView.axaml`

**Interfaces:**
- Consumes: `IContractService`, `IBillService`, `IDashboardService`
- Produces: Bộ lọc trạng thái hóa đơn (Nợ/Đã trả), bộ lọc hợp đồng sắp hết hạn trong 30 ngày, và thẻ cảnh báo trực quan trên Dashboard.

- [ ] **Step 1: Update `ContractListViewModel` and `ContractListView.axaml`**

- Bổ sung bộ lọc: `FilterMode` ("Tất cả", "Đang hiệu lực", "Sắp hết hạn (30 ngày)", "Đã kết thúc").
- Khi chọn "Sắp hết hạn (30 ngày)": Lọc các hợp đồng `Active` có `EndDate <= DateTime.Today.AddDays(30) && EndDate >= DateTime.Today`.

- [ ] **Step 2: Update `BillListViewModel` and `BillListView.axaml`**

- Bổ sung bộ lọc nhanh: `StatusFilter` (ComboBox: "Tất cả", "Chưa thanh toán (Còn nợ)", "Đã thanh toán").

- [ ] **Step 3: Update `DashboardViewModel` and `DashboardView.axaml`**

- Thêm danh sách `ExpiringContracts` (hợp đồng sắp hết hạn trong 30 ngày tới).
- Thêm thẻ cảnh báo màu vàng (Warning Card) trên Dashboard: "⚠️ Có X hợp đồng sắp hết hạn trong 30 ngày tới cần gia hạn/thanh lý" và "💵 Có Y hóa đơn chưa thanh toán".

- [ ] **Step 4: Verify build & tests**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln`
Expected: 100% pass.

- [ ] **Step 5: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): add advanced filters for contracts/bills and dashboard expiry alerts"
```

---

### Task 5: Full Verification, Documentation & GitNexus Sync

**Files:**
- Modify: `docs/user-guide.md`
- Modify: `README.md`

- [ ] **Step 1: Run all unit tests**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Expected: Tất cả unit tests (bao gồm các test mới cho ExportService) đều PASS.

- [ ] **Step 2: Run GitNexus graph change detection**

Run: `node .gitnexus/run.cjs detect-changes --scope all --repo .`
Expected: Clean.

- [ ] **Step 3: Re-index GitNexus knowledge graph**

Run: `node .gitnexus/run.cjs analyze --index-only`
Expected: Đồng bộ toàn bộ symbols mới vào knowledge graph.

- [ ] **Step 4: Update documentation**

Cập nhật `docs/user-guide.md` và `README.md` mô tả các tính năng mới:
- Xuất dữ liệu Excel.
- Hướng dẫn đăng nhập / đăng xuất và vai trò người dùng (Admin vs Manager).
- Bộ lọc nâng cao và cảnh báo hợp đồng sắp hết hạn.

- [ ] **Step 5: Final commit and push**

```bash
git add docs/ README.md .gitnexus/
git commit -m "docs: update user guide and README for Excel export, Auth flow, and advanced filters"
git push origin master
```
