using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DecimalFormat : ExcelConverter<decimal>
{
    protected override string? _format => "0.00";
    public override decimal? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetDecimal();
        }
        return default;
    }

    public override void Write(ICell cell, decimal? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue((double)value.Value);
        }
    }
}