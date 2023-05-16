using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class TimeSpanFormat : ExcelConverter<TimeSpan>
{
    public override TimeSpan? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetTimeSpan();
        }
        return default;
    }

    public override void Write(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToString(@"hh\:mm\:ss"));//待定
        }
    }
}