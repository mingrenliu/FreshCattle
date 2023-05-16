namespace ExcelTest.Entitys;

internal class Record
{
    /// <summary>
    /// 计量名称
    /// </summary>
    [Display("计量名称", Order = 0)]
    public string? Name { get; set; }

    /// <summary>
    /// 计量类别
    /// </summary>
    [Display("计量类别", Order = 1)]
    public string? GroupName { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    [Display("重量", Order = 3)]
    public double Mass { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Display("创建时间", Order = 4)]
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    [Display("是否有效", Order = 5)]
    public bool IsEnable { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("标签", Order = 6, IsRequired = false)]
    public string? Tags { get; set; }
}