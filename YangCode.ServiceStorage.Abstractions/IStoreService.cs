namespace YangCode.ServiceStorage.Abstractions;

public interface IStoreService
{
    /// <summary>
    /// 获取服务注册信息
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    ServiceInfo? Get(string serviceName);
    /// <summary>
    /// 获取所有服务注册信息
    /// </summary>
    /// <returns></returns>
    IEnumerable<ServiceInfo> GetServices();
    /// <summary>
    /// 清空所有服务注册信息
    /// </summary>
    void Clear();
    /// <summary>
    /// 新增或更新服务信息
    /// </summary>
    /// <param name="serviceInfo"></param>
    void AddOrUpdate(ServiceInfo serviceInfo);

}
