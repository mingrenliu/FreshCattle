using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DecimalFormat : ExcelStructConverter<decimal>
{
    public DecimalFormat() : this((string?)null)
    {
    }

    public DecimalFormat(int? precision) : base()
    {
        if (precision.HasValue)
        {
            Format = precision == 0 ? "0" : "0.0" + new string(precision > 0 ? '0' : '#', Math.Abs(precision.Value) - 1);
        }
        else
        {
            Format = null;
        }
    }

    public DecimalFormat(string? format) : base()
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            Format = null;
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