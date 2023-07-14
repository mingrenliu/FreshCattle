using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
[Comment("ֵ���¼��")]
public class DutyTable
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// ֵ�������
    /// </summary>
    [Comment("ֵ�������")]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// ��֯id
    /// </summary>
    [StringLength(50)]
    [Comment("��֯id")]
    public string OrgId { get; set; }

    /// <summary>
    /// ֵ������ͣ��磺��ֵͨ���פ��ֵ���
    /// </summary>
    [StringLength(50)]
    [Comment("ֵ�������")]
    public string TableType { get; set; }

    /// <summary>
    /// ���������id
    /// </summary>
    [StringLength(50)]
    [Comment("���������id")]
    public string AuditorId { get; set; }

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    [Comment("�Ƿ�����")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// �ϰ�ʱ��
    /// </summary>
    [Precision(0)]
    [Comment("�ϰ�ʱ��")]
    public virtual DateTime BeginTime { get; set; }

    /// <summary>
    /// �°�ʱ��
    /// </summary>
    [Precision(0)]
    [Comment("�°�ʱ��")]
    public virtual DateTime EndTime { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    [Precision(0)]
    [Comment("����ʱ��")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}