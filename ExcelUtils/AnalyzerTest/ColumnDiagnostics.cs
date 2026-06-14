using ExcelUtile;
using ExcelUtile.Converters;

// LY0012-1: 两个 [ExcelColumn] 指定同名
public class DuplicateColumnExplicit
{
    [ExcelColumn(Name = "A")]
    public string X { get; set; } = "";

    [ExcelColumn(Name = "A")]  // 应报 LY0012
    public string Y { get; set; } = "";
}

// LY0012-2: [ExcelColumn] 名称与裸属性名冲突
public class DuplicateColumnImplicit
{
    public string A { get; set; } = "";   // 列名 = "A"

    [ExcelColumn(Name = "A")]  // 应报 LY0012
    public string B { get; set; } = "";
}

// LY0014: [ExcelColumn] 和 [ExcelIgnore] 冲突
public class ColumnIgnoreConflict
{
    [ExcelColumn]
    [ExcelIgnore]  // 应报 LY0014
    public string X { get; set; } = "";
}
