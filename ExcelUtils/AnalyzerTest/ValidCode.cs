using ExcelUtile;
using ExcelUtile.Converters;

// 正常代码，不会触发任何诊断
public class ValidClass
{
    [ExcelUtile.ExcelColumn(Name = "Name")]
    public string Name { get; set; } = "";

    [ExcelUtile.ExcelColumn(Name = "Age")]
    public int Age { get; set; }
}
