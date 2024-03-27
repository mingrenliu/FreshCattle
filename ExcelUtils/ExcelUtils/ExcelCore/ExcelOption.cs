namespace ExcelUtile.ExcelCore;

public class ExcelOption
{
    /// <summary>
    /// header row index (0-based)
    /// </summary>
    public int HeaderLineIndex { get; set; } = 0;

    /// <summary>
    /// 开始行
    /// </summary>

    private int startLine = 1;

    /// <summary>
    /// data start row index (0-based)
    /// </summary>
    public int StartLineIndex { get => Math.Max(startLine, HeaderLineIndex + 1); set => startLine = value; }
}

public class ExcelOption<T> : ExcelOption where T : class, new()
{
    /// <summary>
    /// 属性选择器
    /// </summary>
    private Func<IEnumerable<PropertyTypeInfo>>? _selector;

    /// <summary>
    /// 属性选择器
    /// </summary>
    public Func<IEnumerable<PropertyTypeInfo>> Selector { get => _selector ?? (() => DefaultPropertySelector.GetTypeInfo<T>()); set => _selector = value; }
}

public class ExcelExportOption<T> : ExcelOption<T> where T : class, new()
{
    /// <summary>
    /// 默认列宽
    /// </summary>
    public int DefaultColumnWidth { get; set; } = 15;

    /// <summary>
    /// 合并区域(导出时使用)
    /// </summary>
    public IEnumerable<MergedRegion>? MergedRegions { get; set; }

    /// <summary>
    /// 动态导出
    /// </summary>
    public IExportDynamicWrite<T>? DynamicExport { get; set; }
}

public class ExcelImportOption<T> : ExcelOption<T> where T : class, new()
{
    /// <summary>
    /// 校验导入字段
    /// </summary>
    public bool ValidImportField { get; set; } = true;

    /// <summary>
    /// 动态导出
    /// </summary>
    public IImportDynamicRead<T>? DynamicImport { get; set; }
}