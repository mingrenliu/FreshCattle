namespace ExcelUtile.Converters;

/// <summary>
/// 转换器注册表，管理所有 ExcelConverter 实例。
/// 对应 System.Text.Json 的 JsonSerializerOptions.Converters。
/// </summary>
public class ConverterRegistry
{
    private readonly Dictionary<Type, ExcelConverter> _converters = new();
    private readonly Dictionary<Type, ExcelConverter> _builtinCache = new();
    public void AddConverter<T>(ExcelConverter<T> converter)
    {
        _converters[typeof(T)] = converter;
    }

    public void AddConverter(Type type, ExcelConverter converter)
    {
        _converters[type] = converter;
    }

    public ExcelConverter GetConverter(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (_converters.TryGetValue(type, out var custom))
            return custom;

        if (_builtinCache.TryGetValue(type, out var cached))
            return cached;

        if (BuiltinConverters.TryGetConverter(type, out var builtin))
        {
            _builtinCache[type] = builtin;
            return builtin;
        }

        return BuiltinConverters.Fallback;
    }

    /// <summary>
    /// 获取指定类型的泛型转换器。
    /// </summary>
    public ExcelConverter<T>? GetConverter<T>()
    {
        return GetConverter(typeof(T)) as ExcelConverter<T>;
    }
}
