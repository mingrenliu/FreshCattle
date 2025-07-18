using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanFormat : ExcelStructConverter<TimeSpan>
{
    private readonly string DefaultFormat = @"d\.hh\:mm\:ss";

    public TimeSpanFormat()
    {
    }

    public TimeSpanFormat(string? format)
    {
        DefaultFormat = format ?? DefaultFormat;
    }

    public override TimeSpan? Read(ICell cell)
    {
        if (CanConvert())
        {
            if (cell.IsString())
            {
                var s = cell.GetString();
                if (!string.IsNullOrWhiteSpace(s) && TimeSpan.TryParseExact(s, DefaultFormat, null, out var result))
                {
                    return result;
                }
            }
            return cell.GetTimeSpan();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToString(DefaultFormat));
        }
    }
}