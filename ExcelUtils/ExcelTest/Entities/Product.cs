namespace ExcelUtileTest.Entities;

public class Product
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

    /// <summary>
    /// 产品名称
    /// </summary>
    [Display("产品名称")]
    public virtual string Name { get; set; }

    /// <summary>
    /// 产品序列码
    /// </summary>
    [Display("产品序列码")]
    public virtual string Code { get; set; }

    /// <summary>
    /// 产品型号
    /// </summary>
    [Display("产品型号")]
    public virtual string Std { get; set; }

    /// <summary>
    /// 标准容积
    /// </summary>
    [Display("标准容积")]
    public virtual double? DefaultVolume { get; set; }

    /// <summary>
    ///  标准数量
    /// </summary>
    [Display("标准数量")]
    public virtual double? DefaultAmount { get; set; }

    /// <summary>
    /// 标准颜色
    /// </summary>
    [Display("标准颜色")]
    public virtual string Color { get; set; }

    /// <summary>
    /// 产品单位
    /// </summary>
    [Display("计量单位")]
    public virtual string Unit { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Display("备注")]
    public virtual string Remark { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Display("序号")]
    public virtual double? Sort { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Display("是否启用")]
    public bool IsEnabled { get; set; } = true;

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
}