using ExcelTool.Converters;

namespace ExcelTool.ExcelCore;

/// <summary>
/// Reader 列接口：匹配表头、将单元格数据写入行对象。
/// </summary>
public interface IExcelColumnReader
{
    /// <summary>导入时是否必填。</summary>
    bool Required { get; }
    /// <summary>关联的转换器。</summary>
    ExcelConverter? Converter { get; }
    /// <summary>判断该列是否匹配给定的表头名称。</summary>
    bool Match(string columnName);
    /// <summary>将单元格数据写入行对象（columnName 供委托列使用）。</summary>
    void SetValue(object obj, string columnName, object? value);
}

/// <summary>
/// 集合列 Reader：一个实例处理集合（Dict/List/Array）中所有列。
/// 通过 <c>FromDict</c> / <c>FromList</c> / <c>FromArray</c> 工厂方法创建。
/// </summary>
public class CollectionColumnReader<T> : IExcelColumnReader
{
    private readonly Action<T, string, object?> _setter;
    private readonly HashSet<string>? _knownColumns; // null → 匹配任意列名（Dict 模式）

    private CollectionColumnReader(Action<T, string, object?> setter,
        HashSet<string>? knownColumns, ExcelConverter? converter)
    {
        _setter = setter;
        _knownColumns = knownColumns;
        Converter = converter;
    }

    // ---- 工厂方法 ----

    /// <summary>从 Dictionary 创建：Match 匹配任意列名，SetValue 按 key 写入。</summary>
    public static CollectionColumnReader<T> FromDict<TValue>(IEnumerable<string>? columnNames,
        Func<T, Dictionary<string, TValue>> getDict, ExcelConverter? converter = null)
    {
        return new CollectionColumnReader<T>(
            (obj, col, val) => { if (val is TValue tv) getDict(obj)[col] = tv; },
            columnNames?.ToHashSet(StringComparer.OrdinalIgnoreCase), converter);
    }

    /// <summary>从 List 创建：Match 仅匹配指定列名，SetValue 按列名→索引写入。</summary>
    public static CollectionColumnReader<T> FromList<TValue>(
        string[] columnNames, Func<T, IList<TValue>> getList, ExcelConverter? converter = null)
    {
        var indexMap = columnNames
            .Select((name, i) => (name, i))
            .ToDictionary(x => x.name, x => x.i, System.StringComparer.OrdinalIgnoreCase);

        return new CollectionColumnReader<T>(
            (obj, col, val) =>
            {
                if (obj is not T t || val is not TValue tv) return;
                if (!indexMap.TryGetValue(col, out var idx)) return;
                var list = getList(t);
                while (list.Count <= idx) list.Add(default!);
                list[idx] = tv;
            },
            new HashSet<string>(columnNames, System.StringComparer.OrdinalIgnoreCase),
            converter);
    }

    /// <summary>从 Array 创建：Match 仅匹配指定列名，SetValue 按列名→索引写入。</summary>
    public static CollectionColumnReader<T> FromArray<TValue>(
        string[] columnNames, Func<T, TValue[]> getArray, ExcelConverter? converter = null)
    {
        var indexMap = columnNames
            .Select((name, i) => (name, i))
            .ToDictionary(x => x.name, x => x.i, System.StringComparer.OrdinalIgnoreCase);

        return new CollectionColumnReader<T>(
            (obj, col, val) =>
            {
                if (obj is not T t || val is not TValue tv) return;
                if (!indexMap.TryGetValue(col, out var idx)) return;
                var arr = getArray(t);
                if (idx < arr.Length) arr[idx] = tv;
            },
            new HashSet<string>(columnNames, System.StringComparer.OrdinalIgnoreCase),
            converter);
    }

    // ==================== IExcelColumnReader ====================

    /// <inheritdoc/>
    public bool Required => false;

    /// <inheritdoc/>
    public ExcelConverter? Converter { get; set; }

    /// <inheritdoc/>
    public bool Match(string columnName)
        => _knownColumns == null || _knownColumns.Contains(columnName);

    /// <inheritdoc/>
    public void SetValue(object obj, string columnName, object? value)
    {
        if (obj is T t) _setter(t, columnName, value);
    }

    /// <inheritdoc/>
    public override string ToString() => _knownColumns == null ? "Collection(*)" : $"Collection({_knownColumns.Count} cols)";
}

/// <summary>
/// 委托 Reader：通过委托直接实现 <see cref="IExcelColumnReader"/>，无反射、无集合逻辑。
/// 构造时传入 <c>setter</c>、<c>columnName</c>、<c>converter</c> 即可。
/// </summary>
public class DelegateColumnReader<T> : IExcelColumnReader
{
    private readonly Action<T, object?> _setter;

    public DelegateColumnReader(Action<T, object?> setter,
        string column, ExcelConverter? converter)
    {
        _setter = setter;
        ColumnName = column ;
        Converter = converter;
    }

    public string ColumnName { get; }

    /// <inheritdoc/>
    public bool Required => false;

    /// <inheritdoc/>
    public ExcelConverter? Converter { get; set; }

    /// <inheritdoc/>
    public bool Match(string columnName)=>
        string.Equals(ColumnName, columnName, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public void SetValue(object obj, string columnName, object? value)
    {
        if (obj is T t) _setter(t, value);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Delegate({ColumnName})";
}
