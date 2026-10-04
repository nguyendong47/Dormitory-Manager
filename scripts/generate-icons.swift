import Foundation
import AppKit
import CoreGraphics

func createDormitoryIcon(size: Int) -> NSImage {
    let s = CGFloat(size)
    let img = NSImage(size: NSSize(width: s, height: s))
    img.lockFocus()
    guard let ctx = NSGraphicsContext.current?.cgContext else {
        img.unlockFocus()
        return img
    }

    ctx.setAllowsAntialiasing(true)
    ctx.setShouldAntialias(true)
    ctx.interpolationQuality = .high

    let scale = s / 1024.0

    // MARK: - 1. Background Badge (Modern Fluent Rounded Squircle)
    let margin: CGFloat = 36.0 * scale
    let badgeRect = CGRect(x: margin, y: margin, width: s - 2 * margin, height: s - 2 * margin)
    let cornerRadius: CGFloat = 216.0 * scale
    let badgePath = CGPath(roundedRect: badgeRect, cornerWidth: cornerRadius, cornerHeight: cornerRadius, transform: nil)

    // Ambient drop shadow for the squircle
    ctx.saveGState()
    ctx.setShadow(offset: CGSize(width: 0, height: -22 * scale), blur: 36 * scale, color: NSColor(calibratedRed: 0.0, green: 0.1, blue: 0.25, alpha: 0.40).cgColor)
    ctx.addPath(badgePath)
    ctx.setFillColor(NSColor(calibratedRed: 0.0, green: 0.35, blue: 0.65, alpha: 1.0).cgColor)
    ctx.fillPath()
    ctx.restoreGState()

    // Badge Gradient (Fluent Blue: #1586EC -> #0078D4 -> #004578)
    ctx.saveGState()
    ctx.addPath(badgePath)
    ctx.clip()

    let colorSpace = CGColorSpaceCreateDeviceRGB()
    let bgColors = [
        NSColor(calibratedRed: 0.08, green: 0.54, blue: 0.94, alpha: 1.0).cgColor, // Top vibrant blue
        NSColor(calibratedRed: 0.00, green: 0.47, blue: 0.83, alpha: 1.0).cgColor, // Fluent Blue #0078D4
        NSColor(calibratedRed: 0.00, green: 0.27, blue: 0.50, alpha: 1.0).cgColor  // Deep base #004578
    ] as CFArray
    let bgLocations: [CGFloat] = [0.0, 0.45, 1.0]
    if let bgGradient = CGGradient(colorsSpace: colorSpace, colors: bgColors, locations: bgLocations) {
        ctx.drawLinearGradient(bgGradient,
                               start: CGPoint(x: s * 0.5, y: s - margin),
                               end: CGPoint(x: s * 0.5, y: margin),
                               options: [])
    }

    // Top rim highlight (acrylic edge)
    let rimPath = CGPath(roundedRect: badgeRect.insetBy(dx: 2 * scale, dy: 2 * scale),
                         cornerWidth: cornerRadius - 2 * scale,
                         cornerHeight: cornerRadius - 2 * scale,
                         transform: nil)
    ctx.addPath(rimPath)
    ctx.setStrokeColor(NSColor(calibratedWhite: 1.0, alpha: 0.25).cgColor)
    ctx.setLineWidth(3.5 * scale)
    ctx.strokePath()

    // Subtle warm sunrise glow behind buildings
    let glowColors = [
        NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 0.22).cgColor, // #FFB900 glow
        NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 0.0).cgColor
    ] as CFArray
    if let glowGrad = CGGradient(colorsSpace: colorSpace, colors: glowColors, locations: [0.0, 1.0]) {
        ctx.drawRadialGradient(glowGrad,
                               startCenter: CGPoint(x: s * 0.76, y: s * 0.72),
                               startRadius: 0,
                               endCenter: CGPoint(x: s * 0.76, y: s * 0.72),
                               endRadius: 300 * scale,
                               options: [])
    }

    ctx.restoreGState() // End badge clip

    // MARK: - 2. Dormitory Architecture (Twin Connected Towers + Modern Slanted Roof)
    // Left Main Tower (Tall): 3 columns x 4 rows of rooms
    // Right Tower (Wide): 2 columns x 3 rows of rooms

    // Outer shadow for buildings
    ctx.saveGState()
    ctx.setShadow(offset: CGSize(width: 0, height: -14 * scale), blur: 24 * scale, color: NSColor(calibratedRed: 0.0, green: 0.12, blue: 0.30, alpha: 0.45).cgColor)

    // Left Tower Body
    let leftTowerRect = CGRect(x: 230 * scale, y: 220 * scale, width: 330 * scale, height: 490 * scale)
    let leftTowerPath = CGPath(roundedRect: leftTowerRect, cornerWidth: 20 * scale, cornerHeight: 20 * scale, transform: nil)
    ctx.addPath(leftTowerPath)
    ctx.setFillColor(NSColor(calibratedWhite: 0.98, alpha: 1.0).cgColor)
    ctx.fillPath()

    // Right Tower Body
    let rightTowerRect = CGRect(x: 520 * scale, y: 220 * scale, width: 280 * scale, height: 380 * scale)
    let rightTowerPath = CGPath(roundedRect: rightTowerRect, cornerWidth: 18 * scale, cornerHeight: 18 * scale, transform: nil)
    ctx.addPath(rightTowerPath)
    ctx.setFillColor(NSColor(calibratedWhite: 0.91, alpha: 1.0).cgColor)
    ctx.fillPath()

    // Connecting Base Foundation
    let baseRect = CGRect(x: 210 * scale, y: 200 * scale, width: 604 * scale, height: 50 * scale)
    let basePath = CGPath(roundedRect: baseRect, cornerWidth: 14 * scale, cornerHeight: 14 * scale, transform: nil)
    ctx.addPath(basePath)
    ctx.setFillColor(NSColor(calibratedRed: 0.0, green: 0.25, blue: 0.48, alpha: 1.0).cgColor)
    ctx.fillPath()

    ctx.restoreGState() // End building shadow

    // Tower 1 Facade Gradient (Clean white to soft light silver)
    ctx.saveGState()
    ctx.addPath(leftTowerPath)
    ctx.clip()
    let t1Colors = [
        NSColor(calibratedWhite: 1.0, alpha: 1.0).cgColor,
        NSColor(calibratedWhite: 0.92, alpha: 1.0).cgColor
    ] as CFArray
    if let t1Grad = CGGradient(colorsSpace: colorSpace, colors: t1Colors, locations: [0.0, 1.0]) {
        ctx.drawLinearGradient(t1Grad,
                               start: CGPoint(x: 230 * scale, y: 710 * scale),
                               end: CGPoint(x: 560 * scale, y: 220 * scale),
                               options: [])
    }
    ctx.restoreGState()

    // Tower 2 Facade Gradient (Soft depth shadow)
    ctx.saveGState()
    ctx.addPath(rightTowerPath)
    ctx.clip()
    let t2Colors = [
        NSColor(calibratedWhite: 0.94, alpha: 1.0).cgColor,
        NSColor(calibratedWhite: 0.86, alpha: 1.0).cgColor
    ] as CFArray
    if let t2Grad = CGGradient(colorsSpace: colorSpace, colors: t2Colors, locations: [0.0, 1.0]) {
        ctx.drawLinearGradient(t2Grad,
                               start: CGPoint(x: 520 * scale, y: 600 * scale),
                               end: CGPoint(x: 800 * scale, y: 220 * scale),
                               options: [])
    }
    ctx.restoreGState()

    // MARK: - 3. Modern Roof Canopy & Accents (#FFB900 Warm Gold & Fluent Blue)
    // Left Tower Slanted Roof Peak with Warm Gold #FFB900 Accent
    let roofLeftPath = CGMutablePath()
    roofLeftPath.move(to: CGPoint(x: 205 * scale, y: 700 * scale))
    roofLeftPath.addLine(to: CGPoint(x: 395 * scale, y: 785 * scale))
    roofLeftPath.addLine(to: CGPoint(x: 585 * scale, y: 700 * scale))
    roofLeftPath.addLine(to: CGPoint(x: 585 * scale, y: 680 * scale))
    roofLeftPath.addLine(to: CGPoint(x: 205 * scale, y: 680 * scale))
    roofLeftPath.closeSubpath()

    ctx.saveGState()
    ctx.setShadow(offset: CGSize(width: 0, height: -6 * scale), blur: 14 * scale, color: NSColor(calibratedRed: 0.85, green: 0.45, blue: 0.0, alpha: 0.4).cgColor)
    let goldColors = [
        NSColor(calibratedRed: 1.0, green: 0.84, blue: 0.32, alpha: 1.0).cgColor, // #FFD652
        NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 1.0).cgColor, // #FFB900
        NSColor(calibratedRed: 0.95, green: 0.58, blue: 0.10, alpha: 1.0).cgColor  // #F2941A
    ] as CFArray
    if let goldGrad = CGGradient(colorsSpace: colorSpace, colors: goldColors, locations: [0.0, 0.5, 1.0]) {
        ctx.saveGState()
        ctx.addPath(roofLeftPath)
        ctx.clip()
        ctx.drawLinearGradient(goldGrad,
                               start: CGPoint(x: 395 * scale, y: 785 * scale),
                               end: CGPoint(x: 395 * scale, y: 680 * scale),
                               options: [])
        ctx.restoreGState()
    }
    ctx.restoreGState()

    // Right Tower Roof Accent: Sleek modern Fluent blue canopy with gold top line
    let roofRightRect = CGRect(x: 505 * scale, y: 590 * scale, width: 310 * scale, height: 24 * scale)
    let roofRightPath = CGPath(roundedRect: roofRightRect, cornerWidth: 8 * scale, cornerHeight: 8 * scale, transform: nil)
    ctx.saveGState()
    ctx.setShadow(offset: CGSize(width: 0, height: -4 * scale), blur: 10 * scale, color: NSColor(calibratedRed: 0.0, green: 0.2, blue: 0.4, alpha: 0.3).cgColor)
    ctx.addPath(roofRightPath)
    ctx.setFillColor(NSColor(calibratedRed: 0.0, green: 0.47, blue: 0.83, alpha: 1.0).cgColor)
    ctx.fillPath()

    // Gold trim on top of right roof
    let roofTrim = CGRect(x: 505 * scale, y: 610 * scale, width: 310 * scale, height: 4 * scale)
    ctx.addPath(CGPath(roundedRect: roofTrim, cornerWidth: 2 * scale, cornerHeight: 2 * scale, transform: nil))
    ctx.setFillColor(NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 1.0).cgColor)
    ctx.fillPath()
    ctx.restoreGState()

    // MARK: - 4. Dormitory Windows (Illuminated Rooms Grid)
    let warmWinColor = NSColor(calibratedRed: 1.0, green: 0.76, blue: 0.18, alpha: 1.0).cgColor
    let coolWinColor = NSColor(calibratedRed: 0.0, green: 0.47, blue: 0.83, alpha: 0.85).cgColor
    let softWinColor = NSColor(calibratedRed: 0.35, green: 0.68, blue: 0.95, alpha: 0.85).cgColor

    // Left Tower Windows: 4 rows x 3 columns
    let cols1 = 3
    let rows1 = 4
    let winW1: CGFloat = 62 * scale
    let winH1: CGFloat = 66 * scale
    let gapX1: CGFloat = 34 * scale
    let startX1: CGFloat = 266 * scale
    let startY1: CGFloat = 370 * scale
    let gapY1: CGFloat = 24 * scale

    for r in 0..<rows1 {
        for c in 0..<cols1 {
            let wx = startX1 + CGFloat(c) * (winW1 + gapX1)
            let wy = startY1 + CGFloat(r) * (winH1 + gapY1)
            let wRect = CGRect(x: wx, y: wy, width: winW1, height: winH1)
            let wPath = CGPath(roundedRect: wRect, cornerWidth: 8 * scale, cornerHeight: 8 * scale, transform: nil)

            let isWarm = (r == 0 && c == 1) || (r == 1 && c == 0) || (r == 2 && c == 2) || (r == 3 && c == 1) || (r == 2 && c == 0)
            let winColor = isWarm ? warmWinColor : ( (r + c) % 2 == 0 ? coolWinColor : softWinColor )

            ctx.saveGState()
            if isWarm {
                ctx.setShadow(offset: CGSize(width: 0, height: 0), blur: 9 * scale, color: NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 0.75).cgColor)
            }
            ctx.addPath(wPath)
            ctx.setFillColor(winColor)
            ctx.fillPath()

            // Subtle vertical window divider
            ctx.setStrokeColor(NSColor(calibratedWhite: 1.0, alpha: 0.35).cgColor)
            ctx.setLineWidth(2 * scale)
            ctx.move(to: CGPoint(x: wx + winW1 * 0.5, y: wy))
            ctx.addLine(to: CGPoint(x: wx + winW1 * 0.5, y: wy + winH1))
            ctx.strokePath()

            ctx.restoreGState()
        }
    }

    // Right Tower Windows: 3 rows x 2 columns
    let cols2 = 2
    let rows2 = 3
    let winW2: CGFloat = 68 * scale
    let winH2: CGFloat = 64 * scale
    let gapX2: CGFloat = 36 * scale
    let startX2: CGFloat = 595 * scale
    let startY2: CGFloat = 350 * scale
    let gapY2: CGFloat = 26 * scale

    for r in 0..<rows2 {
        for c in 0..<cols2 {
            let wx = startX2 + CGFloat(c) * (winW2 + gapX2)
            let wy = startY2 + CGFloat(r) * (winH2 + gapY2)
            let wRect = CGRect(x: wx, y: wy, width: winW2, height: winH2)
            let wPath = CGPath(roundedRect: wRect, cornerWidth: 7 * scale, cornerHeight: 7 * scale, transform: nil)

            let isWarm = (r == 0 && c == 0) || (r == 1 && c == 1) || (r == 2 && c == 0)
            let winColor = isWarm ? warmWinColor : softWinColor

            ctx.saveGState()
            if isWarm {
                ctx.setShadow(offset: CGSize(width: 0, height: 0), blur: 9 * scale, color: NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 0.65).cgColor)
            }
            ctx.addPath(wPath)
            ctx.setFillColor(winColor)
            ctx.fillPath()

            // Divider
            ctx.setStrokeColor(NSColor(calibratedWhite: 1.0, alpha: 0.3).cgColor)
            ctx.setLineWidth(2 * scale)
            ctx.move(to: CGPoint(x: wx + winW2 * 0.5, y: wy))
            ctx.addLine(to: CGPoint(x: wx + winW2 * 0.5, y: wy + winH2))
            ctx.strokePath()

            ctx.restoreGState()
        }
    }

    // MARK: - 5. Main Entrance Lobby (Archway Door in Warm Amber)
    let doorRect = CGRect(x: 345 * scale, y: 220 * scale, width: 100 * scale, height: 105 * scale)
    let doorPath = CGMutablePath()
    doorPath.move(to: CGPoint(x: doorRect.minX, y: doorRect.minY))
    doorPath.addLine(to: CGPoint(x: doorRect.minX, y: doorRect.maxY - 20 * scale))
    doorPath.addArc(tangent1End: CGPoint(x: doorRect.minX, y: doorRect.maxY),
                    tangent2End: CGPoint(x: doorRect.midX, y: doorRect.maxY),
                    radius: 20 * scale)
    doorPath.addArc(tangent1End: CGPoint(x: doorRect.maxX, y: doorRect.maxY),
                    tangent2End: CGPoint(x: doorRect.maxX, y: doorRect.maxY - 20 * scale),
                    radius: 20 * scale)
    doorPath.addLine(to: CGPoint(x: doorRect.maxX, y: doorRect.minY))
    doorPath.closeSubpath()

    ctx.saveGState()
    ctx.setShadow(offset: CGSize(width: 0, height: 0), blur: 16 * scale, color: NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 0.85).cgColor)
    ctx.addPath(doorPath)
    ctx.setFillColor(warmWinColor)
    ctx.fillPath()

    // Door center line
    ctx.setStrokeColor(NSColor(calibratedRed: 0.85, green: 0.55, blue: 0.0, alpha: 0.85).cgColor)
    ctx.setLineWidth(3 * scale)
    ctx.move(to: CGPoint(x: doorRect.midX, y: doorRect.minY))
    ctx.addLine(to: CGPoint(x: doorRect.midX, y: doorRect.maxY))
    ctx.strokePath()
    ctx.restoreGState()

    // Entrance Canopy in Fluent Blue
    let canopyRect = CGRect(x: 325 * scale, y: 320 * scale, width: 140 * scale, height: 16 * scale)
    let canopyPath = CGPath(roundedRect: canopyRect, cornerWidth: 6 * scale, cornerHeight: 6 * scale, transform: nil)
    ctx.saveGState()
    ctx.setShadow(offset: CGSize(width: 0, height: -3 * scale), blur: 6 * scale, color: NSColor(calibratedRed: 0, green: 0, blue: 0, alpha: 0.25).cgColor)
    ctx.addPath(canopyPath)
    ctx.setFillColor(NSColor(calibratedRed: 0.0, green: 0.47, blue: 0.83, alpha: 1.0).cgColor)
    ctx.fillPath()
    ctx.restoreGState()

    // MARK: - 6. Stylized Golden Room Key Emblem
    let keyCenterX: CGFloat = 710 * scale
    let keyCenterY: CGFloat = 730 * scale
    let keyHeadRadius: CGFloat = 46 * scale

    ctx.saveGState()
    ctx.setShadow(offset: CGSize(width: 0, height: -6 * scale), blur: 16 * scale, color: NSColor(calibratedRed: 0.0, green: 0.15, blue: 0.35, alpha: 0.45).cgColor)

    // Key loop
    let keyCirclePath = CGMutablePath()
    keyCirclePath.addArc(center: CGPoint(x: keyCenterX, y: keyCenterY),
                         radius: keyHeadRadius,
                         startAngle: 0,
                         endAngle: CGFloat.pi * 2,
                         clockwise: false)
    keyCirclePath.addArc(center: CGPoint(x: keyCenterX, y: keyCenterY),
                         radius: keyHeadRadius * 0.45,
                         startAngle: 0,
                         endAngle: CGFloat.pi * 2,
                         clockwise: true)

    ctx.addPath(keyCirclePath)
    ctx.setFillColor(NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 1.0).cgColor)
    ctx.fillPath(using: .evenOdd)

    // Key stem
    ctx.saveGState()
    ctx.translateBy(x: keyCenterX, y: keyCenterY)
    ctx.rotate(by: -CGFloat.pi * 0.75) // 135 deg

    let stemRect = CGRect(x: 32 * scale, y: -9 * scale, width: 88 * scale, height: 18 * scale)
    let stemPath = CGPath(roundedRect: stemRect, cornerWidth: 5 * scale, cornerHeight: 5 * scale, transform: nil)
    ctx.addPath(stemPath)
    ctx.setFillColor(NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 1.0).cgColor)
    ctx.fillPath()

    // Key teeth
    let tooth1 = CGRect(x: 90 * scale, y: -26 * scale, width: 14 * scale, height: 18 * scale)
    let tooth2 = CGRect(x: 108 * scale, y: -22 * scale, width: 12 * scale, height: 14 * scale)
    ctx.addPath(CGPath(roundedRect: tooth1, cornerWidth: 3 * scale, cornerHeight: 3 * scale, transform: nil))
    ctx.addPath(CGPath(roundedRect: tooth2, cornerWidth: 3 * scale, cornerHeight: 3 * scale, transform: nil))
    ctx.setFillColor(NSColor(calibratedRed: 1.0, green: 0.725, blue: 0.0, alpha: 1.0).cgColor)
    ctx.fillPath()

    ctx.restoreGState()
    ctx.restoreGState() // End key shadow

    img.unlockFocus()
    return img
}

func getPNGData(image: NSImage, pixelSize: Int) -> Data? {
    guard let rep = NSBitmapImageRep(
        bitmapDataPlanes: nil,
        pixelsWide: pixelSize,
        pixelsHigh: pixelSize,
        bitsPerSample: 8,
        samplesPerPixel: 4,
        hasAlpha: true,
        isPlanar: false,
        colorSpaceName: .deviceRGB,
        bytesPerRow: 0,
        bitsPerPixel: 0
    ) else { return nil }

    rep.size = NSSize(width: pixelSize, height: pixelSize)
    NSGraphicsContext.saveGraphicsState()
    NSGraphicsContext.current = NSGraphicsContext(bitmapImageRep: rep)
    image.draw(in: NSRect(x: 0, y: 0, width: pixelSize, height: pixelSize),
               from: NSRect(origin: .zero, size: image.size),
               operation: .copy,
               fraction: 1.0)
    NSGraphicsContext.restoreGraphicsState()

    return rep.representation(using: .png, properties: [:])
}

func buildIcoData(pngEntries: [(size: Int, data: Data)]) -> Data {
    var ico = Data()
    let count = UInt16(pngEntries.count)

    var reserved: UInt16 = 0
    var type: UInt16 = 1
    var countLE = count.littleEndian

    ico.append(Data(bytes: &reserved, count: 2))
    ico.append(Data(bytes: &type, count: 2))
    ico.append(Data(bytes: &countLE, count: 2))

    var offset = UInt32(6 + 16 * pngEntries.count)
    for entry in pngEntries {
        var w: UInt8 = entry.size >= 256 ? 0 : UInt8(entry.size)
        var h: UInt8 = entry.size >= 256 ? 0 : UInt8(entry.size)
        var colorCount: UInt8 = 0
        var bReserved: UInt8 = 0
        var planes: UInt16 = 1
        var bitCount: UInt16 = 32
        var bytesInRes = UInt32(entry.data.count).littleEndian
        var imageOffset = offset.littleEndian

        ico.append(Data(bytes: &w, count: 1))
        ico.append(Data(bytes: &h, count: 1))
        ico.append(Data(bytes: &colorCount, count: 1))
        ico.append(Data(bytes: &bReserved, count: 1))
        ico.append(Data(bytes: &planes, count: 2))
        ico.append(Data(bytes: &bitCount, count: 2))
        ico.append(Data(bytes: &bytesInRes, count: 4))
        ico.append(Data(bytes: &imageOffset, count: 4))

        offset += UInt32(entry.data.count)
    }

    for entry in pngEntries {
        ico.append(entry.data)
    }

    return ico
}

// -------------------------------------------------------------
// Generation Pipeline
// -------------------------------------------------------------
let fileManager = FileManager.default
let currentDir = fileManager.currentDirectoryPath

let assetsDir = "\(currentDir)/src/Dormitory.Desktop/Assets"
try? fileManager.createDirectory(atPath: assetsDir, withIntermediateDirectories: true, attributes: nil)

let packageDir = "\(currentDir)/scripts/package"
try? fileManager.createDirectory(atPath: packageDir, withIntermediateDirectories: true, attributes: nil)

print("🎨 Đang kết xuất hình ảnh gốc vector độ phân giải cao 1024x1024...")
let masterImage = createDormitoryIcon(size: 1024)

// 1. Lưu AppIcon.png (512x512)
let png512Path = "\(assetsDir)/AppIcon.png"
if let data512 = getPNGData(image: masterImage, pixelSize: 512) {
    try data512.write(to: URL(fileURLWithPath: png512Path))
    print("✅ Đã tạo AppIcon.png (512x512) tại: \(png512Path)")
} else {
    print("❌ Lỗi: Không thể kết xuất AppIcon.png")
    exit(1)
}

// 2. Tạo AppIcon.ico đa độ phân giải (16, 24, 32, 48, 64, 128, 256)
print("🎨 Đang sinh AppIcon.ico đa kích thước...")
let icoSizes = [16, 24, 32, 48, 64, 128, 256]
var icoEntries: [(size: Int, data: Data)] = []
for sz in icoSizes {
    if let data = getPNGData(image: masterImage, pixelSize: sz) {
        icoEntries.append((size: sz, data: data))
    }
}
let icoData = buildIcoData(pngEntries: icoEntries)
let icoPath = "\(assetsDir)/AppIcon.ico"
try icoData.write(to: URL(fileURLWithPath: icoPath))
print("✅ Đã tạo AppIcon.ico chứa [\(icoSizes.map { String($0) }.joined(separator: ", "))] tại: \(icoPath)")

// 3. Tạo thư mục tạm AppIcon.iconset cho macOS
print("🍎 Đang chuẩn bị iconset cho macOS...")
let iconsetDir = "\(packageDir)/AppIcon.iconset"
try? fileManager.removeItem(atPath: iconsetDir)
try fileManager.createDirectory(atPath: iconsetDir, withIntermediateDirectories: true, attributes: nil)

let iconsetConfigs: [(name: String, pixelSize: Int)] = [
    ("icon_16x16.png", 16),
    ("icon_16x16@2x.png", 32),
    ("icon_32x32.png", 32),
    ("icon_32x32@2x.png", 64),
    ("icon_128x128.png", 128),
    ("icon_128x128@2x.png", 256),
    ("icon_256x256.png", 256),
    ("icon_256x256@2x.png", 512),
    ("icon_512x512.png", 512),
    ("icon_512x512@2x.png", 1024)
]

for item in iconsetConfigs {
    if let data = getPNGData(image: masterImage, pixelSize: item.pixelSize) {
        let destPath = "\(iconsetDir)/\(item.name)"
        try data.write(to: URL(fileURLWithPath: destPath))
    }
}

// 4. Chuyển đổi iconset sang AppIcon.icns bằng iconutil
let icnsPath = "\(packageDir)/AppIcon.icns"
let iconutilProcess = Process()
iconutilProcess.executableURL = URL(fileURLWithPath: "/usr/bin/iconutil")
iconutilProcess.arguments = ["-c", "icns", iconsetDir, "-o", icnsPath]
try iconutilProcess.run()
iconutilProcess.waitUntilExit()

if iconutilProcess.terminationStatus == 0 {
    print("✅ Đã tạo thành công AppIcon.icns tại: \(icnsPath)")
} else {
    print("❌ Lỗi khi chạy iconutil, mã lỗi: \(iconutilProcess.terminationStatus)")
    exit(1)
}

// 5. Xóa thư mục iconset tạm
try? fileManager.removeItem(atPath: iconsetDir)
print("🧹 Đã dọn dẹp thư mục tạm AppIcon.iconset")

print("✨ Hoàn tất tạo toàn bộ bộ nhận diện ứng dụng (PNG, ICO, ICNS)!")
