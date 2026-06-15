using ExcelTool;
using ExcelTool.Converters;

// LY0011: ExcelConverter 类型不匹配
public class ConverterTypeMismatch
{
    [ExcelTool.ExcelColumn]
    [ExcelTool.ExcelConverter(typeof(ExcelTool.Converters.BooleanConverter))]
    public int X { get; set; }  // BooleanConverter 应用于 int → LY0011
}

// LY0013: 类型不是 ExcelConverter
public class InvalidConverterType
{
    [ExcelTool.ExcelColumn]
    [ExcelTool.ExcelConverter(typeof(string))]  // string 不是 ExcelConverter → LY0013
    public string X { get; set; } = "";
}

// 正确用法（不报错）
public class ValidConverter
{
    [ExcelTool.ExcelColumn]
    [ExcelTool.ExcelConverter(typeof(ExcelTool.Converters.Int32Converter))]
    public int X { get; set; }
}
