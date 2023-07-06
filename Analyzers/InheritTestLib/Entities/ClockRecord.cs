namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
public class ClockRecord
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 人员ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 打卡类型（0:上班打卡,1:下班打卡）
    /// </summary>
    public bool ClockType { get; set; } = true;

    /// <summary>
    /// 上班时间
    /// </summary>
    public DateTime ClockTime { get; set; }
}