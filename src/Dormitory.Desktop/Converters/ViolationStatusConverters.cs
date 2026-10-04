using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.Converters;

/// <summary>
/// Chuyển đổi mức độ nghiêm trọng vi phạm sang màu nền badge tương ứng
/// </summary>
public class ViolationSeverityToBadgeBackgroundConverter : IValueConverter
{
    public static readonly ViolationSeverityToBadgeBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ViolationSeverity severity)
        {
            return severity switch
            {
                ViolationSeverity.Minor => Brush.Parse("#E7F4E8"),     // Xanh lá nhạt
                ViolationSeverity.Moderate => Brush.Parse("#FFF4CE"),  // Vàng nhạt
                ViolationSeverity.Severe => Brush.Parse("#FDE7E9"),    // Đỏ nhạt
                ViolationSeverity.Critical => Brush.Parse("#4A154B"),  // Tím đậm / Báo động
                _ => Brush.Parse("#F3F2F1")
            };
        }
        return Brush.Parse("#F3F2F1");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Chuyển đổi mức độ nghiêm trọng vi phạm sang màu chữ badge tương ứng
/// </summary>
public class ViolationSeverityToBadgeForegroundConverter : IValueConverter
{
    public static readonly ViolationSeverityToBadgeForegroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ViolationSeverity severity)
        {
            return severity switch
            {
                ViolationSeverity.Minor => Brush.Parse("#107C41"),     // Xanh lá đậm
                ViolationSeverity.Moderate => Brush.Parse("#795B00"),  // Vàng nâu đậm
                ViolationSeverity.Severe => Brush.Parse("#A80000"),    // Đỏ đậm
                ViolationSeverity.Critical => Brush.Parse("#FFFFFF"),  // Chữ trắng nổi bật
                _ => Brush.Parse("#323130")
            };
        }
        return Brush.Parse("#323130");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Chuyển đổi mức độ nghiêm trọng vi phạm sang màu viền badge tương ứng
/// </summary>
public class ViolationSeverityToBadgeBorderConverter : IValueConverter
{
    public static readonly ViolationSeverityToBadgeBorderConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ViolationSeverity severity)
        {
            return severity switch
            {
                ViolationSeverity.Minor => Brush.Parse("#8AD49A"),
                ViolationSeverity.Moderate => Brush.Parse("#FFC83B"),
                ViolationSeverity.Severe => Brush.Parse("#F1707B"),
                ViolationSeverity.Critical => Brush.Parse("#750B1C"),
                _ => Brush.Parse("#C8C6C4")
            };
        }
        return Brush.Parse("#C8C6C4");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Chuyển đổi trạng thái xử lý vi phạm sang màu nền badge tương ứng
/// </summary>
public class ViolationStatusToBadgeBackgroundConverter : IValueConverter
{
    public static readonly ViolationStatusToBadgeBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ViolationStatus status)
        {
            return status switch
            {
                ViolationStatus.Pending => Brush.Parse("#FFF4CE"),    // Chờ xử lý: Vàng cam nhạt
                ViolationStatus.Resolved => Brush.Parse("#E7F4E8"),   // Đã xử lý: Xanh lá nhạt
                ViolationStatus.Dismissed => Brush.Parse("#F3F2F1"),  // Hủy bỏ: Xám nhạt
                _ => Brush.Parse("#F3F2F1")
            };
        }
        return Brush.Parse("#F3F2F1");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Chuyển đổi trạng thái xử lý vi phạm sang màu chữ badge tương ứng
/// </summary>
public class ViolationStatusToBadgeForegroundConverter : IValueConverter
{
    public static readonly ViolationStatusToBadgeForegroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ViolationStatus status)
        {
            return status switch
            {
                ViolationStatus.Pending => Brush.Parse("#795B00"),    // Chờ xử lý: Vàng nâu đậm
                ViolationStatus.Resolved => Brush.Parse("#107C41"),   // Đã xử lý: Xanh lá đậm
                ViolationStatus.Dismissed => Brush.Parse("#605E5C"),  // Hủy bỏ: Xám trung tính
                _ => Brush.Parse("#323130")
            };
        }
        return Brush.Parse("#323130");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Chuyển đổi trạng thái xử lý vi phạm sang màu viền badge tương ứng
/// </summary>
public class ViolationStatusToBadgeBorderConverter : IValueConverter
{
    public static readonly ViolationStatusToBadgeBorderConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ViolationStatus status)
        {
            return status switch
            {
                ViolationStatus.Pending => Brush.Parse("#FFC83B"),
                ViolationStatus.Resolved => Brush.Parse("#8AD49A"),
                ViolationStatus.Dismissed => Brush.Parse("#C8C6C4"),
                _ => Brush.Parse("#C8C6C4")
            };
        }
        return Brush.Parse("#C8C6C4");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
