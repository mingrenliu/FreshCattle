namespace ExcelUtileTest.Entities;

internal class Person
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Display("姓名", 0)]
    public string? Name { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Display("性别", 1)]
    public string? Sex { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    [Display("年龄", 2)]
    public int Age { get; set; }

    /// <summary>
    /// 工资
    /// </summary>
    [Display("工资", 3)]
    [DataFormat(typeof(DoubleFormat),1)]
    public double Money { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [Display("生日", 4)]
    [DataFormat(typeof(OnlyTimeFormat))]
    public DateTime Birthday { get; set; }

    /// <summary>
    /// 是否在职
    /// </summary>
    [Display("是否在职", 5)]
    [DataFormat(typeof(BooleanFormat),"对","错")]

    public bool IsEnable { get; set; }
    /// <summary>
    /// 是否在职
    /// </summary>
    [Display("是否不在职", 6,false)]

    public bool IsDisable { get; set; }

    /// <summary>
    /// 父亲名字
    /// </summary>
    [Display("父亲名字", 7, false,DynamicWidth =true)]
    public object? FeatherName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 动态数组
    /// </summary>

    public Dictionary<string, decimal?> Data { get; set; } = new();
}