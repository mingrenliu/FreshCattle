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
}