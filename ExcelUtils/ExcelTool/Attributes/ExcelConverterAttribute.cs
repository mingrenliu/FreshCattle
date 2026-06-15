namespace ExcelTool;

/// <summary>
/// 为属性指定自定义 Excel 转换器。
/// 对应 System.Text.Json 的 <c>[JsonConverter(typeof(...))]</c>。
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ExcelConverterAttribute : Attribute
{
    /// <summary>
    /// 转换器类型（必须继承 <see cref="Converters.ExcelConverter"/> 且有无参构造函数）。
    /// </summary>
    public Type ConverterType { get; }

    /// <summary>
    /// 可选：传递给转换器构造函数的参数。
    /// </summary>
    public object?[]? Args { get; init; }

    /// <param name="converterType">转换器类型，如 <c>typeof(MyDateTimeConverter)</c></param>
    public ExcelConverterAttribute(Type converterType)
    {
        ConverterType = converterType;
    }
}
