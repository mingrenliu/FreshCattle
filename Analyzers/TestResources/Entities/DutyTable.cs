using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
public class DutyTable
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 值班表名称
    /// </summary>
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 组织id
    /// </summary>
    public string OrgId { get; set; }

    /// <summary>
    /// 值班表类型（如：普通值班表，驻厂值班表）
    /// </summary>
    public string TableType { get; set; }

    /// <summary>
    /// 调班审核人id
    /// </summary>
    public string AuditorId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 上班时间
    /// </summary>
    public virtual DateTime BeginTime { get; set; }

    /// <summary>
    /// 下班时间
    /// </summary>
    public virtual DateTime EndTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}