namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
public class ClockRecord
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// ��ԱID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// �����ͣ�0:�ϰ��,1:�°�򿨣�
    /// </summary>
    public bool ClockType { get; set; } = true;

    /// <summary>
    /// �ϰ�ʱ��
    /// </summary>
    public DateTime ClockTime { get; set; }
}