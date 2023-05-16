using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.InMemory;

public class InMemoryStoreManager : StoreManager
{
    private readonly Dictionary<string,ServiceInfo> _info;
    public InMemoryStoreManager(StorageOptions options):base(options)
    {
        _info=new Dictionary<string,ServiceInfo>();
    }
    public override void AddOrUpdate(ServiceInfo serviceInfo)
    {
        _info[GetCacheKey(serviceInfo.ServiceName)]=serviceInfo;
    }

    public override Task AddOrUpdateAsync(ServiceInfo serviceInfo)
    {
        _info[GetCacheKey(serviceInfo.ServiceName)]=serviceInfo;
        return Task.CompletedTask;
    }

    public override void Clear()
    {
        _info.Clear();
    }

    public override Task ClearAsync()
    {
        _info.Clear();
        return Task.CompletedTask;
    }

    public override void Delete(string serviceName)
    {
        var key=GetCacheKey(serviceName);
        _info.Remove(key);
    }

    public override Task DeleteAsync(string serviceName)
    {
        Delete(serviceName);
        return Task.CompletedTask;
    }

    public override ServiceInfo? Get(string serviceName)
    {
        return _info.TryGetValue(serviceName, out var info)&&(!_options.ExpireTime.HasValue||(info.RegisterTime + _options.ExpireTime.Value)>=DateTime.Now) ? info : null; 
    }

    public override Task<ServiceInfo?> GetAsync(string serviceName)
    {
        var result= Get(serviceName); 
        return Task.FromResult(result);
    }

    public override IEnumerable<ServiceInfo> GetServices()
    {
        var results= _info.Values;
        if (_options.ExpireTime.HasValue)
        {
            var time=DateTime.Now-_options.ExpireTime.Value;
            return results.Where(x => x.RegisterTime >= time);
        }
        return results;
    }

    public override Task<IEnumerable<ServiceInfo>> GetServicesAsync()
    {
        IEnumerable<ServiceInfo> result= GetServices();
        return Task.FromResult(result);
    }
}