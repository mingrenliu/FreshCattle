using ExcelUtile.Formats;
using System.Reflection;

namespace ExcelUtile.ExcelCore;

public abstract class PropertyTypeInfo
{
    public virtual bool IsRequired { get; }

    private readonly string _name;
    public string Name => _name;
    public abstract int Order { get; }
    public abstract int? Width { get;}
    public PropertyInfo Info => _info;
    public Type BaseType { get; private set; }

    private readonly PropertyInfo _info;
    public DataFormatAttribute? DataFormat { get; private set; }

    protected PropertyTypeInfo(PropertyInfo info, string name)
    {
        _info = info;
        _name = name;
        BaseType = Nullable.GetUnderlyingType(_info.PropertyType) ?? info.PropertyType;
        DataFormat=Info.GetCustomAttribute<DataFormatAttribute>();
    }
    internal ExcelConverter? GetConverter(DefaultConverterFactory _factory)
    {
        if (DataFormat != null) return DataFormat.Converter;
        return _factory.GetDefaultConverter(BaseType);
    }
}
