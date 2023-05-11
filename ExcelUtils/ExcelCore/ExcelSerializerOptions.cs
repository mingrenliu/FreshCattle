using System.Reflection;

namespace ExcelUtils.ExcelCore;

//待定：后面根据功能再重构
internal class ExcelSerializerOptions
{
    private Func<Type, InfoWrapper<PropertyTypeInfo>>? _propertySelector;

    public Func<Type, InfoWrapper<PropertyTypeInfo>> PropertySelector => _propertySelector ?? DefaultPropertySelector.GetTypeInfo;

    public void SetProperty(Func<Type, InfoWrapper<PropertyTypeInfo>> selector)
    {
        _propertySelector = selector;
    }
}
internal static class DefaultPropertySelector
{
    internal static InfoWrapper<PropertyTypeInfo> GetTypeInfo(Type type)
    {
        var result= new InfoWrapper<PropertyTypeInfo>();
        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                result.Add(new DefaultPropertyInfo(property,attribute));
            }
        }
        return result;
    }
}