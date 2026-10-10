# Kế Hoạch Triển Khai Giai Đoạn 9: Tự Động Gạch Nợ Hóa Đơn Qua Webhook Biến Động Số Dư (Bank Transaction Auto-Reconciliation)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hiện đại hóa toàn diện khâu thu phí ký túc xá bằng cách tự động tiếp nhận biến động số dư tài khoản ngân hàng qua Webhook (PayOS, Casso, Open Banking), bóc tách nội dung chuyển khoản (`KTX [Mã HĐ]`), đối soát số tiền và tự động chuyển trạng thái hóa đơn sang `Paid` (Đã thanh toán) thời gian thực trên giao diện Avalonia Desktop mà không cần nhân viên bấm xác nhận thủ công.

**Architecture:**
- **Core / Domain:** Thực thể `PaymentTransaction` lưu lịch sử giao dịch ngân hàng, mã giao dịch, số tiền, nội dung CK, raw payload, trạng thái đối soát (`Success`, `PartiallyPaid`, `Unmatched`, `Duplicate`).
- **Application Services:** `IPaymentReconciliationService` (bóc tách regex mã hóa đơn, đối soát số tiền và hóa đơn), `IWebhookListenerService` (tiếp nhận và xác thực chữ ký bảo mật Webhook HMAC-SHA256/Token), `IPaymentNotificationService` (bắn sự kiện gạch nợ thời gian thực sang UI thread).
- **Infrastructure:** EF Core SQLite migration bảng `PaymentTransactions`, Embedded Webhook Listener (sử dụng `HttpListener` siêu nhẹ không phụ thuộc ASP.NET Core cồng kềnh, cấu hình port và secret token linh hoạt).
- **Presentation (Avalonia UI 11):** Màn hình tra cứu và đối soát thủ công `PaymentTransactionListView`, cập nhật tự động thời gian thực trên `BillListView`, cấu hình Webhook trong `SystemSettingsView`.
- **Testing:** `Dormitory.UnitTests` + `Dormitory.E2ETests` (Headless UI).

**Tech Stack:** C# 12, .NET 8 LTS, Avalonia UI 11, CommunityToolkit.Mvvm, EF Core 8 SQLite, `HttpListener` (embedded lightweight server), HMAC-SHA256, xUnit, FluentAssertions.

---

## Global Constraints

- **Language Conventions:** Mã định danh (classes, interfaces, methods, properties, variables) 100% bằng **Tiếng Anh**. Chú thích mã nguồn (comments, XML docs), thông báo người dùng, nhãn giao diện và tài liệu 100% bằng **Tiếng Việt**.
- **Performance & Reliability:** Dịch vụ lắng nghe Webhook chạy bất đồng bộ trong background thread, không làm block giao diện người dùng. Có cơ chế lọc request giả mạo, kiểm tra trùng lặp mã giao dịch (`Idempotency`).
- **Security:** Hỗ trợ xác thực Webhook Token / HMAC SHA256 signature để chống giả mạo biến động số dư.
- **Backward Compatibility:** Không làm ảnh hưởng đến tính năng quét VietQR và xác nhận thu tiền tại quầy hiện có ở Phase 8.

---

## Review Focus

1. **Bóc tách nội dung chuyển khoản đa dạng:** Xử lý các biến thể nội dung như `KTX HD001`, `KTX-HD001`, `KTXHD001`, `ktxhd001`, hoặc sinh viên viết thêm tên/lời nhắn mà vẫn nhận diện chính xác mã hóa đơn.
2. **Đối soát số tiền nợ:** Xử lý đúng đắn các trường hợp: Chuyển khoản đủ tiền (gạch nợ thành công), chuyển khoản thiếu tiền (đánh dấu nộp thiếu, không gạch nợ hoàn toàn), chuyển khoản thừa tiền (gạch nợ thành công và ghi chú số tiền thừa).
3. **Chống trùng lặp giao dịch (Idempotency):** Cùng một mã giao dịch ngân hàng (`TransactionId`) gửi lại nhiều lần qua Webhook chỉ được xử lý đúng 1 lần duy nhất, tránh gạch nợ hoặc ghi nhận kép.
4. **Giao dịch không khớp mã hóa đơn (Unmatched):** Lưu vào danh sách giao dịch chưa khớp để kế toán/thủ quỹ có thể tra cứu và gán thủ công vào hóa đơn đúng.
5. **Đồng bộ hóa giao diện thời gian thực:** Đảm bảo khi nhận được Webhook ở background thread, sự kiện cập nhật giao diện được điều phối an toàn về Avalonia UI Thread (`Dispatcher.UIThread.InvokeAsync`).

---

## Danh Sách Tasks Triển Khai

- [x] **Task 1: Thiết kế Thực thể `PaymentTransaction`, Cấu hình EF Core SQLite & DTOs**
- [x] **Task 2: Thuật toán Bóc tách Cú pháp Chuyển khoản & Đối soát Hóa đơn (`IPaymentReconciliationService`) (TDD)**
- [x] **Task 3: Dịch vụ Nhúng Lắng nghe Webhook Biến Động Số Dư & Bảo mật Chữ Ký (`IWebhookListenerService`)**
- [ ] **Task 4: Cơ Chế Bắn Sự Kiện Thời Gian Thực & Tự Động Cập Nhật Danh Sách Hóa Đơn (`BillListViewModel`)**
- [ ] **Task 5: Giao Diện Quản Lý Lịch Sử Giao Dịch Đối Soát & Cấu Hình Webhook Trong Cài Đặt Hệ Thống**
- [ ] **Task 6: Kiểm Thử Tự Động E2E Headless & Mở Rộng Unit Tests**
- [ ] **Task 7: Tài Liệu Hướng Dẫn Webhook, GitNexus Sync & Phát Hành Phiên Bản v2.5.0**

---

### Task 1: Thiết kế Thực thể `PaymentTransaction`, Cấu hình EF Core SQLite & DTOs

**Mục tiêu:** Định nghĩa mô hình dữ liệu lưu trữ các giao dịch chuyển khoản ngân hàng được đẩy về từ Webhook, ánh xạ DbContext SQLite và các DTOs phục vụ tầng Application.

**Files:**
- Create: `src/Dormitory.Core/Enums/PaymentTransactionStatus.cs`
- Create: `src/Dormitory.Core/Entities/PaymentTransaction.cs`
- Create: `src/Dormitory.Application/DTOs/PaymentTransactionDtos.cs`
- Create: `src/Dormitory.Application/DTOs/WebhookPayloadDto.cs`
- Modify: `src/Dormitory.Infrastructure/Data/DormitoryDbContext.cs`
- Test: `tests/Dormitory.UnitTests/Entities/PaymentTransactionEntityTests.cs`

**Chi tiết các bước thực hiện:**
- [x] **Step 1.1:** Tạo enum `PaymentTransactionStatus` (`Success`, `PartiallyPaid`, `Unmatched`, `Duplicate`, `Failed`) với XML docs tiếng Việt.
- [x] **Step 1.2:** Tạo entity `PaymentTransaction` kế thừa `BaseEntity`:
  - `TransactionId` (string, mã giao dịch từ ngân hàng)
  - `BankBin` (string, mã ngân hàng)
  - `AccountNumber` (string, số tài khoản nhận)
  - `Amount` (decimal, số tiền giao dịch)
  - `Description` (string, nội dung chuyển khoản)
  - `TransactionDate` (DateTime, thời gian giao dịch ngân hàng)
  - `BillId` (int?, khóa ngoại liên kết tới hóa đơn, null nếu chưa khớp)
  - `BillCode` (string, mã hóa đơn bóc tách được)
  - `Status` (`PaymentTransactionStatus`)
  - `RawPayload` (string, JSON gốc phục vụ kiểm tra đối soát)
  - `Note` (string, ghi chú xử lý)
  - Navigation property: `Bill? Bill`
- [x] **Step 1.3:** Tạo `PaymentTransactionDto`, `PaymentTransactionFilterDto`, và `WebhookPayloadDto` (hỗ trợ các trường chuẩn: `gateway`, `transactionId`, `amount`, `description`, `accountNumber`, `bankBin`, `transactionDate`, `signature`).
- [x] **Step 1.4:** Cấu hình `DbSet<PaymentTransaction> PaymentTransactions` trong `DormitoryDbContext` với Fluent API (chỉ mục duy nhất cho `TransactionId`, quan hệ 1-N với `Bills`).
- [x] **Step 1.5:** Viết unit test xác minh khởi tạo Entity và DTOs. Chạy `dotnet test`.
- [x] **Step 1.6:** Commit thay đổi: `feat(payment): add PaymentTransaction entity, DTOs, and DbContext configuration`.

---

### Task 2: Thuật toán Bóc tách Cú pháp Chuyển khoản & Đối soát Hóa đơn (`IPaymentReconciliationService`) (TDD)

**Mục tiêu:** Xây dựng dịch vụ phân tích nội dung chuyển khoản, trích xuất mã hóa đơn bằng Regex thông minh, đối soát số tiền và tự động gạch nợ hóa đơn tương ứng.

**Files:**
- Create: `src/Dormitory.Application/Interfaces/IPaymentReconciliationService.cs`
- Create: `src/Dormitory.Infrastructure/Services/PaymentReconciliationService.cs`
- Test: `tests/Dormitory.UnitTests/Services/PaymentReconciliationServiceTests.cs`

**Chi tiết các bước thực hiện:**
- [x] **Step 2.1:** Khai báo interface `IPaymentReconciliationService`:
  - `Task<PaymentReconciliationResultDto> ProcessTransactionAsync(WebhookPayloadDto payload);`
  - `string? ExtractBillCode(string description, string prefix = "KTX");`
  - `Task<bool> ManuallyAssignBillAsync(int transactionId, int billId, string note = "");`
  - `Task<List<PaymentTransactionDto>> GetTransactionsAsync(PaymentTransactionFilterDto filter);`
- [x] **Step 2.2:** Viết Unit Tests trước (TDD) trong `PaymentReconciliationServiceTests.cs` cho các trường hợp:
  - Bóc tách mã hóa đơn từ đa dạng cú pháp: `"KTX HD001 NGUYEN VAN A"`, `"KTX-HD001"`, `"ktxhd001"`, `"Chuyen tien phong KTX HD005"`.
  - Giao dịch đúng số tiền: Hóa đơn được chuyển sang `BillStatus.Paid`, ngày thanh toán được ghi nhận, trạng thái giao dịch `Success`.
  - Giao dịch nộp thiếu tiền: Hóa đơn giữ nguyên `Unpaid`, trạng thái giao dịch `PartiallyPaid`.
  - Giao dịch thừa tiền: Hóa đơn chuyển `Paid`, giao dịch lưu ghi chú thừa tiền.
  - Giao dịch trùng `TransactionId`: Bỏ qua không gạch nợ 2 lần, trạng thái `Duplicate`.
  - Giao dịch không tìm thấy mã hóa đơn: Trạng thái `Unmatched`.
- [x] **Step 2.3:** Triển khai `PaymentReconciliationService` trong `src/Dormitory.Infrastructure/Services/`:
  - Sử dụng Regex bóc tách linh hoạt: `new Regex($@"(?:^|\s|\b){Regex.Escape(prefix)}[-_\s]*([A-Za-z0-9_-]+)", RegexOptions.IgnoreCase)`.
  - Truy vấn CSDL, kiểm tra idempotency của `TransactionId`.
  - Cập nhật `Bill` và lưu `PaymentTransaction`.
- [x] **Step 2.4:** Chạy `dotnet test` và đảm bảo toàn bộ tests PASS.
- [x] **Step 2.5:** Commit thay đổi: `feat(payment): implement PaymentReconciliationService with smart regex parsing and auto-reconciliation`.

---

### Task 3: Dịch vụ Nhúng Lắng nghe Webhook Biến Động Số Dư & Bảo mật Chữ Ký (`IWebhookListenerService`)

**Mục tiêu:** Cung cấp dịch vụ embedded HTTP listener chạy ngầm trên máy tính để tiếp nhận Webhook POST từ các cổng thanh toán ngân hàng (PayOS, Casso, Open Banking), kiểm tra chữ ký xác thực an toàn và chuyển giao cho `IPaymentReconciliationService`.

**Files:**
- Create: `src/Dormitory.Application/DTOs/WebhookSettingsDto.cs`
- Create: `src/Dormitory.Application/Interfaces/IWebhookListenerService.cs`
- Create: `src/Dormitory.Infrastructure/Services/WebhookListenerService.cs`
- Modify: `src/Dormitory.Desktop/App.axaml.cs`
- Modify: `tests/Dormitory.E2ETests/TestFixture.cs`
- Test: `tests/Dormitory.UnitTests/Services/WebhookListenerServiceTests.cs`

**Chi tiết các bước thực hiện:**
- [x] **Step 3.1:** Tạo `WebhookSettingsDto` (thuộc tính: `IsEnabled`, `Port` = 5005, `Path` = `"/api/webhook/payment"`, `SecretKey`, `Provider` = `"PayOS"` / `"Casso"` / `"Generic"`).
- [x] **Step 3.2:** Khai báo `IWebhookListenerService` (`StartAsync()`, `StopAsync()`, `IsRunning`, `GetSettingsAsync()`, `SaveSettingsAsync()`, `TestWebhookAsync()`).
- [x] **Step 3.3:** Viết unit tests kiểm tra: Khởi động listener, dừng listener, xử lý chuỗi JSON webhook PayOS / Casso / Generic, xác thực chữ ký HMAC SHA256 an toàn, từ chối request sai secret key.
- [x] **Step 3.4:** Triển khai `WebhookListenerService` bằng `HttpListener` chuẩn .NET (nhẹ, không chiếm dụng tài nguyên):
  - Hỗ trợ endpoint POST `/api/webhook/payment`.
  - Trả về HTTP 200 OK ngay lập tức cho ngân hàng để tránh timeout.
  - Phân tích JSON và gọi `IPaymentReconciliationService.ProcessTransactionAsync`.
- [x] **Step 3.5:** Đăng ký DI trong `App.axaml.cs` và `TestFixture.cs`. Tự động khởi động listener khi ứng dụng mở (nếu cấu hình `IsEnabled == true`).
- [x] **Step 3.6:** Chạy `dotnet test`. Commit thay đổi: `feat(payment): implement embedded WebhookListenerService with HMAC security`.

---

### Task 4: Cơ Chế Bắn Sự Kiện Thời Gian Thực & Tự Động Cập Nhật Danh Sách Hóa Đơn (`BillListViewModel`)

**Mục tiêu:** Khi có thanh toán gạch nợ thành công từ Webhook ở background thread, hệ thống tự động bắn sự kiện an toàn sang Avalonia UI Thread để cập nhật danh sách hóa đơn tức thì và hiển thị thông báo thành công cho người dùng.

**Files:**
- Create: `src/Dormitory.Application/Interfaces/IPaymentNotificationService.cs`
- Create: `src/Dormitory.Infrastructure/Services/PaymentNotificationService.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/MainWindowViewModel.cs`
- Test: `tests/Dormitory.UnitTests/Services/PaymentNotificationServiceTests.cs`
- Test: `tests/Dormitory.UnitTests/ViewModels/BillListViewModelNotificationTests.cs`

**Chi tiết các bước thực hiện:**
- [x] **Step 4.1:** Tạo interface `IPaymentNotificationService` với event `event EventHandler<PaymentReceivedEventArgs>? OnPaymentReceived`.
- [x] **Step 4.2:** Triển khai `PaymentNotificationService`: Được gọi từ `PaymentReconciliationService` khi gạch nợ thành công để phát event.
- [x] **Step 4.3:** Cập nhật `BillListViewModel`:
  - Lắng nghe event `OnPaymentReceived`.
  - Khi nhận event, sử dụng `Dispatcher.UIThread.InvokeAsync` để tự động nạp lại danh sách hóa đơn (`LoadBillsAsync()`).
  - Hiển thị thông báo Toast / In-app notification: *"Hóa đơn [Mã HĐ] đã được thanh toán tự động qua chuyển khoản ngân hàng!"*.
- [x] **Step 4.4:** Đăng ký Singleton `IPaymentNotificationService` trong `App.axaml.cs` và `TestFixture.cs`.
- [x] **Step 4.5:** Viết unit test xác minh: Khi event được kích hoạt, `BillListViewModel` tự động refresh danh sách hóa đơn.
- [x] **Step 4.6:** Chạy `dotnet test`. Commit thay đổi: `feat(desktop): integrate real-time payment notification and auto-refresh in BillListViewModel`.

---

### Task 5: Giao Diện Quản Lý Lịch Sử Giao Dịch Đối Soát & Cấu Hình Webhook Trong Cài Đặt Hệ Thống

**Mục tiêu:** Xây dựng màn hình tra cứu toàn bộ giao dịch ngân hàng đã nhận, hỗ trợ kế toán gán thủ công các giao dịch sai cú pháp, và tích hợp khối cấu hình Webhook trong Cài đặt hệ thống.

**Files:**
- Create: `src/Dormitory.Desktop/ViewModels/PaymentTransactionListViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/PaymentTransactionListView.axaml`
- Create: `src/Dormitory.Desktop/Views/PaymentTransactionListView.axaml.cs`
- Create: `src/Dormitory.Desktop/Views/AssignBillDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/AssignBillDialogWindow.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/SystemSettingsViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/SystemSettingsView.axaml`
- Modify: `src/Dormitory.Desktop/Views/MainWindow.axaml`
- Modify: `src/Dormitory.Desktop/ViewModels/MainWindowViewModel.cs`
- Test: `tests/Dormitory.UnitTests/ViewModels/PaymentTransactionListViewModelTests.cs`

**Chi tiết các bước thực hiện:**
- [ ] **Step 5.1:** Xây dựng `PaymentTransactionListViewModel`:
  - Danh sách `ObservableCollection<PaymentTransactionDto> Transactions`.
  - Các bộ lọc: Trạng thái đối soát (`All`, `Success`, `PartiallyPaid`, `Unmatched`, `Duplicate`), Từ ngày - Đến ngày, Tìm kiếm từ khóa.
  - Các thẻ KPI: Tổng số tiền thu tự động, Giao dịch thành công, Giao dịch chưa khớp cần xử lý.
  - Command: `AssignBillCommand` (mở hộp thoại chọn hóa đơn để gán thủ công cho giao dịch `Unmatched`).
  - Command: `RefreshCommand`.
- [ ] **Step 5.2:** Thiết kế giao diện `PaymentTransactionListView.axaml` với DataGrid hiện đại, các huy hiệu màu trạng thái (Xanh = Thành công, Cam = Chưa khớp, Đỏ = Trùng lặp), các nút bấm thao tác nhanh.
- [ ] **Step 5.3:** Thêm mục điều hướng **"💳 Giao Dịch Đối Soát"** vào Sidebar trong `MainWindow.axaml` và `MainWindowViewModel.cs`.
- [ ] **Step 5.4:** Nâng cấp `SystemSettingsViewModel` và `SystemSettingsView.axaml`:
  - Thêm Card: **"🌐 Cấu Hình Webhook Tự Động Gạch Nợ (Open Banking)"**.
  - Checkbox bật/tắt Webhook, ô nhập Port (mặc định 5005), ô nhập Secret Key xác thực, ComboBox chọn cổng (PayOS, Casso, Generic).
  - Nút "💾 Lưu cấu hình Webhook" và nút "🧪 Thử nghiệm gửi Webhook mẫu".
- [ ] **Step 5.5:** Viết unit test cho ViewModel và kiểm tra biên dịch.
- [ ] **Step 5.6:** Chạy `dotnet test`. Commit thay đổi: `feat(desktop): add PaymentTransactionListView and Webhook settings UI`.

---

### Task 6: Kiểm Thử Tự Động E2E Headless & Mở Rộng Unit Tests

**Mục tiêu:** Mở rộng toàn diện bộ kiểm thử tự động, xây dựng các kịch bản E2E Headless kiểm thử luồng nhận webhook và tự động gạch nợ hóa đơn từ đầu đến cuối.

**Files:**
- Create: `tests/Dormitory.E2ETests/Journeys/PaymentAutoReconciliationE2ETests.cs`
- Modify: `tests/Dormitory.UnitTests/Services/PaymentReconciliationServiceTests.cs`
- Modify: `tests/Dormitory.UnitTests/Services/WebhookListenerServiceTests.cs`

**Chi tiết các bước thực hiện:**
- [ ] **Step 6.1:** Viết kịch bản E2E `Should_Auto_Reconcile_And_Mark_Bill_Paid_Via_Webhook_Successfully`:
  - Chuẩn bị hóa đơn chưa thanh toán trong CSDL mẫu.
  - Khởi tạo listener và gửi HTTP POST request giả lập Webhook với nội dung chứa mã hóa đơn và đủ số tiền.
  - Xác minh hóa đơn trong CSDL tự động chuyển sang trạng thái `Paid` mà không cần thao tác tay.
  - Xác minh `PaymentTransaction` được tạo thành công với trạng thái `Success`.
- [ ] **Step 6.2:** Viết kịch bản E2E `Should_Handle_Unmatched_Webhook_And_Allow_Manual_Assignment`:
  - Gửi webhook với nội dung không chứa mã hóa đơn hợp lệ.
  - Xác minh giao dịch được lưu với trạng thái `Unmatched`.
  - Giả lập người dùng mở giao diện gán hóa đơn thủ công và xác nhận.
  - Xác minh hóa đơn được gạch nợ và giao dịch chuyển thành `Success`.
- [ ] **Step 6.3:** Viết kịch bản E2E `Should_Reject_Webhook_With_Invalid_Secret_Key`:
  - Gửi webhook với sai signature / token.
  - Xác minh hệ thống từ chối (HTTP 401/403) và không gạch nợ hóa đơn.
- [ ] **Step 6.4:** Chạy toàn bộ test suite `dotnet test` và đảm bảo đạt **100% Tests Pass**.
- [ ] **Step 6.5:** Commit thay đổi: `test(payment): add automated E2E headless journeys and unit tests for auto-reconciliation`.

---

### Task 7: Tài Liệu Hướng Dẫn Webhook, GitNexus Sync & Phát Hành Phiên Bản v2.5.0

**Mục tiêu:** Hoàn thiện tài liệu kỹ thuật, hướng dẫn cấu hình Webhook tích hợp với PayOS / Casso / Ngân hàng thực tế, đồng bộ đồ thị tri thức GitNexus và phát hành phiên bản `v2.5.0`.

**Files:**
- Create: `docs/phase9/webhook-auto-reconciliation-spec.md`
- Modify: `README.md`
- Modify: `docs/user-guide.md`
- Modify: `docs/architecture.md`
- Modify: `docs/packaging-and-deployment.md`
- Modify: `src/Dormitory.Desktop/appsettings.json`

**Chi tiết các bước thực hiện:**
- [ ] **Step 7.1:** Tạo tài liệu kỹ thuật `docs/phase9/webhook-auto-reconciliation-spec.md` mô tả chi tiết đặc tả cấu trúc Webhook payload của PayOS, Casso, và kịch bản kết nối thực tế qua Cloudflare Tunnel / ngrok.
- [ ] **Step 7.2:** Cập nhật `README.md` với mục tính năng mới: Tự động gạch nợ qua Webhook biến động số dư ngân hàng, nâng phiên bản lên `v2.5.0`, cập nhật số lượng test.
- [ ] **Step 7.3:** Cập nhật `docs/user-guide.md`: Thêm mục hướng dẫn thiết lập Webhook và sử dụng màn hình Giao dịch đối soát.
- [ ] **Step 7.4:** Cập nhật `docs/architecture.md`: Thêm sơ đồ Mermaid kiến trúc Webhook Listener và luồng đối soát tự động.
- [ ] **Step 7.5:** Cập nhật `docs/packaging-and-deployment.md`: Bổ sung Release Notes cho phiên bản `v2.5.0`.
- [ ] **Step 7.6:** Chạy đồng bộ hóa GitNexus: `node .gitnexus/run.cjs analyze --index-only`.
- [ ] **Step 7.7:** Chạy kiểm thử xác thực cuối: `dotnet test`.
- [ ] **Step 7.8:** Commit thay đổi: `docs: finalize Phase 9 auto-reconciliation documentation and release notes for v2.5.0`.
- [ ] **Step 7.9:** Gắn Git Tag `v2.4.0` -> `v2.5.0` và cập nhật roadmap hoàn thành.
