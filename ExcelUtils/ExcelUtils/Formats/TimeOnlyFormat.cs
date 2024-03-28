using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeOnlyFormat : ExcelStructConverter<TimeOnly>
{
    protected override string? Format => "hh:mm:ss";

    public override TimeOnly? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetTime();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, TimeOnly? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToLongTimeString());//待定
        }
    }
}