using ExcelUtile.Formats;

namespace ExcelUtile.ExcelCore;

public static class CellParseExtension
{
    public static void WriteCell(this ExcelConverter? converter, ICell cell, object? value)
    {
        if (converter != null)
        {
            converter.WriteToCell(cell, value);
        }
        else
        {
            var str = value?.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                cell.SetCellValue(str);
            }
        }
    }

    public static object? ReadCell(this ExcelConverter? converter, ICell cell)
    {
        if (converter != null)
        {
            return converter.ReadFromCell(cell);
        }
        else
        {
            return cell.ToString();
        }
    }

    public static IRow GetOrCreateRow(this ISheet sheet, int rowNum, int height = 20)
    {
        var row = sheet.GetRow(rowNum);
        if (row == null)
        {
            row = sheet.CreateRow(rowNum);
            row.HeightInPoints = height < 0 ? 20 : height;
        }
        return row;
    }

    public static ICell GetOrCreateCell(this IRow row, int cellNum, ICellStyle? style = null)
    {
        var cell = row.GetCell(cellNum);
        if (cell == null)
        {
            cell = row.CreateCell(cellNum);
            if (style != null)
            {
                cell.CellStyle = style;
            }
        }
        return cell;
    }

    public static bool? GetBoolean(this ICell cell)
    {
        if (IsBoolean(cell)) return cell.BooleanCellValue;
        var str = GetString(cell);
        if (string.IsNullOrWhiteSpace(str)) return null;
        if (string.Equals(str, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "是", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(str, "false", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "否", StringComparison.OrdinalIgnoreCase)) return false;
        return null;
    }

    public static double? GetDouble(this ICell cell)
    {
        if (IsInValid(cell)) return null;
        if (IsNumeric(cell)) return cell.NumericCellValue;
        var str = GetString(cell);
        if (string.IsNullOrWhiteSpace(str) || !double.TryParse(str, out var value)) return null;
        return value;
    }

    public static decimal? GetDecimal(this ICell cell)
    {
        var value = GetDouble(cell);
        if (!value.HasValue || value.Value > (double)decimal.MaxValue || value.Value < (double)decimal.MinValue) return null;
        return (decimal?)GetDouble(cell);
    }

    public static int? GetInt(this ICell cell)
    {
        var value = GetDouble(cell);
        if (!value.HasValue || value.Value > int.MaxValue || value.Value < int.MinValue) return null;
        return (int?)GetDouble(cell);
    }

    public static short? GetShort(this ICell cell)
    {
        var value = GetDouble(cell);
        if (!value.HasValue || value.Value > short.MaxValue || value.Value < short.MinValue) return null;
        return (short?)GetDouble(cell);
    }

    public static long? GetLong(this ICell cell)
    {
        var value = GetDouble(cell);
        if (!value.HasValue || value.Value > long.MaxValue || value.Value < long.MinValue) return null;
        return (long?)GetDouble(cell);
    }

    public static float? GetFloat(this ICell cell)
    {
        var value = GetDouble(cell);
        if (!value.HasValue || value.Value > float.MaxValue || value.Value < float.MinValue) return null;
        return (float?)GetDouble(cell);
    }

    public static bool IsInValid(this ICell cell)
    {
        return cell.CellType == CellType.Unknown || cell.CellType == CellType.Blank || cell.CellType == CellType.Error;
    }

    public static bool IsNumeric(this ICell cell)
    {
        return cell.CellType == CellType.Numeric || cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Numeric;
    }

    public static bool IsString(this ICell cell)
    {
        return cell.CellType == CellType.String || cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.String;
    }

    public static bool IsBoolean(this ICell cell)
    {
        return cell.CellType == CellType.Boolean || cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Boolean;
    }

    public static object? GetObject(this ICell cell)
    {
        if (IsInValid(cell)) return null;
        if (IsNumeric(cell)) return cell.NumericCellValue;
        if (IsBoolean(cell)) return cell.BooleanCellValue;
        return GetString(cell);
    }

    public static string? GetString(this ICell cell)
    {
        if (IsString(cell)) return cell.StringCellValue;
        var str = cell.ToString()?.Trim();
        return string.IsNullOrWhiteSpace(str) ? null : str;
    }

    private static string? GetStringForDateTime(this ICell cell)
    {
        var str = cell.GetString()?.Replace("：", ":").Trim();
        return string.IsNullOrWhiteSpace(str) ? null : str;
    }

    public static DateTime? GetDateTime(this ICell cell)
    {
        if (IsInValid(cell)) return null;
        if (IsNumeric(cell)) return cell.DateCellValue;
        var str = GetStringForDateTime(cell);
        if (string.IsNullOrWhiteSpace(str) || !DateTime.TryParse(str, out var value)) return null;
        return value;
    }

    public static TimeSpan? GetTimeSpanFromMinutes(this ICell cell)
    {
        var value = cell.GetDouble();
        return value.HasValue ? TimeSpan.FromMinutes(value.Value) : null;
    }

    public static TimeSpan? GetTimeSpanFromHours(this ICell cell)
    {
        var value = cell.GetDouble();
        return value.HasValue ? TimeSpan.FromHours(value.Value) : null;
    }

    public static TimeSpan? GetTimeSpanFromDateTime(this ICell cell)
    {
        var value = cell.GetDateTime();
        return value.HasValue ? value.Value.TimeOfDay : null;
    }

    public static TimeSpan? GetTimeSpan(this ICell cell)
    {
        if (IsInValid(cell)) return null;
        if (IsNumeric(cell)) return cell.DateCellValue.TimeOfDay;
        var str = GetStringForDateTime(cell);
        if (!string.IsNullOrWhiteSpace(str))
        {
            if (DateTime.TryParse(str, out var date))
            {
                return date.TimeOfDay;
            }
            if (TimeSpan.TryParse(str, out var time))
            {
                return time;
            }
        }
        return null;
    }

    public static DateOnly? GetDate(this ICell cell)
    {
        var date = GetDateTime(cell);
        return date.HasValue ? DateOnly.FromDateTime(date.Value) : null;
    }

    public static DateTimeOffset? GetDateTimeOffset(this ICell cell)
    {
        var date = GetDateTime(cell);
        return date.HasValue ? new DateTimeOffset(date.Value) : null;
    }

    public static TimeOnly? GetTime(this ICell cell)
    {
        if (IsInValid(cell)) return null;
        if (IsNumeric(cell)) return TimeOnly.FromDateTime(cell.DateCellValue);
        var str = GetStringForDateTime(cell);
        if (!string.IsNullOrWhiteSpace(str))
        {
            if (DateTime.TryParse(str, out var date))
            {
                return TimeOnly.FromDateTime(date);
            }
            if (TimeOnly.TryParse(str, out var time))
            {
                return time;
            }
        }
        return null;
    }
}