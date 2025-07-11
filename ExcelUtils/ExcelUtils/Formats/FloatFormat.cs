using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;
public class FloatFormat : ExcelStructConverter<float>
{
    public FloatFormat() : this(3)
    {

    }
    public FloatFormat(int? precision) : base()
    {
        precision ??= 3;
        Format = precision <= 0 ? "0" : "0." + new string('0', precision.Value);
    }
    public FloatFormat(string? format) : base()
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
    public override float? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetFloat();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, float? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value);
        }
    }
}