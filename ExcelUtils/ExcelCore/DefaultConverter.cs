using ExcelUtile.Formats;

namespace ExcelUtile.ExcelCore;

internal class DefaultConverterFactory
{
    private readonly object _lock = new();
    private readonly Dictionary<string, ExcelConverter> _defaultConverters = new();

    internal ExcelConverter? GetDefaultConverter(Type type)
    {
        if (_defaultConverters.TryGetValue(type.Name, out ExcelConverter? value)) return value;
        return Add(type);
    }
    internal ExcelConverter? GetDefaultConverter(string type)
    {
        if (_defaultConverters.TryGetValue(type, out ExcelConverter? value)) return value;
        return Add(type);
    }
    internal ExcelConverter? Add(string type)
    {
        if (defaultConverterMap.ContainsKey(type))
        {
            lock (_lock)
            {
                if (!_defaultConverters.ContainsKey(type))
                {
                    if (Activator.CreateInstance(defaultConverterMap[type]) is ExcelConverter converter)
                    {
                        _defaultConverters[type] = converter;
                        return converter;
                    }
                }
            }
        }
        return null;
    }
    internal ExcelConverter? Add(Type type)
    {
        return Add(type.Name);
    }

    private static readonly Dictionary<string, Type> defaultConverterMap = new()
    {
        [nameof(Double)] = typeof(DoubleFormat),
        [nameof(Int32)] = typeof(IntFormat),
        [nameof(Int64)] = typeof(LongFormat),
        [nameof(Decimal)] = typeof(DecimalFormat),
        [nameof(TimeSpan)] = typeof(TimeSpan),
        [nameof(DateOnly)] = typeof(DateOnlyFormat),
        [nameof(TimeOnly)] = typeof(TimeOnlyFormat),
        [nameof(DateTimeOffset)] = typeof(DateTimeOffsetFormat),
        [nameof(DateTime)] = typeof(ShortTimeFormat),
        [nameof(Boolean)] = typeof(BooleanFormat),
        [nameof(Single)] = typeof(FloatFormat),
        [nameof(String)] = typeof(StringFormat)
    };
}