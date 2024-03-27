using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class BooleanFormat : ExcelStructConverter<bool>
{
    public override bool? Read(ICell cell)
    {
        if (CanConvert(typeof(bool)))
        {
            return cell.GetBoolean();
        }
        return null;
    }

    public override void Write(ICell cell, bool? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value ? "是" : "否");//待定
        }
    }
}