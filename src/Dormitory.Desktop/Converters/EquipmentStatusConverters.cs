using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Dormitory.Core.Enums;

namespace Dormitory.Desktop.Converters;

/// <summary>
/// Chuyển đổi trạng thái thiết bị sang màu nền badge tương ứng
/// </summary>
public class EquipmentStatusToBadgeBackgroundConverter : IValueConverter
{
    public static readonly EquipmentStatusToBadgeBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EquipmentStatus status)
        {
            return status switch
            {
                EquipmentStatus.Good => Brush.Parse("#E7F4E8"),        // Xanh lá nhạt
                EquipmentStatus.NeedsRepair => Brush.Parse("#FFF4CE"), // Vàng cam nhạt
                EquipmentStatus.Broken => Brush.Parse("#FDE7E9"),      // Đỏ nhạt
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
/// Chuyển đổi trạng thái thiết bị sang màu chữ badge tương ứng
/// </summary>
public class EquipmentStatusToBadgeForegroundConverter : IValueConverter
{
    public static readonly EquipmentStatusToBadgeForegroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EquipmentStatus status)
        {
            return status switch
            {
                EquipmentStatus.Good => Brush.Parse("#107C41"),        // Xanh lá đậm
                EquipmentStatus.NeedsRepair => Brush.Parse("#795B00"), // Vàng cam đậm
                EquipmentStatus.Broken => Brush.Parse("#A80000"),      // Đỏ đậm
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
/// Chuyển đổi trạng thái thiết bị sang màu viền badge tương ứng
/// </summary>
public class EquipmentStatusToBadgeBorderConverter : IValueConverter
{
    public static readonly EquipmentStatusToBadgeBorderConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EquipmentStatus status)
        {
            return status switch
            {
                EquipmentStatus.Good => Brush.Parse("#8AD49A"),
                EquipmentStatus.NeedsRepair => Brush.Parse("#FFC83B"),
                EquipmentStatus.Broken => Brush.Parse("#F1707B"),
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
