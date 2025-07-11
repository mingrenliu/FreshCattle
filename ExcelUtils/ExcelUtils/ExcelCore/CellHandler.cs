namespace ExcelUtile.ExcelCore;

/// <summary>
/// 动态导出操作
/// </summary>
/// <typeparam name="T"> </typeparam>
internal class DynamicExportCellHandler<T> : IExportCellHandler<T> where T : class
{
    public ColumnInfo Info { get; private set; }
    private ICellStyle? _style;
    public int Order => Info.Order;

    private readonly ICellWriter<T> _export;

    public DynamicExportCellHandler(ICellWriter<T> export, ColumnInfo info)
    {
        _export = export;
        Info = info;
    }

    public void WriteToCell(ICell cell, T value, IConverterFactory factory, ICellStyle? style = null)
    {
        _export.WriteToCell(value, cell, Info.Name, factory, style ?? GetCellStyle(cell));
    }

    ICellStyle? GetCellStyle(ICell cell)
    {
        _style ??= Info.FormatCellStyle?.Invoke(cell);
        return _style;
    }

    public static IEnumerable<DynamicExportCellHandler<T>> Create(ICellWriter<T> data)
    {
        foreach (var item in data.Headers())
        {
            yield return new DynamicExportCellHandler<T>(data, item);
        }
    }
}

/// <summary>
/// 准确导入操作
/// </summary>
/// <typeparam name="T"> </typeparam>
internal class ExactImportCellHandler<T> : IExactImportCellHandler<T> where T : class
{
    public ColumnInfo Info { get; private set; }
    private readonly ICellReader<T> _import;

    public ExactImportCellHandler(ICellReader<T> import, ColumnInfo info)
    {
        _import = import;
        Info = info;
    }

    public void ReadFromCell(ICell cell, T value, string title, IConverterFactory factory)
    {
        _import.ReadFromCell(value, cell, Info.Name, factory);
    }

    public static IEnumerable<ExactImportCellHandler<T>> Create(IExactCellReader<T> data)
    {
        foreach (var item in data.Headers())
        {
            yield return new ExactImportCellHandler<T>(data, item);
        }
    }

    public bool Match(string title) => Info.Name == title;
}

/// <summary>
/// 动态导入操作
/// </summary>
/// <typeparam name="T"> </typeparam>
internal class DynamicImportCellHandler<T> : IImportCellHandler<T> where T : class
{
    private readonly IDynamicCellReader<T> _import;

    public DynamicImportCellHandler(IDynamicCellReader<T> import)
    {
        _import = import;
    }

    public void ReadFromCell(ICell cell, T value, string title, IConverterFactory factory)
    {
        _import.ReadFromCell(value, cell, title, factory);
    }

    public static DynamicImportCellHandler<T> Create(IDynamicCellReader<T> data)
    {
        return new DynamicImportCellHandler<T>(data);
    }

    public bool Match(string title) => _import.Match(title);
}

/// <summary>
/// 默认的类属性导入导出处理器
/// </summary>
/// <typeparam name="T"> </typeparam>
internal class DefaultCellHandler<T> : IExactImportCellHandler<T>, IExportCellHandler<T> where T : class
{
    public ColumnInfo Info { get => _info; }

    public int Order => Info.Order;

    private readonly PropertyTypeInfo _info;

    private ICellStyle? _style;

    public DefaultCellHandler(PropertyTypeInfo info)
    {
        _info = info;
    }

    public void WriteToCell(ICell cell, T value, IConverterFactory factory, ICellStyle? style = null)
    {
        var propertyValue = _info.Info.GetValue(value);
        Info.GetConverter(factory)?.WriteCell(cell, propertyValue, style ?? GetCellStyle(cell));
    }

    public void ReadFromCell(ICell cell, T value, string title, IConverterFactory factory)
    {
        var propertyValue = Info.GetConverter(factory).ReadCell(cell);
        if (propertyValue != null)
        {
            try
            {
                _info.Info.SetValue(value, Convert.ChangeType(propertyValue, _info.BaseType));
            }
            catch (Exception)
            {
            }
        }
    }

    ICellStyle? GetCellStyle(ICell cell)
    {
        _style ??= Info.FormatCellStyle?.Invoke(cell);
        return _style;
    }

    public bool Match(string title) => title == Info.Name;
}