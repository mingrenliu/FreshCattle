using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public  class DateTimeFormat : ExcelStructConverter<DateTime>
{
    private const string DefaultFormat = "yyyy-mm-dd";

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