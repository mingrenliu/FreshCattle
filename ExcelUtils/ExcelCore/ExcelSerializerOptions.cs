using System.Reflection;

namespace ExcelUtile.ExcelCore;

internal class ExcelSerializeOptions
{
    /// <summary>
    /// header row index (0-based)
    /// </summary>
    public int HeaderLineIndex { get; set; } = 0;

    private int startLine = 1;
    public int DefaultColumnWidth { get; set; } = 15;
    /// <summary>
    /// data start row index (0-based)
    /// </summary>
    public int StartLineIndex { get => Math.Max(startLine, HeaderLineIndex + 1); set => startLine = value; }
    /// <summary>
    /// 字段筛选
    /// </summary>

    private Func<Type, IEnumerable<PropertyTypeInfo>>? _propertySelector;

    public Func<Type, IEnumerable<PropertyTypeInfo>> PropertySelector => _propertySelector ?? DefaultPropertySelector.GetTypeInfo;

    public void SetProperty(Func<Type, IEnumerable<PropertyTypeInfo>> selector)
    {
        _propertySelector = selector;
    }
}

internal static class DefaultPropertySelector
{
    internal static IEnumerable<PropertyTypeInfo> GetTypeInfo(Type type)
    {
        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                yield return new DefaultPropertyInfo(property, attribute);
            }
        }
    }
}