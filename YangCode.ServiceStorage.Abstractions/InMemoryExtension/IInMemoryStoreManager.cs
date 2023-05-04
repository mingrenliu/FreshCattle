using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.InMemory;

public class InMemoryStoreManager : IStoreManager
{
    private readonly StorageOptions _options;
    private readonly Dictionary<string,ServiceInfo> _info;
    public InMemoryStoreManager(StorageOptions options)
    {
        _options = options;
        _info=new Dictionary<string,ServiceInfo>();
    }
    private string GetCacheKey(string key)
    {
        return _options.Prefix + key;
    }
    public void AddOrUpdate(ServiceInfo serviceInfo)
    {
        _info[GetCacheKey(serviceInfo.ServiceName)]=serviceInfo;
    }

    public void Clear()
    {
        _info.Clear();
    }

    public ServiceInfo? Get(string serviceName)
    {
        return _info.TryGetValue(serviceName, out var info) ? info : null; 
    }

    public IEnumerable<ServiceInfo> GetServices()
    {
        return _info.Values;
    }
}