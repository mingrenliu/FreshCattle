using DatabaseHandler.DbContexts;
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
        public static IServiceCollection AddEntityFramework(this IServiceCollection services, Action<DbContextOptionsBuilder> optionBuilder)
        {
            services.TryAddSingleton<SnowflakeAlgorithm>();
            services.AddCurrentUser();
            services.AddFactory(optionBuilder);
            return services;
        }

        internal static IServiceCollection AddCurrentUser(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped(sp =>
            {
                return sp.GetRequiredService<IHttpContextAccessor>().HttpContext.GetUserFromHeader();
            });
            return services;
        }

        public static IServiceCollection AddFactory(this IServiceCollection services, Action<DbContextOptionsBuilder> optionBuilder)
        {
            services.AddPooledDbContextFactory<BaseDbContext>(config =>
            {
                optionBuilder(config);
            });
            services.AddScoped<DbContextFactory>();
            services.AddScoped(sp => sp.GetRequiredService<DbContextFactory>().CreateDbContext());
            return services;
        }
    }
}