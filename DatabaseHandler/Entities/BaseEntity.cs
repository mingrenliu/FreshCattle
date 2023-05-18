using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DatabaseHandler.Entities
{
    public class BaseEntity
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// id
        /// </summary>
        [Comment("id")]
        [StringLength(50)]
        public string Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        [StringLength(50)]
        public string TenantId { get; set; }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
}