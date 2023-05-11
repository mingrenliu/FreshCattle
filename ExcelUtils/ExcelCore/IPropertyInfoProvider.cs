using System.Reflection;

namespace ExcelUtils.ExcelCore;

public abstract class PropertyInfoProvider
{
    public abstract InfoWrapper<PropertyTypeInfo> GetPropertyInfos<T>();
}

internal class DefaultPropertyInfoProvider : PropertyInfoProvider
{
    public override InfoWrapper<PropertyTypeInfo> GetPropertyInfos<T>()
    {
        var result=new InfoWrapper<DefaultPropertyInfo>();
        foreach (var property in typeof(T).GetProperties())
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