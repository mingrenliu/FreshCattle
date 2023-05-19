namespace YangCode.ServiceStorage.Abstractions;

public interface IStoreManager
{
    /// <summary>
    /// 获取服务注册信息
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    ServiceInfo? Get(string serviceName);
    /// <summary>
    /// 异步注册
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    Task<ServiceInfo?> GetAsync(string serviceName);
    /// <summary>
    /// 获取所有服务注册信息
    /// </summary>
    /// <returns></returns>
    IEnumerable<ServiceInfo> GetServices();
    Task<IEnumerable<ServiceInfo>> GetServicesAsync();
    /// <summary>
    /// 清空所有服务注册信息
    /// </summary>
    void Clear();
    Task ClearAsync();
    /// <summary>
    /// 删除特定服务
    /// </summary>
    /// <param name="serviceName"></param>
    void Delete(string serviceName);
    Task DeleteAsync(string serviceName);
    /// <summary>
    /// 新增或更新服务信息
    /// </summary>
    /// <param name="serviceInfo"></param>
    void AddOrUpdate(ServiceInfo serviceInfo);
    Task AddOrUpdateAsync(ServiceInfo serviceInfo);

}
