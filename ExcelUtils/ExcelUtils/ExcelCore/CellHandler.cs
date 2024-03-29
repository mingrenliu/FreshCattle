namespace ExcelUtile.ExcelCore;

internal interface IColumnBaseInfo
{
    ColumnInfo Info { get; }
}

internal interface IExportCellHandler<T> : IColumnBaseInfo where T : class
{
    public int Order { get; }

    void WriteToCell(ICell cell, T value, IConverterFactory factory);
}

internal interface IExactImportCellHandler<T> : IImportCellHandler<T>, IColumnBaseInfo where T : class
{
}

internal interface IImportCellHandler<T> : IDynamicHeader<T> where T : class
{
    void ReadFromCell(ICell cell, T value, string field, IConverterFactory factory);
}

internal class DynamicExportCellHandler<T> : IExportCellHandler<T> where T : class
{
    public ColumnInfo Info { get; private set; }

    public int Order => Info.Order;

    private readonly IExportDynamicWrite<T> _export;

    public DynamicExportCellHandler(IExportDynamicWrite<T> export, ColumnInfo info)
    {
        _export = export;
        Info = info;
    }

    public void WriteToCell(ICell cell, T value, IConverterFactory factory)
    {
        _export.WriteToCell(value, cell, Info.Name, factory);
    }

    public static IEnumerable<DynamicExportCellHandler<T>> Create(IExportDynamicWrite<T> data)
    {
        foreach (var item in data.Headers())
        {
            yield return new DynamicExportCellHandler<T>(data, item);
        }
    }
}

internal class ExactImportCellHandler<T> : IExactImportCellHandler<T> where T : class
{
    public ColumnInfo Info { get; private set; }
    private readonly IImportDynamicRead<T> _import;

    public ExactImportCellHandler(IImportDynamicRead<T> import, ColumnInfo info)
    {
        _import = import;
        Info = info;
    }

    public void ReadFromCell(ICell cell, T value, string field, IConverterFactory factory)
    {
        _import.ReadFromCell(value, cell, Info.Name, factory);
    }

    public static IEnumerable<ExactImportCellHandler<T>> Create(IImportExactRead<T> data)
    {
        foreach (var item in data.Headers())
        {
            yield return new ExactImportCellHandler<T>(data, item);
        }
    }

    public bool Match(string field) => Info.Name == field;
}

internal class DynamicImportCellHandler<T> : IImportCellHandler<T> where T : class
{
    private readonly IImportDynamicRead<T> _import;

    public DynamicImportCellHandler(IImportDynamicRead<T> import)
    {
        _import = import;
    }

    public void ReadFromCell(ICell cell, T value, string field, IConverterFactory factory)
    {
        _import.ReadFromCell(value, cell, field, factory);
    }

    public static DynamicImportCellHandler<T> Create(IImportDynamicRead<T> data)
    {
        return new DynamicImportCellHandler<T>(data);
    }

    public bool Match(string field) => _import.Match(field);
}

internal class PropertyCellHandler<T> : IExactImportCellHandler<T>, IExportCellHandler<T> where T : class
{
    public ColumnInfo Info { get => _info; }

    public int Order => Info.Order;

    private readonly PropertyTypeInfo _info;

    public PropertyCellHandler(PropertyTypeInfo info)
    {
        _info = info;
    }

    public void WriteToCell(ICell cell, T value, IConverterFactory factory)
    {
        var propertyValue = _info.Info.GetValue(value);
        Info.GetConverter(factory)?.WriteCell(cell, propertyValue);
    }

    public void ReadFromCell(ICell cell, T value, string field, IConverterFactory factory)
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

    public bool Match(string field) => field == Info.Name;
}