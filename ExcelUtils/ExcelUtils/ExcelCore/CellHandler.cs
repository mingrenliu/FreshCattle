namespace ExcelUtile.ExcelCore;

public interface IColumnBaseInfo
{
    ColumnInfo Info { get; }
}

public interface IExportCellHandler<T> : IColumnBaseInfo where T : class
{
    public int Order { get; }

    void WriteToCell(ICell cell, T value, IConverterFactory factory);
}

public interface IImportCellHandler<T> : IColumnBaseInfo where T : class
{
    void ReadFromCell(ICell cell, T value, IConverterFactory factory);
}

public class DynamicExportCellHandler<T> : IExportCellHandler<T> where T : class
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

public class DynamicImportCellHandler<T> : IImportCellHandler<T> where T : class
{
    public ColumnInfo Info { get; private set; }
    private readonly IImportDynamicRead<T> _import;

    public DynamicImportCellHandler(IImportDynamicRead<T> import, ColumnInfo info)
    {
        _import = import;
        Info = info;
    }

    public void ReadFromCell(ICell cell, T value, IConverterFactory factory)
    {
        _import.ReadFromCell(value, cell, Info.Name, factory);
    }

    public static IEnumerable<DynamicImportCellHandler<T>> Create(IImportDynamicRead<T> data)
    {
        foreach (var item in data.Headers())
        {
            yield return new DynamicImportCellHandler<T>(data, item);
        }
    }
}

public class PropertyCellHandler<T> : IExportCellHandler<T>, IImportCellHandler<T> where T : class
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
        factory.GetDefaultConverter(_info.BaseType)?.WriteCell(cell, propertyValue);
    }

    public void ReadFromCell(ICell cell, T value, IConverterFactory factory)
    {
        var propertyValue = factory.GetDefaultConverter(_info.BaseType).ReadCell(cell);
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
}