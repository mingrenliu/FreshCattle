namespace ExcelUtils.Formats;

public abstract class ExcelConverter
{
    private readonly Type _type;
    public ExcelConverter(Type type)
    {
        _type=type;
    }
    protected object? ReadCoreAsObject()
    {
        return null;
    }
}