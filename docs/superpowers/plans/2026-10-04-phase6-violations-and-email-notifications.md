# Kế Hoạch Triển Khai Giai Đoạn 6: Quản Lý Vi Phạm Kỷ Luật & Gửi Email Hóa Đơn Tự Động

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hoàn thiện 2 nghiệp vụ thiết thực còn thiếu của Ban Quản lý Ký túc xá:
1. **Phân hệ Quản lý Vi phạm Nội quy & Kỷ luật (`Violation`)**: Lập biên bản vi phạm KTX (nấu ăn, hút thuốc, làm ồn, dẫn người ngoài qua đêm), phân loại mức kỷ luật (Nhắc nhở, Khiển trách, Cảnh cáo, Buộc rời KTX), trừ điểm rèn luyện, phạt tiền và theo dõi lịch sử sinh viên.
2. **Gửi Email Thông Báo Hóa Đơn Tự Động (`MailKit / SMTP`)**: Tự động gửi email thông báo chi phí tiền phòng & điện nước đính kèm tệp PDF phiếu thu (`QuestPDF`) tới sinh viên, tích hợp cấu hình SMTP linh hoạt trong Cài đặt hệ thống.
3. **Mở rộng bộ kiểm thử tự động E2E (`Avalonia.Headless.XUnit`)** bảo đảm toàn bộ các hành trình nghiệp vụ mới chạy thông suốt trên CI/CD và phát hành phiên bản `v2.2.0`.

**Architecture & Tech Stack:**
- **Domain**: Entity `Violation`, Enums `ViolationSeverity`, `ViolationStatus`.
- **Application**: `IViolationService`, `IEmailService`, DTOs.
- **Infrastructure**: EF Core 8 SQLite (`DbSet<Violation>`), `MailKit` 4.8.x + `MimeKit`, `QuestPDF`.
- **Presentation**: Avalonia UI 11 + CommunityToolkit.Mvvm (`ViolationListView`, `ViolationDialogWindow`, `ViolationResolutionDialogWindow`, Cập nhật `BillListView`, Cập nhật `SystemSettingsView`).
- **Testing**: `Dormitory.UnitTests` (xUnit + FluentAssertions), `Dormitory.E2ETests` (Avalonia Headless).

---

## Danh Sách Tệp Sẽ Tạo & Chỉnh Sửa

| Tệp tin | Trách nhiệm |
|---|---|
| `src/Dormitory.Core/Enums/ViolationSeverity.cs` | Enum mức độ vi phạm (Minor, Moderate, Severe, Critical) |
| `src/Dormitory.Core/Enums/ViolationStatus.cs` | Enum trạng thái biên bản (Pending, Resolved, Dismissed) |
| `src/Dormitory.Core/Entities/Violation.cs` | Entity biên bản vi phạm (Mã, sinh viên, phòng, tiêu đề, điểm trừ, tiền phạt, ngày) |
| `src/Dormitory.Application/DTOs/ViolationDtos.cs` | DTOs danh sách, thêm mới, cập nhật và xử lý giải quyết vi phạm |
| `src/Dormitory.Application/DTOs/EmailDtos.cs` | Cấu hình EmailSettings và DTO gửi thư thông báo |
| `src/Dormitory.Application/Interfaces/IViolationService.cs` | Giao diện dịch vụ nghiệp vụ xử lý vi phạm KTX |
| `src/Dormitory.Application/Interfaces/IEmailService.cs` | Giao diện dịch vụ gửi email hóa đơn và kiểm tra kết nối SMTP |
| `src/Dormitory.Infrastructure/Data/DormitoryDbContext.cs` | Bổ sung `DbSet<Violation>` và cấu hình quan hệ với Sinh viên / Phòng |
| `src/Dormitory.Infrastructure/Services/ViolationService.cs` | Triển khai CRUD, lọc đa năng và xử lý giải quyết vi phạm |
| `src/Dormitory.Infrastructure/Services/EmailService.cs` | Triển khai MailKit gửi email HTML đính kèm tệp PDF phiếu thu |
| `src/Dormitory.Desktop/ViewModels/ViolationListViewModel.cs` | ViewModel quản lý danh sách vi phạm, KPI và bộ lọc |
| `src/Dormitory.Desktop/ViewModels/ViolationDialogViewModel.cs` | ViewModel modal lập biên bản vi phạm mới / chỉnh sửa |
| `src/Dormitory.Desktop/Views/ViolationListView.axaml` | Màn hình danh sách vi phạm với DataGrid và thẻ KPI |
| `src/Dormitory.Desktop/Views/ViolationDialogWindow.axaml` | Hộp thoại modal lập biên bản vi phạm |
| `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs` | Bổ sung lệnh gửi email hóa đơn `SendBillEmailCommand` |
| `src/Dormitory.Desktop/Views/BillListView.axaml` | Thêm nút "📧 Gửi email hóa đơn" trên Toolbar và DataGrid |
| `src/Dormitory.Desktop/ViewModels/SystemSettingsViewModel.cs` | Thêm quản lý cấu hình máy chủ gửi thư SMTP |
| `src/Dormitory.Desktop/Views/SystemSettingsView.axaml` | Thêm Card "📧 Cấu hình Máy Chủ Gửi Email (SMTP)" |
| `tests/Dormitory.UnitTests/Services/ViolationServiceTests.cs` | Unit tests cho phân hệ Vi phạm KTX (TDD) |
| `tests/Dormitory.UnitTests/Services/EmailServiceTests.cs` | Unit tests cho dịch vụ gửi Email thông báo (TDD) |
| `tests/Dormitory.E2ETests/Journeys/ViolationJourneysTests.cs` | E2E Headless test cho quy trình lập và xử lý vi phạm |

---

## Chi Tiết Các Task Triển Khai

### Task 1: Phân Hệ Quản Lý Vi Phạm KTX Backend (Domain, EF Core & TDD)

**Mục tiêu:** Xây dựng Domain, DTOs, DbContext, Service nghiệp vụ và Unit Tests cho phân hệ Quản lý Vi phạm Nội quy KTX.

- [x] **Step 1.1:** Tạo Enum `ViolationSeverity.cs` (`Minor = 1`, `Moderate = 2`, `Severe = 3`, `Critical = 4`) và `ViolationStatus.cs` (`Pending = 1`, `Resolved = 2`, `Dismissed = 3`).
- [x] **Step 1.2:** Tạo Entity `Violation.cs` (Id, ViolationCode, StudentId, Student, RoomId, Room, Title, Description, Severity, Status, FineAmount, DemeritPoints, ViolationDate, CreatedAt, ResolutionNotes, RecordedBy).
- [x] **Step 1.3:** Cập nhật `DormitoryDbContext.cs` & `IDormitoryDbContext.cs`: thêm `DbSet<Violation> Violations` và cấu hình Fluent API (khóa ngoại với Student và Room).
- [x] **Step 1.4:** Định nghĩa `ViolationDtos.cs` (`ViolationDto`, `CreateViolationDto`, `UpdateViolationDto`, `ResolveViolationDto`).
- [x] **Step 1.5:** Định nghĩa interface `IViolationService.cs`:
  - `GetAllViolationsAsync(string? searchTerm = null, ViolationSeverity? severity = null, ViolationStatus? status = null, int? studentId = null)`
  - `GetViolationByIdAsync(int id)`
  - `CreateViolationAsync(CreateViolationDto dto)`
  - `UpdateViolationAsync(int id, UpdateViolationDto dto)`
  - `ResolveViolationAsync(int id, ResolveViolationDto dto)`
  - `DeleteViolationAsync(int id)`
- [x] **Step 1.6:** Viết Unit Tests TDD trong `tests/Dormitory.UnitTests/Services/ViolationServiceTests.cs`.
- [x] **Step 1.7:** Triển khai `ViolationService.cs` trong `Dormitory.Infrastructure`.
- [x] **Step 1.8:** Cập nhật `DataSeeder.cs` nạp các biên bản vi phạm mẫu (nấu ăn bằng bếp điện, gây ồn sau 23h).
- [x] **Step 1.9:** Chạy `dotnet test Dormitory.sln` xác minh toàn bộ test pass.
- [x] **Step 1.10:** Commit: `feat(violation): implement dormitory violation and disciplinary management backend with unit tests`.

---

### Task 2: Giao Diện Quản Lý Vi Phạm Desktop (Avalonia MVVM UI)

**Mục tiêu:** Xây dựng màn hình danh sách vi phạm, bộ lọc, thẻ KPI và hộp thoại lập biên bản / giải quyết vi phạm.

- [x] **Step 2.1:** Tạo `ViolationDialogViewModel.cs` và `ViolationDialogWindow.axaml` (Lập biên bản vi phạm, chọn sinh viên, chọn phòng, chọn mức độ, điểm trừ, tiền phạt).
- [x] **Step 2.2:** Tạo `ViolationListViewModel.cs`:
  - 4 thẻ KPI: `TotalCount`, `PendingCount` (Cam), `ResolvedCount` (Xanh), `CriticalCount` (Đỏ).
  - Bộ lọc: Từ khóa tìm kiếm, Mức độ vi phạm, Trạng thái xử lý.
  - Commands: `LoadViolationsCommand`, `AddViolationCommand`, `EditViolationCommand`, `ResolveViolationCommand`, `DeleteViolationCommand`.
- [x] **Step 2.3:** Tạo `ViolationListView.axaml`:
  - Header: "⚖️ Quản Lý Vi Phạm Nội Quy & Kỷ Luật", phụ đề.
  - 4 thẻ KPI thống kê.
  - Thanh công cụ tìm kiếm và lọc.
  - DataGrid: Mã BB, Sinh viên, Phòng/Tòa, Tiêu đề, Mức độ (Badge màu), Điểm trừ, Tiền phạt, Trạng thái (Badge), Ngày vi phạm, Nút Sửa / Xử lý / Xóa.
- [x] **Step 2.4:** Tích hợp Sidebar Navigation trong `MainWindow.axaml` và `MainWindowViewModel.cs`:
  - Thêm nút Sidebar: "⚖️  Kỷ luật & Vi phạm".
  - Thêm DataTemplate cho `ViolationListViewModel` -> `ViolationListView`.
- [x] **Step 2.5:** Đăng ký DI trong `App.axaml.cs` và chạy biên dịch.
- [x] **Step 2.6:** Commit: `feat(desktop): add Dormitory Violation management desktop UI and dialogs`.

---

### Task 3: Dịch Vụ Gửi Email Hóa Đơn Tự Động Kèm PDF (MailKit / SMTP & TDD)

**Mục tiêu:** Tích hợp MailKit gửi email thông báo hóa đơn tiền phòng/điện nước đính kèm tệp PDF phiếu thu (`QuestPDF`) tới sinh viên, tích hợp UI gửi thư và cấu hình SMTP.

- [x] **Step 3.1:** Thêm package `MailKit` 4.8.0 vào `Dormitory.Infrastructure.csproj`.
- [x] **Step 3.2:** Tạo DTO cấu hình `EmailSettings.cs` (SmtpServer, SmtpPort, SenderEmail, SenderName, Username, Password, EnableSsl).
- [x] **Step 3.3:** Tạo interface `IEmailService.cs`:
  - `Task<bool> SendBillInvoiceEmailAsync(int billId, string recipientEmail, string recipientName, byte[] pdfReceiptBytes);`
  - `Task<bool> TestSmtpConnectionAsync(EmailSettings settings);`
- [x] **Step 3.4:** Viết Unit Tests TDD trong `tests/Dormitory.UnitTests/Services/EmailServiceTests.cs`.
- [x] **Step 3.5:** Triển khai `EmailService.cs` trong `Dormitory.Infrastructure`:
  - Xây dựng mẫu thư HTML sang trọng: Logo KTX, lời chào, chi tiết các khoản phí đến hạn, thông tin chuyển khoản ngân hàng và tệp đính kèm `PhieuThu_{BillCode}.pdf`.
  - Hỗ trợ chế độ Local/Mock an toàn khi chưa cấu hình máy chủ SMTP thật.
- [x] **Step 3.6:** Cập nhật `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs`:
  - Bổ sung lệnh `SendBillEmailCommand(BillDto bill)`.
  - Tự động sinh PDF qua `IPdfExportService` và gửi qua `_emailService.SendBillInvoiceEmailAsync`.
- [x] **Step 3.7:** Cập nhật `src/Dormitory.Desktop/Views/BillListView.axaml`: Thêm nút Toolbar "📧 Gửi email hóa đơn" và nút thao tác nhanh 📧 trên DataGrid row.
- [x] **Step 3.8:** Cập nhật `SystemSettingsView.axaml` & `SystemSettingsViewModel.cs`: Thêm Card cấu hình máy chủ SMTP và nút "Kiểm tra kết nối SMTP".
- [x] **Step 3.9:** Đăng ký DI trong `App.axaml.cs` và chạy kiểm thử.
- [x] **Step 3.10:** Commit: `feat(email): integrate MailKit for automated bill invoice email notification with PDF attachment`.

---

### Task 4: Kiểm Thử Tự Động Giao Diện E2E Headless (Vi phạm & Email)

**Mục tiêu:** Mở rộng dự án `Dormitory.E2ETests` kiểm thử tự động toàn diện luồng Lập biên bản vi phạm KTX và Gửi email hóa đơn.

- [x] **Step 4.1:** Viết kịch bản E2E `ViolationManagementE2ETests.cs`:
  - Khởi động `ViolationListViewModel` với SQLite test DB.
  - Xác minh DataGrid nạp danh sách biên bản mẫu và tính toán KPI.
  - Lập biên bản vi phạm mới cho sinh viên -> kiểm tra dữ liệu xuất hiện trên UI và lưu DB.
  - Thực thi lệnh giải quyết vi phạm -> trạng thái chuyển sang `Resolved`.
- [x] **Step 4.2:** Viết kịch bản E2E `BillEmailE2ETests.cs`:
  - Kích hoạt lệnh gửi email hóa đơn trên `BillListViewModel`.
  - Xác minh luồng sinh PDF và gửi email thành công.
- [x] **Step 4.3:** Chạy `dotnet test tests/Dormitory.E2ETests` và `dotnet test Dormitory.sln` -> 100% tests pass.
- [x] **Step 4.4:** Commit: `test(e2e): add E2E tests for violation management and automated email invoicing`.

---

### Task 5: CI/CD, Tài Liệu, GitNexus Sync & Phát Hành v2.2.0

**Mục tiêu:** Hoàn thiện tài liệu, đồng bộ đồ thị tri thức GitNexus, kiểm thử toàn diện và gắn tag phát hành `v2.2.0`.

- [x] **Step 5.1:** Cập nhật `docs/user-guide.md`:
  - Bổ sung hướng dẫn Phân hệ Quản lý Vi phạm Nội quy & Kỷ luật (lập biên bản, mức xử lý, điểm rèn luyện).
  - Bổ sung hướng dẫn Cấu hình SMTP và Gửi email hóa đơn tự động kèm PDF.
- [x] **Step 5.2:** Cập nhật `README.md`:
  - Nâng Badge phiên bản `v2.2.0` và cập nhật số lượng test mới.
  - Bổ sung Phase 6 vào Feature Matrix (100% hoàn thành).
- [x] **Step 5.3:** Cập nhật `docs/packaging-and-deployment.md`.
- [x] **Step 5.4:** Chạy toàn bộ giải pháp:
  - `dotnet build Dormitory.sln` -> 0 errors, 0 warnings.
  - `dotnet test Dormitory.sln` -> 100% tests pass (UnitTests + E2ETests).
- [x] **Step 5.5:** Đồng bộ đồ thị tri thức GitNexus:
  - `node .gitnexus/run.cjs detect-changes --scope all --repo .`
  - `node .gitnexus/run.cjs analyze --index-only`
- [x] **Step 5.6:** Commit: `docs: finalize Phase 6 violations and email invoicing for v2.2.0 release`.
- [x] **Step 5.7:** Tạo tag `v2.2.0` và đẩy lên GitHub kích hoạt CI/CD phát hành tự động:
  - `git tag -a v2.2.0 -m "Release v2.2.0: Student Violations Management and Automated Email Invoicing"`
  - `git push origin master --tags`

---

## Tiêu Chí Nghiệm Thu (Acceptance Criteria)

1. **Quản lý vi phạm nội quy**: Lập biên bản, phân loại 4 mức độ, trừ điểm rèn luyện, phạt tiền và theo dõi lịch sử sinh viên trơn tru.
2. **Gửi email hóa đơn tự động**: Tự động sinh nội dung email HTML trang nhã đính kèm tệp PDF phiếu thu gửi tới sinh viên, có cấu hình SMTP linh hoạt.
3. **Kiểm thử E2E Headless**: Kịch bản vi phạm và email chạy tự động, tin cậy trên Avalonia Headless.
4. **Chất lượng**: 100% tests pass (80+ tests), 0 cảnh báo, 0 lỗi biên dịch.
5. **Đồng bộ GitNexus**: Đồ thị tri thức phản ánh trọn vẹn kiến trúc mới.

## Phase 6 hoàn thành ✅

# Phase 7 – Báo cáo & Phân tích

## Mục tiêu
Xây dựng mô-đun báo cáo tổng hợp cho KTX, cung cấp các báo cáo:
- **Vi phạm theo thời gian** (thống kê số vi phạm, mức độ).
- **Thu phí** (tổng tiền thu, chi tiết phí phòng, điện, nước).
- **KPI phòng học** (số phòng còn trống, thiết bị).
- **Xuất PDF & Excel** (QuestPDF, ClosedXML).

## Tasks
- [x] **Task 1 – Phân tích yêu cầu báo cáo**
- [x] **Task 2 – Mở rộng mô hình dữ liệu** (entity `Report`, view `ReportDto`).
- [x] **Task 3 – Service báo cáo** (`IReportService`, `ReportService`).
- [x] **Task 4 – UI báo cáo** (list, generate dialog). 
- [x] **Task 5 – Kết nối UI‑Service** (DI, commands).
- [x] **Task 6 – Kiểm thử** (unit + E2E).
- [x] **Task 7 – Tài liệu** (user‑guide, README).
- [x] **Task 8 – CI/CD** (test, artifact).
- [x] **Task 9 – Phát hành v2.3.0** (tag).

## Phase 7 hoàn thành ✅
