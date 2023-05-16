using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class TimeOnlyFormat : ExcelConverter<TimeOnly>
{
    public override TimeOnly? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetTime();
        }
        return default;
    }

    public override void Write(ICell cell, TimeOnly? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToLongTimeString());//待定
        }
    }
}