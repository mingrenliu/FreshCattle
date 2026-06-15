using NPOI.SS.UserModel;
using ExcelUtile.ExcelCore;
using System.Globalization;

namespace ExcelUtile.Converters;

public class DateTimeConverter : ExcelConverter<DateTime>
{
    public DateTimeConverter() => ExcelFormat = "yyyy-MM-dd";
    public DateTimeConverter(string? format) { if (!string.IsNullOrWhiteSpace(format)) ExcelFormat = format; }

    public override DateTime Read(ICell cell)
    {
        var dt = cell.GetDateTime();
        if (dt.HasValue) return dt.Value;

        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;

        if (!string.IsNullOrWhiteSpace(ExcelFormat) &&
            DateTime.TryParseExact(s, ExcelFormat, null, DateTimeStyles.AllowWhiteSpaces, out var exact))
            return exact;

        if (DateTime.TryParse(s, out var parsed))
            return parsed;

        return default;
    }

    public override void Write(ICell cell, DateTime value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.ToString(ExcelFormat ?? "yyyy-MM-dd"));
        if (style != null) cell.CellStyle = style;
    }
}

public class LongDateTimeConverter : ExcelConverter<DateTime>
{
    public LongDateTimeConverter() => ExcelFormat = "yyyy-MM-dd HH:mm:ss";
    public LongDateTimeConverter(string? format) { if (!string.IsNullOrWhiteSpace(format)) ExcelFormat = format; }

    public override DateTime Read(ICell cell)
    {
        var dt = cell.GetDateTime();
        if (dt.HasValue) return dt.Value;

        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;

        if (!string.IsNullOrWhiteSpace(ExcelFormat) &&
            DateTime.TryParseExact(s, ExcelFormat, null, DateTimeStyles.AllowWhiteSpaces, out var exact))
            return exact;

        if (DateTime.TryParse(s, out var parsed))
            return parsed;

        return default;
    }

    public override void Write(ICell cell, DateTime value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.ToString(ExcelFormat ?? "yyyy-MM-dd HH:mm:ss"));
        if (style != null) cell.CellStyle = style;
    }
}

public class DateTimeOffsetConverter : ExcelConverter<DateTimeOffset>
{
    public DateTimeOffsetConverter() => ExcelFormat = "yyyy-MM-dd HH:mm:ss zzz";

    public override DateTimeOffset Read(ICell cell)
    {
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        return DateTimeOffset.TryParse(s, out var dto) ? dto : default;
    }

    public override void Write(ICell cell, DateTimeOffset value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.ToString(ExcelFormat ?? "O"));
        if (style != null) cell.CellStyle = style;
    }
}

public class DateOnlyConverter : ExcelConverter<DateOnly>
{
    public DateOnlyConverter() => ExcelFormat = "yyyy-MM-dd";

    public override DateOnly Read(ICell cell)
    {
        var dt = cell.GetDateTime();
        if (dt.HasValue) return DateOnly.FromDateTime(dt.Value);
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        return DateOnly.TryParse(s, out var d) ? d : default;
    }

    public override void Write(ICell cell, DateOnly value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.ToString(ExcelFormat ?? "yyyy-MM-dd"));
        if (style != null) cell.CellStyle = style;
    }
}

public class TimeOnlyConverter : ExcelConverter<TimeOnly>
{
    public TimeOnlyConverter() => ExcelFormat = "HH:mm:ss";

    public override TimeOnly Read(ICell cell)
    {
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        var ts = cell.GetTimeSpan();
        if (ts.HasValue) return TimeOnly.FromTimeSpan(ts.Value);
        return TimeOnly.TryParse(s, out var t) ? t : default;
    }

    public override void Write(ICell cell, TimeOnly value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.ToString(ExcelFormat ?? "HH:mm:ss"));
        if (style != null) cell.CellStyle = style;
    }
}

public class TimeSpanConverter : ExcelConverter<TimeSpan>
{
    public TimeSpanConverter() => ExcelFormat = @"hh\:mm\:ss";

    public override TimeSpan Read(ICell cell)
    {
        var ts = cell.GetTimeSpan();
        if (ts.HasValue) return ts.Value;
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        return TimeSpan.TryParse(s, out var parsed) ? parsed : default;
    }

    public override void Write(ICell cell, TimeSpan value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.ToString(ExcelFormat ?? @"hh\:mm\:ss"));
        if (style != null) cell.CellStyle = style;
    }
}

public class TimeSpanMinutesConverter : ExcelConverter<TimeSpan>
{
    public override TimeSpan Read(ICell cell)
    {
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        if (double.TryParse(s, out var minutes))
            return TimeSpan.FromMinutes(minutes);
        var ts = cell.GetTimeSpan();
        return ts ?? default;
    }

    public override void Write(ICell cell, TimeSpan value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.TotalMinutes.ToString("F2"));
        if (style != null) cell.CellStyle = style;
    }
}

public class TimeSpanHoursConverter : ExcelConverter<TimeSpan>
{
    public override TimeSpan Read(ICell cell)
    {
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        if (double.TryParse(s, out var hours))
            return TimeSpan.FromHours(hours);
        var ts = cell.GetTimeSpan();
        return ts ?? default;
    }

    public override void Write(ICell cell, TimeSpan value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.TotalHours.ToString("F2"));
        if (style != null) cell.CellStyle = style;
    }
}

public class GuidConverter : ExcelConverter<Guid>
{
    public override Guid Read(ICell cell)
    {
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        return Guid.TryParse(s, out var g) ? g : default;
    }

    public override void Write(ICell cell, Guid value, ICellStyle? style = null)
    {
        cell.SetCellValue(value.ToString());
        if (style != null) cell.CellStyle = style;
    }
}

public class EnumConverter<T> : ExcelConverter<T> where T : struct, Enum
{
    public override T Read(ICell cell)
    {
        var s = cell.GetString();
        if (string.IsNullOrWhiteSpace(s)) return default;
        try { return (T)Enum.Parse(typeof(T), s, ignoreCase: true); }
        catch { return default; }
    }

    public override void Write(ICell cell, T value, ICellStyle? style = null)
    {
        cell.SetCellValue(Convert.ToInt32(value).ToString());
        if (style != null) cell.CellStyle = style;
    }
}
