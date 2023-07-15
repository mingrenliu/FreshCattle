using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
[Comment("ֵ���¼��")]
public class ClockRecord
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// ��ԱID
    /// </summary>
    [Comment("��ԱID")]
    [StringLength(50)]
    public string UserId { get; set; }

    /// <summary>
    /// �����ͣ�0:�ϰ��,1:�°�򿨣�
    /// </summary>
    [Comment("������(false:�ϰ࣬true:�°�)")]
    public bool ClockType { get; set; } = true;

    /// <summary>
    /// �ϰ�ʱ��
    /// </summary>
    [Precision(0)]
    [Comment("��ʱ��")]
    public DateTime ClockTime { get; set; }
}