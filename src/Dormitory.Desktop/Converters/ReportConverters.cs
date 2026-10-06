using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.Converters;

/// <summary>
/// Chuyển đổi loại báo cáo sang màu nền badge
/// </summary>
public class ReportTypeToBadgeBackgroundConverter : IValueConverter
{
    public static readonly ReportTypeToBadgeBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReportType type)
        {
            return type switch
            {
                ReportType.Violations => Brush.Parse("#FDE7E9"),
                ReportType.Financial => Brush.Parse("#E6F4EA"),
                ReportType.Occupancy => Brush.Parse("#E8F3FF"),
                ReportType.Equipment => Brush.Parse("#E6F8F5"),
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
/// Chuyển đổi loại báo cáo sang màu chữ badge
/// </summary>
public class ReportTypeToBadgeForegroundConverter : IValueConverter
{
    public static readonly ReportTypeToBadgeForegroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReportType type)
        {
            return type switch
            {
                ReportType.Violations => Brush.Parse("#C42B1C"),
                ReportType.Financial => Brush.Parse("#107C41"),
                ReportType.Occupancy => Brush.Parse("#0078D4"),
                ReportType.Equipment => Brush.Parse("#008272"),
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
/// Chuyển đổi loại báo cáo sang màu viền badge
/// </summary>
public class ReportTypeToBadgeBorderConverter : IValueConverter
{
    public static readonly ReportTypeToBadgeBorderConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReportType type)
        {
            return type switch
            {
                ReportType.Violations => Brush.Parse("#F19999"),
                ReportType.Financial => Brush.Parse("#A8DAB5"),
                ReportType.Occupancy => Brush.Parse("#A0C5E8"),
                ReportType.Equipment => Brush.Parse("#80D1C7"),
                _ => Brush.Parse("#E1DFDD")
            };
        }
        return Brush.Parse("#E1DFDD");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Chuyển đổi định dạng báo cáo sang màu nền badge
/// </summary>
public class ReportFormatToBadgeBackgroundConverter : IValueConverter
{
    public static readonly ReportFormatToBadgeBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReportFormat format)
        {
            return format switch
            {
                ReportFormat.Pdf => Brush.Parse("#FDE7E9"),
                ReportFormat.Excel => Brush.Parse("#E6F4EA"),
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
/// Chuyển đổi định dạng báo cáo sang màu chữ badge
/// </summary>
public class ReportFormatToBadgeForegroundConverter : IValueConverter
{
    public static readonly ReportFormatToBadgeForegroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReportFormat format)
        {
            return format switch
            {
                ReportFormat.Pdf => Brush.Parse("#C42B1C"),
                ReportFormat.Excel => Brush.Parse("#107C41"),
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
/// Chuyển đổi định dạng báo cáo sang màu viền badge
/// </summary>
public class ReportFormatToBadgeBorderConverter : IValueConverter
{
    public static readonly ReportFormatToBadgeBorderConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReportFormat format)
        {
            return format switch
            {
                ReportFormat.Pdf => Brush.Parse("#F19999"),
                ReportFormat.Excel => Brush.Parse("#A8DAB5"),
                _ => Brush.Parse("#E1DFDD")
            };
        }
        return Brush.Parse("#E1DFDD");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
