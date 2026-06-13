using ExcelUtile.Formats;
using System.Xml.Linq;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 类型转化器工厂
/// </summary>
public interface IConverterFactory
{
    public ExcelConverter GetDefaultConverter(Type type);

    public IConverter<T>? GetDefaultConverter<T>() where T : notnull;

    public ExcelConverter GetDefaultConverter(string type);
}

/// <summary>
/// 默认的转换器工厂(也是Object类型的转换器)
/// </summary>
public class DefaultConverterFactory : ExcelReferenceConverter<object>, IConverterFactory
{
    private const string DefaultType = "DefaultType";
    private readonly Dictionary<string, ExcelConverter> _converters;

    public DefaultConverterFactory()
    {
        _converters = new()
        {
            [nameof(Object)] = this
        };
    }

    public ExcelConverter GetDefaultConverter(Type type)
    {
        return GetDefaultConverter((Nullable.GetUnderlyingType(type) ?? type).Name);
    }

    public IConverter<T>? GetDefaultConverter<T>() where T : notnull
    {
        return GetDefaultConverter(typeof(T).Name) as IConverter<T>;
    }

    public ExcelConverter GetDefaultConverter(string type)
    {
        if (_converters.TryGetValue(type, out ExcelConverter? value)) return value;
        if (defaultConverterMap.ContainsKey(type))
        {
            if (Activator.CreateInstance(defaultConverterMap[type]) is ExcelConverter converter)
            {
                _converters[type] = converter;
                return converter;
            }
        }
        if (_converters.ContainsKey(DefaultType) is false )
        {
            _converters[DefaultType] = new DefaultFormat();
        }
        return this;
    }

    protected override void WriteValue(ICell cell, object? value)
    {
        if (value == null) return;
        var convert = GetDefaultConverter(value.GetType());
        if (convert == null || convert == this)
        {
            convert = GetDefaultConverter(DefaultType);
        }
        convert.WriteToCell(cell, value);
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

    private static readonly Dictionary<string, Type> defaultConverterMap = new(StringComparer.OrdinalIgnoreCase)
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