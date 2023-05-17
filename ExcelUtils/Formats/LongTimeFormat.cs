namespace ExcelUtile.Formats;

internal class LongTimeFormat : DateTimeFormat
{
    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
    public override ICellStyle? CreateCellType(ICell cell)
    {
        var style = cell.Sheet.Workbook.CreateCellStyle();
        var format = cell.Sheet.Workbook.CreateDataFormat();
        var formatIndex = format.GetFormat("yyyy-mm-dd hh:mm:ss");
        style.DataFormat = formatIndex;
        return style;
    }
}