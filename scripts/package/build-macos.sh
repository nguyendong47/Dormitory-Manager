#!/usr/bin/env bash
# ==============================================================================
# Dormitory Manager - Script đóng gói cho macOS (Apple Silicon arm64 & Intel x64)
# Phiên bản: 2.0.0
# Tự động tạo .app bundle chuẩn macOS và tệp đĩa cài đặt .dmg
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

# Tự động nhận diện kiến trúc nếu không chỉ định tham số
ARCH="${1:-}"
if [ -z "$ARCH" ]; then
    RAW_ARCH="$(uname -m)"
    case "$RAW_ARCH" in
        arm64|aarch64)
            ARCH="arm64"
            ;;
        x86_64|amd64)
            ARCH="x64"
            ;;
        *)
            ARCH="arm64"
            ;;
    esac
fi

# Chuẩn hóa giá trị kiến trúc
if [ "$ARCH" = "x86_64" ] || [ "$ARCH" = "amd64" ]; then
    ARCH="x64"
elif [ "$ARCH" = "aarch64" ]; then
    ARCH="arm64"
fi

RID="osx-${ARCH}"
DIST_DIR="$REPO_ROOT/dist/macos-${ARCH}"
PUBLISH_DIR="$DIST_DIR/publish"
APP_BUNDLE="$DIST_DIR/DormitoryManager.app"
DMG_FILE="$DIST_DIR/DormitoryManager-v2.0.0-macOS-${ARCH}.dmg"

echo "======================================================================"
echo "🚀 [macOS Packaging] Bắt đầu đóng gói Dormitory Manager"
echo "   Kiến trúc mục tiêu : $ARCH (Runtime Identifier: $RID)"
echo "   Chế độ phát hành   : Self-Contained (PublishSingleFile=false)"
echo "   Thư mục đích       : $DIST_DIR"
echo "======================================================================"

# 1. Dọn dẹp thư mục dist trước khi build
rm -rf "$PUBLISH_DIR" "$APP_BUNDLE"

# 2. Thực hiện dotnet publish
echo "🔨 [1/4] Đang biên dịch và xuất bản ứng dụng bằng dotnet publish..."
dotnet publish "$REPO_ROOT/src/Dormitory.Desktop/Dormitory.Desktop.csproj" \
    -c Release \
    -r "$RID" \
    --self-contained true \
    -p:PublishSingleFile=false \
    -o "$PUBLISH_DIR"

if [ ! -f "$PUBLISH_DIR/Dormitory.Desktop" ]; then
    echo "❌ Lỗi: Không tìm thấy tệp nhị phân 'Dormitory.Desktop' trong $PUBLISH_DIR" >&2
    exit 1
fi

# 3. Tạo cấu trúc macOS App Bundle
echo "📂 [2/4] Đang tạo cấu trúc macOS App Bundle ($APP_BUNDLE)..."
mkdir -p "$APP_BUNDLE/Contents/MacOS"
mkdir -p "$APP_BUNDLE/Contents/Resources"

# Tạo file Info.plist chuẩn mực
cat << 'EOF' > "$APP_BUNDLE/Contents/Info.plist"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>DormitoryManager</string>
    <key>CFBundleDisplayName</key>
    <string>Dormitory Manager</string>
    <key>CFBundleIdentifier</key>
    <string>vn.edu.dormitory.manager</string>
    <key>CFBundleVersion</key>
    <string>2.0.0</string>
    <key>CFBundleShortVersionString</key>
    <string>2.0.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>????</string>
    <key>CFBundleExecutable</key>
    <string>Dormitory.Desktop</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHumanReadableCopyright</key>
    <string>Copyright © 2026 Dormitory Manager Team. All rights reserved.</string>
</dict>
</plist>
EOF

# Sao chép toàn bộ tệp xuất bản vào Contents/MacOS
echo "📋 [3/4] Sao chép tệp thực thi và thư viện phụ thuộc vào App Bundle..."
cp -R "$PUBLISH_DIR/"* "$APP_BUNDLE/Contents/MacOS/"
chmod +x "$APP_BUNDLE/Contents/MacOS/Dormitory.Desktop"

echo "✅ App Bundle đã được tạo thành công tại: $APP_BUNDLE"

# 4. Tạo tệp đĩa DMG nếu có công cụ hdiutil
if command -v hdiutil &> /dev/null; then
    echo "📦 [4/4] Đang tạo tệp ảnh đĩa DMG cài đặt ($DMG_FILE)..."
    STAGING_DIR="$DIST_DIR/dmg_staging"
    rm -rf "$STAGING_DIR" "$DMG_FILE"
    mkdir -p "$STAGING_DIR"

    # Sao chép App Bundle vào thư mục tạm staging
    cp -R "$APP_BUNDLE" "$STAGING_DIR/"

    # Tạo symlink Applications để người dùng kéo-thả cài đặt thuận tiện
    ln -s /Applications "$STAGING_DIR/Applications"

    hdiutil create \
        -volname "Dormitory Manager" \
        -srcfolder "$STAGING_DIR" \
        -ov \
        -format UDZO \
        "$DMG_FILE"

    rm -rf "$STAGING_DIR"
    echo "🎉 Đã tạo thành công tệp DMG: $DMG_FILE"
else
    echo "ℹ️ Bỏ qua tạo DMG: Lệnh 'hdiutil' không khả dụng (môi trường không phải macOS)."
fi

echo "======================================================================"
echo "✨ [HOÀN TẤT] Quá trình đóng gói cho macOS-$ARCH đã thành công!"
echo "   App Bundle : $APP_BUNDLE"
if [ -f "$DMG_FILE" ]; then
    echo "   DMG Disk   : $DMG_FILE"
fi
echo "======================================================================"
