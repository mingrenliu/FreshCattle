using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFEntityAnalyzer.Test
{
    /// <summary>
    ///
    /// </summary>
    [Table("sample")]
    public class Sample
    {
        [StringLength(50)]
        public string Name { get; set; }

        public DateTime Date { get; set; }
        public UploadStatus Status { get; set; }
    }

    public class CommentAttribute : Attribute
    {
        public CommentAttribute(string comment)
        {
        }
    }

    public class PrecisionAttribute : Attribute
    {
        public PrecisionAttribute(int value)
        {
        }

        public PrecisionAttribute(int value, int value2)
        {
        }
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
}