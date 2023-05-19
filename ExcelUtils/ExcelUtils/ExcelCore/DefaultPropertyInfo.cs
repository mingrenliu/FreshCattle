using System.Reflection;

namespace ExcelUtile.ExcelCore;

public class DefaultPropertyInfo : PropertyTypeInfo
{
    private readonly DisplayAttribute _display;

    public DefaultPropertyInfo(PropertyInfo info, DisplayAttribute attribute,string? name=null) : base(info, name??attribute.Name)
    {
        _display = attribute;
    }
    public override int? Width => _display.Width<=0?null: _display.Width;
    public override bool IsRequired => _display.IsRequired;

    public override int Order => _display.Order;
}