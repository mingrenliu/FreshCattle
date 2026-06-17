namespace AnalyzerTest;

// 正常代码，不会触发任何诊断
public class ValidClass
{
    [ExcelColumn(Name = "Name")]
    public string Name { get; set; } = "";

    [ExcelColumn(Name = "Age")]
    public int Age { get; set; }
}
