using System.Reflection;
using ExcelTool.Converters;

namespace ExcelTool.ExcelCore;

/// <summary>
/// Reader 专用的类型元数据：仅包含读取所需的列信息，表头匹配使用 <see cref="IExcelColumnReader.Match"/>。
/// </summary>
public class ExcelReaderTypeInfo
{
    public Type Type { get; }

    /// <summary>列列表（用于遍历匹配和 Required 校验）。</summary>
    public IReadOnlyList<IExcelColumnReader> Columns { get; }

    public ExcelReaderTypeInfo(Type type, IEnumerable<IExcelColumnReader> columns)
    {
        Type = type;
        Columns = columns.ToList().AsReadOnly();
    }
}

/// <summary>
/// Writer 专用的类型元数据：仅包含写入所需的列信息（按 Order 排序）。
/// </summary>
public class ExcelWriterTypeInfo
{
    public Type Type { get; }

    /// <summary>按 Order 排序的列列表。</summary>
    public IReadOnlyList<IExcelColumnWriter> Columns { get; }

    public ExcelWriterTypeInfo(Type type, IEnumerable<IExcelColumnWriter> columns)
    {
        Type = type;
        Columns = columns.OrderBy(p => p.Order).ThenBy(p => p.ColumnName).ToList().AsReadOnly();
    }
}

/// <summary>
/// 类型元数据解析器。一次反射解析，同时产出 Read / Write 两份元数据。
/// </summary>
public static class ExcelTypeInfoResolver
{
    /// <summary>解析读取专用元数据（反射属性 + 自定义 Reader）。</summary>
    public static ExcelReaderTypeInfo ResolveRead(Type type, ExcelOptions? options)
    {
        options ??= ExcelOptions.Default;
        // reader 需要字段读写，将从Excel读取的内容写入对象,并且对象字段可被应用读取,因此 canWrite=true
        var columns = ResolveColumns(type, options, true);
        var readColumns = columns.Select(c => (IExcelColumnReader)new ExcelColumnReader(c.Property, c.ColumnName, c.Required) { Converter = c.Converter }).ToList();

        // 追加自定义 Reader
        if (options.Readers != null)
            readColumns.AddRange(options.Readers);

        return new ExcelReaderTypeInfo(type, readColumns);
    }

    /// <summary>解析写入专用元数据（反射属性 + 自定义 Writer）。</summary>
    public static ExcelWriterTypeInfo ResolveWrite(Type type, ExcelOptions? options)
    {
        options ??= ExcelOptions.Default;
        // writer 只需要字段可读即可，将读取内容写入Excel,因此 canWrite=false
        var columns = ResolveColumns(type, options, false);
        var writeColumns = columns.Select(c => (IExcelColumnWriter)new ExcelColumnWriter(c.Property, c.ColumnName, c.Order, c.Width) { Converter = c.Converter }).ToList();

        // 追加自定义 Writer
        if (options.Writers != null)
            writeColumns.AddRange(options.Writers);

        return new ExcelWriterTypeInfo(type, writeColumns);
    }

    /// <summary>核心解析：反射遍历属性。</summary>
    public static List<RawColumnInfo> ResolveColumns(Type type, ExcelOptions options, bool canWrite = true)
    {
        var list = new List<RawColumnInfo>();
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        Func<PropertyInfo, bool> w = canWrite ? p => p.CanRead && p.CanWrite : p => p.CanRead;
        var props = type.GetProperties(bindingFlags).Where(w);
        var scope = options.FieldScope?.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in props)
        {
            var columnAttr = prop.GetCustomAttribute<ExcelColumnAttribute>();
            var ignoreAttr = prop.GetCustomAttribute<ExcelIgnoreAttribute>();
            var converterAttr = prop.GetCustomAttribute<ExcelConverterAttribute>();

            if (options.AutoInclude && ignoreAttr != null || !options.AutoInclude && columnAttr == null)
                continue;
            // 动态导出：只包含指定字段
            if (scope != null && !scope.Contains(prop.Name))
                continue;

            var columnName = options.ColumnNameMap?.GetValueOrDefault(prop.Name) ?? columnAttr?.Name ?? prop.Name;
            var order = columnAttr?.Order ?? 0;
            var width = columnAttr?.Width ?? 0;
            if (width == 0) width = GetTypeDefaultWidth(prop.PropertyType);
            var required = columnAttr?.Required ?? false;

            var converter = ResolveConverter(prop, converterAttr, options);

            list.Add(new RawColumnInfo(prop, columnName, order, width, required, converter));
        }

        return list;
    }

    public static ExcelConverter ResolveConverter(PropertyInfo prop,
        ExcelConverterAttribute? converterAttr, ExcelOptions options)
    {
        if (converterAttr != null)
        {
            var instance = converterAttr.Args is { Length: > 0 }
                ? Activator.CreateInstance(converterAttr.ConverterType, converterAttr.Args)
                : Activator.CreateInstance(converterAttr.ConverterType);
            if (instance is ExcelConverter custom)
                return custom;
        }

        return options.Converters.GetConverter(prop.PropertyType)!;
    }

    /// <summary>常见类型的默认列宽（字符数）。</summary>
    public static readonly Dictionary<Type, int> TypeDefaultWidths = new()
    {
        [typeof(string)] = 25,
        [typeof(DateTime)] = 25,
        [typeof(DateTimeOffset)] = 30,
        [typeof(DateOnly)] = 15,
        [typeof(TimeOnly)] = 15,
        [typeof(TimeSpan)] = 15,
        [typeof(Guid)] = 40,

    };

    private static int GetTypeDefaultWidth(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return TypeDefaultWidths.TryGetValue(type, out var w) ? w : 0;
    }
}

public class RawColumnInfo
{
    public PropertyInfo Property;
    public string ColumnName;
    public int Order;
    public int Width;
    public bool Required;
    public ExcelConverter? Converter;

    public RawColumnInfo(PropertyInfo property, string columnName,
        int order, int width, bool required, ExcelConverter? converter)
    {
        Property = property;
        ColumnName = columnName;
        Order = order;
        Width = width;
        Required = required;
        Converter = converter;
    }
}