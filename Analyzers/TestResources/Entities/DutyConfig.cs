namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 常用值班配置
/// </summary>
public class DutyConfig
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 配置名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 值班表ID
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// 对应的值班表
    /// </summary>
    public DutyTable? DutyTable { get; set; }
}