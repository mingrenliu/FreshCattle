using ExcelUtile.Formats;

namespace ExcelTest.Entities;

internal class Test
{
    /// <summary>
    /// 计量名称
    /// </summary>
    [Display("计量名称", 0)]
    public string? Name { get; set; }

    /// <summary>
    /// 顺序
    /// </summary>
    [Display("重量1", 1)]
    public int? Order { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    [Display("重量", 3)]
    public double Mass { get; set; }

    /// <summary>
    /// 体积
    /// </summary>
    [Display("体积", 10)]
    public decimal? Volume { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Display("创建时间", 4, true, 20)]
    public virtual DateTime CreatedTime { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    [Display("是否有效", 5)]
    public bool IsEnable { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("时间跨度", 6, false)]
    [DataFormat(typeof(TimeSpanMinuteFormat))]
    public TimeSpan? Spans { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("日期", 7)]
    public DateOnly? Date { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("时间", 8)]
    public TimeOnly? Time { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("时间偏移", 9)]
    public DateTimeOffset? TimeOffset { get; set; }
}