using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal abstract class DateTimeFormat : ExcelConverter<DateTime>
{
    public override DateTime? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetDateTime();
        }
        return null ;
    }
}