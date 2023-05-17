namespace ExcelUtile.Formats;

public abstract class ExcelConverter
{
    protected internal ICellStyle? _cellStyle;

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
    public void WriteToCell(ICell cell,object? obj)
    {
        WriteAsObject(cell, obj);
        _cellStyle ??= CreateCellType(cell);
        if (_cellStyle != null)
        {
            cell.CellStyle = _cellStyle;
        }
    }

    public virtual void WriteAsObject(ICell cell, object? obj)
    {
        cell.SetCellValue(obj?.ToString());
    }

    public virtual ICellStyle? CreateCellType(ICell cell)
    {
        return cell.CellStyle;
    }
}