using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DecimalFormat : ExcelStructConverter<decimal>
{
    protected override string? Format => "0.00";

    public override decimal? Read(ICell cell)
    {
        if (CanConvert())
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