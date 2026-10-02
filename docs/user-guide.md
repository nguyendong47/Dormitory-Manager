# Hướng Dẫn Sử Dụng: Hệ Thống Quản Lý Ký Túc Xá

## 1. Khởi Động Ứng Dụng
1. Mở Terminal tại thư mục `Dormitory-Manager`.
2. Gõ lệnh:
   ```bash
   dotnet run --project src/Dormitory.Desktop
   ```
3. Hệ thống sẽ mở cửa sổ ứng dụng Desktop với giao diện Fluent Dashboard.

---

## 2. Các Phân Hệ Chức Năng

### 2.1. Bảng điều khiển (Dashboard)
- Hiển thị tổng số phòng, phòng trống, sinh viên và doanh thu.
- Bấm nút **"Làm mới"** để cập nhật số liệu mới nhất từ cơ sở dữ liệu.

### 2.2. Quản lý phòng ở (Rooms)
- Xem danh sách toàn bộ phòng thuộc các tòa nhà.
- Theo dõi cột **"Sức chứa"** và **"Đang ở"** để biết phòng còn chỗ hay đã đầy.
- Đơn giá thuê được hiển thị định dạng VND rõ ràng.

### 2.3. Quản lý sinh viên (Students)
- Nhập từ khóa vào ô tìm kiếm (tìm theo tên, mã sinh viên, CCCD, lớp, quê quán) và bấm **"Tìm"**.
- Xem thông tin phòng hiện tại của sinh viên tại cột **"Phòng đang ở"**.

### 2.4. Quản lý hợp đồng (Contracts)
- Xem danh sách hợp đồng, thời hạn bắt đầu/kết thúc và số tiền đặt cọc.
- Để chấm dứt hoặc thanh lý hợp đồng: Chọn hợp đồng trên bảng và bấm **"Thanh lý HĐ"**. Hệ thống sẽ tự động cập nhật giảm sĩ số phòng và giải phóng sinh viên khỏi phòng.

### 2.5. Quản lý hóa đơn & thu tiền (Bills)
- Xem chi tiết chỉ số điện nước cũ/mới, số tiền điện, tiền nước và tổng số tiền phải nộp.
- Khi sinh viên nộp tiền: Chọn hóa đơn và bấm **"Thu tiền (Đã thanh toán)"**. Trạng thái hóa đơn sẽ chuyển sang `Paid` và được cộng vào doanh thu tháng trên Dashboard.
