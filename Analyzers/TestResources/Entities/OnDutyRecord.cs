namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
public class OnDutyRecord
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// ֵ������
    /// </summary>
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// ��ԱID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// ֵ���ID
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// ֵ�ಿ��ID
    /// </summary>
    public bool IsExchanged { get; set; } = false;

    /// <summary>
    /// ֵ���¼
    /// </summary>
    public string? Remark { get; set; }
}