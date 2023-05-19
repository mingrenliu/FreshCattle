using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanMinuteFormat : ExcelConverter<TimeSpan>
{
    protected override string? _format => "0";
    public override TimeSpan? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetTimeSpanFromMinutes();
        }
        return default;
    }

    public override void Write(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.TotalMinutes);
        }
    }
}