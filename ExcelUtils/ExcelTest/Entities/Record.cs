using ExcelUtile.Formats;

namespace ExcelTest.Entities;

internal class Record
{
    /// <summary>
    /// 计量名称
    /// </summary>
    [Display(nameof(Name), Order = 0)]
    public string? Name { get; set; }

    /// <summary>
    /// 计量类别
    /// </summary>
    [Display("顺序", Order = 1)]
    public int? Order { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    [Display("重量", Order = 3)]
    public double Mass { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    [Display("体积", Order = 10)]
    [DataFormat(typeof(LongTimeFormat))]
    public decimal? Volume { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Display("创建时间", Order = 4, Width = 20)]
    public virtual DateTime CreatedTime { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    [Display("是否有效", Order = 5)]
    public bool IsEnable { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("时间跨度", Order = 6, IsRequired = false)]
    [DataFormat(typeof(TimeSpanMinuteFormat))]
    public TimeSpan? Spans { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("日期", Order = 7)]
    public DateOnly? Date { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("时间", Order = 8)]
    public TimeOnly? Time { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Display("时间偏移", Order = 9)]
    public DateTimeOffset? TimeOffset { get; set; }
}