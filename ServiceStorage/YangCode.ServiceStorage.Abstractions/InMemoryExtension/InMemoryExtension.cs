using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.InMemory;

public static class InMemoryExtension
{
    public static StorageOptionsBuilder AddInMemory(this StorageOptionsBuilder builder)
    {
        builder.WithExtension(new InMemoryStorageOptionsExtension());
        return builder;
    }
}