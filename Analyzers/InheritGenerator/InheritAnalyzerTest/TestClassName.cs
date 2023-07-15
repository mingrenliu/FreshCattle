using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InheritAnalyzerTest
{
    /// <summary>
    /// 
    /// </summary>
    internal class TestClassName
    {
        /// <summary>
        /// 
        /// </summary>
        public int Name { get; set; }
        /// <summary>
        /// id
        /// </summary>
        [StringLength(50)]
        public string Id { get; set; }
    }

    /// <summary>
    /// 值班最低天数要求配置
    /// </summary>
    [Comment("值班最低天数要求配置")]
    public class DutyRequire
    {
        /// <summary>
        /// id
        /// </summary>
        [StringLength(50)]
        [Comment("id")]
        public string Id { get; set; }

        /// <summary>
        /// 值班表id
        /// </summary>
        [Comment("值班表id")]
        [StringLength(50)]
        public string DutyTableId { get; set; }

        /// <summary>
        /// 值班人员id
        /// </summary>
        [StringLength(50)]
        [Comment("值班人员id")]
        public string UserId { get; set; }

        /// <summary>
        /// 每月最低值班天数
        /// </summary>
        [Comment("最低值班天数")]
        public int DutyDays { get; set; }

        /// <summary>
        ///  月份（yyyyMM）
        /// </summary>
        [Comment("月份")]
        public int Month { get; set; }
    }
}
