using Mapster;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// 常用值班配置
/// </summary>
[Comment("常用值班配置")]
public class DutyConfig
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// 配置名称
    /// </summary>
    [Comment("配置名称")]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 值班表ID
    /// </summary>
    [StringLength(50)]
    [Comment("组织id")]
    public string DutyTableId { get; set; }

    /// <summary>
    /// 对应的值班表
    /// </summary>
    [JsonIgnore]
    [AdaptIgnore]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    [ForeignKey(nameof(DutyTableId))]
    public DutyTable? DutyTable { get; set; }
}