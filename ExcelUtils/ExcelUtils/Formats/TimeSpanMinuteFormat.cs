using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanMinuteFormat : ExcelStructConverter<TimeSpan>
{
    protected override string? Format => "0";

    public override TimeSpan? Read(ICell cell)
    {
        if (CanConvert())
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