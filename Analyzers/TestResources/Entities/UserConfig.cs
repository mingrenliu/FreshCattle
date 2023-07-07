namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
public class UserConfig
{
    /// <summary>
    /// id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// ��ԱID(��Ϊ�գ���ռλ��������)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// ֵ������ID
    /// </summary>
    public string DutyConfId { get; set; }

    /// <summary>
    /// ���
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// ��Ӧ������
    /// </summary>
    public DutyConfig? Config { get; set; }
}