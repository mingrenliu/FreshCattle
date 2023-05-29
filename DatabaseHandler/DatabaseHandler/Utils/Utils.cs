using DatabaseHandler.Entities;
using Microsoft.AspNetCore.Http;

namespace DatabaseHandler.Utils
{
    public static class Utils
    {
        public static CurrentUser GetUserFromQuery(this HttpContext context)
        {
            var userId = FilterSpace(context.Request.Query[UserInfoConstants.UserId].ToString());
            var tenantId = FilterSpace(context.Request.Query[UserInfoConstants.TenantId].ToString());
            var userName = FilterSpace(context.Request.Query[UserInfoConstants.UserName].ToString());
            return new CurrentUser(tenantId, userId, userName);
        }

        public static CurrentUser GetUserFromHeader(this HttpContext context)
        {
            var userId = FilterSpace(context.Request.Headers[UserInfoConstants.UserId].ToString());
            var tenantId = FilterSpace(context.Request.Headers[UserInfoConstants.TenantId].ToString());
            var userName = FilterSpace(context.Request.Headers[UserInfoConstants.UserName].ToString());
            return new CurrentUser(tenantId, userId, userName);
        }

        public static string? FilterSpace(string? str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str.Trim();
        }
    }
}