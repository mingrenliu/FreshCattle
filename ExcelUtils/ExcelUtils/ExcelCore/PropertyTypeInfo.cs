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