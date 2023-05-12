using System.Diagnostics.CodeAnalysis;

namespace ExcelUtils.Formats;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DataFormatAttribute : Attribute
{
    /// <summary>
    /// Weight
    /// </summary>
    public int Order { get; set; }

    public ExcelConverter? Converter => _converter;

    /// <summary>
    /// Converter Instance
    /// </summary>
    private readonly ExcelConverter? _converter;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="type"></param>
    public DataFormatAttribute(Type type)
    {
        if (Activator.CreateInstance(type) is ExcelConverter result)
        {
            _converter = result;
        }
    }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="converter"></param>
    public DataFormatAttribute(ExcelConverter converter)
    {
        _converter = converter;
    }
}