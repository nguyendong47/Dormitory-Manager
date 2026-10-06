# Đặc Tả Kỹ Thuật & Yêu Cầu Nghiệp Vụ Phân Hệ Báo Cáo & Phân Tích (Reporting & Analytics)
## Dự Án: Hệ Thống Quản Lý Ký Túc Xá (Dormitory-Manager) - Phiên Bản v2.3.0 (Phase 7)

---

## 1. Giới Thiệu & Mục Tiêu Tổng Quan

### 1.1. Bối cảnh dự án
Hệ thống **Dormitory-Manager** đã hoàn thiện các phân hệ cốt lõi từ Phase 1 đến Phase 6: Quản lý phòng ở, Quản lý sinh viên, Hợp đồng thuê phòng, Hóa đơn điện nước & dịch vụ, Phân quyền người dùng, Quản lý nhân viên, Cơ sở dữ liệu sao lưu tự động, Kiểm kê trang thiết bị phòng ở, Quản lý vi phạm nội quy kỷ luật và Gửi email hóa đơn tự động kèm PDF.

Để nâng cao năng lực quản trị dữ liệu, cung cấp góc nhìn tổng thể cho Ban Giám hiệu nhà trường, Ban Quản lý Ký túc xá (BQL KTX) và Tổ Kế toán - Tài vụ, **Phase 7 (v2.3.0)** tập trung phát triển phân hệ **Báo Cáo & Phân Tích Chuyên Sâu (Reporting & Analytics)**.

### 1.2. Mục tiêu kỹ thuật & nghiệp vụ
1. **Chuẩn hóa 4 phân hệ báo cáo quản trị trọng điểm**:
   - Báo cáo Vi phạm Nội quy & Kỷ luật (Violations Report).
   - Báo cáo Tài chính & Thu phí KTX (Financial & Revenue Report).
   - Báo cáo Tỷ lệ Lấp đầy & Tình trạng Phòng (Occupancy & Room Utilization Report).
   - Báo cáo Kiểm kê Tài sản & Trang thiết bị (Assets & Equipment Inventory Report).
2. **Hỗ trợ đa định dạng xuất bản đạt chuẩn doanh nghiệp**:
   - **Microsoft Excel (.xlsx)** thông qua thư viện `ClosedXML`: Phục vụ lưu trữ bảng kê, đối soát chi tiết, tính toán công thức và tái sử dụng dữ liệu.
   - **Adobe PDF (.pdf)** thông qua thư viện `QuestPDF`: Bản in chuẩn mẫu hành chính, trình bày trang trọng, có phân trang, bảng biểu và chữ ký xác nhận của các bên liên quan.
3. **Lưu vết và quản lý lịch sử xuất báo cáo (Report Generation History)**:
   - Ghi nhận mọi lượt xuất báo cáo vào cơ sở dữ liệu (`ReportHistory`), cho phép xem lại, tải lại tệp và tra cứu lịch sử mà không cần sinh lại từ đầu.
4. **Trải nghiệm người dùng đồng nhất trên Avalonia UI (MVVM Pattern)**:
   - Màn hình trực quan với các thẻ hành động nhanh (Quick Action Cards), hộp thoại tùy biến tham số lọc đa năng (`ReportGenerateDialogWindow`), và bộ quản lý lịch sử báo cáo.

---

## 2. Đặc Tả Chi Tiết 4 Loại Báo Cáo Cốt Lõi

```
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                        PHÂN HỆ BÁO CÁO & PHÂN TÍCH KÝ TÚC XÁ                          │
├────────────────────┬────────────────────┬────────────────────┬─────────────────────────┤
│ 1. Vi Phạm Kỷ Luật │ 2. Tài Chính & Thu │ 3. Tỷ Lệ Lấp Đầy & │ 4. Kiểm Kê Tài Sản &    │
│    (Violations)    │    (Financial)     │   Phòng (Occupancy)│    Thiết Bị (Equipment) │
├────────────────────┼────────────────────┼────────────────────┼─────────────────────────┤
│ • Số lượng vụ việc │ • Doanh thu phòng  │ • Tổng giường/phòng│ • Số lượng tài sản      │
│ • Mức độ vi phạm   │ • Tiền điện, nước  │ • Giường đang ở    │ • Tình trạng sử dụng    │
│ • Điểm trừ & Phạt  │ • Công nợ tồn đọng │ • Chỗ trống khả dụng│• Giá trị tài sản       │
│ • Top SV vi phạm   │ • Tỷ lệ hoàn thành │ • Tỷ lệ lấp đầy (%)│ • Chi phí sửa chữa      │
└────────────────────┴────────────────────┴────────────────────┴─────────────────────────┘
```

---

### 2.1. Báo Cáo 1: Báo Cáo Vi Phạm Nội Quy & Kỷ Luật (Violations Report)

#### 2.1.1. Mục đích nghiệp vụ
Cung cấp số liệu chính xác, minh bạch cho BQL KTX và Phòng Công tác Sinh viên (CTSV) về tình hình an ninh, trật tự và ý thức kỷ luật của sinh viên nội trú trong từng giai đoạn (tuần cao điểm, tháng, học kỳ, năm học). Hỗ trợ hội đồng khen thưởng - kỷ luật xem xét hạ điểm rèn luyện, ra quyết định cảnh cáo hoặc chấm dứt hợp đồng lưu trú đối với sinh viên vi phạm nhiều lần.

#### 2.1.2. Các chỉ số KPI & Công thức tính toán
| Chỉ số (KPI) | Tên kỹ thuật | Mô tả & Công thức tính toán |
|---|---|---|
| **Tổng số vụ vi phạm** | `TotalViolations` | $Total = \sum \text{Violations}$ thỏa mãn điều kiện lọc thời gian/phạm vi. |
| **Số vụ theo mức độ** | `MinorCount`, `ModerateCount`, `SevereCount`, `CriticalCount` | Đếm theo `Severity`: <br>• Nhắc nhở (`Minor`): $Count(Severity == Minor)$ <br>• Khiển trách (`Moderate`): $Count(Severity == Moderate)$ <br>• Cảnh cáo (`Severe`): $Count(Severity == Severe)$ <br>• Buộc rời KTX (`Critical`): $Count(Severity == Critical)$ |
| **Tỷ lệ vi phạm nghiêm trọng** | `CriticalSeverityRate` | $Rate = \frac{SevereCount + CriticalCount}{TotalViolations} \times 100\%$ |
| **Số vụ theo trạng thái** | `PendingCount`, `ResolvedCount`, `DismissedCount` | Đếm theo `Status`: <br>• Chờ xử lý (`Pending`) <br>• Đã xử lý / Khắc phục (`Resolved`) <br>• Miễn trừ / Bãi bỏ (`Dismissed`) |
| **Tỷ lệ giải quyết dứt điểm** | `ResolutionRate` | $ResolutionRate = \frac{ResolvedCount + DismissedCount}{TotalViolations} \times 100\%$ |
| **Tổng điểm rèn luyện bị trừ** | `TotalDemeritPoints` | $TotalPoints = \sum DemeritPoints$ của các vi phạm trong kỳ. |
| **Tổng tiền xử phạt** | `TotalFineAmount` | $TotalFines = \sum FineAmount$ (VNĐ) phát sinh trong kỳ. |
| **Top Sinh viên vi phạm** | `TopViolatingStudents` | Danh sách 5 hoặc 10 sinh viên có tổng số lần vi phạm nhiều nhất hoặc tổng điểm trừ cao nhất. |
| **Top Phòng vi phạm nhiều nhất** | `TopViolatingRooms` | Danh sách các phòng có số biên bản vi phạm phát sinh nhiều nhất (điểm nóng an ninh). |

#### 2.1.3. Nguồn dữ liệu & Tham số lọc (Filters)
- **Thực thể liên quan**: `Violation`, `Student`, `Room`, `User`.
- **Tham số lọc đầu vào**:
  - `FromDate` (DateTime?): Ngày bắt đầu vi phạm (mặc định: ngày đầu tháng hiện tại).
  - `ToDate` (DateTime?): Ngày kết thúc vi phạm (mặc định: ngày hiện tại).
  - `Building` (string?): Lọc theo tòa nhà (Tòa A, Tòa B, Tòa C, Tất cả).
  - `RoomId` (int?): Lọc theo một phòng cụ thể.
  - `Severity` (ViolationSeverity?): Mức độ (Minor, Moderate, Severe, Critical, Tất cả).
  - `Status` (ViolationStatus?): Trạng thái (Pending, Resolved, Dismissed, Tất cả).
  - `StudentSearch` (string?): Tìm theo Mã sinh viên hoặc Tên sinh viên.

#### 2.1.4. Cấu trúc xuất bản Excel (ClosedXML)
- **Workbook Name**: `BaoCao_ViPham_KTX_[Timestamp].xlsx`
- **Sheet 1: Tổng Hợp & Thống Kê (`TongQuan`)**:
  - Tiêu đề: "BÁO CÁO TỔNG HỢP TÌNH HÌNH VI PHẠM NỘI QUY KÝ TÚC XÁ"
  - Thẻ thông tin thời gian lọc, người xuất báo cáo.
  - Khối KPI: Tổng số vi phạm, Đã giải quyết, Đang xử lý, Tổng điểm trừ, Tổng tiền phạt.
  - Bảng thống kê phân loại theo mức độ vi phạm (Mức độ, Số lượng, Tỷ lệ %, Tổng tiền phạt).
  - Bảng xếp hạng Top 5 sinh viên có nhiều vi phạm nhất.
  - Bảng xếp hạng Top 5 phòng xảy ra nhiều vi phạm nhất.
- **Sheet 2: Bảng Kê Chi Tiết (`ChiTietViPham`)**:
  - Các cột: `STT`, `Mã Biên Bản`, `Thời Gian`, `Mã SV`, `Họ Và Tên`, `Phòng`, `Tòa Nhà`, `Hành Vi Vi Phạm`, `Mức Độ`, `Điểm Trừ`, `Tiền Phạt (VNĐ)`, `Trạng Thái`, `Người Lập`, `Biện Pháp Xử Lý / Ghi Chú`.
  - Định dạng: Hàng tiêu đề nền xanh `#0078D4`, chữ trắng, căn giữa; Cột tiền tệ format `#,##0 đ`; Cột ngày tháng format `dd/MM/yyyy HH:mm`.

#### 2.1.5. Cấu trúc xuất bản PDF (QuestPDF)
- **Khổ giấy**: A4 Portrait (Đứng), lề chuẩn 36pt (0.5 inch).
- **Header**:
  - Bên trái: TÊN CƠ QUAN CHỦ QUẢN - TRƯỜNG ĐẠI HỌC / BAN QUẢN LÝ KÝ TÚC XÁ.
  - Bên phải: CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM (Độc lập - Tự do - Hạnh phúc).
- **Tiêu đề chính**: "BÁO CÁO TÌNH HÌNH VI PHẠM NỘI QUY & KỶ LUẬT"
- **Thời gian áp dụng**: "Từ ngày dd/MM/yyyy đến ngày dd/MM/yyyy"
- **Nội dung chính**:
  1. *Số liệu tổng hợp*: Bảng 4 thẻ số (Tổng số vụ, Đã xử lý, Tổng điểm rèn luyện bị trừ, Tổng tiền phạt).
  2. *Cơ cấu vi phạm theo mức độ*: Bảng tóm tắt số lượng và tỷ lệ % từng mức độ vi phạm.
  3. *Danh sách các trường hợp nghiêm trọng (Severe & Critical)*: Bảng liệt kê chi tiết sinh viên, phòng, hành vi và mức kỷ luật đề xuất.
  4. *Danh sách các biên bản vi phạm*: Bảng rút gọn danh sách các biên bản trong kỳ.
- **Footer**:
  - Đánh số trang tự động: "Trang X / Y".
  - Chữ ký xác nhận: Người Lập Báo Cáo (Ký, họ tên) và Trưởng Ban Quản Lý Ký Túc Xá (Ký, đóng dấu).

---

### 2.2. Báo Cáo 2: Báo Cáo Tài Chính & Thu Phí KTX (Financial & Revenue Report)

#### 2.2.1. Mục đích nghiệp vụ
Phục vụ công tác quản lý tài chính, đối soát thu nợ giữa Kế toán KTX và Nhà trường. Báo cáo phản ánh chi tiết nguồn thu từ tiền thuê phòng nội trú, tiền điện, tiền nước tiêu thụ thực tế và các phụ phí dịch vụ phát sinh hàng tháng. Xác định chính xác tỷ lệ thu hồi công nợ, phát hiện danh sách phòng nợ đọng kéo dài để áp dụng chế tài thu phí hoặc ngừng cung cấp dịch vụ.

#### 2.2.2. Các chỉ số KPI & Công thức tính toán
| Chỉ số (KPI) | Tên kỹ thuật | Mô tả & Công thức tính toán |
|---|---|---|
| **Tổng doanh thu dự kiến (Phải thu)** | `TotalExpectedRevenue` | $TotalExpected = \sum TotalAmount$ của toàn bộ hóa đơn trong kỳ báo cáo. |
| **Tổng doanh thu thực thu** | `TotalCollectedRevenue` | $TotalCollected = \sum TotalAmount$ của các hóa đơn có `Status == BillStatus.Paid`. |
| **Doanh thu tiền phòng thực thu** | `RoomFeeCollected` | $RoomFee = \sum RoomFee$ của các hóa đơn đã thanh toán. |
| **Doanh thu tiền điện thực thu** | `ElectricFeeCollected` | $ElectricFee = \sum ElectricFee$ của các hóa đơn đã thanh toán. |
| **Doanh thu tiền nước thực thu** | `WaterFeeCollected` | $WaterFee = \sum WaterFee$ của các hóa đơn đã thanh toán. |
| **Doanh thu dịch vụ khác thực thu** | `OtherFeeCollected` | $OtherFee = \sum OtherServiceFee$ của các hóa đơn đã thanh toán. |
| **Tổng công nợ tồn đọng** | `TotalOutstandingDebt` | $Debt = TotalExpectedRevenue - TotalCollectedRevenue = \sum TotalAmount$ với hóa đơn `Unpaid` hoặc `Overdue`. |
| **Công nợ trong hạn** | `PendingDebt` | $PendingDebt = \sum TotalAmount$ với `Status == Unpaid` và `DueDate >= Now`. |
| **Công nợ quá hạn** | `OverdueDebt` | $OverdueDebt = \sum TotalAmount$ với `Status == Overdue` hoặc (`Status == Unpaid` và `DueDate < Now`). |
| **Tỷ lệ thu hồi công nợ** | `CollectionRate` | $CollectionRate = \frac{TotalCollectedRevenue}{TotalExpectedRevenue} \times 100\%$ |
| **Tỷ lệ hóa đơn hoàn thành** | `PaidBillRate` | $PaidBillRate = \frac{Count(Paid)}{TotalBills} \times 100\%$ |
| **Tổng điện năng tiêu thụ** | `TotalElectricUsageKwh` | $TotalElectric = \sum ElectricUsage$ (kWh) của toàn bộ phòng trong kỳ. |
| **Tổng khối lượng nước tiêu thụ** | `TotalWaterUsageM3` | $TotalWater = \sum WaterUsage$ ($m^3$) của toàn bộ phòng trong kỳ. |

#### 2.2.3. Nguồn dữ liệu & Tham số lọc (Filters)
- **Thực thể liên quan**: `Bill`, `Room`, `Contract`, `Student`.
- **Tham số lọc đầu vào**:
  - `Month` (int?): Tháng phát sinh hóa đơn (1 - 12).
  - `Year` (int?): Năm phát sinh hóa đơn (Ví dụ: 2026).
  - `FromDate` (DateTime?): Ngày tạo hóa đơn bắt đầu (nếu lọc theo khoảng ngày).
  - `ToDate` (DateTime?): Ngày tạo hóa đơn kết thúc.
  - `Building` (string?): Lọc theo tòa nhà (Tòa A, Tòa B, Tòa C, Tất cả).
  - `RoomId` (int?): Lọc theo phòng cụ thể.
  - `BillStatus` (BillStatus?): Trạng thái hóa đơn (Paid, Unpaid, Overdue, Tất cả).
  - `OnlyOverdue` (bool): Chỉ lọc danh sách hóa đơn đang bị quá hạn nợ.

#### 2.2.4. Cấu trúc xuất bản Excel (ClosedXML)
- **Workbook Name**: `BaoCao_TaiChinh_ThuPhi_[ThangNam]_[Timestamp].xlsx`
- **Sheet 1: Tổng Hợp Tài Chính (`TongHopThuChi`)**:
  - Tiêu đề: "BÁO CÁO DOANH THU & TÌNH HÌNH THU TIỀN PHÒNG, ĐIỆN NƯỚC KÝ TÚC XÁ"
  - Thẻ tóm tắt tổng thể: Tổng phải thu, Thực thu, Công nợ tồn, Tỷ lệ thu hồi (%).
  - Bảng cơ cấu nguồn thu:
    - Tiền phòng: Số tiền, Tỷ lệ % trên tổng doanh thu.
    - Tiền điện sinh hoạt: Sản lượng (kWh), Thành tiền, Tỷ lệ %.
    - Tiền nước sinh hoạt: Sản lượng ($m^3$), Thành tiền, Tỷ lệ %.
    - Dịch vụ khác (Internet, vệ sinh): Thành tiền, Tỷ lệ %.
  - Bảng tổng hợp theo từng Tòa nhà: Tòa nhà, Số hóa đơn, Doanh thu dự kiến, Thực thu, Công nợ, Tỷ lệ hoàn thành.
- **Sheet 2: Bảng Kê Hóa Đơn Chi Tiết (`BangKeHoaDon`)**:
  - Các cột: `STT`, `Mã Hóa Đơn`, `Phòng`, `Tòa Nhà`, `Tháng/Năm`, `Tiền Phòng (VNĐ)`, `Điện Cũ`, `Điện Mới`, `Sản Lượng Điện (kWh)`, `Tiền Điện (VNĐ)`, `Nước Cũ`, `Nước Mới`, `Sản Lượng Nước (m3)`, `Tiền Nước (VNĐ)`, `Phụ Phí Khác (VNĐ)`, `Tổng Cộng (VNĐ)`, `Hạn Thanh Toán`, `Ngày Đóng Tiền`, `Trạng Thái`.
- **Sheet 3: Danh Sách Nợ Đọng Quá Hạn (`CongNoQuaHan`)**:
  - Danh sách các phòng nợ phí quá hạn kèm số ngày trễ hạn, tổng tiền nợ và danh sách sinh viên hiện đang ở trong phòng để liên hệ đôn đốc.

#### 2.2.5. Cấu trúc xuất bản PDF (QuestPDF)
- **Khổ giấy**: A4 Landscape (Ngang) hoặc Portrait (Đứng) tối ưu bảng số liệu.
- **Header**: Quốc hiệu, Đơn vị quản lý, Ngày lập báo cáo.
- **Tiêu đề**: "BÁO CÁO TỔNG HỢP THU CHI & CÔNG NỢ KÝ TÚC XÁ"
- **Nội dung chính**:
  1. *Khối chỉ số KPI tài chính*: Thẻ hiển thị lớn: Tổng Doanh Thu Dự Kiến | Thực Thu | Công Nợ Còn Lại | Tỷ Lệ Thu Hồi.
  2. *Biểu đồ/Bảng cơ cấu doanh thu theo thành phần*: Tiền phòng vs Tiền điện vs Tiền nước vs Phụ phí.
  3. *Bảng tổng hợp theo từng tòa nhà*: Thống kê doanh số và tỷ lệ thu theo Tòa A, Tòa B, Tòa C.
  4. *Bảng các khoản nợ quá hạn cần xử lý*: Danh sách phòng có số nợ lớn và quá hạn lâu nhất.
- **Footer**:
  - Ghi chú: "Số liệu được trích xuất tự động từ hệ thống quản lý KTX".
  - Chữ ký 3 bên: Người Lập Báo Cáo - Kế Toán Trưởng - Trưởng Ban Quản Lý KTX.

---

### 2.3. Báo Cáo 3: Báo Cáo Tỷ Lệ Lấp Đầy & Tình Trạng Phòng KTX (Occupancy & Room Utilization Report)

#### 2.3.1. Mục đích nghiệp vụ
Cung cấp bức tranh toàn cảnh về sức chứa, hiện trạng phân bổ sinh viên và năng lực tiếp nhận của ký túc xá. Báo cáo này đặc biệt quan trọng vào đầu mỗi học kỳ mới để Hội đồng tuyển sinh KTX điều phối phân bổ phòng ở cho tân sinh viên, sinh viên ưu tiên chính sách, đồng thời theo dõi số lượng phòng cần sửa chữa bảo dưỡng, đảm bảo tối ưu hóa công suất khai thác cơ sở vật chất.

#### 2.3.2. Các chỉ số KPI & Công thức tính toán
| Chỉ số (KPI) | Tên kỹ thuật | Mô tả & Công thức tính toán |
|---|---|---|
| **Tổng số phòng** | `TotalRooms` | $TotalRooms = \sum Rooms$ được quản lý trong hệ thống. |
| **Tổng công suất giường** | `TotalBedsCapacity` | $TotalBeds = \sum Capacity$ của toàn bộ các phòng. |
| **Số giường đang ở (Thực tế)** | `OccupiedBeds` | $OccupiedBeds = \sum CurrentOccupancy$ của toàn bộ các phòng. |
| **Số giường còn trống khả dụng** | `AvailableBeds` | $AvailableBeds = TotalBedsCapacity - OccupiedBeds$. |
| **Tỷ lệ lấp đầy toàn KTX** | `OccupancyRate` | $OccupancyRate = \frac{OccupiedBeds}{TotalBedsCapacity} \times 100\%$ |
| **Phân loại phòng theo trạng thái** | `OccupiedRoomsCount`, `AvailableRoomsCount`, `MaintenanceRoomsCount` | • Phòng đã đầy: $Count(Status == Occupied \lor CurrentOccupancy == Capacity)$ <br>• Phòng còn chỗ: $Count(Status == Available \land CurrentOccupancy < Capacity)$ <br>• Phòng bảo trì: $Count(Status == Maintenance)$ |
| **Số phòng trống hoàn toàn** | `EmptyRoomsCount` | $EmptyRooms = Count(CurrentOccupancy == 0 \land Status == Available)$ |
| **Năng lực tiếp nhận theo giới tính** | `MaleCapacity`, `MaleOccupied`, `FemaleCapacity`, `FemaleOccupied` | Sức chứa, số chỗ đang ở và số chỗ còn trống tách biệt theo Phòng Nam (`Gender.Male`) và Phòng Nữ (`Gender.Female`). |
| **Tỷ lệ phòng sẵn sàng đón tiếp** | `VacantRoomRate` | $Rate = \frac{AvailableRoomsCount}{TotalRooms} \times 100\%$ |

#### 2.3.3. Nguồn dữ liệu & Tham số lọc (Filters)
- **Thực thể liên quan**: `Room`, `Contract`, `Student`.
- **Tham số lọc đầu vào**:
  - `Building` (string?): Lọc theo tòa nhà (Tòa A, Tòa B, Tòa C, Tất cả).
  - `Floor` (int?): Lọc theo tầng (Tầng 1, Tầng 2, ...).
  - `RoomType` (RoomType?): Lọc theo loại phòng (Standard, Premium, VIP).
  - `AllowedGender` (Gender?): Lọc theo giới tính (Nam, Nữ).
  - `RoomStatus` (RoomStatus?): Trạng thái phòng (Available, Occupied, Maintenance).
  - `OnlyVacant` (bool): Chỉ lọc các phòng còn chỗ trống để bố trí sinh viên.

#### 2.3.4. Cấu trúc xuất bản Excel (ClosedXML)
- **Workbook Name**: `BaoCao_LapDay_TinhTrangPhong_[Timestamp].xlsx`
- **Sheet 1: Tổng Hợp Lấp Đầy (`TongHopLapDay`)**:
  - Tiêu đề: "BÁO CÁO TỶ LỆ LẤP ĐẦY & CÔNG SUẤT PHÒNG Ở KÝ TÚC XÁ"
  - Thẻ KPI: Tổng số phòng, Tổng số giường, Đang lưu trú, Chỗ trống khả dụng, Tỷ lệ lấp đầy (%).
  - Bảng thống kê theo Tòa nhà: Tên tòa, Số phòng, Sức chứa giường, Giường đang ở, Giường còn trống, Phòng bảo trì, Tỷ lệ lấp đầy (%).
  - Bảng phân bổ theo Giới tính & Loại phòng.
- **Sheet 2: Danh Sách Chi Tiết Từng Phòng (`ChiTietPhong`)**:
  - Các cột: `STT`, `Số Phòng`, `Tòa Nhà`, `Tầng`, `Loại Phòng`, `Đối Tượng (Nam/Nữ)`, `Sức Chứa (Giường)`, `Hiện Tại Đang Ở`, `Còn Trống`, `Đơn Giá Thuê (VNĐ)`, `Trạng Thái Phòng`, `Tỷ Lệ Lấp Đầy (%)`, `Ghi Chú Cơ Sở Vật Chất`.
- **Sheet 3: Danh Sách Phòng Còn Chỗ Khả Dụng (`PhongTrongDieuPhoi`)**:
  - Danh sách các phòng có `HasVacancy == true` sắp xếp theo tòa và số chỗ trống giảm dần, phục vụ xếp phòng tân sinh viên.

#### 2.3.5. Cấu trúc xuất bản PDF (QuestPDF)
- **Khổ giấy**: A4 Portrait.
- **Header & Tiêu đề**: "BÁO CÁO CÔNG SUẤT & TỶ LỆ LẤP ĐẦY PHÒNG Ở KÝ TÚC XÁ"
- **Nội dung chính**:
  1. *Khối KPI năng lực lưu trú*: Tổng số phòng, Tổng giường, Đang ở, Còn trống, Tỷ lệ lấp đầy (in đậm, kích thước lớn).
  2. *Bảng phân tích tỷ lệ lấp đầy theo từng tòa nhà*: So sánh tỷ lệ sử dụng giường giữa các tòa nhà.
  3. *Bảng hiện trạng theo phân loại phòng*: Chi tiết phòng tiêu chuẩn, phòng dịch vụ cao cấp, phòng nam, phòng nữ.
  4. *Danh sách các phòng đang bảo trì / hư hỏng*: Báo cáo các phòng không thể khai thác kèm lý do.
- **Footer**:
  - Thông tin xuất báo cáo, phân trang, chữ ký Cán bộ quản lý phòng ở và Lãnh đạo BQL KTX.

---

### 2.4. Báo Cáo 4: Báo Cáo Kiểm Kê Tài Sản & Trang Thiết Bị (Assets & Equipment Inventory Report)

#### 2.4.1. Mục đích nghiệp vụ
Kiểm soát toàn bộ tài sản, máy móc và tiện nghi sinh hoạt được trang cấp tại các phòng ở trong ký túc xá (điều hòa, bình nóng lạnh, quạt trần, giường tầng, bàn ghế học tập, thiết bị vệ sinh). Đánh giá tỷ lệ thiết bị hoạt động tốt, phát hiện thiết bị hư hỏng cần bảo trì hoặc thay mới, ước tính ngân sách chi phí sửa chữa để đề xuất Ban Giám hiệu phê duyệt kinh phí trước khi bước vào năm học mới.

#### 2.4.2. Các chỉ số KPI & Công thức tính toán
| Chỉ số (KPI) | Tên kỹ thuật | Mô tả & Công thức tính toán |
|---|---|---|
| **Tổng số lượng thiết bị** | `TotalEquipmentCount` | $Total = \sum Quantity$ của các trang thiết bị theo bộ lọc. |
| **Tổng giá trị tài sản** | `TotalAssetValue` | $TotalValue = \sum (Quantity \times Price)$ (VNĐ). |
| **Số lượng thiết bị Hoạt động tốt** | `GoodConditionCount` | $Good = \sum Quantity$ với `Status == EquipmentStatus.Good`. |
| **Số lượng thiết bị Cần sửa chữa** | `NeedsRepairCount` | $NeedsRepair = \sum Quantity$ với `Status == EquipmentStatus.NeedsRepair`. |
| **Số lượng thiết bị Hỏng hóc** | `BrokenCount` | $Broken = \sum Quantity$ với `Status == EquipmentStatus.Broken`. |
| **Tỷ lệ thiết bị khả dụng (Sức khỏe)** | `EquipmentHealthRate` | $HealthRate = \frac{GoodConditionCount}{TotalEquipmentCount} \times 100\%$ |
| **Tỷ lệ thiết bị hư hỏng/lỗi** | `FaultyRate` | $FaultyRate = \frac{NeedsRepairCount + BrokenCount}{TotalEquipmentCount} \times 100\%$ |
| **Ước tính giá trị tài sản bị hư hại** | `DamagedAssetValue` | $DamagedValue = \sum_{Broken} (Quantity \times Price)$. |
| **Ước tính kinh phí sửa chữa/bảo dưỡng** | `EstimatedRepairCost` | Ước tính chi phí khắc phục các thiết bị `NeedsRepair` và `Broken`. |
| **Thiết bị quá hạn kiểm tra bảo trì** | `OverdueMaintenanceCount` | Số thiết bị có `LastMaintainedAt` cách hiện tại > 180 ngày hoặc chưa từng bảo trì. |

#### 2.4.3. Nguồn dữ liệu & Tham số lọc (Filters)
- **Thực thể liên quan**: `Equipment`, `Room`.
- **Tham số lọc đầu vào**:
  - `Building` (string?): Lọc theo tòa nhà (Tòa A, Tòa B, Tòa C, Tất cả).
  - `RoomId` (int?): Lọc theo phòng cụ thể.
  - `EquipmentStatus` (EquipmentStatus?): Trạng thái (Good, NeedsRepair, Broken, Tất cả).
  - `SearchKeyword` (string?): Từ khóa tìm tên thiết bị (ví dụ: "điều hòa", "bình nóng lạnh") hoặc mã thiết bị.
  - `OnlyFaulty` (bool): Chỉ lọc các thiết bị cần sửa chữa hoặc bị hỏng hóc.

#### 2.4.4. Cấu trúc xuất bản Excel (ClosedXML)
- **Workbook Name**: `BaoCao_KiemKe_TaiSanThietBi_[Timestamp].xlsx`
- **Sheet 1: Tổng Hợp Tài Sản (`TongHopKiemKe`)**:
  - Tiêu đề: "BÁO CÁO TỔNG HỢP KIỂM KÊ TÀI SẢN & TRANG THIẾT BỊ PHÒNG Ở"
  - Thẻ KPI: Tổng số thiết bị, Tổng giá trị tài sản (VNĐ), Số hoạt động tốt, Cần sửa chữa, Hỏng hóc, Tỷ lệ khả dụng (%).
  - Bảng thống kê theo Tòa nhà: Tên tòa, Tổng số lượng, Trạng thái Tốt, Cần sửa, Hỏng, Giá trị tài sản.
  - Bảng thống kê theo Loại danh mục thiết bị: Điện máy (Điều hòa, Nóng lạnh), Nội thất (Giường, Tủ, Bàn), Khác.
- **Sheet 2: Danh Mục Kiểm Kê Chi Tiết (`ChiTietThietBi`)**:
  - Các cột: `STT`, `Mã Tài Sản`, `Tên Trang Thiết Bị`, `Phòng Ở`, `Tòa Nhà`, `Số Lượng`, `Đơn Giá (VNĐ)`, `Thành Tiền (VNĐ)`, `Hiện Trạng`, `Ngày Trang Bị`, `Lần Bảo Trì Cuối`, `Ghi Chú Kỹ Thuật / Hiện Trạng`.
- **Sheet 3: Danh Sách Thiết Bị Cần Sửa Chữa & Đề Xuất (`DeXuatSuaChua`)**:
  - Bảng lọc riêng các thiết bị `NeedsRepair` và `Broken` kèm phòng ở, hiện trạng lỗi, ước tính chi phí sửa chữa để gửi Phòng Quản trị Cơ sở vật chất.

#### 2.4.5. Cấu trúc xuất bản PDF (QuestPDF)
- **Khổ giấy**: A4 Portrait.
- **Header & Tiêu đề**: "BIÊN BẢN KIỂM KÊ & ĐÁNH GIÁ HIỆN TRẠNG TRANG THIẾT BỊ KÝ TÚC XÁ"
- **Nội dung chính**:
  1. *Khối chỉ số kiểm kê*: Tổng số lượng thiết bị, Tổng giá trị, Tỷ lệ thiết bị hoạt động tốt.
  2. *Bảng phân bố tình trạng kỹ thuật*: Tốt, Cần sửa chữa, Hỏng hóc theo từng tòa nhà.
  3. *Danh sách các thiết bị hư hỏng cần xử lý gấp*: Bảng chi tiết mã TB, tên, phòng, mô tả sự cố kỹ thuật.
  4. *Đề xuất kế hoạch bảo dưỡng, thay thế*: Kế hoạch kinh phí khắc phục.
- **Footer**:
  - Chữ ký xác nhận: Cán bộ kiểm kê cơ sở vật chất - Kỹ thuật viên bảo trì - Đại diện BQL KTX.

---

## 3. Thiết Kế Mô Hình Dữ Liệu (Data Model Architecture)

### 3.1. Các Enum Định Danh (Enums)

#### `ReportType.cs` (`Dormitory.Core.Enums`)
Phân loại các loại báo cáo được hỗ trợ trong hệ thống:
```csharp
namespace Dormitory.Core.Enums;

/// <summary>
/// Loại báo cáo nghiệp vụ trong hệ thống Ký túc xá
/// </summary>
public enum ReportType
{
    /// <summary>
    /// Báo cáo vi phạm nội quy & kỷ luật sinh viên
    /// </summary>
    Violation = 1,

    /// <summary>
    /// Báo cáo tài chính, doanh thu tiền phòng và điện nước
    /// </summary>
    Financial = 2,

    /// <summary>
    /// Báo cáo tỷ lệ lấp đầy, sức chứa và tình trạng phòng ở
    /// </summary>
    Occupancy = 3,

    /// <summary>
    /// Báo cáo kiểm kê tài sản và trang thiết bị phòng ở
    /// </summary>
    Equipment = 4
}
```

#### `ReportFormat.cs` (`Dormitory.Core.Enums`)
Định dạng tệp xuất bản:
```csharp
namespace Dormitory.Core.Enums;

/// <summary>
/// Định dạng tệp xuất bản báo cáo
/// </summary>
public enum ReportFormat
{
    /// <summary>
    /// Định dạng tệp Adobe PDF (.pdf)
    /// </summary>
    Pdf = 1,

    /// <summary>
    /// Định dạng bảng tính Microsoft Excel (.xlsx)
    /// </summary>
    Excel = 2
}
```

#### `ReportStatus.cs` (`Dormitory.Core.Enums`)
Trạng thái tiến trình sinh báo cáo:
```csharp
namespace Dormitory.Core.Enums;

/// <summary>
/// Trạng thái tiến trình tạo và lưu báo cáo
/// </summary>
public enum ReportStatus
{
    /// <summary>
    /// Đang trong quá trình tổng hợp dữ liệu và xuất tệp
    /// </summary>
    Generating = 1,

    /// <summary>
    /// Đã hoàn thành xuất báo cáo thành công
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Xảy ra lỗi trong quá trình tổng hợp hoặc ghi tệp
    /// </summary>
    Failed = 3
}
```

---

### 3.2. Thực Thể Cơ Sở Dữ Liệu: `ReportHistory`

Thực thể đại diện cho lịch sử các lần xuất báo cáo được lưu trong cơ sở dữ liệu SQLite:

```csharp
namespace Dormitory.Core.Entities;

using Dormitory.Core.Enums;

/// <summary>
/// Thực thể ghi nhận lịch sử các lượt xuất báo cáo nghiệp vụ
/// </summary>
public class ReportHistory
{
    /// <summary>
    /// Khóa chính bản ghi lịch sử
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Loại báo cáo (Vi phạm, Tài chính, Lấp đầy, Thiết bị)
    /// </summary>
    public ReportType ReportType { get; set; }

    /// <summary>
    /// Tiêu đề báo cáo người dùng đặt hoặc sinh tự động
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Định dạng tệp xuất bản (PDF hoặc Excel)
    /// </summary>
    public ReportFormat Format { get; set; }

    /// <summary>
    /// Trạng thái kết quả xuất báo cáo
    /// </summary>
    public ReportStatus Status { get; set; } = ReportStatus.Completed;

    /// <summary>
    /// Tên tệp lưu trữ trên hệ thống (Ví dụ: "BaoCao_TaiChinh_T10_2026.xlsx")
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn tương đối hoặc tuyệt đối tới tệp báo cáo đã lưu
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Dung lượng tệp tính bằng Bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Chuỗi JSON lưu trữ các tham số lọc áp dụng khi sinh báo cáo
    /// </summary>
    public string? ParametersJson { get; set; }

    /// <summary>
    /// Thông điệp lỗi chi tiết nếu trạng thái là Failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Tên hoặc tài khoản cán bộ thực hiện xuất báo cáo
    /// </summary>
    public string GeneratedBy { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm sinh báo cáo (mặc định UTC)
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
```

#### Cập nhật DbContext:
- Thêm `DbSet<ReportHistory> ReportHistories { get; }` vào `IDormitoryDbContext` và `DormitoryDbContext`.
- Fluent API cấu hình bảng `ReportHistories`: Index trên `ReportType`, `GeneratedAt` để tăng tốc độ truy vấn danh sách lịch sử.

---

### 3.3. Thiết Kế Data Transfer Objects (DTOs)

Các DTOs được đặt trong `src/Dormitory.Application/DTOs/ReportDtos.cs`:

#### 1. DTO Yêu Cầu Sinh Báo Cáo (`GenerateReportRequestDto`)
```csharp
public class GenerateReportRequestDto
{
    public ReportType Type { get; set; }
    public ReportFormat Format { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public string? Building { get; set; }
    public int? RoomId { get; set; }
    public string? AdditionalFiltersJson { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
}
```

#### 2. DTO Lịch Sử Báo Cáo (`ReportHistoryDto`)
```csharp
public class ReportHistoryDto
{
    public int Id { get; set; }
    public ReportType ReportType { get; set; }
    public string ReportTypeName => ReportType switch
    {
        ReportType.Violation => "Vi phạm kỷ luật",
        ReportType.Financial => "Tài chính & Thu phí",
        ReportType.Occupancy => "Tỷ lệ lấp đầy phòng",
        ReportType.Equipment => "Kiểm kê tài sản thiết bị",
        _ => "Khác"
    };
    public string Title { get; set; } = string.Empty;
    public ReportFormat Format { get; set; }
    public string FormatName => Format == ReportFormat.Pdf ? "PDF" : "Excel";
    public ReportStatus Status { get; set; }
    public string StatusName => Status switch
    {
        ReportStatus.Generating => "Đang tạo",
        ReportStatus.Completed => "Hoàn thành",
        ReportStatus.Failed => "Thất bại",
        _ => "Không rõ"
    };
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted => FileSizeBytes < 1024 ? $"{FileSizeBytes} B" :
                                       FileSizeBytes < 1024 * 1024 ? $"{FileSizeBytes / 1024.0:F1} KB" :
                                       $"{FileSizeBytes / (1024.0 * 1024.0):F2} MB";
    public string? ParametersJson { get; set; }
    public string? ErrorMessage { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}
```

#### 3. DTO Tổng Hợp Báo Cáo Vi Phạm (`ViolationReportSummaryDto`)
```csharp
public class ViolationReportSummaryDto
{
    public int TotalViolations { get; set; }
    public int MinorCount { get; set; }
    public int ModerateCount { get; set; }
    public int SevereCount { get; set; }
    public int CriticalCount { get; set; }
    public int PendingCount { get; set; }
    public int ResolvedCount { get; set; }
    public int DismissedCount { get; set; }
    public int TotalDemeritPoints { get; set; }
    public decimal TotalFineAmount { get; set; }
    public decimal ResolutionRate => TotalViolations > 0 
        ? Math.Round((decimal)(ResolvedCount + DismissedCount) / TotalViolations * 100, 2) : 0;
    
    public List<ViolationDto> Violations { get; set; } = new();
    public List<TopViolatingStudentDto> TopStudents { get; set; } = new();
    public List<TopViolatingRoomDto> TopRooms { get; set; } = new();
}

public class TopViolatingStudentDto
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int ViolationCount { get; set; }
    public int DemeritPoints { get; set; }
    public decimal TotalFines { get; set; }
}

public class TopViolatingRoomDto
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int ViolationCount { get; set; }
}
```

#### 4. DTO Tổng Hợp Báo Cáo Tài Chính (`FinancialReportSummaryDto`)
```csharp
public class FinancialReportSummaryDto
{
    public decimal TotalExpectedRevenue { get; set; }
    public decimal TotalCollectedRevenue { get; set; }
    public decimal RoomFeeCollected { get; set; }
    public decimal ElectricFeeCollected { get; set; }
    public decimal WaterFeeCollected { get; set; }
    public decimal OtherFeeCollected { get; set; }
    public decimal TotalOutstandingDebt => Math.Max(0, TotalExpectedRevenue - TotalCollectedRevenue);
    public decimal PendingDebt { get; set; }
    public decimal OverdueDebt { get; set; }
    public decimal CollectionRate => TotalExpectedRevenue > 0 
        ? Math.Round(TotalCollectedRevenue / TotalExpectedRevenue * 100, 2) : 0;
    
    public int TotalBillsCount { get; set; }
    public int PaidBillsCount { get; set; }
    public int UnpaidBillsCount { get; set; }
    public int OverdueBillsCount { get; set; }
    public decimal TotalElectricUsageKwh { get; set; }
    public decimal TotalWaterUsageM3 { get; set; }

    public List<BillDto> Bills { get; set; } = new();
    public List<BillDto> OverdueBills { get; set; } = new();
    public List<BuildingRevenueSummaryDto> BuildingRevenues { get; set; } = new();
}

public class BuildingRevenueSummaryDto
{
    public string BuildingName { get; set; } = string.Empty;
    public int BillsCount { get; set; }
    public decimal ExpectedRevenue { get; set; }
    public decimal CollectedRevenue { get; set; }
    public decimal OutstandingDebt => Math.Max(0, ExpectedRevenue - CollectedRevenue);
}
```

#### 5. DTO Tổng Hợp Báo Cáo Tỷ Lệ Lấp Đầy (`OccupancyReportSummaryDto`)
```csharp
public class OccupancyReportSummaryDto
{
    public int TotalRooms { get; set; }
    public int TotalBedsCapacity { get; set; }
    public int OccupiedBeds { get; set; }
    public int AvailableBeds => Math.Max(0, TotalBedsCapacity - OccupiedBeds);
    public decimal OccupancyRate => TotalBedsCapacity > 0 
        ? Math.Round((decimal)OccupiedBeds / TotalBedsCapacity * 100, 2) : 0;

    public int OccupiedRoomsCount { get; set; }
    public int AvailableRoomsCount { get; set; }
    public int MaintenanceRoomsCount { get; set; }
    public int EmptyRoomsCount { get; set; }

    public int MaleBedsCapacity { get; set; }
    public int MaleBedsOccupied { get; set; }
    public int FemaleBedsCapacity { get; set; }
    public int FemaleBedsOccupied { get; set; }

    public List<BuildingOccupancyDto> Buildings { get; set; } = new();
    public List<RoomDto> VacantRooms { get; set; } = new();
}
```

#### 6. DTO Tổng Hợp Báo Cáo Kiểm Kê Tài Sản (`EquipmentReportSummaryDto`)
```csharp
public class EquipmentReportSummaryDto
{
    public int TotalEquipmentCount { get; set; }
    public decimal TotalAssetValue { get; set; }
    public int GoodConditionCount { get; set; }
    public int NeedsRepairCount { get; set; }
    public int BrokenCount { get; set; }
    public decimal HealthRate => TotalEquipmentCount > 0 
        ? Math.Round((decimal)GoodConditionCount / TotalEquipmentCount * 100, 2) : 0;
    public decimal FaultyRate => TotalEquipmentCount > 0 
        ? Math.Round((decimal)(NeedsRepairCount + BrokenCount) / TotalEquipmentCount * 100, 2) : 0;
    public decimal DamagedAssetValue { get; set; }
    public decimal EstimatedRepairCost { get; set; }
    public int OverdueMaintenanceCount { get; set; }

    public List<EquipmentDto> Equipments { get; set; } = new();
    public List<EquipmentDto> FaultyEquipments { get; set; } = new();
    public List<BuildingEquipmentSummaryDto> BuildingEquipments { get; set; } = new();
}

public class BuildingEquipmentSummaryDto
{
    public string BuildingName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public int GoodQuantity { get; set; }
    public int NeedsRepairQuantity { get; set; }
    public int BrokenQuantity { get; set; }
    public decimal TotalValue { get; set; }
}
```

---

## 4. Thiết Kế Tầng Dịch Vụ Nghiệp Vụ (Service Architecture)

### 4.1. Giao Diện Dịch Vụ: `IReportService`

Interface `IReportService` đặt tại `src/Dormitory.Application/Interfaces/IReportService.cs`:

```csharp
namespace Dormitory.Application.Interfaces;

using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

/// <summary>
/// Giao diện dịch vụ tổng hợp và xuất bản báo cáo phân tích ký túc xá
/// </summary>
public interface IReportService
{
    // ==========================================
    // 1. NHÓM TRUY VẤN VÀ TỔNG HỢP DỮ LIỆU
    // ==========================================

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo vi phạm nội quy & kỷ luật
    /// </summary>
    Task<ViolationReportSummaryDto> GetViolationReportDataAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo tài chính & thu phí
    /// </summary>
    Task<FinancialReportSummaryDto> GetFinancialReportDataAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo tỷ lệ lấp đầy và tình trạng phòng
    /// </summary>
    Task<OccupancyReportSummaryDto> GetOccupancyReportDataAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Tổng hợp dữ liệu báo cáo kiểm kê tài sản & thiết bị
    /// </summary>
    Task<EquipmentReportSummaryDto> GetEquipmentReportDataAsync(GenerateReportRequestDto request);

    // ==========================================
    // 2. NHÓM XUẤT BẢN FILE NHỊ PHÂN (BINARY GENERATION)
    // ==========================================

    /// <summary>
    /// Xuất báo cáo định dạng Microsoft Excel (.xlsx) qua ClosedXML
    /// </summary>
    Task<byte[]> GenerateExcelReportAsync(GenerateReportRequestDto request);

    /// <summary>
    /// Xuất báo cáo định dạng Adobe PDF (.pdf) qua QuestPDF
    /// </summary>
    Task<byte[]> GeneratePdfReportAsync(GenerateReportRequestDto request);

    // ==========================================
    // 3. NHÓM QUẢN LÝ LỊCH SỬ VÀ LƯU TRỮ (HISTORY & STORAGE)
    // ==========================================

    /// <summary>
    /// Thực hiện trọn gói: Sinh báo cáo, lưu tệp vào thư mục đích và ghi bản ghi ReportHistory
    /// </summary>
    Task<ReportHistoryDto> GenerateAndSaveReportAsync(GenerateReportRequestDto request, string? targetDirectory = null);

    /// <summary>
    /// Lấy danh sách lịch sử các lượt xuất báo cáo có phân loại và giới hạn
    /// </summary>
    Task<List<ReportHistoryDto>> GetReportHistoryAsync(ReportType? type = null, int limit = 50);

    /// <summary>
    /// Lấy chi tiết một bản ghi lịch sử báo cáo theo ID
    /// </summary>
    Task<ReportHistoryDto?> GetReportHistoryByIdAsync(int id);

    /// <summary>
    /// Xóa một bản ghi lịch sử báo cáo và tùy chọn xóa tệp đính kèm trên đĩa
    /// </summary>
    Task<bool> DeleteReportHistoryAsync(int id, bool deletePhysicalFile = true);
}
```

### 4.2. Cơ Chế Lưu Trữ Tệp (File Storage Mechanism)
- Khi người dùng xuất báo cáo, tệp sẽ được lưu trữ tự động vào thư mục mặc định:
  - Đường dẫn mặc định: `Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "[ReportType]", "[yyyy-MM]")`
  - Ví dụ: `reports/Financial/2026-10/BaoCao_TaiChinh_T10_2026_20261006183000.xlsx`
- Lưu ý quyền hạn: Đảm bảo kiểm tra tồn tại thư mục và tạo mới an toàn (`Directory.CreateDirectory`).
- Tên tệp chuẩn hóa không dấu, có gắn timestamp chống trùng lặp.

---

## 5. Thiết Kế Luồng Giao Diện Người Dùng (Desktop UI/UX Workflow)

```
[ Menu Bên Trái: 📊 Báo Cáo & Thống Kê ]
                  │
                  ▼
┌────────────────────────────────────────────────────────────────────────┐
│                        ReportListView (Avalonia UI)                    │
│                                                                        │
│ ┌────────────────────────────────────────────────────────────────────┐ │
│ │                  4 Quick Action Cards (Thao Tác Nhanh)             │ │
│ │  [🚨 Vi Phạm Kỷ Luật] [💰 Tài Chính] [🏢 Lấp Đầy] [🛠️ Thiết Bị]     │ │
│ │  Nút: [Tạo Báo Cáo]   Nút: [...]     Nút: [...]   Nút: [...]       │ │
│ └────────────────────────────────────────────────────────────────────┘ │
│                                                                        │
│ ┌────────────────────────────────────────────────────────────────────┐ │
│ │  Thanh Công Cụ Lịch Sử:                                             │ │
│ │  [Tìm kiếm...] | Loại: [Tất cả ▼] | [🔄 Làm mới] | [➕ Xuất Báo Cáo] │ │
│ ├────────────────────────────────────────────────────────────────────┤ │
│ │  DataGrid: Lịch Sử Xuất Báo Cáo (ReportHistory)                    │ │
│ │  • STT | Tiêu Đề | Loại | Định Dạng (Badge) | Kích Thước | Ngày Tạo│ │
│ │  • Thao Tác: [👁️ Mở Tệp]  [💾 Tải Về]  [🗑️ Xóa]                   │ │
│ └────────────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────┬───────────────────────────────────┘
                                     │ Nhấn [Tạo Báo Cáo]
                                     ▼
┌────────────────────────────────────────────────────────────────────────┐
│                 ReportGenerateDialogWindow (Modal Dialog)              │
│                                                                        │
│ 1. Loại Báo Cáo:   (o) Vi Phạm   ( ) Tài Chính   ( ) Lấp Đầy  ( ) TB   │
│ 2. Tiêu Đề:        [Báo Cáo Vi Phạm Nội Quy Tháng 10/2026            ] │
│ 3. Định Dạng:      (o) Adobe PDF (.pdf)      ( ) Microsoft Excel (.xlsx│
│ 4. Thời Gian Lọc:  Từ Ngày: [ 01/10/2026 ]   Đến Ngày: [ 06/10/2026 ]  │
│ 5. Phạm Vi:        Tòa Nhà: [ Tất cả ▼ ]     Phòng:    [ Tất cả ▼ ]    │
│ 6. Tham Số Phụ:    (Tùy biến theo từng loại báo cáo được chọn)        │
│                                                                        │
│    [ ProgressBar: Đang tổng hợp dữ liệu... (khi IsGenerating) ]        │
│                                                                        │
│                           [❌ Hủy Bỏ]    [🚀 Xuất Báo Cáo Ngay]        │
└────────────────────────────────────────────────────────────────────────┘
```

### 5.1. Tích Hợp Vào `MainWindowViewModel`
- Thêm thuộc tính `_reportListVm` (inject qua DI).
- Thêm lệnh `NavigateToReportsCommand`:
  ```csharp
  public void NavigateToReports()
  {
      CurrentView = _reportListVm;
      ActiveMenu = "Reports";
      _ = _reportListVm.LoadHistoryAsync();
  }
  ```
- Thêm nút Menu Sidebar trên `MainWindow.axaml`:
  - Icon: `📊` hoặc Path SVG báo cáo.
  - Text: `Báo Cáo & Thống Kê`.
  - Command: `{Binding NavigateToReportsCommand}`.
  - IsVisible / IsEnabled: Phân quyền `Admin` và `Manager` (nhân viên `Staff` có thể xem nếu được cấp quyền).

### 5.2. Màn Hình Danh Sách & Lịch Sử: `ReportListView`
- **Thành phần**:
  - **Header Bar**: Tiêu đề trang "Báo Cáo & Thống Kê Nghiệp Vụ", mô tả "Tổng hợp chỉ số, xuất khẩu dữ liệu định dạng Excel và PDF".
  - **Khối 4 Quick Action Cards**:
    - Mỗi card hiển thị: Icon đại diện, Tiêu đề loại báo cáo, Mô tả tóm tắt, Nút "Tạo Báo Cáo" màu sắc theo phong cách Fluent UI (Blue/Green/Teal/Orange).
  - **Bảng Lịch Sử (History DataGrid)**:
    - Hiển thị danh sách các báo cáo đã tạo gần nhất.
    - Cột định dạng hiển thị Badge màu (Đỏ cho PDF, Xanh lá cho Excel).
    - Cột thao tác:
      - Nút "Mở Tệp": Kích hoạt mở tệp trực tiếp bằng ứng dụng mặc định trên hệ điều hành (`Process.Start`).
      - Nút "Lưu Vào...": Mở hộp thoại `SaveFileDialog` cho phép người dùng chọn vị trí lưu tùy ý.
      - Nút "Xóa": Hiển thị `ConfirmDialogWindow` trước khi xóa lịch sử.

### 5.3. Hộp Thoại Modal: `ReportGenerateDialogWindow`
- Cho phép người dùng cấu hình chi tiết:
  - Khi thay đổi loại báo cáo (Radio button), giao diện động ẩn/hiện các ô nhập phù hợp (Ví dụ: Báo cáo tài chính hiển thị chọn Tháng/Năm; Báo cáo vi phạm hiển thị chọn Mức độ kỷ luật; Báo cáo lấp đầy hiển thị chọn Loại phòng/Giới tính).
  - Nút "Xuất Báo Cáo Ngay" hiển thị trạng thái Loading và vô hiệu hóa nút bấm trong quá trình sinh để tránh click đúp.
  - Xử lý mở tệp ngay khi xuất xong thông qua `DialogService`.

---

## 6. Kế Hoạch Triển Khai Kỹ Thuật (Phân Rã Tasks Tiếp Theo)

| Task | Tên Công Việc | Chi Tiết Thực Hiện |
|---|---|---|
| **Task 1** | **Phân tích yêu cầu & Đặc tả nghiệp vụ** *(Hiện tại)* | Khảo sát mã nguồn, phân tích KPI, thiết kế DTOs, Enums, Service interface, viết `docs/phase7/reporting-requirements-spec.md`. |
| **Task 2** | **Thiết kế Data Model & DTOs** | Tạo các Enums, Entity `ReportHistory`, cập nhật `IDormitoryDbContext`, cấu hình Migration/DbSet trong `DormitoryDbContext`, triển khai đầy đủ DTOs trong `ReportDtos.cs`. |
| **Task 3** | **Xây dựng `IReportService` & Xuất File** | Triển khai `ReportService`: Tổng hợp số liệu từ EF Core, xuất file Excel bằng `ClosedXML` đa sheet đẹp mắt, xuất file PDF bằng `QuestPDF` chuẩn trang trọng, ghi nhận lịch sử vào `ReportHistory`. |
| **Task 4** | **Thiết kế Giao Diện Avalonia Desktop** | Xây dựng `ReportListView.axaml`, `ReportListViewModel.cs`, `ReportGenerateDialogWindow.axaml`, `ReportGenerateDialogViewModel.cs`, các ValueConverters hiển thị badge định dạng/trạng thái. |
| **Task 5** | **Kết Nối Hệ Thống & Điều Hướng** | Đăng ký dịch vụ trong `App.axaml.cs` DI container, cập nhật thanh điều hướng `MainWindowViewModel` và `MainWindow.axaml`, kiểm tra phân quyền người dùng. |
| **Task 6** | **Kiểm Thử Toàn Diện & Verification** | Viết Unit Tests cho `ReportService` (các phép tính KPI, sinh Excel/PDF), kiểm thử E2E Headless Avalonia UI, chạy GitNexus impact & detect-changes, chuẩn bị phát hành. |

---

## 7. Phân Tích Rủi Ro & Giải Pháp Kỹ Thuật

1. **Hiệu năng khi tổng hợp dữ liệu quy mô lớn**:
   - *Rủi ro*: Truy vấn nhiều bảng kèm quan hệ sâu (`Include`, `ThenInclude`) có thể làm chậm ứng dụng hoặc tốn RAM khi ký túc xá có hàng nghìn sinh viên và hóa đơn.
   - *Giải pháp*: Sử dụng truy vấn `.AsNoTracking()`, chiếu trực tiếp dữ liệu (projection) bằng `.Select(...)` để chỉ lấy các trường cần thiết, tính toán tổng hợp (aggregate) trên cơ sở dữ liệu thay vì tải toàn bộ thực thể về bộ nhớ.
2. **Hiển thị tiếng Việt và Font chữ trên QuestPDF**:
   - *Rủi ro*: QuestPDF có thể bị lỗi font hoặc vỡ ký tự tiếng Việt có dấu nếu hệ điều hành thiếu font chuẩn.
   - *Giải pháp*: Cấu hình font chữ tiêu chuẩn hỗ trợ đầy đủ Unicode tiếng Việt (ví dụ: `Roboto`, `Segoe UI`, `Arial` hoặc font dự phòng của hệ điều hành), thiết lập `QuestPDF.Settings.License = LicenseType.Community` ở constructor.
3. **Mở tệp đa nền tảng (Cross-platform Process.Start)**:
   - *Rủi ro*: Lệnh mở tệp sau khi xuất trên macOS (`open`), Windows (`explorer.exe` / `cmd /c start`), Linux (`xdg-open`) có sự khác biệt.
   - *Giải pháp*: Tận dụng dịch vụ `IFileService` hoặc đóng gói phương thức trợ giúp mở tệp kiểm tra `RuntimeInformation.IsOSPlatform` tương thích hoàn toàn trên cả 3 nền tảng.
4. **Xử lý tệp vật lý khi xóa lịch sử báo cáo**:
   - *Rủi ro*: Tệp vật lý còn sót lại gây lãng phí dung lượng đĩa hoặc ngoại lệ truy cập tệp khi đang được mở bởi ứng dụng khác (Excel/Adobe Reader).
   - *Giải pháp*: Bọc thao tác `File.Delete` trong khối `try-catch` an toàn; nếu tệp đang bị khóa hoặc không tìm thấy, hệ thống vẫn xóa bản ghi lịch sử trong cơ sở dữ liệu và ghi log cảnh báo thay vì làm sập ứng dụng.

---
*Tài liệu được hoàn thiện phục vụ làm cơ sở kỹ thuật chuẩn mực cho việc triển khai Task 2 đến Task 6 của Phase 7.*
