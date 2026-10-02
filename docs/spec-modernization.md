# Đặc tả Kỹ thuật: Hiện đại hóa Hệ thống Quản lý Ký túc xá (KTX)

## 1. Tổng quan & Mục tiêu
Dự án nhằm tái thiết kế và nâng cấp toàn diện ứng dụng Quản lý Ký túc xá từ nền tảng cũ (Windows Forms / .NET Framework 4.7.2 / EF6 EDMX) sang ứng dụng Desktop hiện đại, đa nền tảng và chuẩn kiến trúc doanh nghiệp:
- **Nền tảng mục tiêu**: .NET 8 LTS + Avalonia UI 11.
- **Hệ điều hành hỗ trợ**: macOS, Windows và Linux.
- **Phong cách UI**: Dashboard hiện đại (Fluent Design), Dark/Light theme, thanh điều hướng sidebar mượt mà.

---

## 2. Quy chuẩn Ngôn ngữ & Tài liệu
- **Mã nguồn (Codebase)**:
  - Tên class, struct, interface, method, variable: **Tiếng Anh chuẩn** (PascalCase cho class/method, camelCase cho local variables, prefix `I` cho interfaces).
  - Comment trong code: **Tiếng Việt**, giải thích mục đích nghiệp vụ và logic quan trọng.
- **Tài liệu & Báo cáo**:
  - Toàn bộ tài liệu trong thư mục `docs/`: **Tiếng Việt**.
  - File `README.md`: **Tiếng Việt**, kèm sơ đồ kiến trúc Mermaid và hình ảnh mô tả luồng hoạt động.

---

## 3. Kiến trúc Hệ thống (Clean Architecture + MVVM)

Hệ thống được tổ chức thành cấu trúc Solution phân tách trách nhiệm rõ ràng:

```
Dormitory-Manager/
├── docs/                             # Tài liệu kỹ thuật tiếng Việt
├── src/
│   ├── Dormitory.Core/               # Domain Layer: Entities, Enums, Exceptions
│   │   ├── Entities/                 # Room, Student, Contract, Bill, Employee, User
│   │   └── Enums/                    # RoomStatus, BillStatus, ContractStatus, UserRole
│   │
│   ├── Dormitory.Application/        # Application Layer: DTOs, Services, Interfaces
│   │   ├── Common/                   # Result pattern, PagedList, Exceptions
│   │   ├── DTOs/                     # Data Transfer Objects cho từng phân hệ
│   │   ├── Interfaces/               # IStudentService, IRoomService, IBillService...
│   │   └── Services/                 # Triển khai logic nghiệp vụ (thay thế Stored Procedures cũ)
│   │
│   ├── Dormitory.Infrastructure/     # Infrastructure Layer: Database, Security, Export
│   │   ├── Data/                     # DormitoryDbContext, EntityConfigurations, Migrations
│   │   ├── Security/                 # PasswordHasher (BCrypt)
│   │   └── Services/                 # ExportService (Excel/PDF)
│   │
│   └── Dormitory.Desktop/            # Presentation Layer: Avalonia UI 11 + MVVM
│       ├── Assets/                   # Icons, Fonts, Images
│       ├── ViewModels/               # MainWindowViewModel, DashboardViewModel, StudentViewModel...
│       ├── Views/                    # MainView, DashboardView, StudentView, RoomView...
│       └── App.axaml                 # Styles, FluentTheme, Dependency Injection Setup
│
└── tests/
    └── Dormitory.UnitTests/          # Kiểm thử tự động cho Core logic (xUnit + FluentAssertions)
```

---

## 4. Các Phân hệ Nghiệp vụ Chính

### 4.1. Phân hệ Phòng ở (`Room`)
- Quản lý danh sách tòa nhà, số phòng, tầng.
- Thuộc tính: Số người tối đa, số người hiện tại, loại phòng (Nam/Nữ, Tiêu chuẩn/VIP), đơn giá thuê.
- Trạng thái: Còn trống (`Available`), Đã đầy (`Occupied`), Đang sửa chữa (`Maintenance`).

### 4.2. Phân hệ Sinh viên (`Student`)
- Quản lý hồ sơ: Mã sinh viên, họ tên, ngày sinh, giới tính, quê quán, số CCCD/CMND, lớp, số điện thoại, thông tin người giám hộ.
- Kiểm tra tính hợp lệ dữ liệu (validation CCCD, SĐT, Email).

### 4.3. Phân hệ Hợp đồng (`Contract`)
- Đăng ký và gia hạn hợp đồng thuê phòng KTX.
- Logic kiểm tra: Sinh viên chưa có hợp đồng hiệu lực khác, phòng còn chỗ trống phù hợp giới tính.
- Cập nhật tự động số lượng sinh viên đang ở trong phòng khi ký hoặc kết thúc hợp đồng.

### 4.4. Hóa đơn & Dịch vụ (`Bill`)
- Quản lý hóa đơn định kỳ hàng tháng theo từng phòng.
- Các khoản phí: Tiền phòng, tiền điện (chỉ số mới - chỉ số cũ * đơn giá), tiền nước, dịch vụ vệ sinh/internet.
- Trạng thái: Chưa thanh toán (`Unpaid`), Đã thanh toán (`Paid`), Quá hạn (`Overdue`).

### 4.5. Phân hệ Thống kê & Dashboard (`Statistic / Dashboard`)
- Biểu đồ tổng quan: Tỷ lệ lấp đầy phòng, doanh thu hàng tháng theo dịch vụ, số lượng sinh viên theo khoa/lớp.
- Cảnh báo các hợp đồng sắp hết hạn và hóa đơn quá hạn.

---

## 5. Kế hoạch Lưu trữ Dữ liệu (Database)
- **Công nghệ**: Entity Framework Core 8 (Code-First với Migrations).
- **Môi trường phát triển**: SQLite (`dormitory.db`) độc lập, tự động seed dữ liệu mẫu khi khởi chạy lần đầu mà không cần cài đặt SQL Server nặng nề.
- **Môi trường triển khai**: Dễ dàng chuyển đổi sang Microsoft SQL Server hoặc PostgreSQL chỉ bằng việc thay đổi Connection String trong cấu hình.

