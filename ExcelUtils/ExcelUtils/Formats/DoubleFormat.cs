using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DoubleFormat : ExcelStructConverter<double>
{
    protected override string? Format => "0.00";

    public override double? Read(ICell cell)
    {
        if (CanConvert())
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