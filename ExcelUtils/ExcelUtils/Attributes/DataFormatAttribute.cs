namespace ExcelUtile.Formats;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DataFormatAttribute : Attribute
{
    public ExcelConverter? Converter => _converter;

    /// <summary>
    /// Converter Instance
    /// </summary>
    private readonly ExcelConverter? _converter;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="type"></param>
    public DataFormatAttribute(Type type, params object?[]? args)
    {
        if (Activator.CreateInstance(type,args) is ExcelConverter result)
        {
            _converter = result;
        }
    }
}