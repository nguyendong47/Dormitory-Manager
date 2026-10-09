#!/usr/bin/env bash
# ==============================================================================
# Dormitory Manager - Script đóng gói cho Linux (linux-x64)
# Phiên bản: 2.0.0
# Tự động xuất bản Self-Contained Binary và nén tệp .tar.gz phân phối
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

RID="${1:-linux-x64}"
DIST_DIR="$REPO_ROOT/dist/linux-x64"
VERSION="${VERSION:-2.4.0}"
TAR_FILE="$DIST_DIR/DormitoryManager-v${VERSION}-Linux-x64.tar.gz"

echo "======================================================================"
echo "🚀 [Linux Packaging] Bắt đầu đóng gói Dormitory Manager"
echo "   Kiến trúc mục tiêu : $RID"
echo "   Chế độ phát hành   : Self-Contained (PublishSingleFile=true)"
echo "   Thư mục đích       : $DIST_DIR"
echo "======================================================================"

# 1. Dọn dẹp thư mục dist trước khi build
rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"

# 2. Thực hiện dotnet publish
echo "🔨 [1/3] Đang biên dịch và xuất bản ứng dụng cho Linux..."
dotnet publish "$REPO_ROOT/src/Dormitory.Desktop/Dormitory.Desktop.csproj" \
    -c Release \
    -r "$RID" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -o "$DIST_DIR"

BIN_FILE="$DIST_DIR/Dormitory.Desktop"
if [ ! -f "$BIN_FILE" ]; then
    echo "❌ Lỗi: Không tìm thấy tệp nhị phân 'Dormitory.Desktop' trong $DIST_DIR" >&2
    exit 1
fi

# Cấp quyền thực thi
chmod +x "$BIN_FILE"
echo "✅ Tệp thực thi Linux đã sẵn sàng: $BIN_FILE"

# Sao chép cơ sở dữ liệu mẫu ban đầu nếu có vào gói phát hành
if [ -f "$REPO_ROOT/dormitory.db" ]; then
    echo "📦 Sao chép cơ sở dữ liệu mẫu 'dormitory.db' vào gói phát hành..."
    cp "$REPO_ROOT/dormitory.db" "$DIST_DIR/dormitory.db"
fi

# 3. Tạo tệp nén TAR.GZ nếu có công cụ tar
if command -v tar &> /dev/null; then
    echo "📦 [2/3] Đang tạo tệp lưu trữ TAR.GZ phân phối ($TAR_FILE)..."
    rm -f "$TAR_FILE"
    (
        cd "$DIST_DIR"
        # Nén file thực thi, appsettings.json và các thư viện .so native
        tar -czf "DormitoryManager-v${VERSION}-Linux-x64.tar.gz" \
            Dormitory.Desktop \
            appsettings.json \
            *.so 2>/dev/null || tar -czf "DormitoryManager-v${VERSION}-Linux-x64.tar.gz" Dormitory.Desktop appsettings.json
    )
    echo "🎉 Đã tạo thành công tệp TAR.GZ: $TAR_FILE"
else
    echo "ℹ️ Bỏ qua tạo TAR.GZ: Lệnh 'tar' không khả dụng."
fi

echo "======================================================================"
echo "✨ [HOÀN TẤT] Quá trình đóng gói cho Linux ($RID) đã thành công!"
echo "   Tệp thực thi: $BIN_FILE"
if [ -f "$TAR_FILE" ]; then
    echo "   Gói cài đặt : $TAR_FILE"
fi
echo "======================================================================"
