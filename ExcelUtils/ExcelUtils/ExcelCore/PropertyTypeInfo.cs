using ExcelUtile.Formats;
using System.Reflection;

namespace ExcelUtile.ExcelCore;

public class PropertyTypeInfo : ColumnInfo
{
    public PropertyInfo Info { get; private set; }

    public PropertyTypeInfo(PropertyInfo info, string name, int order = 0, bool isRequired = true, int width = 0) : base(name, info.PropertyType, order, isRequired, width)
    {
        Info = info;
        Converter = Info.GetCustomAttribute<DataFormatAttribute>()?.Converter;
    }
}

public class DefaultPropertyInfo : PropertyTypeInfo
{
    public DefaultPropertyInfo(PropertyInfo info, DisplayAttribute attribute, string? name = null) : base(info, name ?? attribute.Name, attribute.Order, attribute.IsRequired, attribute.Width)
    {
    }
}