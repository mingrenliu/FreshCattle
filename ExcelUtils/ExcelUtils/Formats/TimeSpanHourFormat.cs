using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanHourFormat : ExcelStructConverter<TimeSpan>
{
    public TimeSpanHourFormat() : this(0)
    {

    }
    public TimeSpanHourFormat(int? precision) : base()
    {
        precision ??= 0;
        Format = precision <= 0 ? "0" : "0." + new string('0', precision.Value);
    }


    public override TimeSpan? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetTimeSpanFromHours();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.TotalHours);
        }
    }
}