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

public class ExcelOption<T> : ExcelOption where T : class
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

public class ExcelExportOption<T> : ExcelOption<T> where T : class
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

    /// <summary>
    /// 多个动态导出
    /// </summary>
    public IEnumerable<IExportDynamicWrite<T>>? DynamicExports { get; set; }
}

public class ExcelExportSheetOption<T> where T : class
{
    /// <summary>
    /// 合并区域(导出时使用)
    /// </summary>
    public IEnumerable<MergedRegion>? MergedRegions { get; set; }

    /// <summary>
    /// 动态导出
    /// </summary>
    public IExportDynamicWrite<T>? DynamicExport { get; set; }

    /// <summary>
    /// 多个动态导出
    /// </summary>
    public IEnumerable<IExportDynamicWrite<T>>? DynamicExports { get; set; }
}

public class ExcelImportOption<T> : ExcelOption<T> where T : class
{
    /// <summary>
    /// 校验导入字段
    /// </summary>
    public bool ValidImportField { get; set; } = true;

    /// <summary>
    /// 忽略导入字段
    /// </summary>
    public Func<string, bool>? IgnoreField { get; set; }

    /// <summary>
    /// 动态导出
    /// </summary>
    public IImportDynamicRead<T>? DynamicImport { get; set; }

    /// <summary>
    /// 动态导出数组
    /// </summary>

    public IEnumerable<IImportDynamicRead<T>>? DynamicImports { get; set; }
}