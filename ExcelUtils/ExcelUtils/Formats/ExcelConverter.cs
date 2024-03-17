using NPOI.HSSF.UserModel;

namespace ExcelUtile.Formats;

public abstract class ExcelConverter
{
    protected ICellStyle? _cellStyle;
    protected virtual string? _format { get; }

    protected readonly Type _type;

    public ExcelConverter(Type type)
    {
        _type = type;
    }

    public virtual bool CanConvert(Type type)
    {
        return _type == type;
    }

    public abstract object? ReadFromCell(ICell cell);

    public void WriteToCell(ICell cell, object? obj)
    {
        WriteAsObject(cell, obj);
        cell.CellStyle = _cellStyle ?? CreateCellType(cell);
    }

    public virtual void WriteAsObject(ICell cell, object? obj)
    {
        cell.SetCellValue(obj?.ToString());
    }

    public virtual ICellStyle? CreateCellType(ICell cell)
    {
        var style = cell.Sheet.Workbook.CreateCellStyle();
        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;
        if (!string.IsNullOrWhiteSpace(_format))
        {
            var format = cell.Sheet.Workbook.CreateDataFormat();
            var formatIndex = format.GetFormat(_format);
            style.DataFormat = formatIndex;
        }
        _cellStyle = style;
        return style;
    }
}