global using ExcelTool;
global using ExcelTool.Converters;
namespace AnalyzerTest;

// LY0011: ExcelConverter 类型不匹配
public class ConverterTypeMismatch
{
    [ExcelColumn]
    [ExcelConverter(typeof(BooleanConverter))]
    public int X { get; set; }  // BooleanConverter 应用于 int → LY0011
}

// LY0013: 类型不是 ExcelConverter
public class InvalidConverterType
{
    [ExcelColumn]
    [ExcelConverter(typeof(string))]  // string 不是 ExcelConverter → LY0013
    public string X { get; set; } = "";
}

// 正确用法（不报错）
public class ValidConverter
{
    [ExcelColumn]
    [ExcelConverter(typeof(Int32Converter))]
    public int X { get; set; }
}
