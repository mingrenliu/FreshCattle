namespace DatabaseHandler.Entities
{
    public interface ICurrentUser
    {
        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <returns></returns>
        public string? GetUserName();
        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <returns></returns>

        public string? GetUserId();
        /// <summary>
        /// 获取租户id
        /// </summary>
        /// <returns></returns>

        public string GetTenantId();
    }
}