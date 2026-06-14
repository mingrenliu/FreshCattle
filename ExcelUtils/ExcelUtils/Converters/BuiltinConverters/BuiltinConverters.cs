using NPOI.SS.UserModel;
using System.Diagnostics.CodeAnalysis;
namespace ExcelUtile.Converters;

/// <summary>
/// 内置转换器工厂，维护类型到转换器的映射。
/// </summary>
internal static class BuiltinConverters
{
    private static readonly Dictionary<Type, ExcelConverter> _cache = new();

    /// <summary>兜底转换器——对未知类型 Write 时按运行时类型委托，Read 返回 NPOI 原始值。</summary>
    public static readonly ExcelConverter Fallback = new ObjectConverter();

    static BuiltinConverters()
    {
        // 注册所有内置映射
        var map = new Dictionary<Type, Func<ExcelConverter>>
        {
            [typeof(string)]   = () => new StringConverter(),
            [typeof(int)]      = () => new Int32Converter(),
            [typeof(long)]     = () => new Int64Converter(),
            [typeof(short)]    = () => new Int16Converter(),
            [typeof(double)]   = () => new DoubleConverter(),
            [typeof(float)]    = () => new SingleConverter(),
            [typeof(decimal)]  = () => new DecimalConverter(),
            [typeof(bool)]     = () => new BooleanConverter(),
            [typeof(DateTime)] = () => new DateTimeConverter(),
            [typeof(DateTimeOffset)] = () => new DateTimeOffsetConverter(),
            [typeof(DateOnly)] = () => new DateOnlyConverter(),
            [typeof(TimeOnly)] = () => new TimeOnlyConverter(),
            [typeof(TimeSpan)] = () => new TimeSpanConverter(),
            [typeof(Guid)]     = () => new GuidConverter(),
            [typeof(byte)]     = () => new ByteConverter(),
        };

        foreach (var kvp in map)
        {
            _cache[kvp.Key] = kvp.Value();
        }
    }

    /// <summary>
    /// 尝试获取内置转换器。
    /// </summary>
    public static bool TryGetConverter(Type type, [NotNullWhen(true)] out ExcelConverter? converter)
    {
        if (_cache.TryGetValue(type, out converter!))
            return true;

        if (type.IsEnum)
        {
            var genericType = typeof(EnumConverter<>).MakeGenericType(type);
            converter = (ExcelConverter)Activator.CreateInstance(genericType)!;
            _cache[type] = converter;
            return true;
        }

        converter = null;
        return false;
    }
}
