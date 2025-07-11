using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DateTimeOffsetFormat : ExcelStructConverter<DateTimeOffset>
{
    private const string DefaultFormat = "yyyy-mm-dd";

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