using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.Redis;

public class RedisStorageOptionsExtension : IStorageOptionsExtension
{
    private readonly ConfigurationOptions _option;

    public ConfigurationOptions Option => _option;

    public RedisStorageOptionsExtension(string ip,string password,int port=6379, int dbIndex=-1)
    {
        _option = new ConfigurationOptions
        {
            Password = password,
            DefaultDatabase = dbIndex,
            AbortOnConnectFail = false
        };
        _option.EndPoints.Add(ip + ":" + port);
    }
    public RedisStorageOptionsExtension(ConfigurationOptions option)
    {
        _option=option;
    }

    public void ApplyServices(IServiceCollection services)
    {
        services.AddCoreService(this);
    }
}
internal static class RedisStorageExtension
{
    public static IServiceCollection AddCoreService(this IServiceCollection services, RedisStorageOptionsExtension option)
    {
        var multiplexer = ConnectionMultiplexer.Connect(option.Option);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddScoped(sp=>multiplexer.GetDatabase(option.Option.DefaultDatabase??-1));
        services.AddScoped<IStoreManager, RedisStoreManager>();
        return services;
    }
}


