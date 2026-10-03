# 📖 Hướng Dẫn Sử Dụng: Hệ Thống Quản Lý Ký Túc Xá (Dormitory Manager)

Chào mừng bạn đến với tài liệu hướng dẫn sử dụng Hệ Thống Quản Lý Ký Túc Xá. Ứng dụng Desktop được phát triển trên nền tảng **.NET 8** và **Avalonia UI 11**, hỗ trợ giao diện Fluent hiện đại, đồng bộ dữ liệu thời gian thực và đầy đủ các thao tác Thêm / Sửa / Xóa (CRUD) với hộp thoại trực quan.

---

## 1. Khởi Động Ứng Dụng

1. Mở Terminal tại thư mục gốc `Dormitory-Manager`.
2. Chạy lệnh:
   ```bash
   dotnet run --project src/Dormitory.Desktop
   ```
3. Hệ thống sẽ khởi chạy và hiển thị cửa sổ làm việc chính với thanh điều hướng (Sidebar) bên trái và màn hình Tổng quan (Dashboard).

### Tài khoản quản trị mặc định:
- **Quản trị viên (Admin)**: `admin` / `Admin@123456`
- **Quản lý (Manager)**: `manager` / `Manager@123`

---

## 2. Thanh Điều Hướng (Navigation Bar)

Thanh menu bên trái giúp chuyển đổi linh hoạt giữa các phân hệ:
- 📊 **Bảng điều khiển**: Thống kê tổng quan số liệu KTX.
- 🏠 **Phòng ở**: Quản lý danh mục phòng, tình trạng và giá thuê.
- 🎓 **Sinh viên**: Hồ sơ thông tin cá nhân và lớp của sinh viên.
- 📝 **Hợp đồng**: Lập mới, gia hạn và thanh lý hợp đồng thuê.
- 💵 **Hóa đơn**: Lập phiếu thu điện, nước, dịch vụ và xác nhận nộp tiền.
- 👥 **Nhân viên**: Quản lý đội ngũ nhân sự vận hành KTX.

---

## 3. Chi Tiết Các Phân Hệ & Thao Tác CRUD

### 3.1. Bảng Điều Khiển (Dashboard)
- **Thẻ thống kê nhanh**:
  - *Tổng số phòng* & *Số phòng còn trống*.
  - *Số sinh viên đang ở* & *Số hợp đồng đang có hiệu lực*.
  - *Doanh thu tháng*: Tổng số tiền phòng và tiền dịch vụ đã thu trong tháng hiện tại.
  - *Hóa đơn chưa thu*: Cảnh báo các phòng chưa thanh toán để đôn đốc.
- **Tỷ lệ lấp đầy**: Thanh tiến trình thể hiện % công suất sử dụng phòng.
- **Nút "Làm mới"**: Cập nhật lại toàn bộ chỉ số từ cơ sở dữ liệu.

---

### 3.2. Quản Lý Phòng Ở (`Room`)
Giao diện hiển thị danh sách phòng theo bảng với các cột: Mã phòng, Tòa nhà, Tầng, Loại phòng, Giới tính quy định, Sức chứa / Đang ở, Đơn giá và Trạng thái.

#### Các chức năng chính:
1. **Thêm phòng mới (`+ Thêm phòng`)**:
   - Nhấn nút **"+ Thêm phòng"** trên thanh công cụ.
   - Hộp thoại `RoomDialogWindow` xuất hiện:
     - **Mã phòng**: Nhập mã duy nhất (ví dụ: `A101`, `B204`).
     - **Tòa nhà & Tầng**: Chọn tòa nhà (A, B, C...) và số tầng tương ứng.
     - **Loại phòng**: Tiêu chuẩn (Standard), Tiện nghi (Deluxe), VIP.
     - **Giới tính**: Nam, Nữ, hoặc Hỗn hợp/Không quy định.
     - **Sức chứa tối đa**: Số lượng sinh viên tối đa phòng có thể nhận (mặc định 4 hoặc 6).
     - **Đơn giá thuê (VND)**: Đơn giá phòng hàng tháng.
     - **Ghi chú**: Tình trạng tiện nghi (điều hòa, nóng lạnh, ban công,...).
   - Bấm **"Lưu thay đổi"** để tạo phòng.
2. **Chỉnh sửa thông tin phòng (`Sửa`)**:
   - Chọn một dòng phòng trên danh sách và bấm **"Sửa"**.
   - Hộp thoại mở ra chứa sẵn thông tin hiện tại của phòng. Tiến hành chỉnh sửa đơn giá, loại phòng hoặc ghi chú rồi bấm **"Lưu thay đổi"**.
3. **Xóa phòng (`Xóa`)**:
   - Chọn phòng cần xóa và bấm nút **"Xóa"**.
   - Hộp thoại xác nhận sẽ hỏi: *"Bạn có chắc chắn muốn xóa phòng này không?"*.
   - *Ràng buộc an toàn*: Hệ thống không cho phép xóa các phòng đang có sinh viên cư trú hoặc đang có hợp đồng hiệu lực gắn liền.

---

### 3.3. Quản Lý Hồ Sơ Sinh Viên (`Student`)
Quản lý chi tiết lý lịch sinh viên cư trú trong KTX.

#### Các chức năng chính:
1. **Tìm kiếm sinh viên**:
   - Nhập từ khóa vào ô tìm kiếm (họ tên, mã sinh viên, CCCD, lớp, quê quán) và bấm **"Tìm"**.
   - Bấm nút tìm kiếm lại với chuỗi trống để nạp lại danh sách đầy đủ.
2. **Thêm sinh viên mới (`+ Thêm sinh viên`)**:
   - Bấm **"+ Thêm sinh viên"** để mở hộp thoại `StudentDialogWindow`.
   - Nhập các trường thông tin:
     - **Mã sinh viên** (bắt buộc, không trùng lặp).
     - **Họ và tên** (bắt buộc).
     - **Số CCCD / CMND** (bắt buộc).
     - **Ngày sinh & Giới tính**.
     - **Lớp, Khoa, Trường**.
     - **Quê quán**.
     - **Số điện thoại sinh viên** & **SĐT phụ huynh khẩn cấp**.
     - **Email liên hệ**.
   - Bấm **"Lưu thay đổi"** để hoàn tất.
3. **Chỉnh sửa hồ sơ sinh viên (`Sửa`)**:
   - Chọn sinh viên trong bảng và bấm **"Sửa"**. Cập nhật thông tin liên hệ mới, lớp học hoặc địa chỉ rồi lưu lại.
4. **Xóa sinh viên (`Xóa`)**:
   - Chọn sinh viên và bấm **"Xóa"**. Xác nhận qua hộp thoại để xóa hồ sơ khỏi hệ thống.

---

### 3.4. Quản Lý Hợp Đồng Thuê (`Contract`)
Quản lý chu kỳ thuê phòng của sinh viên từ lúc vào ở đến khi chuyển đi.

#### Các chức năng chính:
1. **Lập hợp đồng mới (`+ Ký hợp đồng mới`)**:
   - Bấm **"+ Ký hợp đồng mới"** mở hộp thoại `ContractDialogWindow`.
   - **Chọn Sinh viên**: Danh sách chọn sinh viên khả dụng (chưa có hợp đồng đang hiệu lực).
   - **Chọn Phòng**: Danh sách các phòng còn chỗ trống và đúng giới tính quy định.
   - **Thời hạn thuê**: Ngày bắt đầu và ngày kết thúc hợp đồng (mặc định 6 tháng hoặc 1 năm).
   - **Tiền đặt cọc**: Số tiền cọc quy định theo loại phòng.
   - **Ghi chú**: Các điều khoản hoặc thỏa thuận đặc biệt.
   - Bấm **"Xác nhận ký HĐ"**: Hệ thống sẽ tạo hợp đồng mới, tự động tăng số lượng người ở hiện tại của phòng lên +1 và cập nhật trạng thái phòng sang `Occupied` nếu đã đủ người.
2. **Gia hạn hợp đồng (`Gia hạn HĐ`)**:
   - Chọn hợp đồng sắp hết hạn và bấm **"Gia hạn HĐ"**.
   - Hộp thoại yêu cầu chọn ngày kết thúc mới. Sau khi lưu, trạng thái hợp đồng được tiếp tục duy trì hiệu lực.
3. **Thanh lý / Chấm dứt hợp đồng (`Thanh lý HĐ`)**:
   - Khi sinh viên trả phòng hoặc tốt nghiệp: Chọn hợp đồng và bấm **"Thanh lý HĐ"**.
   - Xác nhận thanh lý: Hệ thống chuyển trạng thái hợp đồng sang `Terminated`, tự động giảm số người ở của phòng đi 1 và giải phóng sinh viên khỏi phòng.

---

### 3.5. Quản Lý Hóa Đơn & Điện Nước (`Bill`)
Hỗ trợ tính toán chi phí hàng tháng cho từng phòng một cách minh bạch, tự động và chính xác.

#### Công thức tính tự động trên hộp thoại:
- **Lượng điện tiêu thụ** = `Chỉ số điện mới` - `Chỉ số điện cũ` (kWh).
- **Tiền điện** = `Lượng điện tiêu thụ` × `Đơn giá điện` (mặc định 3.500 đ/kWh).
- **Lượng nước tiêu thụ** = `Chỉ số nước mới` - `Chỉ số nước cũ` (m³).
- **Tiền nước** = `Lượng nước tiêu thụ` × `Đơn giá nước` (mặc định 15.000 đ/m³).
- **Tổng tiền thanh toán** = `Tiền phòng` + `Tiền điện` + `Tiền nước` + `Internet` + `Vệ sinh`.

#### Các thao tác chính:
1. **Lập hóa đơn mới (`+ Lập hóa đơn`)**:
   - Bấm **"+ Lập hóa đơn"** mở hộp thoại `BillDialogWindow`.
   - **Chọn phòng**: Tiền phòng cơ bản sẽ tự động được điền theo đơn giá phòng đã chọn.
   - **Tháng & Năm thu**: Chọn kỳ thanh toán (ví dụ: Tháng 10 / 2026).
   - **Chỉ số điện**: Nhập chỉ số cũ và chỉ số mới. Tiền điện được tính tự động ngay khi gõ.
   - **Chỉ số nước**: Nhập chỉ số cũ và chỉ số mới. Tiền nước tự động hiển thị tức thì.
   - **Phụ phí dịch vụ**: Mặc định Internet (100.000 đ) và Dịch vụ vệ sinh chung (50.000 đ). Người dùng có thể điều chỉnh linh hoạt.
   - **Hạn nộp**: Ngày kết thúc hạn thanh toán.
   - Hộp thoại hiển thị thẻ nổi bật **"TỔNG CỘNG TIỀN PHẢI THU"** được cập nhật theo thời gian thực (real-time).
   - Bấm **"Lưu hóa đơn"** để phát hành.
2. **Thu tiền hóa đơn (`Thu tiền (Đã thanh toán)`)**:
   - Khi phòng hoàn thành nộp tiền: Chọn hóa đơn và bấm **"Thu tiền (Đã thanh toán)"**.
   - Trạng thái hóa đơn chuyển sang `Paid`, số tiền này được tự động cộng vào chỉ số **Doanh thu tháng** trên Dashboard.

---

### 3.6. Quản Lý Nhân Viên KTX (`Employee`)
Phân hệ chuyên trách quản lý thông tin đội ngũ nhân sự vận hành ký túc xá (quản lý, giám sát, bảo vệ, tạp vụ, kỹ thuật viên).

#### Các chức năng chính:
1. **Danh sách & Tìm kiếm nhân viên**:
   - Bảng thông tin hiển thị: Mã nhân viên, Họ tên, Chức vụ / Vị trí, Số điện thoại, Email, Số CCCD, Lương cơ bản, Ngày vào làm và Trạng thái làm việc.
   - Thanh tìm kiếm: Nhập từ khóa (tên nhân viên, mã NV, chức vụ, số điện thoại) và bấm **"Tìm"** để lọc nhanh.
2. **Thêm nhân viên mới (`+ Thêm nhân viên`)**:
   - Bấm **"+ Thêm nhân viên"** mở hộp thoại `EmployeeDialogWindow`.
   - Nhập thông tin:
     - **Mã nhân viên**: Mã định danh (ví dụ: `NV001`, `BV002`).
     - **Họ và tên**: Họ tên đầy đủ nhân viên.
     - **Chức vụ**: Chọn hoặc nhập vị trí công tác (Quản lý, Bảo vệ, Kỹ thuật, Tạp vụ,...).
     - **Số điện thoại & Email**: Kênh liên lạc chính.
     - **Số CCCD / CMND**: Thông tin căn cước công dân.
     - **Mức lương cơ bản (VND)**: Lương thỏa thuận hàng tháng.
     - **Ngày tuyển dụng / vào làm**.
     - **Trạng thái**: Tích chọn "Đang làm việc" hoặc hủy kích hoạt.
   - Bấm **"Lưu thay đổi"** để tạo hồ sơ nhân viên.
3. **Chỉnh sửa thông tin nhân viên (`Sửa`)**:
   - Chọn nhân viên từ bảng danh sách và bấm **"Sửa"**.
   - Cập nhật số điện thoại, mức lương, chức vụ hoặc trạng thái công tác rồi bấm **"Lưu thay đổi"**.
4. **Xóa nhân viên (`Xóa`)**:
   - Chọn nhân viên và bấm nút **"Xóa"**.
   - Hộp thoại xác nhận yêu cầu kiểm tra kỹ trước khi xóa khỏi danh bạ nhân sự.

---

## 4. Các Lưu Ý Về An Toàn Dữ Liệu & Ràng Buộc
- **Tính toàn vẹn khóa ngoại**: Không xóa phòng đang có sinh viên đang cư trú hoặc đang có hợp đồng chưa thanh lý.
- **Ràng buộc giới tính**: Khi xếp phòng qua hợp đồng, hệ thống ngăn chặn việc xếp sinh viên nam vào phòng quy định nữ và ngược lại.
- **Tính toán chỉ số**: Chỉ số mới điện/nước phải luôn lớn hơn hoặc bằng chỉ số cũ. Nếu nhập sai, hệ thống cảnh báo và giữ nguyên tính toán hợp lệ.
