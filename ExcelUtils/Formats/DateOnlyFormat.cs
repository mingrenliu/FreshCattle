using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DateOnlyFormat : ExcelConverter<DateOnly>
{
    protected override string? _format => "yyyy-mm-dd";
    public override DateOnly? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetDate();
        }
        return default;
    }

    public override void Write(ICell cell, DateOnly? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToShortDateString());//待定
        }
    }
}