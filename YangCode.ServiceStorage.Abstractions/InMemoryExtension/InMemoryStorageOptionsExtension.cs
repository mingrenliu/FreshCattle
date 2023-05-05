using Microsoft.Extensions.DependencyInjection;
using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.InMemory;

public class InMemoryStorageOptionsExtension : IStorageOptionsExtension
{
    public void ApplyServices(IServiceCollection services)
    {
        services.AddStore();
    }
}
public static class StorageExtension
{
    public static IServiceCollection AddStore(this IServiceCollection services)
    {
        services.AddSingleton<IStoreManager,InMemoryStoreManager>();
        return services;
    }
}