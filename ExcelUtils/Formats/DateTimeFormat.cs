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
    public override ICellStyle? CreateCellType(ICell cell)
    {
        var style = cell.Sheet.Workbook.CreateCellStyle();
        var format = cell.Sheet.Workbook.CreateDataFormat();
        var formatIndex = format.GetFormat("yyyy-mm-dd");
        style.DataFormat = formatIndex;
        return style;
    }
}