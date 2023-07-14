using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 调班申请记录
/// </summary>
[Comment("调班申请记录")]
public class ExcRecord
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// 审核状态(0:未处理,1:不同意,2:同意,3:申请驳回,4:申请通过)
    /// </summary>
    [Comment("审核状态")]
    public int AuditStatus { get; set; }

    /// <summary>
    /// 值班表id
    /// </summary>
    [Comment("值班表id")]
    [StringLength(50)]
    public string DutyTableId { get; set; }

    /// <summary>
    /// 换班人员id
    /// </summary>
    [Comment("换班人员id")]
    [StringLength(50)]
    public string ExcUserId { get; set; }

    /// <summary>
    /// 审核人id
    /// </summary>
    [Comment("审核人id")]
    [StringLength(50)]
    public string Auditor { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    [Comment("审核时间")]
    [Precision(0)]
    public DateTime? ProcessTime { get; set; }

    /// <summary>
    /// 调班申请前日期
    /// </summary>
    [Comment("调前日期")]
    [Precision(0)]
    public DateTime TimeBefore { get; set; }

    /// <summary>
    /// 调班申请后日期
    /// </summary>
    [Comment("调后日期")]
    [Precision(0)]
    public DateTime TimeAfter { get; set; }

    /// <summary>
    /// 调班申请发起人
    /// </summary>
    [Comment("发起人")]
    [StringLength(50)]
    public string CreatedBy { get; set; }

    /// <summary>
    /// 调班申请发起时间
    /// </summary>
    [Comment("发起时间")]
    [Precision(0)]
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}