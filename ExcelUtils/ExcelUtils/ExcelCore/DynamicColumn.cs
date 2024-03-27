namespace ExcelUtile.ExcelCore;

/// <summary>
/// 列信息
/// </summary>
public class ColumnInfo
{
    /// <summary>
    /// 字段展示名称
    /// </summary>
    public virtual string Name { get; }

    private int? _width;

    /// <summary>
    /// 列宽
    /// </summary>
    public virtual int? Width { get => _width <= 0 ? null : _width; private set => _width = value; }

    /// <summary>
    /// 导出字段顺序
    /// </summary>
    public virtual int Order { get; }

    /// <summary>
    /// 字段是必须的(导入时没有该字段,会报错)
    /// </summary>
    public virtual bool IsRequired { get; }

    public ColumnInfo(string name, int order = 0, bool isRequired = true, int width = 0)
    {
        Name = name;
        IsRequired = isRequired;
        Order = order;
        Width = width;
    }
}

public interface IDynamicHeader<T>
{
    IEnumerable<ColumnInfo> Headers();
}

public interface IImportDynamicRead<T> : IDynamicHeader<T>
{
    void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory);
}

public interface IExportDynamicWrite<T> : IDynamicHeader<T>
{
    void WriteToCell(T obj, ICell cell, string field, IConverterFactory factory);
}

public class ArrayDynamicHandler<T, Element> : IExportDynamicWrite<T>, IImportDynamicRead<T>
{
    private readonly Dictionary<string, uint> _dic = new();
    private readonly IEnumerable<ColumnInfo> _columns;
    private readonly Func<T, Element[]> _selector;
    private readonly Type _baseType;

    public ArrayDynamicHandler(IEnumerable<ColumnInfo> columns, Func<T, Element[]> selector)
    {
        _baseType = Nullable.GetUnderlyingType(typeof(Element)) ?? typeof(Element);
        _selector = selector;
        _columns = columns;
        uint i = 0;
        foreach (var item in columns)
        {
            if (_dic.ContainsKey(item.Name))
            {
                throw new Exception("动态列名称存在重复");
            }
            _dic[item.Name] = i++;
        }
    }

    public IEnumerable<ColumnInfo> Headers() => _columns;

    public void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        if (_dic.TryGetValue(field, out uint index))
        {
            var value = factory.GetDefaultConverter(_baseType).ReadCell(cell);
            Element[] values = _selector.Invoke(obj);
            if (value != null && values.Length > index)
            {
                try
                {
                    values[index] = (Element)Convert.ChangeType(value, _baseType);
                }
                catch (Exception)
                {
                }
            }
        }
    }

    public void WriteToCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        if (_dic.TryGetValue(field, out uint index))
        {
            var values = _selector.Invoke(obj);
            if (index < values.Length)
            {
                var value = values[index];
                factory.GetDefaultConverter(_baseType)?.WriteCell(cell, value);
            }
        }
    }
}

public class DictionaryDynamicHandler<T, Element> : IExportDynamicWrite<T>, IImportDynamicRead<T>
{
    private readonly IEnumerable<ColumnInfo> _columns;
    private readonly Func<T, Dictionary<string, Element>> _selector;
    private readonly Type _baseType;

    public DictionaryDynamicHandler(IEnumerable<ColumnInfo> columns, Func<T, Dictionary<string, Element>> selector)
    {
        _baseType = Nullable.GetUnderlyingType(typeof(Element)) ?? typeof(Element);
        _selector = selector;
        _columns = columns;
        var names = new HashSet<string>();
        foreach (var item in columns)
        {
            if (names.Add(item.Name) is false)
            {
                throw new Exception("动态列名称存在重复");
            }
        }
    }

    public IEnumerable<ColumnInfo> Headers() => _columns;

    public void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        var value = factory.GetDefaultConverter(_baseType).ReadCell(cell);
        var values = _selector.Invoke(obj);
        if (value != null)
        {
            try
            {
                values[field] = (Element)Convert.ChangeType(value, _baseType);
            }
            catch (Exception)
            {
            }
        }
    }

    public void WriteToCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        var values = _selector.Invoke(obj);
        if (values.TryGetValue(field, out var value))
        {
            factory.GetDefaultConverter(_baseType)?.WriteCell(cell, value);
        }
    }
}