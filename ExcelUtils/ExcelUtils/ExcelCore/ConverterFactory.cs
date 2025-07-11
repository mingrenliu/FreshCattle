using ExcelUtile.Formats;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 类型转化器工厂
/// </summary>
public interface IConverterFactory
{
    public ExcelConverter? GetDefaultConverter(Type type);

    public IConverter<T>? GetDefaultConverter<T>() where T : notnull;

    public ExcelConverter? GetDefaultConverter(string type);
}

/// <summary>
/// 默认的转换器工厂(也是Object类型的转换器)
/// </summary>
public class DefaultConverterFactory : ExcelReferenceConverter<object>, IConverterFactory
{
    private const string DefaultType = "DefaultType";
    private readonly Dictionary<string, ExcelConverter> _defaultConverters;

    public DefaultConverterFactory()
    {
        _defaultConverters = new()
        {
            [nameof(Object)] = this
        };
    }

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

    protected override void WriteValue(ICell cell, object? value)
    {
        if (value == null) return;
        var convert = GetDefaultConverter(value.GetType());
        if (convert == null || convert == this)
        {
            convert = GetDefaultConverter(DefaultType);
        }
        convert?.WriteToCell(cell, value);
    }

    public override object? Read(ICell cell)
    {
        if (cell.IsInValid() is false)
        {
            return null;
        }
        else if (cell.IsString())
        {
            return cell.GetString();
        }
        else if (cell.IsNumeric())
        {
            return cell.GetDecimal();
        }
        else if (cell.IsBoolean())
        {
            return cell.GetBoolean();
        }
        return null;
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