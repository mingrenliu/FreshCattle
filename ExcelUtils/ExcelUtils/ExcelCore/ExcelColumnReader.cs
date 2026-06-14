using System.Reflection;
using ExcelUtile.Converters;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 属性列 Reader：通过反射读写属性值。实现 <see cref="IExcelColumnReader"/>。
/// </summary>
public class ExcelColumnReader : IExcelColumnReader
{
    private readonly PropertyInfo _property;
    private readonly string _columnName;

    public ExcelColumnReader(PropertyInfo property, string columnName, bool required = false)
    {
        _property = property;
        _columnName = columnName;
        Required = required;
    }

    /// <inheritdoc/>
    public bool Required { get; }

    /// <inheritdoc/>
    public ExcelConverter? Converter { get; set; }

    /// <inheritdoc/>
    public bool Match(string columnName)
    {
        return string.Equals(_columnName, columnName, System.StringComparison.OrdinalIgnoreCase);
    }
    private Type? _underlyingType;
    private Type UnderlyingType(Type type) => _underlyingType ??= (Nullable.GetUnderlyingType(type) ?? type);
    /// <inheritdoc/>
    public void SetValue(object obj, string columnName, object? value)
    {
        if (value == null) return;
        try
        {
            var targetType = UnderlyingType(_property.PropertyType);
            _property.SetValue(obj, System.Convert.ChangeType(value, targetType));
        }
        catch { /* 类型转换失败则跳过 */ }
    }

    /// <inheritdoc/>
    public override string ToString() => _columnName;
}
