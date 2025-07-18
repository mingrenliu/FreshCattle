using ExcelUtile.ExcelCore;
using System.Globalization;

namespace ExcelUtile.Formats;

public class DateTimeOffsetFormat : ExcelStructConverter<DateTimeOffset>
{
    private const string DefaultFormat = "yyyy-MM-dd";

    public DateTimeOffsetFormat() : this(DefaultFormat)
    {
    }

    public DateTimeOffsetFormat(string? format)
    {
        Format = format ?? DefaultFormat;
    }
    public override DateTimeOffset? Read(ICell cell)
    {
        if (CanConvert())
        {
            if (cell.IsString())
            {
                var s = cell.GetString();
                if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParseExact(s, Format, null, DateTimeStyles.AllowWhiteSpaces, out var result))
                {
                    return result;
                }
            }
            return cell.GetDateTimeOffset();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, DateTimeOffset? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.Date);
        }
    }
}