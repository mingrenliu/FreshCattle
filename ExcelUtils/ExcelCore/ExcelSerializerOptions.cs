using System.Reflection;

namespace ExcelUtils.ExcelCore;

internal class ExcelSerializerOptions
{
    /// <summary>
    /// header row index (0-based)
    /// </summary>
    public int HeaderLineIndex { get; set; } = 0;

    private int startLine = 1;
    /// <summary>
    /// data start row index (0-based)
    /// </summary>
    public int StartLineIndex { get => Math.Max(startLine, HeaderLineIndex + 1); set => startLine = value; }
    /// <summary>
    /// 字段筛选
    /// </summary>

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
        var result = new InfoWrapper<PropertyTypeInfo>();
        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                result.Add(new DefaultPropertyInfo(property, attribute));
            }
        }
        return result;
    }
}