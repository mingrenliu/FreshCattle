using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
[Comment("值班记录表")]
public class OnDutyRecord
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// 值班日期
    /// </summary>
    [Comment("值班日期")]
    [Precision(0)]
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// 人员ID
    /// </summary>
    [StringLength(50)]
    [Comment("人员ID")]
    public string UserId { get; set; }

    /// <summary>
    /// 值班表ID
    /// </summary>
    [StringLength(50)]
    [Comment("值班表ID")]
    public string DutyTableId { get; set; }

    /// <summary>
    /// 值班部门ID
    /// </summary>
    [Comment("是否调班")]
    public bool IsExchanged { get; set; } = false;

    /// <summary>
    /// 值班记录
    /// </summary>
    [Comment("值班记录")]
    public string? Remark { get; set; }
}