using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class BooleanFormat : ExcelConverter<bool>
{
    public override bool? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetBoolean();
        }
        return default;
    }

    public override void Write(ICell cell, bool? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value?"是":"否");//待定
        }
    }
}