namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���������Ҫ������
/// </summary>
public class DutyRequire
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// ֵ���id
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// ֵ����Աid
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// ÿ�����ֵ������
    /// </summary>
    public int DutyDays { get; set; }

    /// <summary>
    ///  �·ݣ�yyyyMM��
    /// </summary>
    public int Month { get; set; }
}