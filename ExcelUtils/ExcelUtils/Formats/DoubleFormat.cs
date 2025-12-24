using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DoubleFormat : ExcelStructConverter<double>
{
    public DoubleFormat() : this((string?)null)
    {

    }
    public DoubleFormat(int? precision) : base()
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
    public DoubleFormat(string? format) : base()
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
    public override double? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetDouble();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, double? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);
        }
    }
}