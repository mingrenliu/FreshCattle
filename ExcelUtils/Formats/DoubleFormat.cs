using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class DoubleFormat : ExcelConverter<double>
{
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
            cell.SetCellValue(Math.Round(value.Value, 3));//待定
        }
    }
}