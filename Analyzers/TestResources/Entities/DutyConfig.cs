namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ����ֵ������
/// </summary>
public class DutyConfig
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// ֵ���ID
    /// </summary>
    public string DutyTableId { get; set; }

    /// <summary>
    /// ��Ӧ��ֵ���
    /// </summary>
    public DutyTable? DutyTable { get; set; }
}