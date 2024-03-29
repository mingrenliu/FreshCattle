using ExcelUtile.Formats;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 类型转化器
/// </summary>
public interface IConverterFactory
{
    public ExcelConverter? GetDefaultConverter(Type type);

    public IConverter<T>? GetDefaultConverter<T>() where T : notnull;

    public ExcelConverter? GetDefaultConverter(string type);
}

public class DefaultConverterFactory : IConverterFactory
{
    private const string DefaultType = "DefaultType";
    private readonly Dictionary<string, ExcelConverter> _defaultConverters = new();

    public ExcelConverter? GetDefaultConverter(Type type)
    {
        if (_defaultConverters.TryGetValue(type.Name, out ExcelConverter? value)) return value;
        return Add(type);
    }

    public IConverter<T>? GetDefaultConverter<T>() where T : notnull
    {
        var type = typeof(T);
        if (!_defaultConverters.TryGetValue(type.Name, out ExcelConverter? value))
        {
            value = Add(type);
        }
        return value == null ? null : value as IConverter<T>;
    }

    public ExcelConverter? GetDefaultConverter(string type)
    {
        if (_defaultConverters.TryGetValue(type, out ExcelConverter? value)) return value;
        return Add(type);
    }

    private ExcelConverter? Add(string type)
    {
        var result = Create(type);
        result ??= Create(DefaultType);
        return result;
        ExcelConverter? Create(string name)
        {
            if (defaultConverterMap.ContainsKey(name))
            {
                if (Activator.CreateInstance(defaultConverterMap[name]) is ExcelConverter converter)
                {
                    _defaultConverters[type] = converter;
                    return converter;
                }
            }
            return null;
        }
    }

    private ExcelConverter? Add(Type type)
    {
        return Add(type.Name);
    }

    private static readonly Dictionary<string, Type> defaultConverterMap = new()
    {
        [nameof(Double)] = typeof(DoubleFormat),
        [nameof(Int32)] = typeof(IntFormat),
        [nameof(Int64)] = typeof(LongFormat),
        [nameof(Decimal)] = typeof(DecimalFormat),
        [nameof(TimeSpan)] = typeof(TimeSpanFormat),
        [nameof(DateOnly)] = typeof(DateOnlyFormat),
        [nameof(TimeOnly)] = typeof(TimeOnlyFormat),
        [nameof(DateTimeOffset)] = typeof(DateTimeOffsetFormat),
        [nameof(DateTime)] = typeof(ShortTimeFormat),
        [nameof(Boolean)] = typeof(BooleanFormat),
        [nameof(Single)] = typeof(FloatFormat),
        [nameof(String)] = typeof(StringFormat),
        [nameof(Int16)] = typeof(ShortFormat),
        [DefaultType] = typeof(DefaultFormat)
    };
}