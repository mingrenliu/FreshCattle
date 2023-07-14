using Mapster;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
[Comment("ֵ���¼��")]
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
    /// ��ԱID(��Ϊ�գ���ռλ��������)
    /// </summary>
    [StringLength(50)]
    [Comment("��ԱID")]
    public string? UserId { get; set; }

    /// <summary>
    /// ֵ������ID
    /// </summary>
    [StringLength(50)]
    [Comment("ֵ������ID")]
    public string DutyConfId { get; set; }

    /// <summary>
    /// ���
    /// </summary>
    [Comment("���")]
    public int Sort { get; set; }

    /// <summary>
    /// ��Ӧ������
    /// </summary>
    [JsonIgnore]
    [AdaptIgnore]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    [ForeignKey(nameof(DutyConfId))]
    public DutyConfig? Config { get; set; }
}