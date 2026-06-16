namespace ExcelTool.Converters;

/// <summary>
/// 转换器注册表，管理所有 ExcelConverter 实例。
/// 对应 System.Text.Json 的 JsonSerializerOptions.Converters。
/// </summary>
public class ConverterRegistry
{
    private readonly Dictionary<Type, ExcelConverter> _converters = new();

    public void AddConverter<T>(ExcelConverter<T> converter)
    {
        _converters[typeof(T)] = converter;
    }

    public void AddConverter(Type type, ExcelConverter converter)
    {
        _converters[type] = converter;
    }

    public ExcelConverter GetConverter(Type origin)
    {
        if (_converters.TryGetValue(origin, out var custom))
        {
            return custom;
        }
        var type = Nullable.GetUnderlyingType(origin) ?? origin;
        if (_converters.TryGetValue(type, out custom))
        {
            _converters[origin] = custom; // 缓存 Nullable<T> 的转换器
            return custom;
        }

        if (BuiltInConverters.TryGetConverter(type, out var builtIn))
        {
            return builtIn;
        }

        return BuiltInConverters.Fallback;
    }

    /// <summary>
    /// 获取指定类型的泛型转换器。
    /// </summary>
    public ExcelConverter<T>? GetConverter<T>()
    {
        return GetConverter(typeof(T)) as ExcelConverter<T>;
    }
}