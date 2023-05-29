using DatabaseHandler.DbContexts;
using DatabaseHandler.Entities;
using DatabaseHandler.Utils;
using DatabaseHandler.ValueGenerators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DatabaseHandler.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddEntityFramework<T>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionBuilder) where T : BaseDbContext
        {
            services.TryAddSingleton<SnowflakeAlgorithm>();
            services.AddCurrentUser();
            services.AddFactory<T>(optionBuilder);
            return services;
        }
        public static IServiceCollection AddCurrentUser(this IServiceCollection services,Func<IServiceProvider,ICurrentUser> action)
        {
            services.AddHttpContextAccessor();
            services.AddScoped(action);
            return services;
        }
        public static IServiceCollection AddCurrentUser(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddScoped<ICurrentUser>(sp =>
            {
                return sp.GetRequiredService<IHttpContextAccessor>().HttpContext.GetUserFromHeader();
            });
            return services;
        }

        public static IServiceCollection AddFactory<T>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionBuilder) where T:BaseDbContext
        {
            services.AddPooledDbContextFactory<T>(config =>
            {
                optionBuilder(config);
            });
            services.AddScoped(typeof(ScopeDbContextFactory<>));
            services.AddScoped<BaseDbContext>(sp => sp.GetRequiredService<ScopeDbContextFactory<T>>().CreateDbContext());
            services.AddScoped(sp => sp.GetRequiredService<ScopeDbContextFactory<T>>().CreateDbContext());
            return services;
        }
    }
}