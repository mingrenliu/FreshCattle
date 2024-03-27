using System.Reflection;

namespace ExcelUtile.ExcelCore;

public class DefaultPropertyInfo : PropertyTypeInfo
{
    public DefaultPropertyInfo(PropertyInfo info, DisplayAttribute attribute, string? name = null) : base(info, name ?? attribute.Name, attribute.Order, attribute.IsRequired, attribute.Width)
    {
    }
}