# 📖 Hướng Dẫn Sử Dụng: Hệ Thống Quản Lý Ký Túc Xá (Dormitory Manager)

Chào mừng bạn đến với tài liệu hướng dẫn sử dụng Hệ Thống Quản Lý Ký Túc Xá. Ứng dụng Desktop được phát triển trên nền tảng **.NET 8** và **Avalonia UI 11**, hỗ trợ giao diện Fluent hiện đại, đồng bộ dữ liệu thời gian thực, quản lý phiên và phân quyền người dùng, cảnh báo hạn thông minh cùng khả năng xuất báo cáo Excel chuyên nghiệp.

---

## 1. Khởi Động & Đăng Nhập Hệ Thống

### 1.1. Khởi động ứng dụng
Người dùng có thể khởi động ứng dụng theo hai cách:
1. **Sử dụng bản đóng gói sẵn (Khuyến nghị cho người dùng cuối)**:
   - Tải về gói phát hành tương ứng từ trang [GitHub Releases](https://github.com/nguyendong47/Dormitory-Manager/releases).
   - **macOS**: Mở tệp `.dmg` và kéo biểu tượng `DormitoryManager.app` vào thư mục `Applications`.
   - **Windows**: Giải nén tệp `.zip` và nhấp đúp vào `Dormitory.Desktop.exe`.
   - **Linux**: Giải nén tệp `.tar.gz` và chạy lệnh `./Dormitory.Desktop`.
2. **Khởi chạy từ mã nguồn (Dành cho lập trình viên)**:
   - Mở Terminal tại thư mục gốc `Dormitory-Manager`.
   - Chạy lệnh:
     ```bash
     dotnet run --project src/Dormitory.Desktop
     ```
3. Hệ thống sẽ mở màn hình **Đăng Nhập** (`LoginView`).

### 1.2. Màn hình Đăng nhập & Xác thực
- Giao diện đăng nhập cung cấp hai trường thông tin: **Tên đăng nhập** và **Mật khẩu**.
- Hỗ trợ phím tắt **Enter** để gửi yêu cầu đăng nhập nhanh chóng.
- Hệ thống bảo mật mật khẩu bằng thuật toán băm **BCrypt**, ngăn chặn nguy cơ lộ lọt thông tin.

### 1.3. Tài khoản đăng nhập mặc định
Hệ thống được khởi tạo sẵn hai tài khoản phục vụ vận hành:
- **Quản trị viên (Admin)**:
  - Tên đăng nhập: `admin`
  - Mật khẩu: `Admin@123456`
  - *Quyền hạn*: Toàn quyền trên mọi phân hệ, bao gồm quản lý danh mục nhân viên KTX.
- **Quản lý (Manager)**:
  - Tên đăng nhập: `manager`
  - Mật khẩu: `Manager@123`
  - *Quyền hạn*: Quản lý phòng ở, sinh viên, hợp đồng và hóa đơn dịch vụ hàng ngày.

### 1.4. Quản lý phiên làm việc & Đăng xuất
- Khi đăng nhập thành công, góc dưới cùng của thanh điều hướng bên trái sẽ hiển thị thông tin phiên:
  - **Họ và tên** cùng **Tên đăng nhập** của người dùng.
  - Huy hiệu phân quyền: `Quản trị viên` hoặc `Quản lý`.
- Nút **"Đăng xuất"**: Cho phép kết thúc phiên làm việc an toàn, xóa thông tin phiên trong bộ nhớ và quay trở lại màn hình Đăng nhập.

### 1.5. Nhận diện thương hiệu & Biểu tượng ứng dụng (Branding & App Icon)
- Phiên bản v2.0.0 chính thức tích hợp bộ nhận diện thương hiệu hoàn chỉnh:
  - Biểu tượng ứng dụng (`src/Dormitory.Desktop/Assets/AppIcon.ico` & `AppIcon.png`) thể hiện hình tượng tòa nhà ký túc xá hiện đại tông màu xanh thương hiệu.
  - Tích hợp chuẩn sắc nét trên **Thanh tiêu đề cửa sổ (Title Bar)**, **Thanh tác vụ Windows (Taskbar)**, **macOS Dock** và **Launcher Linux**.
  - Tệp cấu hình macOS `Info.plist` hiển thị đầy đủ tên ứng dụng `Dormitory Manager` và bundle identifier `vn.edu.dormitory.manager`.

---

## 2. Thanh Điều Hướng & Phân Quyền Vai Trò (RBAC)

Thanh menu bên trái giúp chuyển đổi linh hoạt giữa các phân hệ:
- 📊 **Bảng điều khiển**: Thống kê tổng quan số liệu KTX và cảnh báo công việc.
- 🏠 **Phòng ở**: Quản lý danh mục phòng, tình trạng, giá thuê và xuất Excel.
- 🎓 **Sinh viên**: Quản lý hồ sơ cá nhân sinh viên và xuất danh bạ Excel.
- 📝 **Hợp đồng**: Lập mới, gia hạn, thanh lý hợp đồng và bộ lọc trạng thái.
- 💵 **Hóa đơn**: Lập phiếu thu điện nước, xác nhận thu tiền, lọc trạng thái và xuất Excel.
- 👥 **Nhân viên**: Quản lý đội ngũ nhân sự vận hành KTX (**Chỉ dành cho Quản trị viên**).
- ⚙️ **Cài đặt**: Quản trị CSDL (sao lưu, phục hồi, kiểm tra toàn vẹn), cấu hình chuỗi kết nối và thông tin hệ thống (**Chỉ dành cho Quản trị viên**).

> 🔒 **Cơ chế phân quyền**: Với tài khoản vai trò `Manager`, hai phân hệ **Nhân viên** và **Cài đặt** sẽ tự động được ẩn hoàn toàn khỏi thanh điều hướng để đảm bảo tính an toàn hạ tầng và bảo mật thông tin nội bộ.

---

## 3. Chi Tiết Các Phân Hệ & Thao Tác Nghiệp Vụ

### 3.1. Bảng Điều Khiển (Dashboard) & Biểu Đồ Thống Kê Trực Quan (LiveCharts2 & ClosedXML)

Bảng điều khiển cung cấp cái nhìn toàn diện và trực quan theo thời gian thực về tình hình vận hành ký túc xá dành cho Ban Quản lý và Lãnh đạo:

1. **Thẻ Cảnh Báo Thông Minh (Smart Alert Card)**:
   - Nổi bật ở đầu trang với tông màu vàng cam cảnh báo khi hệ thống phát hiện có việc cần xử lý.
   - Nội dung cảnh báo tổng hợp: *"Có X hợp đồng sắp hết hạn (trong 30 ngày) và Y hóa đơn chưa thanh toán cần xử lý!"*.
   - **Bảng danh sách ngắn hợp đồng sắp hết hạn**: Hiển thị trực tiếp Mã HĐ, Họ tên sinh viên, Mã phòng và Ngày hết hạn giúp ban quản lý chủ động liên hệ gia hạn hoặc chuẩn bị bàn giao phòng.
   - **Cơ chế tự động ẩn (`HasAlert`)**: Khi không có hợp đồng nào sắp hết hạn trong 30 ngày tới và toàn bộ hóa đơn đã được thu tiền, thẻ cảnh báo sẽ tự động ẩn đi để giữ giao diện thoáng gọn.

2. **Thẻ Thống Kê Nhanh & Chỉ Số Vận Hành**:
   - *Tổng số phòng* & *Số phòng còn trống* (Sẵn sàng tiếp nhận sinh viên mới).
   - *Sinh viên đang ở* & *Hóa đơn chưa thu* (Cần đôn đốc đóng tiền).
   - *Tỷ lệ lấp đầy KTX*: Biểu đồ thanh tiến trình trực quan thể hiện công suất phòng toàn bộ hệ thống.
   - *Doanh thu tháng*: Tổng số tiền phòng và dịch vụ điện nước đã thu trong tháng hiện tại.
   - Nút **"🔄 Làm mới số liệu"**: Đồng bộ lại toàn bộ số liệu thời gian thực từ cơ sở dữ liệu.

3. **Biểu Đồ Tròn Tỷ Lệ Lấp Đầy Theo Tòa Nhà (`PieChart` - LiveCharts2)**:
   - **Mục đích**: Phân tích trực quan tỷ lệ phân bổ sinh viên và công suất sử dụng giường theo từng tòa nhà (ví dụ: Tòa A, Tòa B,...).
   - **Đặc điểm giao diện**:
     - Bảng màu hài hòa (SkiaSharp Fluent Palette: Xanh dương, Xanh lá, Cam đỏ, Tím, Vàng, Xanh cyan, Hồng cánh sen).
     - Bảng chú giải (Legend) chi tiết bên cạnh hiển thị: Tên tòa nhà, số chỗ đã ở, tổng số giường và tỷ lệ lấp đầy tính theo phần trăm (%).
   - **Tương tác trực quan**: Khi di chuột vào từng phần hình quạt, Tooltip thông minh sẽ xuất hiện cung cấp thông tin chi tiết: `[Tên tòa]: X/Y chỗ (Z.Z%)`.

4. **Biểu Đồ Cột Xu Hướng Doanh Thu 6 Tháng Gần Nhất (`CartesianChart` - LiveCharts2)**:
   - **Mục đích**: Theo dõi biến động tài chính của ký túc xá trong nửa năm qua, hỗ trợ dự báo và lập kế hoạch ngân sách.
   - **Cấu trúc biểu đồ cột nhóm (Clustered Column Chart)**:
     - Cột màu xanh dương: Doanh thu **Tiền phòng**.
     - Cột màu xanh lá cây: Doanh thu **Điện nước & Dịch vụ**.
   - **Hệ trục tọa độ chuẩn mực**:
     - Trục hoành (X-Axis): 6 mốc thời gian gần nhất tính đến tháng hiện tại (ví dụ: `T5/2026`, `T6/2026`, ..., `T10/2026`).
     - Trục tung (Y-Axis): Thước đo giá trị định dạng tiền tệ VNĐ có phân cách hàng nghìn (`1,000,000 đ`), bước nhảy tự động mượt mà.
   - **Tương tác Tooltip**: Di chuột vào từng cột hiển thị chính xác số tiền đã thu của khoản mục tương ứng.

5. **Xuất Báo Cáo Quản Trị Tổng Hợp Đa Bảng Tính Ra Excel (`ClosedXML`)**:
   - **Thao tác**: Nhấn nút **"📊 Xuất Báo Cáo Tổng Hợp"** trên thanh tiêu đề của Dashboard.
   - **Phản hồi thị giác**: Hệ thống tự động kích hoạt trạng thái tải dữ liệu (`IsLoading = true`) trong suốt quá trình xử lý, đảm bảo trải nghiệm mượt mà và chống nhấn đúp.
   - **Cấu trúc tệp Excel xuất ra (`BaoCao_TongQuan_KTX_...xlsx`)**:
     Tệp bảng tính được thiết kế theo chuẩn nhận diện thương hiệu chuyên nghiệp với 3 Worksheets riêng biệt:
     - **Sheet 1: Tổng Quan**: Báo cáo tổng kết điều hành (Thời điểm trích xuất, Tổng số phòng, Tỷ lệ lấp đầy toàn hệ thống, Doanh thu tháng hiện tại, Số hợp đồng sắp hết hạn và Hóa đơn chưa thanh toán).
     - **Sheet 2: Công Suất Tòa Nhà**: Bảng số liệu chi tiết theo từng tòa nhà (Mã & Tên tòa, Tổng số phòng, Tổng giường, Số chỗ đã ở, Chỗ trống, Tỷ lệ lấp đầy %), kèm dòng tổng cộng toàn bộ hệ thống.
     - **Sheet 3: Xu Hướng Doanh Thu**: Bảng tổng hợp dòng tiền 6 tháng gần nhất (Tháng, Doanh thu tiền phòng, Doanh thu điện nước/dịch vụ, Tổng doanh thu từng tháng).
   - **Chất lượng định dạng**: Header xanh đậm chữ trắng nổi bật, đường kẻ ô mảnh rõ ràng, định dạng số nguyên và tiền tệ VNĐ chuẩn xác, tự động căn chỉnh độ rộng cột tối ưu.

---

### 3.2. Quản Lý Phòng Ở (`Room`)

Giao diện hiển thị danh sách phòng theo bảng với các cột: Mã phòng, Tòa nhà, Tầng, Loại phòng, Giới tính quy định, Sức chứa / Đang ở, Đơn giá và Trạng thái.

#### Các chức năng chính:
1. **Thêm phòng mới (`+ Thêm phòng`)**:
   - Nhấn nút **"+ Thêm phòng"** trên thanh công cụ để mở hộp thoại `RoomDialogWindow`.
   - Nhập thông tin: Mã phòng (ví dụ `A101`), Tòa nhà, Tầng, Loại phòng (Standard, Deluxe, VIP), Giới tính quy định (Nam/Nữ), Sức chứa tối đa, Đơn giá thuê tháng và Ghi chú tiện nghi.
   - Bấm **"Lưu thay đổi"** để tạo mới.
2. **Chỉnh sửa thông tin phòng (`Sửa`)**:
   - Chọn một dòng phòng trên danh sách và bấm **"Sửa"**.
   - Cập nhật đơn giá, loại phòng hoặc ghi chú rồi bấm **"Lưu thay đổi"**.
3. **Xóa phòng an toàn (`Xóa`)**:
   - Chọn phòng cần xóa và bấm nút **"Xóa"**.
   - Hộp thoại xác nhận an toàn sẽ hiển thị.
   - *Ràng buộc an toàn*: Hệ thống chặn xóa đối với phòng đang có sinh viên cư trú hoặc đang có hợp đồng hiệu lực.
4. **Xuất danh sách phòng ra Excel (`📊 Xuất Excel`)**:
   - Bấm nút **"📊 Xuất Excel"** trên thanh công cụ.
   - Hộp thoại lưu tệp hệ thống hiển thị, gợi ý tên tệp dạng `DanhSachPhong_yyyyMMdd_HHmmss.xlsx`.
   - Tệp Excel xuất ra chuẩn ClosedXML với giao diện đẹp mắt: Header xanh đậm, viền bảng mảnh, căn chỉnh số liệu, định dạng tiền tệ VNĐ và tự động căn độ rộng cột.

---

### 3.3. Quản Lý Hồ Sơ Sinh Viên (`Student`)

Quản lý thông tin chi tiết của sinh viên lưu trú trong ký túc xá.

#### Các chức năng chính:
1. **Tìm kiếm đa tiêu chí**:
   - Nhập từ khóa vào ô tìm kiếm (họ tên, mã sinh viên, CCCD, lớp, quê quán) và bấm **"Tìm"**.
   - Nhập chuỗi trống hoặc bấm tìm kiếm lại để nạp danh sách đầy đủ.
2. **Thêm sinh viên mới (`+ Thêm sinh viên`)**:
   - Bấm **"+ Thêm sinh viên"** mở hộp thoại `StudentDialogWindow`.
   - Nhập đầy đủ: Mã sinh viên, Họ và tên, CCCD/CMND, Ngày sinh, Giới tính, Lớp, Khoa, Quê quán, Số điện thoại sinh viên, Số điện thoại phụ huynh và Email.
   - Bấm **"Lưu thay đổi"** để hoàn tất.
3. **Chỉnh sửa hồ sơ sinh viên (`Sửa`)**:
   - Chọn sinh viên cần sửa, bấm **"Sửa"**, cập nhật thông tin và lưu lại.
4. **Xóa sinh viên (`Xóa`)**:
   - Chọn sinh viên và bấm **"Xóa"** với hộp thoại xác nhận an toàn.
5. **Xuất danh bạ sinh viên ra Excel (`📊 Xuất Excel`)**:
   - Bấm nút **"📊 Xuất Excel"** để xuất toàn bộ danh sách hồ sơ sinh viên ra tệp `.xlsx`.
   - Phục vụ in ấn, báo cáo cho phòng công tác sinh viên hoặc gửi cơ quan công an địa phương.

---

### 3.4. Quản Lý Hợp Đồng Thuê (`Contract`)

Quản lý toàn bộ chu kỳ thuê phòng của sinh viên từ lúc bắt đầu nhận phòng đến khi thanh lý.

#### Các chức năng chính:
1. **Bộ lọc trạng thái nâng cao**:
   - Sử dụng hộp chọn **"Trạng thái:"** trên thanh công cụ để lọc danh sách:
     - **Tất cả**: Hiển thị toàn bộ hợp đồng trong hệ thống.
     - **Đang hiệu lực (Active)**: Chỉ hiển thị các hợp đồng đang thuê phòng.
     - **Hết hạn (Expired)**: Các hợp đồng đã qua ngày kết thúc nhưng chưa làm thủ tục thanh lý/gia hạn.
     - **Đã thanh lý (Terminated)**: Các hợp đồng đã hoàn tất thủ tục bàn giao và chấm dứt.
2. **Lập hợp đồng mới (`+ Ký hợp đồng mới`)**:
   - Bấm **"+ Ký hợp đồng mới"** mở hộp thoại `ContractDialogWindow`.
   - **Chọn Sinh viên**: Chỉ hiển thị những sinh viên chưa có hợp đồng hiệu lực.
   - **Chọn Phòng**: Tự động lọc các phòng còn chỗ trống và phù hợp giới tính của sinh viên.
   - Nhập thời hạn thuê, tiền đặt cọc và ghi chú.
   - Bấm **"Xác nhận ký HĐ"**: Hệ thống tạo hợp đồng, tự động tăng sĩ số phòng +1 và cập nhật trạng thái phòng sang `Occupied` nếu phòng đã đủ người.
3. **Gia hạn hợp đồng (`Gia hạn HĐ`)**:
   - Chọn hợp đồng sắp hết hạn và bấm **"Gia hạn HĐ"**.
   - Nhập ngày kết thúc mới và lưu lại để duy trì hiệu lực hợp đồng.
4. **Thanh lý hợp đồng (`Thanh lý HĐ`)**:
   - Khi sinh viên chuyển đi hoặc tốt nghiệp: Chọn hợp đồng và bấm **"Thanh lý HĐ"**.
   - Xác nhận thanh lý: Hệ thống chuyển trạng thái hợp đồng sang `Terminated`, tự động giảm số người trong phòng đi 1 và giải phóng sinh viên khỏi phòng.

---

### 3.5. Quản Lý Hóa Đơn & Điện Nước (`Bill`)

Tính toán chi phí dịch vụ hàng tháng một cách minh bạch, tự động và chính xác.

#### Công thức tính tiền tự động:
- **Lượng điện tiêu thụ** = `Chỉ số điện mới` - `Chỉ số điện cũ` (kWh).
- **Tiền điện** = `Lượng điện tiêu thụ` × `Đơn giá điện` (3.500 đ/kWh).
- **Lượng nước tiêu thụ** = `Chỉ số nước mới` - `Chỉ số nước cũ` (m³).
- **Tiền nước** = `Lượng nước tiêu thụ` × `Đơn giá nước` (15.000 đ/m³).
- **Tổng tiền thanh toán** = `Tiền phòng` + `Tiền điện` + `Tiền nước` + `Internet` + `Vệ sinh`.

#### Các thao tác chính:
1. **Bộ lọc trạng thái hóa đơn**:
   - Sử dụng hộp chọn **"Trạng thái:"** trên thanh công cụ để lọc:
     - **Tất cả**: Toàn bộ hóa đơn các tháng.
     - **Chưa thanh toán (Unpaid)**: Các hóa đơn cần thu tiền.
     - **Đã thanh toán (Paid)**: Các hóa đơn đã hoàn tất nộp tiền.
2. **Lập hóa đơn mới (`+ Lập hóa đơn`)**:
   - Bấm **"+ Lập hóa đơn"** mở hộp thoại `BillDialogWindow`.
   - Chọn phòng: Tiền phòng cơ bản tự động nạp theo đơn giá phòng.
   - Nhập chỉ số điện cũ/mới và chỉ số nước cũ/mới: Tiền điện nước và **Tổng tiền phải thu** được tính toán và hiển thị ngay lập tức (real-time).
   - Tùy chỉnh phụ phí Internet, Vệ sinh và Ngày hạn nộp.
   - Bấm **"Lưu hóa đơn"** để phát hành.
3. **Thu tiền hóa đơn (`Thu tiền (Đã thanh toán)`)**:
   - Khi phòng nộp tiền: Chọn hóa đơn và bấm **"Thu tiền (Đã thanh toán)"**.
   - Hóa đơn chuyển trạng thái sang `Paid`, số tiền thu được tự động hạch toán vào Doanh thu tháng trên Dashboard.
4. **Xuất sổ hóa đơn ra Excel (`📊 Xuất Excel`)**:
   - Bấm nút **"📊 Xuất Excel"** trên thanh công cụ.
   - Lưu tệp `DanhSachHoaDon_yyyyMMdd_HHmmss.xlsx` với đầy đủ chi tiết: Mã HĐ, Phòng, Tháng/Năm, Tiền phòng, Chỉ số & Tiền điện, Chỉ số & Tiền nước, Phụ phí và Tổng tiền.

---

### 3.6. Quản Lý Nhân Viên KTX (`Employee`)

Phân hệ dành riêng cho Quản trị viên (`Admin`) quản lý đội ngũ nhân sự vận hành ký túc xá (quản lý, giám sát, bảo vệ, tạp vụ, kỹ thuật viên).

#### Các chức năng chính:
1. **Danh sách & Tìm kiếm nhân viên**:
   - Hiển thị đầy đủ: Mã NV, Họ tên, Chức vụ, SĐT, Email, CCCD, Lương cơ bản, Ngày vào làm và Trạng thái làm việc.
   - Thanh tìm kiếm: Lọc nhanh theo tên, mã NV, chức vụ hoặc số điện thoại.
2. **Thêm nhân viên mới (`+ Thêm nhân viên`)**:
   - Mở hộp thoại `EmployeeDialogWindow`, nhập thông tin nhân sự và bấm **"Lưu thay đổi"**.
3. **Chỉnh sửa thông tin nhân viên (`Sửa`)**:
   - Chọn nhân viên, bấm **"Sửa"**, cập nhật mức lương, chức vụ hoặc thông tin liên hệ và lưu lại.
4. **Xóa nhân viên (`Xóa`)**:
   - Xóa hồ sơ nhân sự với hộp thoại xác nhận an toàn.

---

### 3.7. Cài Đặt Hệ Thống & Quản Trị Cơ Sở Dữ Liệu (`SystemSettings`)

Phân hệ dành riêng cho **Quản trị viên (Admin)** để theo dõi tình trạng tệp cơ sở dữ liệu SQLite, thực hiện sao lưu dự phòng định kỳ, phục hồi dữ liệu khi có sự cố và quản trị cấu hình hệ thống.

#### 1. Xem thông số & trạng thái Cơ sở dữ liệu:
- **Đường dẫn tệp CSDL**: Hiển thị đường dẫn tệp `dormitory.db` mà ứng dụng đang kết nối.
- **Dung lượng tệp (Size)**: Kích thước vật lý thực tế của file cơ sở dữ liệu trên ổ đĩa (định dạng `KB` hoặc `MB`).
- **Tổng số bản ghi (Total Records)**: Tổng số lượng bản ghi thực tế được tổng hợp theo thời gian thực từ toàn bộ các bảng trong hệ thống: Phòng ở (`Rooms`), Sinh viên (`Students`), Hợp đồng (`Contracts`), Hóa đơn (`Bills`), Nhân viên (`Employees`) và Người dùng (`Users`).
- **Cập nhật lần cuối**: Thời điểm tệp CSDL được ghi đĩa lần gần nhất.
- Nút **"🔄 Làm mới thông tin"**: Truy vấn lại thông số CSDL từ ổ đĩa và cập nhật ngay lên giao diện.

#### 2. Sao lưu Cơ sở dữ liệu định kỳ (Database Backup):
- **Mục đích**: Bảo vệ an toàn dữ liệu quản lý ký túc xá, phòng ngừa rủi ro hỏng ổ cứng, sự cố hệ điều hành hoặc mất mát dữ liệu ngoài ý muốn.
- **Công nghệ an toàn**: Sử dụng cơ chế SQLite Online Backup API (`VACUUM INTO` / snapshot an toàn) cho phép trích xuất bản sao lưu nguyên vẹn mà **không làm gián đoạn** hay khóa giao dịch đọc/ghi của các phiên làm việc khác.
- **Các bước thực hiện**:
  1. Nhấn nút **"💾 Tạo bản sao lưu (.bak)"** trên thẻ *Sao Lưu Dữ Liệu*.
  2. Hộp thoại lưu tệp hệ thống xuất hiện, tự động gợi ý tên tệp theo thời gian thực: `dormitory_backup_yyyyMMdd_HHmmss.bak`.
  3. Chọn thư mục lưu trữ an toàn (ví dụ: ổ cứng ngoài, USB sao lưu hoặc thư mục đồng bộ đám mây như OneDrive/Google Drive).
  4. Nhấn **Save**: Hệ thống thực hiện snapshot dữ liệu và thông báo *"Sao lưu cơ sở dữ liệu thành công"*.

#### 3. Quy trình phục hồi Cơ sở dữ liệu an toàn (Database Restore):
- **Mục đích**: Khôi phục toàn bộ dữ liệu KTX từ một bản sao lưu `.bak` hoặc `.db` đã lưu trữ trước đó.
- **Cơ chế bảo vệ đa lớp**:
  - **Phân quyền Quản trị viên (RBAC)**: Chỉ tài khoản có vai trò `Admin` mới được phép kích hoạt tính năng này.
  - **Hộp thoại cảnh báo nguy hiểm (`ConfirmDialogWindow`)**: Nhắc nhở rõ ràng rằng toàn bộ dữ liệu hiện tại trong hệ thống sẽ bị thay thế hoàn toàn bởi bản sao lưu.
  - **Kiểm tra tính toàn vẹn dữ liệu (Integrity Check)**: Trước khi tiến hành ghi đè CSDL, hệ thống tự động kiểm tra định dạng SQLite hợp lệ và chạy lệnh `PRAGMA integrity_check` trên tệp sao lưu. Nếu tệp sao lưu bị lỗi, biến dạng hoặc hỏng cấu trúc, hệ thống sẽ **hủy bỏ thao tác ngay lập tức** và giữ nguyên vẹn dữ liệu hiện tại.
- **Các bước thực hiện**:
  1. Nhấn nút **"🔄 Khôi phục CSDL từ file..."** trên thẻ *Phục Hồi Dữ Liệu*.
  2. Chọn tệp sao lưu hợp lệ (`.bak` hoặc `.db`).
  3. Đọc kỹ nội dung cảnh báo xác nhận trong hộp thoại: *"CẢNH BÁO: Toàn bộ dữ liệu hiện tại trong hệ thống sẽ được thay thế bằng dữ liệu từ tệp sao lưu này. Bạn có chắc chắn muốn tiếp tục?"*.
  4. Bấm **"Đồng ý"** để tiến hành phục hồi.
  5. Sau khi nhận thông báo thành công, chuyển đổi giữa các tab danh mục hoặc khởi động lại ứng dụng để toàn bộ giao diện nạp lại dữ liệu mới nhất.

#### 4. Cấu hình chuỗi kết nối động qua `appsettings.json`:
- Ứng dụng hỗ trợ cấu hình động thông qua tệp `appsettings.json` đặt cùng thư mục với tệp thực thi:
  ```json
  {
    "ConnectionStrings": {
      "DormitoryDb": "Data Source=dormitory.db"
    },
    "DatabaseProvider": "Sqlite",
    "AppSettings": {
      "AppName": "Dormitory Manager",
      "Version": "2.0.0",
      "AutoBackupOnExit": false
    }
  }
  ```
- **Lợi ích**: Quản trị viên có thể thay đổi vị trí lưu trữ file CSDL (ví dụ: chuyển sang phân vùng ổ đĩa chuyên dụng hoặc thư mục mạng nội bộ) mà không cần phải biên dịch lại mã nguồn chương trình.
- **Cơ chế dự phòng an toàn (Safe Fallback)**: Nếu tệp `appsettings.json` bị xóa nhầm hoặc chuỗi kết nối để trống, hệ thống sẽ tự động chuyển về chuỗi kết nối mặc định `Data Source=dormitory.db` nhằm đảm bảo ứng dụng luôn khởi chạy ổn định.

#### 5. Hướng dẫn Đóng gói và Cài đặt Đa nền tảng:
- Để biết chi tiết các bước cài đặt và phân phối ứng dụng cho người dùng cuối trên các hệ điều hành khác nhau, vui lòng tham khảo tài liệu:
  👉 [**Hướng Dẫn Đóng Gói Và Triển Khai Đa Nền Tảng (Cross-Platform Packaging & Deployment)**](packaging-and-deployment.md)
  - **macOS**: Cài đặt dạng App Bundle và tệp ảnh đĩa `.dmg` (hỗ trợ cả Apple Silicon ARM64 và Intel x64).
  - **Windows**: Chạy tệp đơn độc lập `.exe` (Single-File Executable) trong gói nén `.zip` (không yêu cầu cài trước .NET Runtime).
  - **Linux**: Giải nén và thực thi gói self-contained `.tar.gz` tương thích các bản phân phối Ubuntu, Debian, Fedora, Arch.

---

## 4. Các Lưu Ý Về An Toàn Dữ Liệu & Ràng Buộc Hệ Thống

1. **Bảo mật và phân quyền**:
   - Luôn đăng xuất khỏi hệ thống khi rời khỏi máy làm việc để bảo vệ dữ liệu nội trú và tài chính.
   - Tài khoản vai trò `Manager` không thể xem hoặc chỉnh sửa danh sách nhân viên KTX cũng như không thể truy cập phân hệ Cài đặt hệ thống.
2. **Tính toàn vẹn dữ liệu**:
   - Không cho phép xóa các phòng đang có sinh viên cư trú hoặc đang có hợp đồng chưa thanh lý.
   - Ràng buộc giới tính tự động ngăn chặn việc xếp nhầm sinh viên nam vào phòng quy định nữ và ngược lại.
3. **Sao lưu dữ liệu định kỳ**:
   - Quản trị viên nên tạo bản sao lưu dữ liệu `.bak` ít nhất một lần mỗi tuần hoặc trước các kỳ quyết toán tài chính, bàn giao phòng đầu/cuối năm học.
   - Lưu trữ các tệp sao lưu tại các thiết bị lưu trữ ngoài hoặc dịch vụ lưu trữ đám mây có bảo mật.
4. **Xuất file Excel an toàn**:
   - Hệ thống sử dụng bộ chọn tệp native của hệ điều hành (`IFileService` kết hợp Avalonia `StorageProvider`), đảm bảo tính tương thích cao và không xảy ra xung đột quyền ghi đĩa.
   - Khi xuất file, có thể mở trực tiếp bằng Microsoft Excel, Google Sheets hoặc LibreOffice mà không bị lỗi font hay định dạng.

