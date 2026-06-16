namespace ExcelTest.Entities;

/// <summary>
/// opt-in 模式实体：只导出标注了 [ExcelColumn] 的属性。
/// Converter 和 Format 使用独立的 Attribute，对应 JSON 的 [JsonConverter]。
/// </summary>
public class Student
{
    [ExcelColumn(Name = "姓名", Order = 0, Required = true)]
    public string Name { get; set; } = string.Empty;

    [ExcelColumn(Name = "年龄", Order = 1)]
    public int Age { get; set; }

    [ExcelColumn(Name = "成绩", Order = 2)]
    [ExcelConverter(typeof(DoubleConverter),"0.00")] 
    public double Score { get; set; }

    [ExcelColumn(Name = "是否毕业", Order = 3)]
    public bool Graduated { get; set; }

    [ExcelColumn(Name = "入学日期", Order = 4)]
    public DateTime EnrollDate { get; set; }

    // 未标注 [ExcelColumn]，opt-in 模式下不导出
    public string? InternalCode { get; set; }
}
