using StackExchange.Redis;
using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.Redis;

public class RedisStoreManager : StoreManager
{
    private readonly IDatabase _database;
    public RedisStoreManager(IDatabase database, StorageOptions option):base(option)
    {
        _database = database;
    }
    public override void AddOrUpdate(ServiceInfo serviceInfo)
    {
        var key = GetCacheKey(serviceInfo.ServiceName);
        _database.HashSet(key, GetHashEntry(serviceInfo));
        if (_options.ExpireTime.HasValue)
        {
            _database.KeyExpire(key, _options.ExpireTime.Value);
        }
    }
    private HashEntry[] GetHashEntry(ServiceInfo serviceInfo)
    {
        return new HashEntry[] { 
        new HashEntry(nameof(serviceInfo.Schema),serviceInfo.Schema),
        new HashEntry(nameof(serviceInfo.ServiceName),serviceInfo.ServiceName),
        new HashEntry(nameof(serviceInfo.IPAddress),serviceInfo.IPAddress),
        new HashEntry(nameof(serviceInfo.Port),serviceInfo.Port),
        new HashEntry(nameof(serviceInfo.RegisterTime),DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
        };
    }
    public override void Clear()
    {
        return;
    }

    public override ServiceInfo? Get(string serviceName)
    {
        var key=GetCacheKey(serviceName);
        if (_database.KeyExists(key))
        {
            var lst=_database.HashGetAll(key);
            if (lst != null && lst.Any())
            {
                var ip = lst.FirstOrDefault(x => x.Name == nameof(ServiceInfo.IPAddress)).Value;
                return new ServiceInfo(serviceName, ip.ToString())
                {
                    Schema = lst.First(x => x.Name == nameof(ServiceInfo.Schema)).Value.ToString(),
                    ServiceName = serviceName,
                    IPAddress = ip.ToString(),
                    Port = (int)lst.First(x => x.Name == nameof(ServiceInfo.Port)).Value
                };
            }
        }
        return null;
    }

    public override IEnumerable<ServiceInfo> GetServices()
    {
        throw new NotImplementedException();
    }

    public override Task<ServiceInfo?> GetAsync(string serviceName)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<ServiceInfo>> GetServicesAsync()
    {
        throw new NotImplementedException();
    }

    public override Task ClearAsync()
    {
        throw new NotImplementedException();
    }

    public override Task AddOrUpdateAsync(ServiceInfo serviceInfo)
    {
        throw new NotImplementedException();
    }

    public override void Delete(string serviceName)
    {
        throw new NotImplementedException();
    }

    public override Task DeleteAsync(string serviceName)
    {
        throw new NotImplementedException();
    }
}