namespace YangCode.ServiceStorage.Abstractions;

public class StorageOptions
{
    private const string defaultPrefix = "service_";
    private IStorageOptionsExtension? _extension;
    private string? _prefixName;
    public IStorageOptionsExtension? Extension => _extension;
    public string Prefix => _prefixName??defaultPrefix;

    public StorageOptions WithPrefix(string prefix)
    {
        _prefixName = prefix;
        return this;
    }

    public StorageOptions WithExtension(IStorageOptionsExtension extension)
    {
        _extension = extension;
        return this;
    }
}