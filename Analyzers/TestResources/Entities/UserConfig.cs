namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
public class UserConfig
{
    public (int,int) MyProperty4 { get; set; }
    /// <summary>
    /// id
    /// </summary>
    public System.Collections.Generic.IEnumerable<string> MyProperty { get; set; }

    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 人员ID(可为空，起到占位符的作用)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 值班配置ID
    /// </summary>
    public string DutyConfId { get; set; }

    /// <summary>
    /// 序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 对应的配置
    /// </summary>
    public DutyConfig? Config { get; set; }

    /// <summary>
    /// 中国
    /// </summary>
    public IEnumerable<DateTime> AllTime { get; set; }
}