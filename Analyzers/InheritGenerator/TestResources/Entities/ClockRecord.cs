using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
[Comment("值班记录表")]
public class ClockRecord
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// 人员ID
    /// </summary>
    [Comment("人员ID")]
    [StringLength(50)]
    public string UserId { get; set; }

    /// <summary>
    /// 打卡类型（0:上班打卡,1:下班打卡）
    /// </summary>
    [Comment("打卡类型(false:上班，true:下班)")]
    public bool ClockType { get; set; } = true;

    /// <summary>
    /// 上班时间
    /// </summary>
    [Precision(0)]
    [Comment("打卡时间")]
    public DateTime ClockTime { get; set; }
}