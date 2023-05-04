using Microsoft.Extensions.DependencyInjection;
using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.InMemory
{
    public class InMemoryStorageOptionsExtension : IStorageOptionsExtension
    {
        public void ApplyServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}