using ExcelUtils.ExcelCore;

namespace ExcelUtils.Formats;

public abstract class ExcelConverter
{
    private readonly Type _type;
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
}