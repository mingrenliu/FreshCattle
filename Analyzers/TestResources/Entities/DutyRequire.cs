namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班最低天数要求配置
/// </summary>
public class DutyRequire
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 值班表id
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// 值班人员id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 每月最低值班天数
    /// </summary>
    public int DutyDays { get; set; }

    /// <summary>
    ///  月份（yyyyMM）
    /// </summary>
    public int Month { get; set; }
}