using System.Collections;

namespace ExcelUtile.ExcelCore;

public class HeaderInfo
{
    private readonly string _name;
    public string Name => _name;
    public bool IsMerged { get; set; } = false;
    private readonly int _order;
    public int Order => _order;

    public HeaderInfo(string name, int order)
    {
        _name = name;
        _order = order;
    }
}
public class KeyValueWrapper<T> where T : class
{
    private readonly Dictionary<string, T> _dic;
    public KeyValueWrapper()
    {
        _dic = new();
    }
    public KeyValueWrapper(IEnumerable<T> data,Func<T,string> selector)
    {
        _dic = new();
        foreach (var item in data)
        {
            _dic[selector.Invoke(item)] = item;
        }
    }
    public void Add(string key, T value)
    {
        _dic[key] = value;
    }
    public T? this[string name]
    {
        get { return _dic.TryGetValue(name, out T? value) ? value : null ; } 
    }
}
public class SortedWrapper<T> :IEnumerable<T> where T : class
{
    private readonly SortedList<int,T> _list;
    public SortedWrapper()
    {
        _list = new();
    }
    public SortedWrapper(IEnumerable<T> data, Func<T, int> selector)
    {
        _list = new();
        foreach (var item in data)
        {
            Add(selector.Invoke(item), item);
        }
    }
    public void Add(int order,T item)
    {
        _list.TryAdd(order,item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in _list)
        {
            yield return item.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var item in _list)
        {
            yield return item.Value;
        }
    }
}