namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���¼��
/// </summary>
public class UserConfig
{
    public (int,int) MyProperty4 { get; set; }
    /// <summary>
    /// id
    /// </summary>
    public System.Collections.Generic.IEnumerable<string> MyProperty { get; set; }

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

    /// <summary>
    /// �й�
    /// </summary>
    public IEnumerable<DateTime> AllTime { get; set; }
}