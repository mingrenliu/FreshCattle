using Mapster;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 值班记录表
/// </summary>
[Comment("值班记录表")]
public class UserConfig
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    [InheritIgnore]
    public string Id { get; set; }

    /// <summary>
    /// 人员ID(可为空，起到占位符的作用)
    /// </summary>
    [StringLength(50)]
    [Comment("人员ID")]
    public string? UserId { get; set; }

    /// <summary>
    /// 值班配置ID
    /// </summary>
    [StringLength(50)]
    [Comment("值班配置ID")]
    public string DutyConfId { get; set; }

    /// <summary>
    /// 序号
    /// </summary>
    [Comment("序号")]
    public int Sort { get; set; }

    /// <summary>
    /// 对应的配置
    /// </summary>
    [JsonIgnore]
    [AdaptIgnore]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    [ForeignKey(nameof(DutyConfId))]
    public DutyConfig? Config { get; set; }
}