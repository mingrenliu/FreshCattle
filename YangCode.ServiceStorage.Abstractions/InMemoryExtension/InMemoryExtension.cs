using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YangCode.ServiceStorage.InMemory;

namespace YangCode.ServiceStorage.Abstractions.InMemoryExtension
{
    public static class InMemoryExtension
    {
        public static StorageOptionsBuilder AddInMemory(this StorageOptionsBuilder builder)
        {
            builder.WithExtension(new InMemoryStorageOptionsExtension());
            return builder;
        }
    }
}
