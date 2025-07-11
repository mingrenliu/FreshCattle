namespace ExcelUtile.ExcelCore;

public class DictionaryWrapper<T> where T : class
{
    private readonly Dictionary<string, T> _dic;

    public DictionaryWrapper()
    {
        _dic = new();
    }

    public DictionaryWrapper(IEnumerable<T> data, Func<T, string> selector)
    {
        _dic = new();
        foreach (var item in data)
        {
            _dic[selector.Invoke(item)] = item;
        }
    }

    public IEnumerable<T> Values => _dic.Values;

    public void Add(string key, T value)
    {
        _dic[key] = value;
    }

    public void AddRange(IEnumerable<T> data, Func<T, string> selector)
    {
        foreach (var item in data)
        {
            _dic[selector.Invoke(item)] = item;
        }
    }

    public void Remove(string key)
    {
        _dic.Remove(key);
    }

    public bool ContainKey(string key)
    {
        return _dic.ContainsKey(key);
    }

    public T? this[string name]
    {
        get { return _dic.TryGetValue(name, out T? value) ? value : null; }
    }
}