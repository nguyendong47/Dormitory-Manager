# 🏢 Hệ Thống Quản Lý Ký Túc Xá (Dormitory Management System)

[![CI/CD Pipeline](https://github.com/nguyendong47/Dormitory-Manager/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/nguyendong47/Dormitory-Manager/actions/workflows/ci-cd.yml)
![Version](https://img.shields.io/badge/version-2.3.0-blue.svg)
![Tests](https://img.shields.io/badge/tests-177%2F177%20passed-success.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

> Ứng dụng Desktop hiện đại quản lý toàn diện Ký túc xá sinh viên, xây dựng trên nền tảng **.NET 8 LTS**, **Avalonia UI 11** và **Entity Framework Core 8** theo chuẩn kiến trúc **Clean Architecture** và mô hình **MVVM**.

---

## 📸 Giao Diện Người Dùng (Modern Desktop App)

![Giao diện Quản lý Ký túc xá](docs/images/dashboard-preview.jpg)

---

## 🌟 Tính Năng Nổi Bật

1. **📊 Bảng Điều Khiển Tổng Quan & Biểu Đồ Trực Quan (Dashboard & LiveCharts2)**:
   - Thống kê thời gian thực: Tổng số phòng, số phòng trống, số sinh viên đang cư trú, hợp đồng hiệu lực.
   - Theo dõi tỷ lệ lấp đầy KTX (Occupancy Rate) với thanh tiến trình trực quan.
   - Thống kê doanh thu tiền phòng và điện nước đã thu trong tháng hiện tại.
   - Thẻ cảnh báo thông minh: Tự động phát hiện và cảnh báo các hợp đồng sắp hết hạn (trong vòng 30 ngày) và hóa đơn chưa thanh toán; tự động ẩn khi không có cảnh báo.
   - **Biểu đồ tròn tỷ lệ lấp đầy theo tòa (`PieChart`)**: Phân bổ sinh viên và công suất sử dụng giường từng tòa nhà kèm chú giải và Tooltip tương tác.
   - **Biểu đồ cột xu hướng doanh thu 6 tháng (`CartesianChart`)**: So sánh doanh thu Tiền phòng vs Điện nước/Dịch vụ qua từng tháng, định dạng tiền tệ VNĐ.
   - **Xuất báo cáo quản trị tổng hợp Excel**: Xuất toàn bộ KPI, công suất tòa nhà và dòng tiền 6 tháng ra tệp `.xlsx` 3 sheet chuẩn mực với ClosedXML.

2. **🏠 Quản Lý Phòng Ở (`Room`)**:
   - Quản lý danh mục phòng theo tòa nhà, số tầng, loại phòng (Tiêu chuẩn, Premium, VIP).
   - Phân chia giới tính phòng (Phòng Nam / Phòng Nữ) và tự động cập nhật sức chứa (`Available`, `Occupied`, `Maintenance`).
   - Hộp thoại **Thêm mới**, **Chỉnh sửa** phòng ở và **Xóa an toàn** có xác nhận người dùng.
   - **Xuất Excel**: Xuất toàn bộ danh sách phòng sang định dạng `.xlsx` chuyên nghiệp với ClosedXML.

3. **🎓 Quản Lý Hồ Sơ Sinh Viên (`Student`)**:
   - Quản lý đầy đủ thông tin: Mã sinh viên, họ tên, CCCD/CMND, quê quán, lớp, khoa, số điện thoại và thông tin liên hệ phụ huynh khẩn cấp.
   - Tìm kiếm nhanh đa tiêu chí (tên, mã số, CCCD, lớp, quê quán).
   - Hộp thoại **Thêm mới**, **Sửa hồ sơ** và **Xóa sinh viên** đồng bộ thời gian thực.
   - **Xuất Excel**: Xuất danh bạ hồ sơ sinh viên sang tệp `.xlsx` hỗ trợ báo cáo nhà trường.

4. **📝 Quản Lý Hợp Đồng Thuê Phòng (`Contract`)**:
   - Lập hợp đồng mới với ràng buộc tự động: Kiểm tra phòng còn chỗ, kiểm tra giới tính sinh viên phù hợp, đảm bảo sinh viên chưa có hợp đồng hiệu lực trùng lặp.
   - Tự động tăng/giảm sĩ số phòng khi ký hoặc thanh lý hợp đồng.
   - Chức năng **Ký hợp đồng mới**, **Gia hạn hợp đồng** và **Thanh lý hợp đồng** giải phóng sinh viên.
   - **Bộ lọc nâng cao**: Lọc hợp đồng theo trạng thái (*Tất cả, Đang hiệu lực, Hết hạn, Đã chấm dứt*).

5. **💵 Lập Hóa Đơn & Thu Tiền Dịch Vụ (`Bill`)**:
   - Hộp thoại lập hóa đơn trực quan với công thức tính điện nước tức thì (real-time meter calculation).
   - Tự động tính tiền điện theo chỉ số cũ/mới và đơn giá chuẩn (3.500 đ/kWh).
   - Tự động tính tiền nước theo mét khối và đơn giá (15.000 đ/m³).
   - Cộng dồn tiền phòng và phụ phí vệ sinh, internet vào tổng tiền thanh toán.
   - Thao tác **Thu tiền (Đã thanh toán)** cập nhật trạng thái `Paid` và tăng doanh thu tháng.
   - **Bộ lọc trạng thái**: Lọc nhanh hóa đơn theo trạng thái (*Tất cả, Chưa thanh toán, Đã thanh toán*).
   - **Xuất Excel**: Xuất sổ chi tiết hóa đơn dịch vụ hàng tháng sang bảng tính `.xlsx`.

6. **👥 Quản Lý Đội Ngũ Nhân Viên KTX (`Employee`)**:
   - Quản lý hồ sơ nhân sự vận hành: Mã NV, họ tên, chức vụ (Quản lý, Bảo vệ, Tạp vụ, Kỹ thuật,...), số điện thoại, email, CCCD, mức lương cơ bản và ngày vào làm.
   - Tìm kiếm đa tiêu chí theo tên, mã nhân viên, chức danh hoặc số điện thoại.
   - Đầy đủ thao tác **Thêm nhân viên mới**, **Chỉnh sửa thông tin** và **Xóa nhân viên** với hộp thoại xác nhận.
   - **Bảo mật phân quyền**: Chỉ tài khoản Quản trị viên (`Admin`) mới có quyền truy cập phân hệ này.

7. **🔐 Xác Thực & Quản Lý Phiên Làm Việc (Authentication & Session)**:
   - Màn hình đăng nhập hiện đại với xác thực mật khẩu băm an toàn (BCrypt).
   - Quản lý phiên làm việc (`IUserSession`), hiển thị thông tin tài khoản và vai trò trên giao diện.
   - Chức năng **Đăng xuất an toàn** đưa người dùng trở về màn hình đăng nhập.
   - Phân quyền theo vai trò (RBAC): `Admin` có toàn quyền hệ thống; `Manager` quản lý vận hành thường nhật.

8. **⚙️ Cài Đặt Hệ Thống & Sao Lưu/Phục Hồi CSDL (System Settings & Backup)**:
   - Giám sát trạng thái file CSDL SQLite thời gian thực: đường dẫn, dung lượng đĩa, tổng số bản ghi từ toàn bộ các bảng trong hệ thống.
   - Sao lưu snapshot CSDL an toàn ra file `.bak` sử dụng SQLite Online Backup API không làm gián đoạn các giao dịch đọc/ghi.
   - Phục hồi CSDL an toàn với cơ chế kiểm tra tính toàn vẹn (`PRAGMA integrity_check`) và hộp thoại xác nhận.
   - Cấu hình chuỗi kết nối động và cấu hình máy chủ gửi thư SMTP qua giao diện và `appsettings.json`.
   - Phân quyền Quản trị viên (RBAC): Chỉ `Admin` mới có quyền truy cập và thao tác phục hồi dữ liệu.

9. **📦 Đóng Gói & Triển Khai Đa Nền Tảng (Cross-Platform Packaging)**:
   - Bộ kịch bản tự động đóng gói ứng dụng độc lập (Self-Contained Deployment), người dùng tải về chạy ngay (Zero Setup).
   - **macOS**: App Bundle chuẩn (`DormitoryManager.app`) & tệp ảnh đĩa `.dmg` (hỗ trợ cả Apple Silicon ARM64 và Intel x64).
   - **Windows**: Single-File Executable `.exe` đóng gói trong tệp `.zip` tiện dụng.
   - **Linux**: Gói nhị phân độc lập `.tar.gz` kèm native libraries tương thích các bản phân phối Ubuntu, Debian, Fedora.
   - Hướng dẫn triển khai và mẫu GitHub Actions CI/CD hoàn chỉnh tại [docs/packaging-and-deployment.md](docs/packaging-and-deployment.md).

10. **🪟 Hạ Tầng Dialog, Xuất Báo Cáo & Xác Nhận Chuẩn Mực**:
    - Dịch vụ hộp thoại phi tập trung (`IDialogService`) và chọn tệp lưu trữ (`IFileService`).
    - Cửa sổ xác nhận an toàn (`ConfirmDialogWindow`) và thông báo (`MessageDialogWindow`) ngăn chặn xóa nhầm dữ liệu.

11. **🛋️ Quản Lý Trang Thiết Bị & Tài Sản Phòng Ở (`Equipment`)**:
    - Quản lý danh mục tài sản theo phòng (giường tầng, bàn ghế, điều hòa, bình nóng lạnh, quạt điện).
    - 4 thẻ KPI thống kê hiện trạng thời gian thực: Tổng số lượng, Hoạt động tốt, Cần bảo trì, Hỏng hóc.
    - Bộ lọc đa năng theo phòng ở, theo trạng thái thiết bị và thanh tìm kiếm từ khóa tức thì.
    - Đầy đủ thao tác Thêm, Sửa, Xóa an toàn (phân quyền Admin) và nút thao tác nhanh **"⚠️ Báo sự cố"**.

12. **📄 Xuất Phiếu Thu Tiền Phòng & Dịch Vụ Ra PDF (QuestPDF)**:
    - Kết xuất phiếu thu tiền phòng, điện nước và dịch vụ hàng tháng ra file PDF chuẩn in ấn A4 chuyên nghiệp.
    - Tích hợp 3 điểm thao tác: Nút thanh công cụ, Menu ngữ cảnh (chuột phải) và Nút thao tác nhanh trên DataGrid.
    - Bố cục trang trọng: Thông tin Ban Quản lý, thông tin sinh viên & phòng ở, bảng chi tiết điện nước lũy tiến, phụ phí, tổng tiền in đậm định dạng VNĐ và hai khối chữ ký xác nhận.

13. **⚖️ Quản Lý Vi Phạm Nội Quy & Kỷ Luật Sinh Viên (`Violation`)**:
    - Lập biên bản vi phạm KTX phân loại 4 mức độ: Nhắc nhở, Khiển trách, Cảnh cáo, Buộc rời KTX.
    - Tự động trừ điểm rèn luyện và áp dụng mức tiền phạt vi phạm; quy trình xử lý biên bản linh hoạt.
    - 4 thẻ KPI theo dõi số lượng biên bản theo trạng thái và mức độ vi phạm.

14. **📧 Gửi Email Hóa Đơn Tự Động Kèm Tệp PDF Thu Tiền (`MailKit / SMTP`)**:
    - Tự động gửi thông báo tiền phòng & dịch vụ điện nước qua thư điện tử đến sinh viên.
    - Tự động đính kèm tệp PDF phiếu thu (`QuestPDF`) với mẫu email HTML trang trọng.
    - Cấu hình linh hoạt máy chủ gửi thư SMTP trong Cài đặt hệ thống.

15. **📊 Phân Hệ Báo Cáo & Phân Tích Tổng Hợp KTX (Reporting & Analytics)**:
    - Trung tâm tổng hợp 4 loại báo cáo chuyên sâu: **Báo cáo Vi phạm KTX**, **Báo cáo Tài chính & Thu phí**, **Báo cáo Tỷ lệ Lấp đầy & Sức chứa**, **Báo cáo Kiểm kê Tài sản & Trang thiết bị**.
    - Kết xuất đa định dạng: Bảng tính Excel đa sheet với ClosedXML (định dạng tiền tệ VNĐ, tỷ lệ %, auto-fit cột) và PDF hành chính chuẩn A4 với QuestPDF (font Unicode, thẻ KPI, bảng sọc, 3 khối chữ ký xác nhận pháp lý).
    - Quản lý lịch sử báo cáo toàn diện: Thẻ KPI thống kê, bộ lọc phân hệ/định dạng, mở tệp trực tiếp, tải về máy và xóa an toàn.

16. **🧪 Khung Kiểm Thử Tự Động Toàn Diện (Unit Tests & Avalonia Headless UI E2E)**:
    - 171 bài kiểm thử đơn vị & tích hợp kiểm soát chặt chẽ toàn bộ logic nghiệp vụ, tính toán tiền điện nước, trích xuất báo cáo và bảo mật.
    - 6 kịch bản kiểm thử giao diện tự động không cần màn hình (`Avalonia.Headless.XUnit`) chạy mượt mà trên môi trường CI/CD.

---

## 📋 Bảng Tổng Hợp Trạng Thái Chức Năng (Feature Matrix - Hoàn Thành 100% Cả 7 Giai Đoạn - v2.3.0)

| Phân hệ / Hạng mục | Xem Danh Sách | Thêm Mới (Create) | Chỉnh Sửa (Update) | Xóa / Hủy (Delete) | Tìm Kiếm / Lọc | Xuất Excel / PDF | Phân Quyền | Trạng Thái |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **Xác thực & Phiên (Auth)** | ✅ | — | — | — | — | — | Admin / Manager | **Hoàn thành (Phase 1)** |
| **Bảng Điều Khiển (Dashboard)** | ✅ | — | — | — | ✅ (Cảnh báo Live & Lọc) | ✅ (`.xlsx` Đa Sheet) | Tất cả | **Hoàn thành (Phase 2 - LiveCharts2 & Analytics)** |
| **Phòng Ở (Rooms)** | ✅ | ✅ (`RoomDialog`) | ✅ (`RoomDialog`) | ✅ (Xác nhận an toàn) | ✅ | ✅ (`.xlsx`) | Tất cả | **Hoàn thành (Phase 1)** |
| **Sinh Viên (Students)** | ✅ | ✅ (`StudentDialog`) | ✅ (`StudentDialog`) | ✅ (Xác nhận an toàn) | ✅ (Đa tiêu chí) | ✅ (`.xlsx`) | Tất cả | **Hoàn thành (Phase 1)** |
| **Hợp Đồng (Contracts)** | ✅ | ✅ (`ContractDialog`) | ✅ (Gia hạn HĐ) | ✅ (Thanh lý & Giải phóng) | ✅ (Lọc trạng thái) | — | Tất cả | **Hoàn thành (Phase 1)** |
| **Hóa Đơn & Điện Nước (Bills)** | ✅ | ✅ (`BillDialog` tự động) | — | — | ✅ (Lọc trạng thái) | ✅ (`.xlsx`) | Tất cả | **Hoàn thành (Phase 1)** |
| **Nhân Viên KTX (Employees)** | ✅ | ✅ (`EmployeeDialog`) | ✅ (`EmployeeDialog`) | ✅ (Xác nhận an toàn) | ✅ (Đa tiêu chí) | — | **Chỉ Admin** | **Hoàn thành (Phase 1)** |
| **Cài Đặt Hệ Thống & CSDL** | ✅ | ✅ (Sao lưu `.bak`) | ✅ (Phục hồi CSDL) | — | ✅ (Kiểm tra toàn vẹn) | — | **Chỉ Admin** | **Hoàn thành (Phase 3 - Infrastructure)** |
| **Đóng Gói Đa Nền Tảng (Scripts)** | ✅ | ✅ (macOS DMG) | ✅ (Win ZIP / Exe) | ✅ (Linux Tar.gz) | — | — | DevOps / Admin | **Hoàn thành (Phase 3 - Packaging)** |
| **Lưu Trữ Mã Kế Thừa (Legacy Archive)** | ✅ | — | — | — | — | — | Toàn bộ dự án | **Hoàn thành (Phase 4 - Code Reorganization)** |
| **Nhận Diện Thương Hiệu (Branding & Icons)** | ✅ | ✅ (`AppIcon.ico`) | ✅ (`AppIcon.png`) | — | — | — | macOS / Win / Linux | **Hoàn thành (Phase 4 - Polish & Branding)** |
| **Tự Động Hóa CI/CD & GitHub Releases** | ✅ | ✅ (CI Build & Test) | ✅ (CD Multi-OS Packaging) | — | — | — | GitHub Actions | **Hoàn thành (Phase 4 - CI/CD Pipeline)** |
| **Quản Lý Tài Sản Phòng (Equipments)** | ✅ | ✅ (`EquipmentDialog`) | ✅ (`EquipmentDialog`) | ✅ (Xác nhận an toàn) | ✅ (Lọc phòng & TT) | — | Admin / Manager | **Hoàn thành (Phase 5 - Equipment Module)** |
| **In Phiếu Thu PDF (QuestPDF)** | ✅ | — | — | — | — | ✅ (Xuất PDF in ấn) | Tất cả | **Hoàn thành (Phase 5 - PDF Receipts)** |
| **Kỷ Luật & Vi Phạm KTX (Violations)** | ✅ | ✅ (`ViolationDialog`) | ✅ (`ViolationDialog`) | ✅ (Xác nhận an toàn) | ✅ (Mức độ, TT, SV) | — | Admin / Manager | **Hoàn thành (Phase 6 - Violations)** |
| **Gửi Email Hóa Đơn Tự Động (MailKit)** | ✅ | ✅ (Gửi thư kèm PDF) | — | — | — | ✅ (Đính kèm PDF) | Admin / Manager | **Hoàn thành (Phase 6 - Email Invoicing)** |
| **Báo Cáo & Phân Tích Tổng Hợp** | ✅ | ✅ (Khởi tạo 4 loại BC) | — | ✅ (Xóa lịch sử an toàn) | ✅ (Phân hệ, Định dạng) | ✅ (Excel ClosedXML & PDF QuestPDF) | Tất cả | **Hoàn thành (Phase 7 - Reporting & Analytics)** |
| **Quản Lý Lịch Sử Báo Cáo** | ✅ | — | — | ✅ (Xóa tệp & bản ghi) | ✅ (Tìm kiếm, Lọc) | ✅ (Mở tệp & Tải về) | Tất cả | **Hoàn thành (Phase 7 - Report History)** |
| **Kiểm Thử E2E Headless (Avalonia)** | ✅ | ✅ (Auth & Nav E2E) | ✅ (Equipments E2E) | ✅ (Bill PDF E2E) | ✅ (Reports E2E) | — | Tự động hóa CI/CD | **Hoàn thành (Phase 7 - 6/6 Headless E2E)** |

---

## 🏗️ Kiến Trúc Hệ Thống (Clean Architecture + MVVM)

Hệ thống được thiết kế theo mô hình 4 tầng phân tách trách nhiệm chặt chẽ:

```mermaid
graph TD
    subgraph UI Layer
        V[Views: Dashboard, Rooms, Students, Contracts, Bills, Employees]
        Dialogs[Dialogs: Room, Student, Contract, Bill, Employee, Modals]
        VM[ViewModels: CommunityToolkit.Mvvm]
        DialogService[DialogService: IDialogService]
    end

    subgraph Application Layer
        AppService[Application Services: Room, Student, Contract, Bill, Employee, Auth, Dashboard]
        Contracts[Business Interfaces & Validators]
        DTOs[DTOs: RoomDto, StudentDto, ContractDto, BillDto, EmployeeDto]
    end

    subgraph Domain Layer
        Entities[Entities: Room, Student, Contract, Bill, Employee, User]
        Enums[Enums: RoomStatus, RoomType, BillStatus, ContractStatus, GenderRequirement]
    end

    subgraph Infrastructure Layer
        DbContext[DormitoryDbContext - EF Core 8]
        DB[(Database: SQLite / PostgreSQL / SQL Server)]
        Security[PasswordHasher - BCrypt]
        Seeder[DataSeeder - Tự động nạp mẫu]
    end

    V --> VM
    Dialogs --> VM
    VM --> DialogService
    VM --> AppService
    AppService --> Contracts
    AppService --> DTOs
    AppService --> Entities
    Contracts --> Entities
    DbContext --> Entities
    DbContext --> DB
    AppService --> DbContext
```

### Cấu Trúc Thư Mục

```
Dormitory-Manager/
├── .github/                          # Cấu hình GitHub Actions CI/CD Pipeline
│   └── workflows/
│       └── ci-cd.yml                 # Pipeline tự động Build, Test (177/177), Đóng gói & Phát hành Release
│
├── dist/                             # Thư mục chứa gói xuất bản thành phẩm (DMG, ZIP, TAR.GZ)
│
├── docs/                             # Tài liệu kỹ thuật, hướng dẫn sử dụng & ảnh chụp
│   ├── images/                       # Ảnh chụp giao diện dashboard
│   ├── architecture.md               # Tổng quan kiến trúc Clean Architecture & MVVM
│   ├── packaging-and-deployment.md   # Hướng dẫn đóng gói và triển khai đa nền tảng
│   ├── phase7/                       # Đặc tả yêu cầu báo cáo & phân tích tổng hợp KTX
│   ├── spec-modernization.md         # Đặc tả kiến trúc hiện đại hóa
│   └── user-guide.md                 # Hướng dẫn sử dụng chi tiết các phân hệ
│
├── legacy/                           # Mã nguồn dự án cũ WinForms 2021 lưu trữ (NET 4.7.2)
│   ├── Dormitory_Management_2021.sln # Solution WinForms cũ
│   ├── KTX2021/                      # Project WinForms cũ
│   └── packages/                     # Thư viện NuGet cũ
│
├── reports/                          # Thư mục lưu trữ cục bộ các tệp báo cáo Excel & PDF xuất bản
│
├── scripts/
│   └── package/                      # Bộ kịch bản tự động đóng gói đa nền tảng
│       ├── build-macos.sh            # Đóng gói macOS App Bundle & tệp DMG (ARM64 & x64)
│       ├── build-windows.sh          # Đóng gói Windows Single-File EXE & ZIP (win-x64)
│       └── build-linux.sh            # Đóng gói Linux Self-Contained Binary & Tar.gz
│
├── src/
│   ├── Dormitory.Core/               # Domain: Thực thể và Enums (Room, Equipment, Student, Contract, Bill, Employee, Violation, ReportHistory, User)
│   ├── Dormitory.Application/        # Application: DTOs, Services, Interfaces, Business Logic (IReportService, IViolationService, IEmailService,...)
│   ├── Dormitory.Infrastructure/     # Data Access: EF Core SQLite, QuestPDF, ClosedXML, MailKit, Migrations, BCrypt, DatabaseService, ReportService
│   └── Dormitory.Desktop/            # Presentation: Avalonia UI 11, FluentTheme, MVVM, Dialogs, appsettings.json
│       ├── Assets/                   # Nhận diện thương hiệu (AppIcon.ico, AppIcon.png) & Fonts
│       ├── Converters/               # Value Converters (ReportFormatConverter, ReportTypeConverter,...)
│       ├── ViewModels/               # ReportListViewModel, ReportGenerateDialogViewModel, ViolationListViewModel,...
│       └── Views/                    # ReportListView, ReportGenerateDialogWindow, ViolationListView,...
│
├── tests/
│   ├── Dormitory.UnitTests/          # Kiểm thử đơn vị & tích hợp xUnit & FluentAssertions (171/171 Passed - 100%)
│   └── Dormitory.E2ETests/           # Kiểm thử giao diện tự động Avalonia Headless UI (6/6 Journeys Passed - 100%)
│
├── Dormitory.sln                     # .NET 8 Solution
└── README.md
```

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Ứng Dụng

### 📥 Tải Về Bản Đóng Gói Sẵn (Pre-Packaged GitHub Releases)

Người dùng cuối và Quản trị viên KTX có thể tải ngay các bản cài đặt hoặc gói chạy độc lập (Self-Contained - không yêu cầu cài đặt trước .NET Runtime) tại trang [**GitHub Releases**](https://github.com/nguyendong47/Dormitory-Manager/releases):

- **macOS (Apple Silicon M1/M2/M3/M4)**: Tải tệp `DormitoryManager-v2.3.0-macOS-arm64.dmg` -> Nhấp đúp và kéo thả `DormitoryManager.app` vào thư mục `Applications`.
- **macOS (Intel x64)**: Tải tệp `DormitoryManager-v2.3.0-macOS-x64.dmg` -> Cài đặt tương tự như trên.
- **Windows (10/11 64-bit)**: Tải tệp `DormitoryManager-v2.3.0-Windows-x64.zip` -> Giải nén và nhấp đúp vào `Dormitory.Desktop.exe` để chạy ngay.
- **Linux (Ubuntu, Debian, Fedora x64)**: Tải tệp `DormitoryManager-v2.3.0-Linux-x64.tar.gz` -> Giải nén và thực thi `./Dormitory.Desktop`.

### Yêu Cầu Môi Trường (Dành Cho Lập Trình Viên)
- **.NET 8 SDK** (hoặc mới hơn) cài đặt trên máy phát triển.
- Hệ điều hành: **macOS** (Apple Silicon / Intel), **Windows 10/11**, hoặc **Linux**.

### Khởi Chạy Ứng Dụng Desktop (Môi Trường Phát Triển)

Chỉ cần mở Terminal tại thư mục gốc dự án và chạy:

```bash
# 1. Khôi phục packages và build toàn bộ Solution
dotnet build Dormitory.sln

# 2. Khởi chạy ứng dụng Desktop
dotnet run --project src/Dormitory.Desktop
```

> 💡 **Ghi chú**: Trong lần khởi chạy đầu tiên, hệ thống sẽ tự động tạo database SQLite cục bộ `dormitory.db` và nạp sẵn dữ liệu mẫu (danh sách phòng, sinh viên, hợp đồng, hóa đơn, nhân viên, vi phạm và tài khoản quản trị).

### Tài Khoản Đăng Nhập Mặc Định
- **Quản trị viên (Admin)**: `admin` / `Admin@123456`
- **Quản lý (Manager)**: `manager` / `Manager@123`

### 📦 Đóng Gói Ứng Dụng Đa Nền Tảng (Cross-Platform Packaging)

Hệ thống cung cấp sẵn các kịch bản đóng gói tự động thành các gói phần mềm độc lập (Self-Contained Deployment - SCD), không yêu cầu người dùng cuối cài đặt trước .NET Runtime:

```bash
# 1. Đóng gói cho macOS (tự nhận diện chip Apple Silicon hoặc Intel, tạo DormitoryManager.app & .dmg):
./scripts/package/build-macos.sh

# 2. Đóng gói cho Windows (tạo Single-File Dormitory.Desktop.exe & file nén .zip):
./scripts/package/build-windows.sh

# 3. Đóng gói cho Linux (tạo gói nhị phân độc lập DormitoryManager-...-Linux-x64.tar.gz):
./scripts/package/build-linux.sh
```

Thành phẩm sau khi đóng gói sẽ nằm tại thư mục `dist/`.
> 📖 Tham khảo hướng dẫn cài đặt và triển khai chi tiết cho từng hệ điều hành tại [**docs/packaging-and-deployment.md**](docs/packaging-and-deployment.md).

---

## 🧪 Chạy Kiểm Thử Tự Động (Unit Tests & Headless UI E2E)

Dự án bao gồm bộ kiểm thử tự động kiểm tra chặt chẽ toàn diện: logic tính toán hóa đơn, quy tắc ràng buộc phòng, quản lý hợp đồng, nghiệp vụ nhân viên, mã hóa mật khẩu, dịch vụ xuất báo cáo Excel ClosedXML, dịch vụ xuất phiếu thu PDF QuestPDF, quản lý tài sản phòng ở, quản lý kỷ luật vi phạm, gửi thư email SMTP hóa đơn, trung tâm báo cáo & phân tích tổng hợp, cấu hình động `appsettings.json`, dịch vụ sao lưu/phục hồi/kiểm tra toàn vẹn CSDL SQLite (`IDatabaseService`), và 6 hành trình người dùng E2E chạy hoàn toàn tự động trên nền tảng Avalonia Headless:

```bash
dotnet test Dormitory.sln
```

Kết quả: **177/177 Tests Passed** (100% Pass: 171 Unit Tests + 6 Avalonia Headless UI E2E Journeys).


