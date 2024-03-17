namespace ExcelUtile.ExcelCore;

public class ExcelSerializeOptions
{
    /// <summary>
    /// header row index (0-based)
    /// </summary>
    public int HeaderLineIndex { get; set; } = 0;

    private int startLine = 1;

    /// <summary>
    /// 默认列宽
    /// </summary>
    public int DefaultColumnWidth { get; set; } = 15;

    /// <summary>
    /// 校验导入字段
    /// </summary>
    public bool ValidImportField { get; set; } = true;

    /// <summary>
    /// data start row index (0-based)
    /// </summary>
    public int StartLineIndex { get => Math.Max(startLine, HeaderLineIndex + 1); set => startLine = value; }

    /// <summary>
    /// 合并区域(导出时使用)
    /// </summary>
    public IEnumerable<MergedRegion>? MergedRegions { get; set; }

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