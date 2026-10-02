# Hiện Đại Hóa Hệ Thống Quản Lý Ký Túc Xá (Dormitory Management) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Chuyển đổi và nâng cấp toàn diện ứng dụng WinForms .NET Framework 4.7.2 cũ sang ứng dụng Desktop hiện đại chạy đa nền tảng (macOS, Windows, Linux) trên .NET 8 LTS + Avalonia UI 11 + EF Core + SQLite với kiến trúc Clean Architecture & MVVM.

**Architecture:** 
- **Presentation:** Avalonia UI 11 + CommunityToolkit.Mvvm (Views, ViewModels, Navigation, FluentTheme).
- **Application:** Business Services, DTOs, Interfaces, Validation.
- **Core / Domain:** Entities (Room, Student, Contract, Bill, User), Enums, Domain Rules.
- **Infrastructure:** EF Core 8 (SQLite provider), Repositories, PasswordHasher (BCrypt).
- **Tests:** xUnit + FluentAssertions kiểm thử tự động toàn bộ logic nghiệp vụ.

**Tech Stack:** C# 12, .NET 8 LTS, Avalonia UI 11, Entity Framework Core 8, SQLite, BCrypt.Net-Next, xUnit.

---

## Quy Chuẩn Ngôn Ngữ & Quy Ước Bắt Buộc
1. **Mã nguồn (Code)**: Tên class, interface, method, property, variable bằng **Tiếng Anh chuẩn**.
2. **Chú thích (Comments)**: 100% bằng **Tiếng Việt**, diễn giải rõ ràng mục đích nghiệp vụ.
3. **Tài liệu (`docs/`)**: 100% bằng **Tiếng Việt**.
4. **README (`README.md`)**: 100% bằng **Tiếng Việt**, có sơ đồ kiến trúc Mermaid và mô tả giao diện trực quan.
5. **Đặt tên Solution & Projects**: Sử dụng tiền tố **Dormitory** (`Dormitory.Core`, `Dormitory.Application`, `Dormitory.Infrastructure`, `Dormitory.Desktop`, `Dormitory.UnitTests`).

---

## Danh Sách Task Triển Khai

### Task 1: Thiết lập môi trường và cấu trúc Solution mới (.NET 8)
**Mục tiêu:** Cài đặt .NET 8 SDK (nếu chưa có) và khởi tạo cấu trúc Solution sạch sẽ theo Clean Architecture.
**Files to create/modify:**
- `Dormitory.sln`
- `src/Dormitory.Core/Dormitory.Core.csproj`
- `src/Dormitory.Application/Dormitory.Application.csproj`
- `src/Dormitory.Infrastructure/Dormitory.Infrastructure.csproj`
- `src/Dormitory.Desktop/Dormitory.Desktop.csproj`
- `tests/Dormitory.UnitTests/Dormitory.UnitTests.csproj`

- [ ] **Step 1.1:** Cài đặt .NET 8 SDK trên macOS thông qua Homebrew (`brew install --cask dotnet-sdk`) và xác minh bằng `dotnet --version`.
- [ ] **Step 1.2:** Tạo thư mục `src/` và `tests/`, khởi tạo `Dormitory.sln`.
- [ ] **Step 1.3:** Tạo các project classlib cho `Core`, `Application`, `Infrastructure`, project xUnit cho `UnitTests`, và project Avalonia cho `Desktop`.
- [ ] **Step 1.4:** Thiết lập Project References đúng quy tắc Clean Architecture:
  - `Dormitory.Application` tham chiếu `Dormitory.Core`.
  - `Dormitory.Infrastructure` tham chiếu `Dormitory.Application` và `Dormitory.Core`.
  - `Dormitory.Desktop` tham chiếu `Dormitory.Application` và `Dormitory.Infrastructure`.
  - `Dormitory.UnitTests` tham chiếu `Dormitory.Core` và `Dormitory.Application`.
- [ ] **Step 1.5:** Build toàn bộ solution với lệnh `dotnet build Dormitory.sln` để xác nhận cấu hình thành công.

---

### Task 2: Xây dựng Domain Layer (Dormitory.Core)
**Mục tiêu:** Định nghĩa toàn bộ Entities và Enums nghiệp vụ của hệ thống Ký túc xá.
**Files to create/modify:**
- `src/Dormitory.Core/Enums/RoomStatus.cs` (Available, Occupied, Maintenance)
- `src/Dormitory.Core/Enums/RoomType.cs` (Standard, Premium, Male, Female)
- `src/Dormitory.Core/Enums/ContractStatus.cs` (Active, Expired, Terminated)
- `src/Dormitory.Core/Enums/BillStatus.cs` (Unpaid, Paid, Overdue)
- `src/Dormitory.Core/Enums/UserRole.cs` (Admin, Manager, Staff)
- `src/Dormitory.Core/Entities/Room.cs`
- `src/Dormitory.Core/Entities/Student.cs`
- `src/Dormitory.Core/Entities/Contract.cs`
- `src/Dormitory.Core/Entities/Bill.cs`
- `src/Dormitory.Core/Entities/Employee.cs`
- `src/Dormitory.Core/Entities/User.cs`

- [ ] **Step 2.1:** Tạo các Enums với comment tiếng Việt mô tả từng trạng thái nghiệp vụ.
- [ ] **Step 2.2:** Tạo Entity `Room`: Số phòng, tòa nhà, tầng, sức chứa tối đa, số người đang ở, đơn giá, trạng thái.
- [ ] **Step 2.3:** Tạo Entity `Student`: Mã SV, họ tên, giới tính, ngày sinh, CCCD, quê quán, lớp, SĐT, thông tin liên hệ phụ huynh.
- [ ] **Step 2.4:** Tạo Entity `Contract`: Mã HĐ, Sinh viên, Phòng, ngày bắt đầu, ngày kết thúc, tiền cọc, trạng thái HĐ.
- [ ] **Step 2.5:** Tạo Entity `Bill`: Mã HĐ, Phòng, tháng/năm, tiền phòng, chỉ số điện cũ/mới, chỉ số nước cũ/mới, đơn giá, phụ phí, tổng tiền, trạng thái thanh toán.
- [ ] **Step 2.6:** Tạo Entity `Employee` và `User`: Tài khoản đăng nhập, hash mật khẩu, phân quyền.
- [ ] **Step 2.7:** Chạy `dotnet build src/Dormitory.Core` để kiểm tra biên dịch.

---

### Task 3: Xây dựng Infrastructure & EF Core Database (Dormitory.Infrastructure)
**Mục tiêu:** Cấu hình DbContext, Fluent API mapping, Migrations SQLite và Data Seeder.
**Files to create/modify:**
- `src/Dormitory.Infrastructure/Data/DormitoryDbContext.cs`
- `src/Dormitory.Infrastructure/Data/DataSeeder.cs`
- `src/Dormitory.Infrastructure/Security/PasswordHasher.cs`

- [ ] **Step 3.1:** Cài đặt các package: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`, `BCrypt.Net-Next`.
- [ ] **Step 3.2:** Xây dựng `DormitoryDbContext` với cấu hình Fluent API cho khóa chính, quan hệ 1-N (Room - Students, Room - Bills, Student - Contracts).
- [ ] **Step 3.3:** Xây dựng `PasswordHasher` sử dụng thuật toán BCrypt để mã hóa và kiểm tra mật khẩu.
- [ ] **Step 3.4:** Viết `DataSeeder` để tự động tạo dữ liệu mẫu khi khởi động (tài khoản admin mặc định, danh sách phòng ở các tòa A/B/C, sinh viên mẫu).
- [ ] **Step 3.5:** Tạo migration ban đầu `InitialCreate` và tạo file database SQLite `dormitory.db`.
- [ ] **Step 3.6:** Chạy `dotnet build src/Dormitory.Infrastructure` để kiểm tra.

---

### Task 4: Xây dựng Application Services & Business Logic (Dormitory.Application)
**Mục tiêu:** Hiện đại hóa toàn bộ các Stored Procedures cũ sang các C# Services hướng đối tượng.
**Files to create/modify:**
- `src/Dormitory.Application/DTOs/*.cs` (RoomDto, StudentDto, ContractDto, BillDto, AuthDto)
- `src/Dormitory.Application/Interfaces/*.cs` (IRoomService, IStudentService, IContractService, IBillService, IAuthService)
- `src/Dormitory.Application/Services/*.cs` (Triển khai các Services tương ứng)

- [ ] **Step 4.1:** Định nghĩa DTOs phục vụ việc truyền dữ liệu giữa các tầng.
- [ ] **Step 4.2:** Viết `RoomService`: Lấy danh sách phòng theo tòa/trạng thái, thêm/sửa phòng, kiểm tra còn chỗ trống.
- [ ] **Step 4.3:** Viết `StudentService`: Thêm/sửa/xóa sinh viên, tìm kiếm theo tên, quê quán, lớp, CCCD.
- [ ] **Step 4.4:** Viết `ContractService`: Tạo hợp đồng mới (tự động kiểm tra sức chứa phòng, giới tính phù hợp, sinh viên chưa có HĐ hiệu lực), gia hạn, thanh lý hợp đồng.
- [ ] **Step 4.5:** Viết `BillService`: Lập hóa đơn hàng tháng (tính tiền điện = số kWh * đơn giá, tiền nước = số m3 * đơn giá, cộng tiền phòng), xác nhận thanh toán.
- [ ] **Step 4.6:** Viết `AuthService`: Đăng nhập, xác thực mật khẩu qua BCrypt, đổi mật khẩu.
- [ ] **Step 4.7:** Chạy `dotnet build src/Dormitory.Application`.

---

### Task 5: Viết Unit Tests tự động cho Core Logic (Dormitory.UnitTests)
**Mục tiêu:** Đảm bảo toàn bộ quy tắc nghiệp vụ quan trọng được kiểm thử chặt chẽ (TDD/Verification).
**Files to create/modify:**
- `tests/Dormitory.UnitTests/Services/RoomServiceTests.cs`
- `tests/Dormitory.UnitTests/Services/ContractServiceTests.cs`
- `tests/Dormitory.UnitTests/Services/BillServiceTests.cs`
- `tests/Dormitory.UnitTests/Security/PasswordHasherTests.cs`

- [ ] **Step 5.1:** Viết Unit Test kiểm tra tính toán hóa đơn tiền điện, nước, phòng.
- [ ] **Step 5.2:** Viết Unit Test kiểm tra việc tạo hợp đồng khi phòng đã đầy (kỳ vọng ném ra ngoại lệ hoặc trả về lỗi nghiệp vụ).
- [ ] **Step 5.3:** Viết Unit Test kiểm tra mã hóa và xác minh mật khẩu.
- [ ] **Step 5.4:** Chạy `dotnet test` và đảm bảo 100% test cases đều PASS (Xanh lá).

---

### Task 6: Xây dựng Giao diện Avalonia UI Desktop (Dormitory.Desktop)
**Mục tiêu:** Xây dựng ứng dụng Desktop giao diện Fluent Dashboard hiện đại, chạy trực tiếp trên macOS & Windows.
**Files to create/modify:**
- `src/Dormitory.Desktop/App.axaml`, `App.axaml.cs`
- `src/Dormitory.Desktop/ViewModels/ViewModelBase.cs`
- `src/Dormitory.Desktop/ViewModels/MainWindowViewModel.cs`
- `src/Dormitory.Desktop/ViewModels/DashboardViewModel.cs`
- `src/Dormitory.Desktop/ViewModels/RoomListViewModel.cs`
- `src/Dormitory.Desktop/ViewModels/StudentListViewModel.cs`
- `src/Dormitory.Desktop/ViewModels/ContractListViewModel.cs`
- `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs`
- `src/Dormitory.Desktop/Views/MainWindow.axaml`
- `src/Dormitory.Desktop/Views/DashboardView.axaml`
- `src/Dormitory.Desktop/Views/RoomListView.axaml`
- `src/Dormitory.Desktop/Views/StudentListView.axaml`
- `src/Dormitory.Desktop/Views/ContractListView.axaml`
- `src/Dormitory.Desktop/Views/BillListView.axaml`

- [ ] **Step 6.1:** Cài đặt package `Avalonia.Themes.Fluent`, `Avalonia.Controls.DataGrid`, `CommunityToolkit.Mvvm`, `Microsoft.Extensions.DependencyInjection`.
- [ ] **Step 6.2:** Cấu hình Dependency Injection trong `App.axaml.cs` để inject `DbContext`, `Services`, `ViewModels`.
- [ ] **Step 6.3:** Thiết kế layout chính `MainWindow.axaml`: Sidebar điều hướng bên trái (Dashboard, Phòng, Sinh viên, Hợp đồng, Hóa đơn) và Content area bên phải.
- [ ] **Step 6.4:** Xây dựng màn hình `DashboardView`: Thống kê tổng số phòng, số sinh viên đang ở, doanh thu tháng, biểu đồ tỷ lệ lấp đầy.
- [ ] **Step 6.5:** Xây dựng màn hình `RoomListView`: Hiển thị danh sách phòng theo thẻ Card/DataGrid, lọc theo tòa/tầng/trạng thái.
- [ ] **Step 6.6:** Xây dựng màn hình `StudentListView`: Tìm kiếm và quản lý sinh viên.
- [ ] **Step 6.7:** Xây dựng màn hình `ContractListView` và `BillListView`: Lập hợp đồng và hóa đơn, in phiếu / xuất file.
- [ ] **Step 6.8:** Chạy thử nghiệm ứng dụng `dotnet run --project src/Dormitory.Desktop`.

---

### Task 7: Cập nhật Tài Liệu, README Tiếng Việt và GitNexus Sync
**Mục tiêu:** Hoàn thiện tài liệu dự án, sơ đồ Mermaid và đồng bộ đồ thị tri thức GitNexus.
**Files to create/modify:**
- `README.md`
- `docs/user-guide.md`
- `docs/architecture.md`

- [ ] **Step 7.1:** Viết file `README.md` bằng tiếng Việt với:
  - Giới thiệu tổng quan dự án.
  - Sơ đồ kiến trúc Mermaid (Clean Architecture & MVVM).
  - Hướng dẫn cài đặt và chạy trên macOS/Windows chỉ với 1 dòng lệnh.
  - Mô tả trực quan các tính năng chính kèm hình ảnh/sơ đồ luồng giao diện.
- [ ] **Step 7.2:** Tạo tài liệu hướng dẫn sử dụng và tài liệu kiến trúc trong thư mục `docs/`.
- [ ] **Step 7.3:** Chạy `gitnexus analyze` để phân tích và cập nhật toàn bộ đồ thị tri thức mã nguồn mới.
- [ ] **Step 7.4:** Chạy `gitnexus detect-changes` để kiểm tra độ tin cậy và sự toàn vẹn của codebase.
