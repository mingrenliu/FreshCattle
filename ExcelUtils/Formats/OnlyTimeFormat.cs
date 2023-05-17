namespace ExcelUtile.Formats;

internal class OnlyTimeFormat : DateTimeFormat
{
    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value.ToString("HH:mm:ss"));
        }
    }
    public override ICellStyle? CreateCellType(ICell cell)
    {
        var style = cell.Sheet.Workbook.CreateCellStyle();
        var format = cell.Sheet.Workbook.CreateDataFormat();
        var formatIndex = format.GetFormat("hh:mm:ss");
        style.DataFormat = formatIndex;
        return style;
    }
}