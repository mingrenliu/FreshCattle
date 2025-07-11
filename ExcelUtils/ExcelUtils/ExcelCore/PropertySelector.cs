using NPOI.SS.Formula.Functions;
using System.Reflection;

namespace ExcelUtile.ExcelCore;

internal static class DefaultPropertySelector
{
    public static IEnumerable<PropertyTypeInfo> GetTypeInfo(Type type)
    {
        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                yield return new DefaultPropertyInfo(property, attribute);
            }
        }
    }

    public static IEnumerable<PropertyTypeInfo> GetTypeInfo<T>()
    {
        return GetTypeInfo(typeof(T));
    }
}

public class MapOverridePropertySelector
{
    private readonly Dictionary<string, string> _dic;
    private readonly bool _strict = false;

    /// <summary>
    /// 映射覆盖属性选择器
    /// </summary>
    /// <param name="dic"> 自定义字段Title </param>
    /// <param name="strict"> </param>
    public MapOverridePropertySelector(Dictionary<string, string> dic, bool strict = false)
    {
        _dic = dic;
        _strict = strict;
    }

    public IEnumerable<PropertyTypeInfo> GetTypeInfo(Type type)
    {
        foreach (var property in type.GetProperties())
        {
            if (_dic.TryGetValue(property.Name, out var name) || !_strict)
            {
                var attribute = property.GetCustomAttribute<DisplayAttribute>();
                if (attribute != null)
                {
                    yield return new DefaultPropertyInfo(property, attribute, name);
                }
            }
        }
    }

    public IEnumerable<PropertyTypeInfo> GetTypeInfo<T>()
    {
        return GetTypeInfo(typeof(T));
    }
}

public class DynamicExportSelector
{
    private IEnumerable<IExportInfo> B_Columns;

    /// <summary>
    /// </summary>
    /// <param name="columns"> </param>
    public DynamicExportSelector(IEnumerable<IExportInfo> columns)
    {
        B_Columns = columns;
    }

    public IEnumerable<PropertyTypeInfo> GetTypeInfo(Type type, Dictionary<string, string>? dic = null)
    {
        dic ??= new Dictionary<string, string>();
        B_Columns = B_Columns?.Where(x => !string.IsNullOrWhiteSpace(x.GetTitle()) && !string.IsNullOrWhiteSpace(x.GetTitle())) ?? Enumerable.Empty<ColumnModel>();
        var results = new List<PropertyTypeInfo>();
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.SetProperty).ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        var i = 1;
        foreach (var item in B_Columns)
        {
            if (props.TryGetValue(item.GetField(), out var info))
            {
                var name = dic.ContainsKey(item.GetField()) ? dic[item.GetField()] : item.GetTitle();
                results.Add(new PropertyTypeInfo(info, name, i++) { DynamicWidth = true });
            }
        }
        return results;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="dic"> 自定义字段Title </param>
    /// <returns> </returns>
    public IEnumerable<PropertyTypeInfo> GetTypeInfo<T>(Dictionary<string, string>? dic = null) where T : class
    {
        return GetTypeInfo(typeof(T), dic);
    }
}

public class ColumnModel : IExportInfo
{
    /// <summary>
    /// Tile
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 字段名
    /// </summary>
    public string DataIndex { get; set; }

    /// <summary>
    /// 获取字段名
    /// </summary>
    /// <returns> </returns>

    public string GetField() => DataIndex;

    /// <summary>
    /// 获取标题
    /// </summary>
    /// <returns> </returns>

    public string GetTitle() => Title;
}

/// <summary>
/// 列信息
/// </summary>
public interface IExportInfo
{
    /// <summary>
    /// 获取标题
    /// </summary>
    /// <returns> </returns>
    string GetTitle();

    /// <summary>
    /// 获取字段名
    /// </summary>
    /// <returns> </returns>

    string GetField();
}