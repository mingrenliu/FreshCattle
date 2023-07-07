using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
public class DutyTable
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// ֵ�������
    /// </summary>
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// ��֯id
    /// </summary>
    public string OrgId { get; set; }

    /// <summary>
    /// ֵ������ͣ��磺��ֵͨ���פ��ֵ���
    /// </summary>
    public string TableType { get; set; }

    /// <summary>
    /// ���������id
    /// </summary>
    public string AuditorId { get; set; }

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// �ϰ�ʱ��
    /// </summary>
    public virtual DateTime BeginTime { get; set; }

    /// <summary>
    /// �°�ʱ��
    /// </summary>
    public virtual DateTime EndTime { get; set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}