using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;
public class FloatFormat : ExcelStructConverter<float>
{
    public FloatFormat() : this((string?)null)
    {

    }
    public FloatFormat(int? precision) : base()
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
    public FloatFormat(string? format) : base()
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