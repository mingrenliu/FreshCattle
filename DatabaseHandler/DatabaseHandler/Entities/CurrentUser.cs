﻿namespace DatabaseHandler.Entities
{
    /// <summary>
    /// 默认用户信息
    /// </summary>
    public class CurrentUser : ICurrentUser
    {
        protected virtual string DefaultTenantId { get; } = "global";

        /// <summary>
        /// 租户id
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string? UserName { get; set; }

        public CurrentUser(string? tenantId, string? userId, string? userName)
        {
            UserId = userId;
            UserName = userName;
            TenantId = string.IsNullOrWhiteSpace(tenantId) ? DefaultTenantId : tenantId;
        }

        public string? GetUserName() => UserName;

        public string? GetUserId() => UserId;

        public string GetTenantId() => TenantId;
    }
}