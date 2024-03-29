using ExcelUtile.Formats;

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
    /// 基础类型
    /// </summary>
    public Type BaseType { get; private set; }

    /// <summary>
    /// 字段是必须的(导入时没有该字段,会报错)
    /// </summary>
    public virtual bool IsRequired { get; }

    /// <summary>
    /// 自定义类型转换器
    /// </summary>

    public ExcelConverter? Converter { get; set; }

    public ColumnInfo(string name, Type type, int order = 0, bool isRequired = true, int width = 0)
    {
        BaseType = Nullable.GetUnderlyingType(type) ?? type;
        Name = name;
        IsRequired = isRequired;
        Order = order;
        Width = width;
    }

    public ExcelConverter? GetConverter(IConverterFactory _factory)
    {
        return Converter ?? _factory.GetDefaultConverter(BaseType);
    }
}

public interface IExactHeader<T> : IDynamicHeader<T>
{
    IEnumerable<ColumnInfo> Headers();
}

public interface IDynamicHeader<T>
{
    bool Match(string field);
}

public interface IImportDynamicRead<T> : IDynamicHeader<T>
{
    void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory);
}

public interface IImportExactRead<T> : IExactHeader<T>, IImportDynamicRead<T>
{
}

public interface IExportDynamicWrite<T> : IExactHeader<T>
{
    void WriteToCell(T obj, ICell cell, string field, IConverterFactory factory);
}

public class ListDynamicHandler<T, Element> : IExportDynamicWrite<T>, IImportExactRead<T>
{
    private readonly Dictionary<string, int> _dic = new();
    private readonly List<ColumnInfo> _columns;
    private readonly Func<T, List<Element>> _selector;

    public ListDynamicHandler(IEnumerable<ColumnInfo> columns, Func<T, List<Element>> selector)
    {
        _selector = selector;
        _columns = columns.ToList();
        for (int i = 0; i < _columns.Count; i++)
        {
            var item = _columns[i];
            if (_dic.ContainsKey(item.Name))
            {
                throw new Exception("动态列名称存在重复");
            }
            _dic[item.Name] = i;
        }
    }

    public IEnumerable<ColumnInfo> Headers() => _columns;

    public bool Match(string field) => _dic.ContainsKey(field);

    public void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        if (_dic.TryGetValue(field, out int index))
        {
            var info = _columns[index];
            var value = info.GetConverter(factory).ReadCell(cell);
            List<Element> values = _selector.Invoke(obj);
            if (values.Count == 0)
            {
                for (int i = 0; i < _columns.Count; i++)//create same lentgh list
                {
                    values.Add(default!);
                }
            }
            if (value != null && values.Count > index)
            {
                try
                {
                    values[index] = (Element)Convert.ChangeType(value, info.BaseType);
                }
                catch (Exception)
                {
                }
            }
        }
    }

    public void WriteToCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        if (_dic.TryGetValue(field, out int index))
        {
            var values = _selector.Invoke(obj);
            if (index < values.Count)
            {
                var value = values[index];
                _columns[index].GetConverter(factory).WriteCell(cell, value);
            }
            else
            {
                _columns[index].GetConverter(factory).WriteCell(cell, null);
            }
        }
    }
}

public class DictionaryDynamicImportHandler<T, Element> : IImportDynamicRead<T>
{
    private readonly Func<T, Dictionary<string, Element>> _selector;
    private readonly Type _type = typeof(Element);

    public DictionaryDynamicImportHandler(Func<T, Dictionary<string, Element>> selector)
    {
        _selector = selector;
    }

    public bool Match(string field) => true;

    public void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        var value = factory.GetDefaultConverter(_type)?.ReadCell(cell);
        var values = _selector.Invoke(obj);
        if (value != null)
        {
            try
            {
                values[field] = (Element)Convert.ChangeType(value, _type);
            }
            catch (Exception)
            {
            }
        }
    }
}

public class DictionaryDynamicHandler<T, Element> : IExportDynamicWrite<T>, IImportExactRead<T>
{
    private readonly IEnumerable<ColumnInfo> _columns;
    private readonly Func<T, Dictionary<string, Element>> _selector;
    private readonly Dictionary<string, ColumnInfo> _dic = new();

    public DictionaryDynamicHandler(IEnumerable<ColumnInfo> columns, Func<T, Dictionary<string, Element>> selector)
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

    public bool Match(string field) => _dic.ContainsKey(field);

    public void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        if (_dic.TryGetValue(field, out var info) is false)
        {
            return;
        }
        var value = info.GetConverter(factory).ReadCell(cell);
        var values = _selector.Invoke(obj);
        if (value != null)
        {
            try
            {
                values[field] = (Element)Convert.ChangeType(value, info.BaseType);
            }
            catch (Exception)
            {
            }
        }
    }

    public void WriteToCell(T obj, ICell cell, string field, IConverterFactory factory)
    {
        var values = _selector.Invoke(obj);
        if (_dic.TryGetValue(field, out var info))
        {
            if (values.TryGetValue(field, out var value))
            {
                info.GetConverter(factory).WriteCell(cell, value);
            }
            else
            {
                info.GetConverter(factory).WriteCell(cell, null);
            }
        }
    }
}