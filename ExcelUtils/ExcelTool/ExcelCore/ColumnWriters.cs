using ExcelTool.Converters;

namespace ExcelTool.ExcelCore;

/// <summary>
/// Writer 列接口：暴露表头、布局信息，并可从行对象提取单元格数据。
/// </summary>
public interface IExcelColumnWriter
{
    /// <summary>Excel 列名（表头）。</summary>
    string ColumnName { get; }
    /// <summary>列顺序（越小越左）。</summary>
    int Order { get; }
    /// <summary>列宽（字符数），0=默认，&gt;0=固定，&lt;0=自适应。</summary>
    int Width { get; }
    /// <summary>关联的转换器。</summary>
    ExcelConverter? Converter { get; }
    /// <summary>从行数据对象获取该列的单元格值。</summary>
    object? GetValue(object obj);
}

/// <summary>
/// 委托驱动的 Writer：通过 Func 委托取值，无需 PropertyInfo，可随意定制。
/// 静态工厂 <c>FromDict</c> / <c>FromList</c> / <c>FromArray</c> 提供常用集合场景的快捷创建。
/// </summary>
public class DelegateColumnWriter<T> : IExcelColumnWriter
{
    private readonly Func<T, object?> _getter;

    private DelegateColumnWriter(string columnName, int order, int width,
        Func<T, object?> getter, ExcelConverter? converter)
    {
        ColumnName = columnName;
        Order = order;
        Width = width;
        _getter = getter;
        Converter = converter;
    }

    // ==================== 工厂方法 ====================

    /// <summary>从 Dictionary 批量创建：每列一个 Writer，GetValue 按 key 取值。</summary>
    public static DelegateColumnWriter<T>[] FromDict<TValue>(
        string[] columnNames, Func<T, Dictionary<string, TValue>> getDict,
        int orderStart = 0, int width = 0, ExcelConverter? converter = null)
    {
        return columnNames.Select((name, i) =>
            new DelegateColumnWriter<T>(name, orderStart + i, width,
                obj => getDict(obj).TryGetValue(name, out var v) ? v : null,
                converter))
            .ToArray();
    }

    /// <summary>从 List 批量创建：每列一个 Writer，GetValue 按索引取值。</summary>
    public static DelegateColumnWriter<T>[] FromList<TValue>(
        string[] columnNames, Func<T, IList<TValue>> getList,
        int orderStart = 0, int width = 0, ExcelConverter? converter = null)
    {
        return columnNames.Select((name, i) =>
        {
            var idx = i;
            return new DelegateColumnWriter<T>(name, orderStart + i, width,
                obj => { var list = getList(obj); return idx < list.Count ? list[idx] : null; },
                converter);
        }).ToArray();
    }

    /// <summary>从 Array 批量创建：每列一个 Writer，GetValue 按索引取值。</summary>
    public static DelegateColumnWriter<T>[] FromArray<TValue>(
        string[] columnNames, Func<T, TValue[]> getArray,
        int orderStart = 0, int width = 0, ExcelConverter? converter = null)
    {
        return columnNames.Select((name, i) =>
        {
            var idx = i;
            return new DelegateColumnWriter<T>(name, orderStart + i, width,
                obj => { var arr = getArray(obj); return idx < arr.Length ? arr[idx] : null; },
                converter);
        }).ToArray();
    }

    // ==================== IExcelColumnWriter ====================

    /// <inheritdoc/>
    public string ColumnName { get; }

    /// <inheritdoc/>
    public int Order { get; }

    /// <inheritdoc/>
    public int Width { get; }

    /// <inheritdoc/>
    public ExcelConverter? Converter { get; set; }

    /// <inheritdoc/>
    public object? GetValue(object obj)
    {
        return obj is T t ? _getter(t) : null;
    }
}
