namespace DatabaseHandler.Entities
{
    /// <summary>
    /// 创建信息
    /// </summary>
    public interface ICreated
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建用户id
        /// </summary>
        public string? CreatedBy { get; set; }
    }
}