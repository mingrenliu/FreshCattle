using NPOI.HSSF.UserModel;

namespace ExcelUtile.Formats;

public abstract class ExcelConverter
{
    protected ICellStyle? _cellStype;
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
        cell.CellStyle = _cellStype?? CreateCellType(cell);
    }

    public virtual void WriteAsObject(ICell cell, object? obj)
    {
        cell.SetCellValue(obj?.ToString());
    }

    public virtual ICellStyle? CreateCellType(ICell cell)
    {
        var style = cell.Sheet.Workbook.CreateCellStyle();
        style.Alignment = HorizontalAlignment.CenterSelection;
        return style;
    }
}