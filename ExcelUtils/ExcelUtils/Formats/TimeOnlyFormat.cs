using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeOnlyFormat : ExcelConverter<TimeOnly>
{
    protected override string? _format => "hh:mm:ss";
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