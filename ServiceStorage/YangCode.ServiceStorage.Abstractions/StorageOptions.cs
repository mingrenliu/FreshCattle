namespace YangCode.ServiceStorage.Abstractions;

public class StorageOptions
{
    private const string defaultPrefix = "service_";
    private IStorageOptionsExtension? _extension;
    private string? _prefixName;
    private TimeSpan? _expireTime;
    private int _defaultPort = 80;
    public IStorageOptionsExtension? Extension => _extension;
    public string Prefix => _prefixName??defaultPrefix;

    public TimeSpan? ExpireTime => _expireTime;

    public int DefaultPort => _defaultPort;

    public StorageOptions WithPrefix(string prefix)
    {
        _prefixName = prefix;
        return this;
    }
    public StorageOptions WithExpire(TimeSpan expireTime)
    {
        _expireTime = expireTime;
        return this;
    }
    public StorageOptions WithDefaultPort(int port)
    {
        _defaultPort = port;
        return this;
    }
    public StorageOptions WithExtension(IStorageOptionsExtension extension)
    {
        _extension = extension;
        return this;
    }
}