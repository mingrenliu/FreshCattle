using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DateTimeOffsetFormat : ExcelStructConverter<DateTimeOffset>
{
    protected override string? Format => "yyyy-mm-dd";

    public override DateTimeOffset? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetDateTimeOffset();
        }
        return default;
    }

    public override void Write(ICell cell, DateTimeOffset? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.Date);
        }
    }
}