using ExcelTool;
using ExcelTool.Converters;

// 正常代码，不会触发任何诊断
public class ValidClass
{
    [ExcelTool.ExcelColumn(Name = "Name")]
    public string Name { get; set; } = "";

    [ExcelTool.ExcelColumn(Name = "Age")]
    public int Age { get; set; }
}
