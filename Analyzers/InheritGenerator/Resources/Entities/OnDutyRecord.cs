using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
[Comment("ֵ���¼��")]
public class OnDutyRecord
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// ֵ������
    /// </summary>
    [Comment("ֵ������")]
    [Precision(0)]
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// ��ԱID
    /// </summary>
    [StringLength(50)]
    [Comment("��ԱID")]
    public string UserId { get; set; }

    /// <summary>
    /// ֵ���ID
    /// </summary>
    [StringLength(50)]
    [Comment("ֵ���ID")]
    public string DutyTableId { get; set; }

    /// <summary>
    /// ֵ�ಿ��ID
    /// </summary>
    [Comment("�Ƿ����")]
    public bool IsExchanged { get; set; } = false;

    /// <summary>
    /// ֵ���¼
    /// </summary>
    [Comment("ֵ���¼")]
    public string? Remark { get; set; }
}