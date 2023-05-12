using ExcelUtils.Formats;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ExcelUtils.ExcelCore;

public abstract class PropertyTypeInfo
{
    public virtual bool AutoAdapt { get; }
    public virtual int Width { get; }
    public virtual bool IsRequird { get; }
    public abstract int Order { get; }

    private readonly string _name;
    public string Name => _name;
    public PropertyInfo Info => _info;

    private readonly PropertyInfo _info;
    public DataFormatAttribute? DataFormats => Info.GetCustomAttribute<DataFormatAttribute>();

    protected PropertyTypeInfo(PropertyInfo info, string name)
    {
        _info = info;
        _name = name;
    }
}

internal class DefaultPropertyInfo : PropertyTypeInfo
{
    private readonly DisplayAttribute _display;

    public DefaultPropertyInfo(PropertyInfo info, DisplayAttribute attribute) : base(info, attribute.Name)
    {
        _display = attribute;
    }

    public override bool AutoAdapt => _display.AutoAdapt;

    public override int Width => _display.Width;

    public override bool IsRequird => _display.IsRequird;

    public override int Order => _display.Order;
}
internal class DefaultComparer : IComparer<PropertyTypeInfo>
{
    public int Compare(PropertyTypeInfo? x, PropertyTypeInfo? y)
    {
        return (x?.Order??0) -(y?.Order??0);
    }
}

public class InfoWrapper<T> : IEnumerable<T> where T : PropertyTypeInfo
{
    private readonly Dictionary<string, T> _dictionary = new();

    private readonly SortedSet<T> _sortedSet ;

    public InfoWrapper(IComparer<T>? compare = null)
    {
        compare ??=new DefaultComparer();
        _sortedSet = new SortedSet<T>(compare);
    }
    public void Add(T value)
    {
        _dictionary[value.Name] = value;
        _sortedSet.Add(value);
    }

    public T? this[string name]
    {
        get { return _dictionary.GetValueOrDefault(name); }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _sortedSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _sortedSet.GetEnumerator();
    }
}