using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeOnlyFormat : ExcelStructConverter<TimeOnly>
{
    private readonly string DefaultFormat = "hh:mm:ss";

    public TimeOnlyFormat()
    {
    }

    public TimeOnlyFormat(string? format)
    {
        DefaultFormat = format ?? DefaultFormat;
    }

    public override TimeOnly? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetTime();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, TimeOnly? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToString(DefaultFormat));//待定
        }
    }
}