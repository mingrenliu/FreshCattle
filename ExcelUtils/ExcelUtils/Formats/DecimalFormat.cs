using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DecimalFormat : ExcelStructConverter<decimal>
{
    public DecimalFormat() : this(3)
    {

    }
    public DecimalFormat(int? precision) : base()
    {
        precision ??= 3;
        Format = precision <= 0 ? "0" : "0." + new string('0', precision.Value);
    }
    public DecimalFormat(string? format) : base()
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            var precision = 3;
            Format = precision <= 0 ? "0" : "0." + new string('0', precision);
        }
        else
        {
            Format = format;
        }
    }
    public override decimal? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetDecimal();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, decimal? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue((double)value.Value);
        }
    }
}