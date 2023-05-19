using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class ShortFormat : ExcelConverter<short>
{
    protected override string? _format => "0";
    public override short? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetShort();
        }
        return default;
    }

    public override void Write(ICell cell, short? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);//待定
        }
    }
}