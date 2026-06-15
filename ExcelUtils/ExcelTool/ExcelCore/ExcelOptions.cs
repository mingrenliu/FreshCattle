using ExcelTool.Converters;

namespace ExcelTool.ExcelCore;

/// <summary>
/// Excel 序列化统一配置选项。
/// 对应 System.Text.Json 的 JsonSerializerOptions。
/// </summary>
public class ExcelOptions
{
    // ==================== 核心行为 ====================

    /// <summary>
    /// 是否自动包含所有 public 属性。
    /// <list type="bullet">
    ///   <item><c>false</c>（默认）：opt-in 模式，只有标注了 <see cref="ExcelColumnAttribute"/> 的属性才参与导入导出。</item>
    ///   <item><c>true</c>：opt-out 模式，默认所有 public 读写属性都参与，标注 <see cref="ExcelIgnoreAttribute"/> 的排除。</item>
    /// </list>
    /// </summary>
    public bool AutoInclude { get; set; } = false;

    // ==================== 行列控制 ====================

    /// <summary>表头行索引（0-based），默认 0。</summary>
    public int HeaderRow { get; set; } = 0;

    /// <summary>数据起始行索引（0-based），默认 1。</summary>
    public int DataStartRow { get; set; } = 1;

    // ==================== 列宽 / 行高 ====================

    /// <summary>默认列宽（字符数），默认 15。</summary>
    public int DefaultColumnWidth { get; set; } = 15;

    /// <summary>默认行高（磅），默认 20。</summary>
    public float DefaultRowHeight { get; set; } = 20;

    // ==================== 属性名映射 ====================

    /// <summary>
    /// 属性名 -> Excel 列名（表头）的覆盖映射。
    /// 优先级高于 <see cref="ExcelColumnAttribute.Name"/>。
    /// 如果要大小写不敏感,创建时用 StringComparer.OrdinalIgnoreCase
    /// </summary>
    public Dictionary<string, string>? ColumnNameMap { get; set; }

    /// <summary>有效的导入导出字段范围，null 表示按正常规则匹配。</summary>
    public IReadOnlyList<string>? FieldScope { get; set; }

    // ==================== 导入 ====================

    /// <summary>导入时是否校验必填字段，默认 true。</summary>
    public bool ValidateRequired { get; set; } = true;

    // ==================== 转换器 ====================

    /// <summary>自定义转换器注册表。</summary>
    public ConverterRegistry Converters { get; } = new();

    // ==================== 合并区域 ====================

    /// <summary>导出时附加的合并区域。</summary>
    public IList<MergeRegion>? MergedRegions { get; set; }

    // ==================== 自定义列 ====================

    /// <summary>自定义读取列（非实体属性）。</summary>
    public IReadOnlyList<IExcelColumnReader>? Readers { get; set; }

    /// <summary>自定义写入列（非实体属性）。</summary>
    public IReadOnlyList<IExcelColumnWriter>? Writers { get; set; }

    /// <summary>确保 DataStartRow 不小于 HeaderRow+1。</summary>
    internal int EffectiveDataStartRow => Math.Max(DataStartRow, HeaderRow + 1);

    // ==================== 工厂方法 ====================

    /// <summary>创建 opt-in 模式的默认配置。</summary>
    public static ExcelOptions Default => new();

    /// <summary>
    /// 创建 opt-out 模式的配置（自动包含所有 public 属性）。
    /// </summary>
    public static ExcelOptions AutoIncludeAll => new() { AutoInclude = true };
}
