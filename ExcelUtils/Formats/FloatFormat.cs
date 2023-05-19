using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class FloatFormat : ExcelConverter<float>
{
    protected override string? _format => "0.00";
    public override float? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetFloat();
        }
        return default;
    }

    public override void Write(ICell cell, float? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);
        }
    }
}