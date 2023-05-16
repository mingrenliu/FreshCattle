using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class IntFormat : ExcelConverter<int>
{
    public override int? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetInt();
        }
        return default;
    }

    public override void Write(ICell cell, int? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);//待定
        }
    }
}