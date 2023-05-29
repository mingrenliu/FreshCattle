using Microsoft.EntityFrameworkCore;

namespace DatabaseHandler.Entities
{
    public interface IUpdate
    {
        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新用户id
        /// </summary>
        public string UpdateUserId { get; set; }
    }
}