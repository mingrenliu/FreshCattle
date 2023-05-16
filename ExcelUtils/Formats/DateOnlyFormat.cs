using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class DateOnlyFormat : ExcelConverter<DateOnly>
{
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