using ExcelUtile.ExcelCore;
using System.Globalization;

namespace ExcelUtile.Formats;

public  class DateTimeFormat : ExcelStructConverter<DateTime>
{
    private const string DefaultFormat = "yyyy-MM-dd";

    public DateTimeFormat() : this(DefaultFormat)
    {
    }

    public DateTimeFormat(string? format)
    {
        Format = format ?? DefaultFormat;
    }
    public override DateTime? Read(ICell cell)
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
            return cell.GetDateTime();
        }
        return null;
    }

    protected override void WriteValue(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value);
        }
    }
}