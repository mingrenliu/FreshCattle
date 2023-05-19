using Microsoft.Extensions.DependencyInjection;

namespace YangCode.ServiceStorage.Abstractions;
public interface IStorageOptionsExtension
{
    /// <summary>
    /// 根据不同的存储方式配置依赖服务
    /// </summary>
    /// <param name="services"></param>
    void ApplyServices(IServiceCollection services);
}
