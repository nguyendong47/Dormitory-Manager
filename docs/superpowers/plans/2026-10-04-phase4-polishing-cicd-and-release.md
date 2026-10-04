# Kế Hoạch Triển Khai Giai Đoạn 4: Dọn Dẹp Mã Nguồn, App Icon, CI/CD & Phát Hành v2.0.0

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Hoàn thiện 4 hạng mục cuối cùng của dự án:
1. **Dọn dẹp mã nguồn cũ**: Di chuyển mã nguồn WinForms .NET 4.7.2 cũ (`KTX2021`, `Dormitory_Management_2021.sln`, `packages`) vào thư mục `legacy/` để kho lưu trữ tinh gọn và thuần .NET 8.
2. **App Icon & Branding**: Thiết kế và tích hợp bộ nhận diện KTX (Icon `.ico` cho Windows/Avalonia, `.icns` cho macOS App Bundle) giúp ứng dụng chuyên nghiệp, mang đậm bản sắc ký túc xá.
3. **GitHub Actions CI/CD**: Xây dựng workflow tự động kiểm thử (`dotnet test`), biên dịch và đóng gói đa nền tảng (macOS DMG, Windows ZIP, Linux Tar.gz), tự động tạo GitHub Release khi gắn thẻ phiên bản.
4. **Nghiệm thu & Hoàn tất phát hành**: Cập nhật tài liệu, kiểm thử toàn bộ giải pháp, đồng bộ đồ thị tri thức GitNexus và tạo thẻ phát hành chính thức `v2.0.0`.

**Architecture & Tooling:**
- **Build & Test**: .NET 8 SDK, xUnit, FluentAssertions (53+ unit tests).
- **Packaging**: Self-Contained DMG (macOS arm64/x64), Single-File Executable ZIP (Windows x64), Tarball (Linux x64).
- **CI/CD**: GitHub Actions (`.github/workflows/ci-cd.yml`).
- **Icons**: `.ico` (Windows & Avalonia WindowIcon), `.icns` (macOS Bundle), `.png` (256x256 / 512x512).

---

## Danh Sách Tệp Sẽ Tạo & Chỉnh Sửa

| Tệp tin | Trách nhiệm |
|---|---|
| `legacy/` | Thư mục lưu trữ dự án WinForms 2021 cũ và gói phụ thuộc cũ |
| `src/Dormitory.Desktop/Assets/AppIcon.ico` | Tệp icon Windows & Avalonia Window |
| `src/Dormitory.Desktop/Assets/AppIcon.png` | Icon logo đồ họa gốc (độ phân giải cao) |
| `scripts/package/AppIcon.icns` | Tệp icon chuẩn macOS Bundle (16x16 -> 512x512@2x) |
| `src/Dormitory.Desktop/Dormitory.Desktop.csproj` | Cấu hình `<ApplicationIcon>` cho tệp .exe Windows |
| `src/Dormitory.Desktop/MainWindow.axaml` | Thiết lập thuộc tính `Icon` cho cửa sổ chính Avalonia |
| `scripts/package/build-macos.sh` | Cập nhật nhúng `AppIcon.icns` vào `Contents/Resources/` và `Info.plist` |
| `scripts/package/build-windows.sh` | Cập nhật đóng gói kèm icon Windows |
| `.github/workflows/ci-cd.yml` | Workflow GitHub Actions tự động kiểm thử và xuất bản đa nền tảng |
| `docs/user-guide.md` & `README.md` | Cập nhật cấu trúc thư mục, hướng dẫn CI/CD và thông tin phát hành |

---

## Chi Tiết Các Task Triển Khai

### Task 1: Dọn Dẹp & Lưu Trữ Mã Nguồn WinForms Cũ (`legacy/`)

**Mục tiêu:** Di chuyển toàn bộ project WinForms .NET Framework 4.7.2 cũ vào thư mục `legacy/` bằng lệnh `git mv` để bảo toàn lịch sử git, đồng thời giữ thư mục gốc sạch sẽ, chỉ chứa mã nguồn .NET 8 hiện đại.

- [ ] **Step 1.1:** Tạo thư mục `legacy/`.
- [ ] **Step 1.2:** Dùng `git mv` di chuyển `KTX2021` và `Dormitory_Management_2021.sln` vào `legacy/`.
- [ ] **Step 1.3:** Di chuyển thư mục `packages/` cũ (nếu có) vào `legacy/packages/`.
- [ ] **Step 1.4:** Cập nhật `.gitignore` để đảm bảo bỏ qua `legacy/packages/` và các thư mục `bin/`, `obj/` trong `legacy/`.
- [ ] **Step 1.5:** Kiểm tra kiểm thử và biên dịch: chạy `dotnet build Dormitory.sln` và `dotnet test Dormitory.sln` để đảm bảo solution .NET 8 hoàn toàn độc lập và pass 53/53 tests.
- [ ] **Step 1.6:** Commit thay đổi: `git commit -m "chore(repo): archive legacy WinForms project into legacy/ directory"`.

---

### Task 2: Thiết Kế & Tích Hợp Bộ Nhận Diện Ứng Dụng (App Icon & Branding)

**Mục tiêu:** Tạo bộ icon ký túc xá chuyên nghiệp đa kích thước (tòa nhà / chìa khóa / ký túc xá), tạo file `.ico` cho Windows/Avalonia và `.icns` cho macOS App Bundle.

- [ ] **Step 2.1:** Tạo script hoặc generator tạo hình ảnh icon ký túc xá vector/PNG độ nét cao (`AppIcon.png`, 512x512) với tông màu Fluent chủ đạo (Xanh dương hiện đại #0078D4 & màu ấm #FFB900).
- [ ] **Step 2.2:** Chuyển đổi thành `AppIcon.ico` (chứa các kích thước 16x16, 32x32, 48x48, 64x64, 128x128, 256x256) đặt tại `src/Dormitory.Desktop/Assets/AppIcon.ico`.
- [ ] **Step 2.3:** Sử dụng công cụ `sips` và `iconutil` của macOS để tạo `AppIcon.icns` đặt tại `scripts/package/AppIcon.icns`.
- [ ] **Step 2.4:** Cấu hình `src/Dormitory.Desktop/Dormitory.Desktop.csproj`:
  - Thêm `<ApplicationIcon>Assets\AppIcon.ico</ApplicationIcon>`.
  - Cấu hình copy `Assets/AppIcon.ico` và `Assets/AppIcon.png` vào output directory hoặc nhúng AvaloniaResource.
- [ ] **Step 2.5:** Cập nhật `src/Dormitory.Desktop/MainWindow.axaml` gắn `Icon="/Assets/AppIcon.ico"`.
- [ ] **Step 2.6:** Cập nhật `scripts/package/build-macos.sh`:
  - Sao chép `AppIcon.icns` vào `$APP_BUNDLE/Contents/Resources/AppIcon.icns`.
  - Cập nhật `Info.plist` thêm `<key>CFBundleIconFile</key><string>AppIcon</string>`.
- [ ] **Step 2.7:** Chạy lại `bash scripts/package/build-macos.sh` để kiểm chứng App Bundle hiển thị icon đầy đủ.
- [ ] **Step 2.8:** Chạy `dotnet test Dormitory.sln` kiểm tra không phát sinh lỗi.
- [ ] **Step 2.9:** Commit thay đổi: `git commit -m "feat(branding): add professional application icons for macOS, Windows, and Linux"`.

---

### Task 3: Xây Dựng Pipeline Tự Động Hóa CI/CD (GitHub Actions)

**Mục tiêu:** Tạo quy trình tự động hóa kiểm thử và đóng gói bản phát hành đa nền tảng trên GitHub Actions.

- [ ] **Step 3.1:** Tạo file cấu hình workflow `.github/workflows/ci-cd.yml`.
- [ ] **Step 3.2:** Cấu hình job `test-and-verify`:
  - Chạy trên `ubuntu-latest`.
  - Cài đặt .NET 8 SDK.
  - Chạy `dotnet test Dormitory.sln --configuration Release`.
- [ ] **Step 3.3:** Cấu hình job `package-macos`:
  - Chạy trên runner `macos-14` (Apple Silicon).
  - Chạy script đóng gói `bash scripts/package/build-macos.sh arm64`.
  - Đẩy artifact tệp DMG lên GitHub Actions Artifacts.
- [ ] **Step 3.4:** Cấu hình job `package-windows`:
  - Chạy trên runner `ubuntu-latest` (hoặc `windows-latest`).
  - Chạy lệnh xuất bản `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`.
  - Nén thành tệp `.zip` và đẩy lên Artifacts.
- [ ] **Step 3.5:** Cấu hình job `package-linux`:
  - Chạy trên runner `ubuntu-latest`.
  - Chạy lệnh xuất bản `dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true`.
  - Nén thành tệp `.tar.gz` và đẩy lên Artifacts.
- [ ] **Step 3.6:** Cấu hình job `release`:
  - Kích hoạt khi có tag `v*` (ví dụ `v2.0.0`).
  - Gom toàn bộ artifacts (DMG, Windows ZIP, Linux Tar.gz).
  - Tạo GitHub Release và đính kèm các tệp cài đặt để người dùng tải trực tiếp.
- [ ] **Step 3.7:** Kiểm tra cú pháp YAML bằng linter hoặc script xác thực.
- [ ] **Step 3.8:** Commit thay đổi: `git commit -m "ci(github-actions): create cross-platform build, test, and release workflow"`.

---

### Task 4: Cập Nhật Tài Liệu, GitNexus Sync & Nghiệm Thu Phát Hành v2.0.0

**Mục tiêu:** Cập nhật tài liệu người dùng, README, sơ đồ thư mục mới, đồng bộ đồ thị tri thức GitNexus và tạo thẻ phát hành chính thức `v2.0.0`.

- [ ] **Step 4.1:** Cập nhật `README.md`:
  - Thêm huy hiệu (Badge) GitHub Actions CI/CD.
  - Cập nhật sơ đồ cấu trúc thư mục mới (với thư mục `legacy/`).
  - Cập nhật hướng dẫn tải bản phát hành đóng gói sẵn (`.dmg`, `.zip`).
  - Chốt trạng thái hoàn thành 100% toàn bộ các giai đoạn phát triển.
- [ ] **Step 4.2:** Cập nhật `docs/user-guide.md` & `docs/packaging-and-deployment.md`:
  - Thêm hình ảnh/mô tả icon ứng dụng.
  - Bổ sung hướng dẫn tải file từ trang GitHub Releases.
- [ ] **Step 4.3:** Chạy toàn bộ 53+ unit tests trên toàn bộ solution:
  `dotnet test Dormitory.sln` -> 100% Pass.
- [ ] **Step 4.4:** Kiểm tra thay đổi đồ thị GitNexus và cập nhật chỉ mục:
  `node .gitnexus/run.cjs detect-changes --scope all --repo .`
  `node .gitnexus/run.cjs analyze --index-only`
- [ ] **Step 4.5:** Commit cập nhật tài liệu:
  `git commit -m "docs: finalize v2.0.0 documentation, architecture codemap, and release notes"`
- [ ] **Step 4.6:** Đẩy toàn bộ lên remote và gắn tag phiên bản `v2.0.0`:
  `git tag -a v2.0.0 -m "Release v2.0.0: Modern Dormitory Management System on .NET 8 & Avalonia UI"`
  `git push origin master --tags`

---

## Tiêu Chí Nghiệm Thu (Acceptance Criteria)

1. **Legacy Cleanup**: Thư mục gốc sạch sẽ, `KTX2021` và `Dormitory_Management_2021.sln` được chuyển an toàn vào `legacy/` mà không làm đứt gãy lịch sử git hay gây lỗi build cho `Dormitory.sln`.
2. **Branding & Icon**: Ứng dụng có icon Ký túc xá trực quan trên thanh tiêu đề cửa sổ (Avalonia), file `.exe` trên Windows và App Bundle `.dmg` trên macOS.
3. **CI/CD Workflow**: File `.github/workflows/ci-cd.yml` chuẩn cú pháp, sẵn sàng kích hoạt kiểm thử và đóng gói tự động khi push code lên GitHub.
4. **Chất lượng**: 100% Unit Tests (53/53+) vượt qua, 0 build warning/error.
5. **Đồng bộ GitNexus**: Đồ thị tri thức phản ánh chính xác cấu trúc repository sau khi hoàn thiện.
