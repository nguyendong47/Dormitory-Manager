#!/usr/bin/env bash
# ==============================================================================
# Dormitory Manager - Script đóng gói cho Windows (win-x64)
# Phiên bản: 2.0.0
# Tự động xuất bản Single-File Executable và nén tệp .zip phân phối
# ==============================================================================

set -euo pipefail

# Xác định thư mục gốc của repository
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

# Thiết lập đường dẫn dotnet nếu chưa có trong PATH
if ! command -v dotnet &> /dev/null; then
    if [ -f "$HOME/.dotnet/dotnet" ]; then
        export PATH="$HOME/.dotnet:$PATH"
    fi
fi

if ! command -v dotnet &> /dev/null; then
    echo "❌ Lỗi: Không tìm thấy 'dotnet' SDK trong hệ thống. Vui lòng cài đặt .NET 8 SDK." >&2
    exit 1
fi

RID="${1:-win-x64}"
DIST_DIR="$REPO_ROOT/dist/windows-x64"
ZIP_FILE="$DIST_DIR/DormitoryManager-v2.0.0-Windows-x64.zip"

echo "======================================================================"
echo "🚀 [Windows Packaging] Bắt đầu đóng gói Dormitory Manager"
echo "   Kiến trúc mục tiêu : $RID"
echo "   Chế độ phát hành   : Self-Contained Single-File (PublishSingleFile=true)"
echo "   Thư mục đích       : $DIST_DIR"
echo "======================================================================"

# 1. Dọn dẹp thư mục dist trước khi build
rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"

# 2. Thực hiện dotnet publish
echo "🔨 [1/3] Đang biên dịch và xuất bản ứng dụng cho Windows..."
dotnet publish "$REPO_ROOT/src/Dormitory.Desktop/Dormitory.Desktop.csproj" \
    -c Release \
    -r "$RID" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -o "$DIST_DIR"

EXE_FILE="$DIST_DIR/Dormitory.Desktop.exe"
if [ ! -f "$EXE_FILE" ]; then
    echo "❌ Lỗi: Không tìm thấy tệp 'Dormitory.Desktop.exe' trong $DIST_DIR" >&2
    exit 1
fi

echo "✅ Tệp thực thi Windows đã sẵn sàng: $EXE_FILE"

# Sao chép cơ sở dữ liệu mẫu ban đầu nếu có vào gói phát hành
if [ -f "$REPO_ROOT/dormitory.db" ]; then
    echo "📦 Sao chép cơ sở dữ liệu mẫu 'dormitory.db' vào gói phát hành..."
    cp "$REPO_ROOT/dormitory.db" "$DIST_DIR/dormitory.db"
fi

# 3. Tạo tệp nén ZIP nếu có công cụ zip
if command -v zip &> /dev/null; then
    echo "📦 [2/3] Đang tạo tệp nén ZIP phân phối ($ZIP_FILE)..."
    rm -f "$ZIP_FILE"
    (
        cd "$DIST_DIR"
        zip -q -9 "DormitoryManager-v2.0.0-Windows-x64.zip" Dormitory.Desktop.exe appsettings.json
    )
    echo "🎉 Đã tạo thành công tệp ZIP: $ZIP_FILE"
else
    echo "ℹ️ Bỏ qua tạo ZIP: Lệnh 'zip' không khả dụng."
fi

echo "======================================================================"
echo "✨ [HOÀN TẤT] Quá trình đóng gói cho Windows ($RID) đã thành công!"
echo "   Tệp thực thi: $EXE_FILE"
if [ -f "$ZIP_FILE" ]; then
    echo "   Gói cài đặt : $ZIP_FILE"
fi
echo "======================================================================"
