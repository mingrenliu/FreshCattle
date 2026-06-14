namespace ExcelUtile;

/// <summary>
/// 指定 Excel 单元格的显示格式字符串。
/// 如 <c>"0.00"</c>、<c>"#,##0"</c>、<c>"yyyy-MM-dd"</c>。
/// 独立于 <see cref="ExcelColumnAttribute"/>，与转换器配合使用。
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ExcelFormatAttribute : Attribute
{
    /// <summary>
    /// Excel 格式字符串。
    /// </summary>
    public string Format { get; }

    /// <param name="format">格式字符串，如 <c>"0.00"</c></param>
    public ExcelFormatAttribute(string format)
    {
        Format = format;
    }
}
