using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
[Comment("值班记录表")]
public class DutyTable
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// 值班表名称
    /// </summary>
    [Comment("值班表名称")]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 组织id
    /// </summary>
    [StringLength(50)]
    [Comment("组织id")]
    public string OrgId { get; set; }

    /// <summary>
    /// 值班表类型（如：普通值班表，驻厂值班表）
    /// </summary>
    [StringLength(50)]
    [Comment("值班表类型")]
    public string TableType { get; set; }

    /// <summary>
    /// 调班审核人id
    /// </summary>
    [StringLength(50)]
    [Comment("调班审核人id")]
    public string AuditorId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Comment("是否启用")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 上班时间
    /// </summary>
    [Precision(0)]
    [Comment("上班时间")]
    public virtual DateTime BeginTime { get; set; }

    /// <summary>
    /// 下班时间
    /// </summary>
    [Precision(0)]
    [Comment("下班时间")]
    public virtual DateTime EndTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Precision(0)]
    [Comment("创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}