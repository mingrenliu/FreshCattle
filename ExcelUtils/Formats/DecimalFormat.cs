using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class DecimalFormat : ExcelConverter<decimal>
{
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
            cell.SetCellValue(Math.Round(value.Value, 3).ToString());//待定
        }
    }
}