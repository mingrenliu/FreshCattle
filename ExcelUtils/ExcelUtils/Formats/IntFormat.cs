using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class IntFormat : ExcelConverter<int>
{
    protected override string? _format => "0";
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