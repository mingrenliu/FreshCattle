using Microsoft.Extensions.DependencyInjection;
using YangCode.ServiceStorage.InMemory;

namespace YangCode.ServiceStorage.Abstractions;

public static class ServiceExtension
{
    public static StorageOptionsBuilder AddServiceStorage(this IServiceCollection services, Action<StorageOptionsBuilder> actions)
    {
        var builder = new StorageOptionsBuilder();
        actions.Invoke(builder);
        var extension = builder.Build().Extension ?? new InMemoryStorageOptionsExtension();
        extension.ApplyServices(services);
        services.AddSingleton(builder);
        services.AddSingleton<StorageOptions>(builder.Build());
        return builder;
    }
}