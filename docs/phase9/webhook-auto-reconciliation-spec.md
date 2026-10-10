# Đặc Tả Kỹ Thuật: Webhook Biến Động Số Dư & Tự Động Đối Soát Gạch Nợ Hóa Đơn (Phase 9)

> **Tài liệu kỹ thuật chính thức** dành cho Phân hệ Webhook Listener nhúng và Cơ chế Tự động Đối soát (Auto-Reconciliation) hóa đơn KTX thời gian thực của dự án **Dormitory Manager v2.5.0**.

---

## 1. Giới Thiệu & Kiến Trúc Tổng Quan

### 1.1. Bối cảnh & Mục tiêu
Trong các phiên bản trước (từ `v2.4.0` trở về trước), hệ thống hỗ trợ quét mã VietQR động tại quầy lễ tân hoặc trên hóa đơn PDF/Email, tuy nhiên việc xác nhận thanh toán vẫn phụ thuộc vào thao tác thủ công của nhân viên thu ngân/kế toán (bấm nút *"Xác nhận đã thu tiền"*). 

Phân hệ **Phase 9: Bank Webhook Auto-Reconciliation** hiện đại hóa toàn diện quy trình thu phí:
- **Tự động nhận biến động số dư**: Tiếp nhận thông báo tức thời (Webhook HTTP POST) từ các cổng thanh toán ngân hàng như **PayOS**, **Casso** hoặc **Open Banking Gateway**.
- **Xác thực bảo mật cao**: Xác minh tính hợp lệ của request qua chữ ký số **HMAC-SHA256** hoặc **Secret Token**, chống giả mạo số dư.
- **Bóc tách nội dung thông minh**: Phân tích nội dung chuyển khoản bằng Regex linh hoạt có cơ chế chống **ReDoS**, trích xuất mã hóa đơn (ví dụ: `KTX HD001`, `KTX-HD001`, `ktxhd001`).
- **Tự động gạch nợ CSDL**: Tự động chuyển trạng thái hóa đơn sang `Đã thanh toán (Paid)`, lưu lịch sử giao dịch `PaymentTransaction`, phòng ngừa trùng lặp (`Idempotency`).
- **Đồng bộ giao diện thời gian thực**: Bắn sự kiện an toàn sang Avalonia UI Thread (`PaymentNotificationService` -> `Dispatcher.UIThread.InvokeAsync`), tự động làm mới danh sách hóa đơn và hiển thị thông báo Toast.

---

### 1.2. Kiến Trúc Máy Chủ Webhook Nhúng (Embedded HttpListener)

Thay vì tích hợp toàn bộ khung làm việc ASP.NET Core cồng kềnh làm tăng dung lượng phân phối và tiêu tốn bộ nhớ máy trạm, Dormitory Manager sử dụng trực tiếp lớp nhúng **`System.Net.HttpListener`** có sẵn trong .NET 8 BCL:

```
[Ngân hàng / Cổng PayOS / Casso]
            │ (HTTP POST Webhook JSON)
            ▼
[Cloudflare Tunnel / ngrok (HTTPS -> Port 5005)]
            │
            ▼
┌─────────────────────────────────────────────────────────────┐
│ Dormitory Manager Desktop (v2.5.0)                          │
│                                                             │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ WebhookListenerService (Embedded HttpListener)          │ │
│ │ - Port: 5005 (Mặc định, cấu hình động)                  │ │
│ │ - Path: /api/webhook/payment                            │ │
│ │ - Xác thực chữ ký HMAC-SHA256 / Secret Token            │ │
│ │ - Trả ngay HTTP 200 OK chống timeout cổng thanh toán    │ │
│ └────────────────────────────┬────────────────────────────┘ │
│                              │ (Task.Run / Background)      │
│                              ▼                              │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ PaymentReconciliationService (Tầng Application)         │ │
│ │ - Kiểm tra Idempotency (TransactionId đã xử lý chưa)    │ │
│ │ - Regex bóc tách mã hóa đơn KTX (Timeout 500ms chống    │ │
│ │   ReDoS)                                                │ │
│ │ - Đối soát số tiền (Đủ / Thiếu / Thừa / Không khớp)     │ │
│ │ - Ghi CSDL SQLite (Bảng PaymentTransactions & Bills)    │ │
│ └────────────────────────────┬────────────────────────────┘ │
│                              │                              │
│                              ▼                              │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ PaymentNotificationService (Sự kiện thời gian thực)     │ │
│ └────────────────────────────┬────────────────────────────┘ │
│                              │ (Dispatcher.UIThread.Post)   │
│                              ▼                              │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ Avalonia UI Presentation Layer                          │ │
│ │ - BillListViewModel: Tự động refresh danh sách          │ │
│ │ - PaymentTransactionListViewModel: Cập nhật giao dịch   │ │
│ │ - Toast Notification: Thông báo thanh toán thành công   │ │
│ └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

#### Ưu điểm kiến trúc:
1. **Siêu nhẹ & Tiết kiệm tài nguyên**: Không khởi chạy Kestrel hay ASP.NET WebHost; chiếm chưa đến 5MB RAM bổ sung.
2. **Khởi chạy ngầm không block UI**: Lắng nghe bằng vòng lặp bất đồng bộ `GetContextAsync()` và xử lý từng request trên `Task.Run()`.
3. **Phản hồi tức thì (Zero-Latency ACK)**: Trả về HTTP 200 OK ngay lập tức trước khi phân tích CSDL nặng, ngăn chặn cổng thanh toán thử lại (retry loop) do quá thời gian chờ (timeout).
4. **Cô lập Scope an toàn**: Mỗi giao dịch đối soát được phân giải qua `IServiceScopeFactory`, đảm bảo `DormitoryDbContext` luôn tươi mới và không xung đột luồng.

---

## 2. Đặc Tả Cấu Trúc JSON Webhook Payload

Hệ thống hỗ trợ 3 định dạng Webhook phổ biến nhất tại Việt Nam: **PayOS**, **Casso** và định dạng chuẩn hóa **Generic Webhook**.

### 2.1. Cổng Thanh Toán PayOS

PayOS đẩy Webhook dạng JSON chứa thông tin giao dịch thanh toán qua mã QR VietQR Pro.

#### Cấu trúc Payload:
```json
{
  "code": "00",
  "desc": "success",
  "data": {
    "orderCode": 123456,
    "amount": 1500000,
    "description": "KTX HD20261001 NGUYEN VAN A",
    "accountNumber": "0381000556677",
    "reference": "FT24284893721",
    "transactionDateTime": "2026-10-10 14:30:00",
    "currency": "VND",
    "paymentLinkId": "c8e2350b-...",
    "code": "00",
    "desc": "Thanh toán thành công",
    "counterAccountBankId": "970422",
    "counterAccountBankName": "MBBank",
    "counterAccountName": "NGUYEN VAN A",
    "counterAccountNumber": "0987654321"
  },
  "signature": "a1b2c3d4e5f60718293a4b5c6d7e8f90123456789abcdef0123456789abcdef0"
}
```

#### Quy tắc bóc tách (`WebhookListenerService.ParseIncomingPayload`):
- `TransactionId`: Ưu tiên trường `reference`, nếu trống lấy `orderCode.ToString()`.
- `Amount`: Giá trị `data.amount`.
- `Description`: Chuỗi `data.description` chứa nội dung thanh toán.
- `AccountNumber`: Số tài khoản nhận `data.accountNumber`.
- `BankBin`: Mã ngân hàng người gửi `data.counterAccountBankId`.
- `TransactionDate`: Thời gian giao dịch `data.transactionDateTime` (hoặc thời gian hiện tại nếu phân tích lỗi).
- `Signature`: Chữ ký số HMAC-SHA256 trong trường `signature` hoặc từ Header `X-PayOS-Signature`.

---

### 2.2. Cổng Thanh Toán Casso (Open Banking Tự Động)

Casso đồng bộ biến động số dư tài khoản ngân hàng của doanh nghiệp/nhà trường thông qua dịch vụ ngân hàng số và đẩy danh sách giao dịch qua Webhook.

#### Cấu trúc Payload:
```json
{
  "error": 0,
  "messages": "success",
  "data": [
    {
      "id": 98765,
      "tid": "FT26284000129",
      "description": "MBVCB.789123456.KTX HD20261002.CT tu NGUYEN VAN B",
      "amount": 1850000,
      "cusum_balance": 54200000,
      "when": "2026-10-10T14:35:12+07:00",
      "bank_sub_acc_id": "0381000556677",
      "subAccId": "0381000556677",
      "bankName": "Vietcombank",
      "bankAbbreviation": "VCB",
      "corresponsiveAccount": "1018273645",
      "corresponsiveBankId": "970422",
      "corresponsiveBankName": "MBBank"
    }
  ]
}
```

#### Quy tắc bóc tách:
- `TransactionId`: Mã định danh giao dịch ngân hàng `tid` (hoặc `id.ToString()`).
- `Amount`: Trường `amount`.
- `Description`: Trường `description`.
- `AccountNumber`: Trường `subAccId` hoặc `bank_sub_acc_id`.
- `BankBin`: Mã `corresponsiveBankId`.
- `TransactionDate`: Giá trị trường `when`.
- Chữ ký bảo mật: Casso thường truyền Secure-Token qua Header `Authorization: Bearer <secret_key>` hoặc `X-Token`.

---

### 2.3. Định Dạng Chuẩn Hóa (Generic Webhook)

Phục vụ các hệ thống cổng ngân hàng khác hoặc kịch bản kiểm thử API tùy biến.

#### Cấu trúc Payload:
```json
{
  "gateway": "Generic",
  "transactionId": "TXN_20261010_0001",
  "amount": 2100000,
  "description": "KTX HD20261003 nop tien ky tuc xa",
  "accountNumber": "0381000556677",
  "bankBin": "970436",
  "transactionDate": "2026-10-10T14:40:00Z",
  "signature": "d3f4a56b7c8d9e0f1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b1c2d3e4f"
}
```

---

## 3. Cơ Chế Bảo Mật & Phòng Thủ Mã Độc

### 3.1. Xác Thực Chữ Ký HMAC-SHA256 & Secret Token
Hệ thống hỗ trợ cơ chế xác thực kép:
1. **Chữ ký số HMAC-SHA256**: Tính toán mã băm mật mã học của toàn bộ nội dung body JSON với khóa bí mật `SecretKey`:
   $$\text{ComputedSignature} = \text{HMAC-SHA256}(\text{RawJsonUtf8}, \text{SecretKeyUtf8})$$
2. **Token Bí Mật (Token-based Secret)**: Trường hợp cổng gửi Token cố định qua Header (`X-Token`, `Authorization: Bearer <token>`).

### 3.2. Chống Tấn Công Timing Attack (`CryptographicOperations.FixedTimeEquals`)
Khi so sánh chữ ký số người dùng gửi lên với chữ ký hệ thống tính toán, việc dùng toán tử so sánh chuỗi thông thường (`==` hoặc `string.Equals`) có nguy cơ bị tấn công dò tìm byte dựa trên thời gian thực thi (Timing Attack).

Dormitory Manager áp dụng giải pháp chuẩn mật mã học:
```csharp
byte[] computedHashBytes = Encoding.UTF8.GetBytes(computedSignature.ToLowerInvariant());
byte[] signatureBytes = Encoding.UTF8.GetBytes(signature.Trim().ToLowerInvariant());

// So sánh độ dài trước, sau đó so sánh thời gian cố định:
if (computedHashBytes.Length == signatureBytes.Length &&
    CryptographicOperations.FixedTimeEquals(computedHashBytes, signatureBytes))
{
    return true; // Chữ ký hợp lệ
}
```

### 3.3. Chống Tấn Công Từ Chối Dịch Vụ Biểu Thức Chính Quy (ReDoS Prevention)
Nội dung chuyển khoản ngân hàng do người dùng tự nhập có thể dài hoặc chứa các chuỗi ký tự bất thường cố ý gây treo CPU (Catastrophic Backtracking).

Hệ thống đặt giới hạn thời gian thực thi (`RegexMatchTimeoutException`) là **500 mili-giây** cho mọi thao tác bóc tách:
```csharp
try
{
    var match = Regex.Match(
        description,
        pattern,
        RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(500));
        
    if (match.Success && match.Groups.Count > 1)
    {
        return match.Groups[1].Value.Trim();
    }
}
catch (RegexMatchTimeoutException)
{
    // Bắt timeout chống ReDoS, ghi nhận không thể bóc tách an toàn
    return null;
}
```

### 3.4. Chống Xử Lý Trùng Lặp Giao Dịch (Idempotency)
Cổng Webhook của ngân hàng có cơ chế tự động gửi lại (retry) khi mạng chập chờn. Để ngăn ngừa việc 1 giao dịch ngân hàng gạch nợ nhiều lần:
1. Bảng `PaymentTransactions` trong SQLite thiết lập chỉ mục duy nhất: `CREATE UNIQUE INDEX IX_PaymentTransactions_TransactionId ON PaymentTransactions(TransactionId)`.
2. Tầng dịch vụ `PaymentReconciliationService` kiểm tra `TransactionId` trước khi đối soát. Nếu mã đã tồn tại, giao dịch được đánh dấu `PaymentTransactionStatus.Duplicate` và không làm thay đổi trạng thái hóa đơn.

---

## 4. Hướng Dẫn Thiết Lập Thực Tế Trong Môi Trường Sản Xuất

Do ứng dụng Desktop chạy trên máy trạm nội bộ của ban quản lý KTX (thường nằm sau Router NAT/Firewall không có IP tĩnh công khai), cần thiết lập một cổng trung chuyển (Tunnel) an toàn từ Internet vào cổng `5005` của máy trạm.

### 4.1. Tùy Chọn 1: Sử Dụng Cloudflare Tunnel (Khuyến Nghị Vận Hành Ổn Định)

Cloudflare Tunnel (`cloudflared`) là giải pháp miễn phí, bảo mật cao và cung cấp tên miền HTTPS cố định, không đổi URL sau mỗi lần khởi động lại.

#### Các bước cài đặt:
1. **Cài đặt cloudflared**:
   - macOS: `brew install cloudflared`
   - Windows: Tải tệp `cloudflared-windows-amd64.exe` từ trang chủ Cloudflare.
   - Linux: `sudo apt-get install cloudflared`

2. **Khởi chạy đường hầm nhanh**:
   ```bash
   cloudflared tunnel --url http://localhost:5005
   ```
   Terminal sẽ in ra một địa chỉ HTTPS công khai, ví dụ:
   `https://random-subdomain.trycloudflare.com`

3. **URL Webhook cấu hình vào PayOS/Casso**:
   ```
   https://random-subdomain.trycloudflare.com/api/webhook/payment
   ```

---

### 4.2. Tùy Chọn 2: Sử Dụng ngrok (Tiện Lợi Thử Nghiệm & Demo)

ngrok là giải pháp phổ biến nhất để mở cổng nhanh phục vụ phát triển phần mềm.

#### Các bước thực hiện:
1. **Khởi chạy ngrok trỏ vào cổng 5005**:
   ```bash
   ngrok http 5005
   ```

2. **Lấy địa chỉ công khai từ ngrok**:
   Giao diện ngrok hiển thị dòng `Forwarding`:
   `https://a1b2-113-161-xx-xx.ngrok-free.app -> http://localhost:5005`

3. **URL Webhook cấu hình**:
   ```
   https://a1b2-113-161-xx-xx.ngrok-free.app/api/webhook/payment
   ```

---

### 4.3. Cấu Hình Trên Trang Quản Trị Cổng Thanh Toán

#### Cổng PayOS (payos.vn):
1. Đăng nhập trang quản trị PayOS.
2. Vào mục **Kênh thanh toán** > **Cài đặt Webhook**.
3. Điền **Webhook URL**: Điền đường dẫn HTTPS từ Cloudflare Tunnel/ngrok kèm `/api/webhook/payment`.
4. Sao chép chuỗi **Checksum Key** do PayOS cung cấp.
5. Mở ứng dụng Dormitory Manager > vào **Cài đặt** > tab **Webhook Đối Soát**:
   - Tích chọn: `Bật Webhook lắng nghe biến động số dư`.
   - Cổng lắng nghe: `5005`.
   - Cổng thanh toán: `PayOS`.
   - Secret Key: Dán chuỗi Checksum Key từ PayOS.
   - Nhấn **"💾 Lưu cấu hình Webhook"**.

#### Cổng Casso (casso.vn):
1. Đăng nhập tài khoản Casso và kết nối tài khoản ngân hàng của KTX.
2. Vào mục **Tích hợp Webhook** > Tạo Webhook mới.
3. Điền **Webhook URL**: URL Tunnel kèm `/api/webhook/payment`.
4. Chọn Header xác thực hoặc Token bí mật.
5. Trong Dormitory Manager, chọn Provider `Casso` và lưu Secret Key tương ứng.

---

### 4.4. Quy Chuẩn Nội Dung Chuyển Khoản & Đối Soát

Để hệ thống bóc tách chính xác 100%, sinh viên chuyển khoản theo cấu trúc:
```
KTX [MÃ_HÓA_ĐƠN]
```
Ví dụ:
- `KTX HD001` -> Trích xuất: `HD001`
- `KTX-HD001 NGUYEN VAN A DONG TIEN DIEN` -> Trích xuất: `HD001`
- `Chuyen tien phong ktxhd005` -> Trích xuất: `HD005`

Khi hóa đơn `HD001` có số tiền nợ là `1.500.000 đ`:
- **Chuyển đủ (>= 1.500.000 đ)**: Hóa đơn tự động chuyển `Paid`, ghi nhận ngày thanh toán, trạng thái giao dịch `Success`.
- **Chuyển thiếu (< 1.500.000 đ)**: Hóa đơn giữ `Unpaid`, trạng thái giao dịch `PartiallyPaid` kèm ghi chú nộp thiếu.
- **Chuyển sai cú pháp (không có mã hóa đơn)**: Trạng thái `Unmatched`, thủ quỹ tra cứu trên màn hình "💳 Giao Dịch Đối Soát" để gán thủ công bằng nút **"🔗 Gán Hóa Đơn"**.
