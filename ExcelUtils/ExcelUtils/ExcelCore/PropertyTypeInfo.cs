using ExcelUtile.Formats;
using System.Reflection;

namespace ExcelUtile.ExcelCore;

public class PropertyTypeInfo : ColumnInfo
{
    public Type BaseType { get; private set; }

    public PropertyInfo Info { get; private set; }
    public DataFormatAttribute? DataFormat { get; private set; }

    public PropertyTypeInfo(PropertyInfo info, string name, int order = 0, bool isRequired = true, int width = 0) : base(name, order, isRequired, width)
    {
        Info = info;
        BaseType = Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType;
        DataFormat = Info.GetCustomAttribute<DataFormatAttribute>();
    }

    internal ExcelConverter? GetConverter(IConverterFactory _factory)
    {
        if (DataFormat != null) return DataFormat.Converter;
        return _factory.GetDefaultConverter(BaseType);
    }
}