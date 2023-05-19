using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DoubleFormat : ExcelConverter<double>
{
    protected override string? _format => "0.00";
    public override double? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetDouble();
        }
        return default;
    }

    public override void Write(ICell cell, double? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);
        }
    }
}