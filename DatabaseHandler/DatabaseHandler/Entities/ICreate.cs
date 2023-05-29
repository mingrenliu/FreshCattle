namespace DatabaseHandler.Entities
{
    /// <summary>
    /// 创建信息
    /// </summary>
    public interface ICreate
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建用户id
        /// </summary>
        public string CreateUserId { get; set; }
    }
}