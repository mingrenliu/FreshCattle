using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

internal class TimeOnlyFormat : ExcelConverter<TimeOnly>
{
    public override TimeOnly? Read(ICell cell, Type type)
    {
        if (CanConvert(type))
        {
            return cell.GetTime();
        }
        return default;
    }

    public override void Write(ICell cell, TimeOnly? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToLongTimeString());//待定
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