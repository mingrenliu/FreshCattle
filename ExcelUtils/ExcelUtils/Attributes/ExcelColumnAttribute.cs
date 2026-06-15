namespace ExcelUtile;

/// <summary>
/// 标识一个属性映射到 Excel 列（列名、顺序、列宽等）。
/// 当 AutoInclude=false 时，只有标注此特性的属性才会参与导入导出。
/// 
/// <para>对应 System.Text.Json 的 <c>[JsonPropertyName]</c>。</para>
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ExcelColumnAttribute : Attribute
{
    /// <summary>
    /// Excel 列名（表头），为 null 时自动使用属性名。
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// 列顺序（越小越靠左），默认 0。
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// 列宽（字符数）。
    /// <list type="bullet">
    ///   <item><c>0</c>：使用 <see cref="ExcelCore.ExcelOptions.DefaultColumnWidth"/>（默认值）。</item>
    ///   <item><c>&gt; 0</c>：固定列宽。</item>
    ///   <item><c>&lt; 0</c>：自适应列宽（AutoFit）。</item>
    /// </list>
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    /// 导入时是否必填（表头必须存在该列）。
    /// 如需使用标准 DataAnnotations，可搭配 <c>[System.ComponentModel.DataAnnotations.Required]</c>。
    /// </summary>
    public bool Required { get; init; }
}
