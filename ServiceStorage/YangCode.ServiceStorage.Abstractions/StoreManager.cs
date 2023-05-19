namespace YangCode.ServiceStorage.Abstractions
{
    public abstract class StoreManager : IStoreManager
    {
        protected readonly StorageOptions _options;

        public StoreManager(StorageOptions options)
        {
            _options = options;
        }

        public virtual string GetCacheKey(string key)
        {
            return _options.Prefix + key;
        }

        public abstract void AddOrUpdate(ServiceInfo serviceInfo);

        public abstract void Clear();

        public abstract ServiceInfo? Get(string serviceName);

        public abstract IEnumerable<ServiceInfo> GetServices();

        public abstract Task<ServiceInfo?> GetAsync(string serviceName);

        public abstract Task<IEnumerable<ServiceInfo>> GetServicesAsync();

        public abstract Task ClearAsync();

        public abstract Task AddOrUpdateAsync(ServiceInfo serviceInfo);

        public abstract void Delete(string serviceName);

        public abstract Task DeleteAsync(string serviceName);
    }
}