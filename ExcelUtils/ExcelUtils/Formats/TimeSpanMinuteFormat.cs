using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanMinuteFormat : ExcelStructConverter<TimeSpan>
{
    public TimeSpanMinuteFormat() : this(0)
    {

    }
    public TimeSpanMinuteFormat(int? precision) : base()
    {
        precision ??= 0;
        Format = precision <= 0 ? "0" : "0." + new string('0', precision.Value);
    }

    public override TimeSpan? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetTimeSpanFromMinutes();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.TotalMinutes);
        }
    }
}