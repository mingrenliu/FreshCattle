using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeOnlyFormat : ExcelStructConverter<TimeOnly>
{
    private readonly string DefaultFormat = "HH:mm:ss";

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
            if(cell.IsString())
            {
                var s = cell.GetString();
                if (!string.IsNullOrWhiteSpace(s) && TimeOnly.TryParseExact(s, DefaultFormat, out var result))
                {
                    return result;
                }
            }
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