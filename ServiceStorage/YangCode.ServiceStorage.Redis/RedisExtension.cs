using StackExchange.Redis;
using YangCode.ServiceStorage.Abstractions;

namespace YangCode.ServiceStorage.Redis
{
    internal static class RedisExtension
    {
        public static StorageOptionsBuilder AddRedis(this StorageOptionsBuilder builder, string ip,string password,int port=6379, int dbIndex = -1)
        {
            builder.WithExtension(new RedisStorageOptionsExtension(ip,password,port,dbIndex));
            return builder;
        }
        public static StorageOptionsBuilder AddRedis(this StorageOptionsBuilder builder, Action<ConfigurationOptions> optionBuilder)
        {
            var config=new ConfigurationOptions();  
            optionBuilder(config);
            builder.WithExtension(new RedisStorageOptionsExtension(config));
            return builder;
        }
    }
}