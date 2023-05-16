namespace ExcelUtile.ExcelCore
{
    internal static class CellParseExtension
    {
        public static bool? GetBoolean(this ICell cell)
        {
            if (IsBoolean(cell)) return cell.BooleanCellValue;
            var str = GetString(cell);
            if (string.IsNullOrWhiteSpace(str)) return null;
            if (string.Equals(str, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "是", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(str, "false", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "否", StringComparison.OrdinalIgnoreCase)) return true;
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

        public static bool IsBoolean(this ICell cell)
        {
            return cell.CellType == CellType.Boolean || cell.CellType == CellType.Formula && cell.CachedFormulaResultType == CellType.Boolean;
        }

        public static string? GetString(this ICell cell)
        {
            var str = cell.ToString()?.Trim();
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }

        private static string? GetStringForDateTime(this ICell cell)
        {
            var str = cell.ToString()?.Replace("：", ":").Trim();
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
}