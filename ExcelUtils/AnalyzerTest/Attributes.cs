// 内联属性定义（不依赖 ExcelTool.dll）
namespace ExcelTool
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnAttribute : System.Attribute
    {
        public string? Name { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public bool Required { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelIgnoreAttribute : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelConverterAttribute : System.Attribute
    {
        public System.Type ConverterType { get; }
        public ExcelConverterAttribute(System.Type converterType) { ConverterType = converterType; }
    }
}

namespace ExcelTool.Converters
{
    public abstract class ExcelConverter
    {
        public abstract System.Type Type { get; }
    }

    public abstract class ExcelConverter<T> : ExcelConverter
    {
        public override System.Type Type => typeof(T);
    }

    public class BooleanConverter : ExcelConverter<bool> { }
    public class Int32Converter : ExcelConverter<int> { }
    public class StringConverter : ExcelConverter<string> { }
    public class DoubleConverter : ExcelConverter<double> { }
    public class DateTimeConverter : ExcelConverter<System.DateTime> { }
}
