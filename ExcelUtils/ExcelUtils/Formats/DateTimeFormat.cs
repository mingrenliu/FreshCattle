using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public abstract class DateTimeFormat : ExcelStructConverter<DateTime>
{
    public override DateTime? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetDateTime();
        }
        return null;
    }

    protected override void WriteValue(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value);
        }
    }
}