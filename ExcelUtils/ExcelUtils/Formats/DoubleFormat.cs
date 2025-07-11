using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DoubleFormat : ExcelStructConverter<double>
{
    public DoubleFormat() : this(3)
    {

    }
    public DoubleFormat(int? precision) : base()
    {
        precision ??= 3;
        Format = precision <= 0 ? "0" : "0." + new string('0', precision.Value);
    }
    public DoubleFormat(string? format) : base()
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