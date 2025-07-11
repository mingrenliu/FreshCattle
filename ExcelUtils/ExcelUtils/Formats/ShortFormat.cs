using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class ShortFormat : ExcelStructConverter<short>
{
    public ShortFormat()
    {
    }

    public ShortFormat(string? format)
    {
        Format = format;
    }

    public override short? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetShort();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, short? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);//待定
        }
    }
}