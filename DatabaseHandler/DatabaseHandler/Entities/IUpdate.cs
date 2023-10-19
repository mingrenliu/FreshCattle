using Microsoft.EntityFrameworkCore;

namespace DatabaseHandler.Entities
{
    public interface IUpdate
    {
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 更新用户id
        /// </summary>
        public string? UpdatedBy { get; set; }
    }
}