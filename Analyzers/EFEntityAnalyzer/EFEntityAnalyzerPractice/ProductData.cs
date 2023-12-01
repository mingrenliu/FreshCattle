using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFEntityAnalyzerPractice;

/// <summary>
/// fwefw
/// </summary>
[Table("test")]
[Comment("fwefw")]
public class ProductData
{
    /// <summary>
    /// fewf
    /// </summary>
    [StringLength(50)]
    [Comment("fewf")]
    public string? Id { get; set; }

    /// <summary>
    /// fewfe
    /// </summary>
    [Precision(0)]
    [Comment("fewfe")]
    public DateTime? UploadDate { get; set; }

    /// <summary>
    /// fwef
    /// </summary>
    [Comment("fwef")]
    public UploadStatus Status { get; set; }

    /// <summary>
    /// few
    /// </summary>
    [StringLength(50)]
    [Comment("few")]
    public string AUFNR { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    [Precision(18, 6)]
    [Comment("日期")]
    public decimal PSMNG { get; set; }

    /// <summary>
    /// fwe
    /// </summary>
    [StringLength(50)]
    [Comment("fwe")]
    public string MEINH { get; set; }

    /// <summary>
    /// fewf
    /// </summary>
    [StringLength(50)]
    [Comment("fewf")]
    public string BATCH { get; set; }

    public IEnumerable<ProductData>? MyProperty { get; set; }
    public ProductData[] MyProperty2 { get; set; }
    public ProductData MyProperty3 { get; set; }
}

public enum UploadStatus
{
    /// <summary>
    /// 待上传
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 上传失败
    /// </summary>
    Fail = 1,

    /// <summary>
    /// 上传中
    /// </summary>
    Uploading = 2,

    /// <summary>
    /// 上传成功
    /// </summary>
    Success = 3
}