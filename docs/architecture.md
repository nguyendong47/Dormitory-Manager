# Tài Liệu Kiến Trúc Phần Mềm: Dormitory Management

## 1. Nguyên Tắc Thiết Kế
Hệ thống được thiết kế tuân thủ nguyên tắc **Clean Architecture** của Robert C. Martin và mô hình **Model-View-ViewModel (MVVM)** cho tầng Presentation:
- **Tính độc lập với Framework**: Tầng Domain (`Dormitory.Core`) không phụ thuộc vào bất kỳ framework ORM hoặc UI nào.
- **Tính độc lập với Giao diện (UI)**: UI có thể thay thế (Avalonia UI, WPF, hoặc Web) mà không ảnh hưởng tới Business Logic.
- **Tính độc lập với Cơ sở dữ liệu**: EF Core trừu tượng hóa việc truy vấn, dễ dàng chuyển đổi giữa SQLite, PostgreSQL và SQL Server.
- **Kiểm thử dễ dàng (Testability)**: Các quy tắc nghiệp vụ trong `Dormitory.Application` được kiểm thử độc lập mà không cần khởi động UI hay kết nối Database thật (thông qua Unit Tests).

---

## 2. Trách Nhiệm Của Từng Tầng

| Tầng (Project) | Trách nhiệm chính | Thư viện sử dụng |
| :--- | :--- | :--- |
| **`Dormitory.Core`** | Chứa Entities, Enums, Value Objects và các quy tắc Domain cốt lõi. | Không có phụ thuộc bên ngoài |
| **`Dormitory.Application`** | Chứa DTOs, UseCases/Services, Interfaces, kiểm tra tính hợp lệ dữ liệu. | `Microsoft.EntityFrameworkCore` (Abstractions) |
| **`Dormitory.Infrastructure`** | Quản trị DbContext, Fluent API, Migrations SQLite, mã hóa mật khẩu, Data Seeder. | `Microsoft.EntityFrameworkCore.Sqlite`, `BCrypt.Net-Next` |
| **`Dormitory.Desktop`** | Giao diện đồ họa người dùng, điều phối ViewModel, Data Binding, Navigation. | `Avalonia 11`, `Avalonia.Themes.Fluent`, `CommunityToolkit.Mvvm` |
| **`Dormitory.UnitTests`** | Kiểm thử tự động logic nghiệp vụ (tính toán tiền, sức chứa phòng, mã hóa). | `xUnit`, `FluentAssertions`, `Moq` |
