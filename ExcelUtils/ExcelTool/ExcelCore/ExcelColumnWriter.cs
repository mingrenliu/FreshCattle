using System.Reflection;
using ExcelTool.Converters;

namespace ExcelTool.ExcelCore;

/// <summary>
/// 属性列 Writer：通过反射读取属性值。实现 <see cref="IExcelColumnWriter"/>。
/// </summary>
public class ExcelColumnWriter : IExcelColumnWriter
{
    private readonly PropertyInfo _property;

    public ExcelColumnWriter(PropertyInfo property, string columnName,
        int order = 0, int width = 0)
    {
        _property = property;
        ColumnName = columnName;
        Order = order;
        Width = width;
    }

    /// <inheritdoc/>
    public string ColumnName { get; }

    /// <inheritdoc/>
    public int Order { get; }

    /// <inheritdoc/>
    public int Width { get; }

    /// <inheritdoc/>
    public ExcelConverter? Converter { get; set; }

    /// <inheritdoc/>
    public object? GetValue(object obj)
    {
        return _property.GetValue(obj);
    }
}
