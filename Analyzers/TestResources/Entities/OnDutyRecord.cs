namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
public class OnDutyRecord
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 值班日期
    /// </summary>
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// 人员ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 值班表ID
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// 值班部门ID
    /// </summary>
    public bool IsExchanged { get; set; } = false;

    /// <summary>
    /// 值班记录
    /// </summary>
    public string? Remark { get; set; }
}