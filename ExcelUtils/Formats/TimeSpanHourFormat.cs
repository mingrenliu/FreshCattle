using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanHourFormat : ExcelConverter<TimeSpan>
{
    protected override string? _format => "0";
    public override TimeSpan? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetTimeSpanFromHours();
        }
        return default;
    }

    public override void Write(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.TotalHours);
        }
    }
}