using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DateTimeOffsetFormat : ExcelConverter<DateTimeOffset>
{
    protected override string? _format => "yyyy-mm-dd";

    public override DateTimeOffset? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
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