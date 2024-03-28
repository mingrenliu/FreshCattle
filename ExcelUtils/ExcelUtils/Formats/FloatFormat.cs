using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class FloatFormat : ExcelStructConverter<float>
{
    protected override string? Format => "0.00";

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