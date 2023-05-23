using ExcelUtile;
namespace ExcelTest.Entities;

internal class Person
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Display("姓名", Order = 0)]
    public string? Name { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Display("性别", Order = 1)]
    public string? Sex { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    [Display("年龄", Order = 2)]
    public int Age { get; set; }

    /// <summary>
    /// 工资
    /// </summary>
    [Display("工资", Order = 3)]
    public double Money { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [Display("生日", Order = 4)]
    public DateTime Birthday { get; set; }

    /// <summary>
    /// 是否在职
    /// </summary>
    [Display("是否在职", Order = 5)]
    public bool IsEnable { get; set; }

    /// <summary>
    /// 父亲名字
    /// </summary>
    [Display("父亲名字", Order = 6, IsRequired = false)]
    public string? FeatherName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}