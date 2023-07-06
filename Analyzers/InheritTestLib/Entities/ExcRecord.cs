namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ���������¼
/// </summary>
public class ExcRecord
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// ���״̬(0:δ����,1:��ͬ��,2:ͬ��,3:���벵��,4:����ͨ��)
    /// </summary>
    public int AuditStatus { get; set; }

    /// <summary>
    /// ֵ���id
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// ������Աid
    /// </summary>
    public string ExcUserId { get; set; }

    /// <summary>
    /// �����id
    /// </summary>
    public string Auditor { get; set; }

    /// <summary>
    /// ���ʱ��
    /// </summary>
    public DateTime? ProcessTime { get; set; }

    /// <summary>
    /// ��������ǰ����
    /// </summary>
    public DateTime TimeBefore { get; set; }

    /// <summary>
    /// �������������
    /// </summary>
    public DateTime TimeAfter { get; set; }

    /// <summary>
    /// �������뷢����
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// �������뷢��ʱ��
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}