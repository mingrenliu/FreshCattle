using System.ComponentModel.DataAnnotations;

namespace Cybstar.MES.Duty.DatabaseAccess.Entities;

/// <summary>
/// ֵ���������Ҫ������
/// </summary>
[Comment("ֵ���������Ҫ������")]
public class DutyRequire
{
    /// <summary>
    /// id
    /// </summary>
    [StringLength(50)]
    [Comment("id")]
    public string Id { get; set; }

    /// <summary>
    /// ֵ���id
    /// </summary>
    [Comment("ֵ���id")]
    [StringLength(50)]
    public string DutyTableId { get; set; }

    /// <summary>
    /// ֵ����Աid
    /// </summary>
    [StringLength(50)]
    [Comment("ֵ����Աid")]
    public string UserId { get; set; }

    /// <summary>
    /// ÿ�����ֵ������
    /// </summary>
    [Comment("���ֵ������")]
    public int DutyDays { get; set; }

    /// <summary>
    ///  �·ݣ�yyyyMM��
    /// </summary>
    [Comment("�·�")]
    public int Month { get; set; }
}