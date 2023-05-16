using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class DateTimeOffsetFormat : ExcelConverter<DateTimeOffset>
{
    public override DateTimeOffset? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetDateTimeOffset();
        }
        return default;
    }

    public override void Write(ICell cell, DateTimeOffset? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.Date.ToShortDateString());//待定
        }
    }
}