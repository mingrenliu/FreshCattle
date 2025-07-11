using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class DateOnlyFormat : ExcelStructConverter<DateOnly>
{
    public override DateOnly? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetDate();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, DateOnly? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToShortDateString());//待定
        }
    }
}