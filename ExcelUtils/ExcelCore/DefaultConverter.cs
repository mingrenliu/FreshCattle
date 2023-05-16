using ExcelUtile.Formats;
using System.Linq;

namespace ExcelUtile.ExcelCore;

internal class DefaultConverter
{
    private static readonly object _lock=new();
    private static readonly Dictionary<string, ExcelConverter> _defaultConverters = new();
    internal static ExcelConverter? GetDefaultConverter(Type type)
    {
        if (_defaultConverters.ContainsKey(type.Name)) return _defaultConverters[type.Name];
        return Add(type);
    }
    internal static ExcelConverter? Add(Type type)
    {
        if (defaultConverterMap.ContainsKey(type.Name))
        {
            lock (_lock)
            {
                if (!_defaultConverters.ContainsKey(type.Name))
                {
                    if (Activator.CreateInstance(defaultConverterMap[type.Name]) is ExcelConverter converter)
                    {
                        _defaultConverters[type.Name]= converter;
                        return converter;
                    }
                }
            }
        }
        return null;
    }
    private static readonly Dictionary<string,Type> defaultConverterMap = new() {
        [nameof(Double)]= typeof(DoubleFormat),
        [nameof(Int32)]= typeof(IntFormat),
        [nameof(Int64)]= typeof(LongFormat),
        [nameof(Decimal)]= typeof(DecimalFormat),
        [nameof(TimeSpan)]= typeof(TimeSpan),
        [nameof(DateOnly)]= typeof(DateOnlyFormat),
        [nameof(TimeOnly)]= typeof(TimeOnlyFormat),
        [nameof(DateTimeOffset)]= typeof(DateTimeOffsetFormat),
        [nameof(DateTime)]= typeof(ShortTimeFormat),
        [nameof(Boolean)]= typeof(BooleanFormat),
        [nameof(Single)]= typeof(FloatFormat),
        [nameof(String)]= typeof(StringFormat)
    };
}