namespace YangCode.ServiceStorage.Abstractions;

public class StorageOptionsBuilder
{
    private readonly StorageOptions _options;

    public StorageOptionsBuilder() : this(new StorageOptions())
    {
    }

    public StorageOptionsBuilder(StorageOptions options)
    {
        _options = options;
    }

    public virtual StorageOptions Build() => _options;

    public StorageOptionsBuilder WithPrefix(string prefix)
    {
        _options.WithPrefix(prefix);
        return this;
    }

    public StorageOptionsBuilder WithExtension(IStorageOptionsExtension extension)
    {
        _options.WithExtension(extension);
        return this;
    }
}