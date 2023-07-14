using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ���������¼
/// </summary>
[Comment("���������¼")]
public class ExcRecord
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// ���״̬(0:δ����,1:��ͬ��,2:ͬ��,3:���벵��,4:����ͨ��)
    /// </summary>
    [Comment("���״̬")]
    public int AuditStatus { get; set; }

    /// <summary>
    /// ֵ���id
    /// </summary>
    [Comment("ֵ���id")]
    [StringLength(50)]
    public string DutyTableId { get; set; }

    /// <summary>
    /// ������Աid
    /// </summary>
    [Comment("������Աid")]
    [StringLength(50)]
    public string ExcUserId { get; set; }

    /// <summary>
    /// �����id
    /// </summary>
    [Comment("�����id")]
    [StringLength(50)]
    public string Auditor { get; set; }

    /// <summary>
    /// ���ʱ��
    /// </summary>
    [Comment("���ʱ��")]
    [Precision(0)]
    public DateTime? ProcessTime { get; set; }

    /// <summary>
    /// ��������ǰ����
    /// </summary>
    [Comment("��ǰ����")]
    [Precision(0)]
    public DateTime TimeBefore { get; set; }

    /// <summary>
    /// �������������
    /// </summary>
    [Comment("��������")]
    [Precision(0)]
    public DateTime TimeAfter { get; set; }

    /// <summary>
    /// �������뷢����
    /// </summary>
    [Comment("������")]
    [StringLength(50)]
    public string CreatedBy { get; set; }

    /// <summary>
    /// �������뷢��ʱ��
    /// </summary>
    [Comment("����ʱ��")]
    [Precision(0)]
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}