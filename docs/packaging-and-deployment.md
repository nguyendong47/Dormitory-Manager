# Hướng Dẫn Đóng Gói Và Triển Khai Đa Nền Tảng (Cross-Platform Packaging & Deployment)

Tài liệu này cung cấp hướng dẫn toàn diện dành cho Quản trị viên Ký túc xá và Lập trình viên để đóng gói, phân phối và triển khai ứng dụng **Dormitory Manager v2.0.0** trên ba nền tảng hệ điều hành: **macOS**, **Windows** và **Linux**.

---

## 1. Tổng Quan Kiến Trúc Phát Hành

Ứng dụng **Dormitory Manager** được xây dựng trên nền tảng:
- **.NET 8 LTS**: Nền tảng thực thi hiện đại, hiệu năng cao và hỗ trợ dài hạn của Microsoft.
- **Avalonia UI 11.2.5**: Khung giao diện đa nền tảng kết xuất đồ họa trực tiếp qua SkiaSharp.
- **SQLite & EF Core 8**: Cơ sở dữ liệu nhúng cục bộ độc lập, lưu trữ dữ liệu tại file `dormitory.db`.

---

## 2. So Sánh Cơ Chế Triển Khai: Self-Contained vs Framework-Dependent

| Tiêu chí | Self-Contained Deployment (SCD) *(Lựa chọn của dự án)* | Framework-Dependent Deployment (FDD) |
| :--- | :--- | :--- |
| **Yêu cầu máy đích** | **Không cần** cài đặt .NET Runtime hay SDK. | **Bắt buộc** người dùng phải tự cài .NET 8 Desktop Runtime tương ứng trước. |
| **Kích thước gói** | Lớn hơn (~90MB - 120MB) do đóng gói kèm CoreCLR, BCL và native binaries. | Nhỏ gọn (~10MB - 30MB) chỉ chứa mã ứng dụng và thư viện quản lý. |
| **Độ độc lập & Tính ổn định** | Ứng dụng chạy trong môi trường Runtime cô lập riêng biệt, không bị ảnh hưởng bởi cập nhật hay xung đột phiên bản .NET trên máy người dùng. | Phụ thuộc vào phiên bản .NET được cài đặt trên máy người dùng; có thể lỗi nếu người dùng cài thiếu phiên bản. |
| **Native Binaries** | Đóng gói sẵn toàn bộ native dylib/so/dll của **SkiaSharp** và **SQLite** tương ứng với từng hệ điều hành và vi kiến trúc CPU. | Cần cơ chế tự động giải nén hoặc tìm kiếm native assets trên hệ thống. |
| **Trải nghiệm người dùng cuối** | **Xuất sắc**: Tải về và chạy ngay (Zero Setup), phù hợp cho nhà trường và văn phòng ban quản lý. | Phức tạp: Phải mở trang chủ Microsoft tải và cài .NET Runtime trước khi chạy. |

> **Quyết định thiết kế:** Dự án lựa chọn **Self-Contained Deployment (SCD)** làm tiêu chuẩn phát hành chính thức để đảm bảo người dùng cuối có trải nghiệm cài đặt và chạy mượt mà nhất mà không cần kiến thức kỹ thuật.

---

## 3. Cấu Trúc Thư Mục Xuất Bản (Build Artifacts)

Sau khi chạy các kịch bản đóng gói, các gói thành phẩm sẽ được lưu tại thư mục `dist/`:

```
Dormitory-Manager/
├── dist/
│   ├── macos-arm64/                              # macOS Apple Silicon (M1/M2/M3/M4)
│   │   ├── publish/                              # Tệp nhị phân thô
│   │   ├── DormitoryManager.app/                 # macOS App Bundle chuẩn
│   │   └── DormitoryManager-v2.0.0-macOS-arm64.dmg # Tệp ảnh đĩa cài đặt DMG
│   ├── macos-x64/                                # macOS Intel x86_64
│   │   ├── DormitoryManager.app/
│   │   └── DormitoryManager-v2.0.0-macOS-x64.dmg
│   ├── windows-x64/                              # Windows 64-bit (x64)
│   │   ├── Dormitory.Desktop.exe                 # Tệp chạy đơn (Single-File Executable)
│   │   ├── appsettings.json                      # Cấu hình hệ thống
│   │   └── DormitoryManager-v2.0.0-Windows-x64.zip # Tệp nén ZIP phân phối
│   └── linux-x64/                                # Linux 64-bit (x64)
│       ├── Dormitory.Desktop                     # Tệp chạy nhị phân Linux
│       ├── appsettings.json                      # Cấu hình hệ thống
│       ├── *.so                                  # Native libraries (libSkiaSharp, libe_sqlite3)
│       └── DormitoryManager-v2.0.0-Linux-x64.tar.gz # Tệp nén lưu trữ TAR.GZ
```

---

## 4. Hướng Dẫn Chạy Kịch Bản Đóng Gói (Dành Cho Lập Trình Viên)

Tất cả các script đóng gói tự động được đặt tại `scripts/package/`.

### 4.1. Đóng gói cho macOS (`build-macos.sh`)

Script tự động nhận diện vi kiến trúc máy Mac đang chạy (`arm64` hoặc `x64`), biên dịch `dotnet publish`, dựng cấu trúc `DormitoryManager.app` và đóng gói thành tệp `.dmg`.

```bash
# Cấp quyền thực thi nếu cần
chmod +x scripts/package/build-macos.sh

# Chạy tự động (tự nhận diện chip Apple Silicon hoặc Intel)
./scripts/package/build-macos.sh

# Hoặc chỉ định rõ kiến trúc đích:
./scripts/package/build-macos.sh arm64   # Dành cho Apple Silicon (M1/M2/M3/M4)
./scripts/package/build-macos.sh x64     # Dành cho Mac chạy chip Intel
```

**Chi tiết kỹ thuật App Bundle:**
- `Contents/MacOS/`: Chứa file thực thi `Dormitory.Desktop`, các assembly .NET và dynamic library (`libSkiaSharp.dylib`, `libe_sqlite3.dylib`).
- `Contents/Info.plist`: Cấu hình nhận diện `vn.edu.dormitory.manager`, kích hoạt Retina HiDPI (`NSHighResolutionCapable = true`), tên hiển thị `Dormitory Manager`.
- `Contents/Resources/`: Chứa tài nguyên icon và giao diện.

### 4.2. Đóng gói cho Windows (`build-windows.sh`)

Script biên dịch ứng dụng thành tệp đơn thực thi `.exe` độc lập (`Single-File Executable`) tích hợp sẵn runtime và các thư viện trích xuất tự động:

```bash
# Cấp quyền thực thi
chmod +x scripts/package/build-windows.sh

# Thực hiện đóng gói win-x64
./scripts/package/build-windows.sh
```

Kết quả:
- `dist/windows-x64/Dormitory.Desktop.exe`: File chạy trực tiếp trên Windows 10/11 64-bit.
- `dist/windows-x64/DormitoryManager-v2.0.0-Windows-x64.zip`: Gói nén ZIP sẵn sàng gửi cho người dùng.

### 4.3. Đóng gói cho Linux (`build-linux.sh`)

Script xuất bản nhị phân self-contained cho các bản phân phối Linux x86_64 (Ubuntu, Debian, Fedora, Arch...):

```bash
# Cấp quyền thực thi
chmod +x scripts/package/build-linux.sh

# Thực hiện đóng gói linux-x64
./scripts/package/build-linux.sh
```

Kết quả:
- `dist/linux-x64/Dormitory.Desktop`: Tệp nhị phân có quyền thực thi `+x`.
- `dist/linux-x64/DormitoryManager-v2.0.0-Linux-x64.tar.gz`: Gói nén chứa đầy đủ file chạy và các thư viện C/C++ cần thiết.

---

## 5. Hướng Dẫn Cài Đặt Và Sử Dụng (Dành Cho Người Dùng Cuối)

### 5.1. Cài đặt trên macOS

1. Tải về file `DormitoryManager-v2.0.0-macOS-arm64.dmg` (cho Apple Silicon) hoặc `macOS-x64.dmg` (cho Intel).
2. Nhấp đúp vào file `.dmg` để mở ảnh đĩa.
3. Trong cửa sổ Finder xuất hiện, kéo thả biểu tượng **DormitoryManager.app** vào biểu tượng thư mục **Applications** (Ứng dụng).
4. Mở ứng dụng từ **Launchpad** hoặc **Applications**.

> **Lưu ý về bảo mật macOS Gatekeeper (Ứng dụng chưa ký số Developer ID):**
> Trong trường hợp macOS thông báo: *"DormitoryManager không thể mở vì nhà phát triển không thể được xác minh"*:
> 1. Mở **System Settings** > **Privacy & Security** (Quyền riêng tư & Bảo mật).
> 2. Cuộn xuống phần *Security*, bạn sẽ thấy thông báo về DormitoryManager > Nhấn **Open Anyway** (Vẫn mở).
> 3. Hoặc mở Terminal và chạy lệnh gỡ thuộc tính cách ly:
>    ```bash
>    xattr -cr /Applications/DormitoryManager.app
>    ```

### 5.2. Cài đặt trên Windows

1. Tải về file `DormitoryManager-v2.0.0-Windows-x64.zip`.
2. Nhấp chuột phải vào file `.zip`, chọn **Extract All...** (Giải nén tất cả) vào thư mục mong muốn (ví dụ: `C:\DormitoryManager`).
3. Mở thư mục vừa giải nén và nhấp đúp vào **`Dormitory.Desktop.exe`** để khởi chạy chương trình.
4. *(Tùy chọn)* Nhấp chuột phải vào `Dormitory.Desktop.exe` > **Send to** > **Desktop (create shortcut)** để tạo lối tắt trên màn hình nền.

> **Lưu ý về Windows Defender SmartScreen:**
> Nếu xuất hiện hộp thoại *"Windows protected your PC"*:
> 1. Nhấn vào dòng chữ **More info** (Thêm thông tin).
> 2. Nhấn nút **Run anyway** (Vẫn chạy).

### 5.3. Cài đặt trên Linux

1. Tải về file `DormitoryManager-v2.0.0-Linux-x64.tar.gz`.
2. Mở Terminal và giải nén tệp:
   ```bash
   tar -xzf DormitoryManager-v2.0.0-Linux-x64.tar.gz -C ~/DormitoryManager
   cd ~/DormitoryManager
   ```
3. Cấp quyền thực thi và khởi chạy:
   ```bash
   chmod +x Dormitory.Desktop
   ./Dormitory.Desktop
   ```
4. Để tạo biểu tượng khởi chạy trên Desktop (GNOME/KDE), tạo file `~/.local/share/applications/dormitory-manager.desktop`:
   ```ini
   [Desktop Entry]
   Name=Dormitory Manager
   Comment=Hệ thống Quản lý Ký túc xá Sinh viên
   Exec=/home/YOUR_USERNAME/DormitoryManager/Dormitory.Desktop
   Path=/home/YOUR_USERNAME/DormitoryManager/
   Terminal=false
   Type=Application
   Categories=Office;Education;
   ```

---

## 6. Quản Trị Cơ Sở Dữ Liệu Và Cấu Hình Khi Nâng Cấp

### 6.1. Tệp cấu hình `appsettings.json`
Tệp cấu hình nằm cùng thư mục với file thực thi:
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

### 6.2. Vị trí tệp Cơ sở dữ liệu SQLite
Mặc định CSDL được lưu trữ tại file **`dormitory.db`** cùng cấp thư mục với ứng dụng.

### 6.3. Quy trình nâng cấp phiên bản an toàn
1. Mở phần mềm Dormitory Manager phiên bản hiện tại.
2. Vào **Cài đặt hệ thống** > chọn **Sao lưu CSDL** để xuất file backup dạng `.db` hoặc `.bak` ra vị trí an toàn.
3. Giải nén/cài đặt phiên bản mới.
4. Copy file `dormitory.db` cũ hoặc vào phần mềm mới chọn **Phục hồi CSDL** để tải lại dữ liệu.

---

## 7. Mẫu Tự Động Hóa CI/CD (GitHub Actions)

Dưới đây là workflow tham khảo `.github/workflows/release.yml` để tự động build và xuất bản các gói phát hành khi tạo Git Tag:

```yaml
name: Release Dormitory Manager

on:
  push:
    tags:
      - 'v*'

jobs:
  build-and-release:
    strategy:
      matrix:
        include:
          - os: macos-latest
            script: ./scripts/package/build-macos.sh
            artifact: dist/macos-*/DormitoryManager-*.dmg
          - os: windows-latest
            script: bash scripts/package/build-windows.sh
            artifact: dist/windows-x64/DormitoryManager-*.zip
          - os: ubuntu-latest
            script: bash scripts/package/build-linux.sh
            artifact: dist/linux-x64/DormitoryManager-*.tar.gz

    runs-on: ${{ matrix.os }}

    steps:
      - name: Checkout mã nguồn
        uses: actions/checkout@v4

      - name: Cài đặt .NET 8 SDK
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Đóng gói ứng dụng
        run: |
          chmod +x scripts/package/*.sh
          ${{ matrix.script }}

      - name: Tải lên Artifact
        uses: actions/upload-artifact@v4
        with:
          name: package-${{ matrix.os }}
          path: ${{ matrix.artifact }}
```
