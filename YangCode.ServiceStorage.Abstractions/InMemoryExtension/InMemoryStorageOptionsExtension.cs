using Microsoft.Extensions.DependencyInjection;
using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.InMemory;

public class InMemoryStorageOptionsExtension : IStorageOptionsExtension
{
    public void ApplyServices(IServiceCollection services)
    {
        services.AddCoreService();
    }
}
internal static class InMemoryStorageExtension
{
    public static IServiceCollection AddCoreService(this IServiceCollection services)
    {
        services.AddSingleton<IStoreManager,InMemoryStoreManager>();
        return services;
    }
}