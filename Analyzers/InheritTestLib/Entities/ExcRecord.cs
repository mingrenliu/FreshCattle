namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 调班申请记录
/// </summary>
public class ExcRecord
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 审核状态(0:未处理,1:不同意,2:同意,3:申请驳回,4:申请通过)
    /// </summary>
    public int AuditStatus { get; set; }

    /// <summary>
    /// 值班表id
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// 换班人员id
    /// </summary>
    public string ExcUserId { get; set; }

    /// <summary>
    /// 审核人id
    /// </summary>
    public string Auditor { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    public DateTime? ProcessTime { get; set; }

    /// <summary>
    /// 调班申请前日期
    /// </summary>
    public DateTime TimeBefore { get; set; }

    /// <summary>
    /// 调班申请后日期
    /// </summary>
    public DateTime TimeAfter { get; set; }

    /// <summary>
    /// 调班申请发起人
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// 调班申请发起时间
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}