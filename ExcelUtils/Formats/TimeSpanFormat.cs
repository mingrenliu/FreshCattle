using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class TimeSpanFormat : ExcelConverter<TimeSpan>
{
    public override TimeSpan? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetTimeSpan();
        }
        return default;
    }

    public override void Write(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToString(@"hh\:mm\:ss"));//待定
        }
    }
    public override ICellStyle? CreateCellType(ICell cell)
    {
        var style = cell.Sheet.Workbook.CreateCellStyle();
        var format = cell.Sheet.Workbook.CreateDataFormat();
        var formatIndex = format.GetFormat("hh:mm:ss");
        style.DataFormat = formatIndex;
        style.Alignment = HorizontalAlignment.CenterSelection;
        return style;
    }
}