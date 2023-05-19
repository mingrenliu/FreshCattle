using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class LongFormat : ExcelConverter<long>
{
    protected override string? _format => "0";
    public override long? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetLong();
        }
        return default;
    }

    public override void Write(ICell cell, long? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);//待定
        }
    }
}