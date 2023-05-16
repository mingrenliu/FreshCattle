using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public abstract class ExcelConverter
{
    protected readonly Type _type;
    public ExcelConverter(Type type)
    {
        _type=type;
    }
    public virtual bool CanConvert(Type type)
    {
        return _type==type;
    }
    public virtual string?  ReadAsString(ICell cell)
    {
        var str= cell.ToString()?.Trim();
        return string.IsNullOrWhiteSpace(str)?null:str; 
    }
    public abstract object? ReadAsObject(ICell cell);
    public virtual void WriteAsObject(ICell cell, object? obj)
    {
        cell.SetCellValue(obj?.ToString());
    }
}