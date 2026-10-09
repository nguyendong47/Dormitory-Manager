# Kế Hoạch Triển Khai Giai Đoạn 8: Tích Hợp Thanh Toán VietQR Động (NAPAS 247)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hiện đại hóa quy trình thu phí ký túc xá bằng cách tự động sinh mã thanh toán **VietQR động chuẩn NAPAS 247 (EMVCo)**:
1. Sinh mã QR động chuẩn hóa chứa đầy đủ số tiền nợ và nội dung chuyển khoản tự động (`KTX [Mã Hóa Đơn]`).
2. Tích hợp trực tiếp mã VietQR vào **Phiếu thu PDF (`QuestPDF`)** và **Email thông báo hóa đơn (`MailKit`)**.
3. Hiển thị cửa sổ xem và quét mã VietQR tức thì ngay tại quầy lễ tân trên giao diện Desktop Avalonia.
4. Quản lý cấu hình tài khoản ngân hàng thụ hưởng linh hoạt trong Cài đặt hệ thống.
5. Bảo đảm kiểm thử tự động toàn diện (Unit Tests + E2E Headless) và phát hành phiên bản `v2.4.0`.

**Architecture & Tech Stack:**
- **Domain & DTOs**: `BankSettingsDto`, `VietQrResultDto`, danh mục ngân hàng chuẩn NAPAS BIN.
- **Core Algorithm**: Thuật toán sinh chuỗi EMVCo Payload (Tag-Length-Value) + mã kiểm tra CRC-16-CCITT (chạy offline 100% không phụ thuộc internet).
- **Libraries**: `QRCoder` (sinh ảnh PNG/Bitmap offline), `QuestPDF`, `MailKit`.
- **Presentation**: Avalonia UI 11 (`SystemSettingsView`, `VietQrDialogWindow`, `BillListView`).
- **Testing**: `Dormitory.UnitTests` + `Dormitory.E2ETests`.

---

## Danh Sách Tasks Triển Khai

- [x] **Task 1: Cấu hình Tài khoản Ngân hàng KTX & Mô hình DTOs (Backend & DTOs)**
- [x] **Task 2: Dịch vụ sinh chuỗi & hình ảnh VietQR EMVCo chuẩn NAPAS 247 (TDD)**
- [x] **Task 3: Tích hợp VietQR vào Phiếu thu PDF QuestPDF & Email thông báo MailKit**
- [ ] **Task 4: Giao diện Desktop Cài đặt Ngân hàng & Cửa sổ quét mã VietQR tại quầy**
- [ ] **Task 5: Kiểm thử tự động E2E Headless & Unit Tests mở rộng**
- [ ] **Task 6: Tài liệu, CI/CD, GitNexus Sync & Phát hành v2.4.0**
