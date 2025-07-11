using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class LongFormat : ExcelStructConverter<long>
{
    public LongFormat()
    {
    }

    public LongFormat(string? format)
    {
        Format = format;
    }

    public override long? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetLong();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, long? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);//待定
        }
    }
}