using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class FloatFormat : ExcelConverter<float>
{
    public override float? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetFloat();
        }
        return default;
    }

    public override void Write(ICell cell, float? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(Math.Round(value.Value,3));//待定
        }
    }
}