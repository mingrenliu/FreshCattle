using Mapster;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ����ֵ������
/// </summary>
[Comment("����ֵ������")]
public class DutyConfig
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    [Comment("��������")]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// ֵ���ID
    /// </summary>
    [StringLength(50)]
    [Comment("��֯id")]
    public string DutyTableId { get; set; }

    /// <summary>
    /// ��Ӧ��ֵ���
    /// </summary>
    [JsonIgnore]
    [AdaptIgnore]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    [ForeignKey(nameof(DutyTableId))]
    public DutyTable? DutyTable { get; set; }
}