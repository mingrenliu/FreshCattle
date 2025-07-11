namespace ExcelUtile.ExcelCore;

/// <summary>
/// 字典类型动态导入读取
/// </summary>
/// <typeparam name="T"> </typeparam>
/// <typeparam name="Element"> </typeparam>
public class DictionaryDynamicReader<T, Element> : IDynamicCellReader<T>
{
    private readonly Func<T, Dictionary<string, Element>> _selector;
    private readonly Type _type = typeof(Element);

    public DictionaryDynamicReader(Func<T, Dictionary<string, Element>> selector)
    {
        _selector = selector;
    }

    public bool Match(string title) => true;

    public void ReadFromCell(T obj, ICell cell, string title, IConverterFactory factory)
    {
        var value = factory.GetDefaultConverter(_type)?.ReadCell(cell);
        var values = _selector.Invoke(obj);
        if (value != null)
        {
            try
            {
                values[title] = (Element)Convert.ChangeType(value, _type);
            }
            catch (Exception)
            {
            }
        }
    }
}

/// <summary>
/// 字典类型根据表头信息准确读写
/// </summary>
/// <typeparam name="T"> </typeparam>
/// <typeparam name="Element"> </typeparam>
public class DictionaryReaderWriter<T, Element> : ICellWriter<T>, IExactCellReader<T>
{
    private readonly IEnumerable<ColumnInfo> _columns;
    private readonly Func<T, Dictionary<string, Element>> _selector;
    private readonly Dictionary<string, ColumnInfo> _dic = new();

    public DictionaryReaderWriter(IEnumerable<ColumnInfo> columns, Func<T, Dictionary<string, Element>> selector)
    {
        _selector = selector;
        _columns = columns;
        foreach (var item in columns)
        {
            if (_dic.TryAdd(item.Name, item) is false)
            {
                throw new Exception("动态列名称存在重复");
            }
        }
    }

    public IEnumerable<ColumnInfo> Headers() => _columns;

    public bool Match(string title) => _dic.ContainsKey(title);

    public void ReadFromCell(T obj, ICell cell, string title, IConverterFactory factory)
    {
        if (_dic.TryGetValue(title, out var info) is false)
        {
            return;
        }
        var value = info.GetConverter(factory).ReadCell(cell);
        var values = _selector.Invoke(obj);
        if (value != null)
        {
            try
            {
                values[title] = (Element)Convert.ChangeType(value, info.BaseType);
            }
            catch (Exception)
            {
            }
        }
    }

    public void WriteToCell(T obj, ICell cell, string title, IConverterFactory factory, ICellStyle? style = null)
    {
        var values = _selector.Invoke(obj);
        if (_dic.TryGetValue(title, out var info))
        {
            if (values.TryGetValue(title, out var value))
            {
                info.GetConverter(factory).WriteCell(cell, value, style);
            }
            else
            {
                info.GetConverter(factory).WriteCell(cell, null, style);
            }
        }
    }
}