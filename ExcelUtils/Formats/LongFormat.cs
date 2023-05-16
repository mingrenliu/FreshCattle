using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class LongFormat : ExcelConverter<long>
{
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