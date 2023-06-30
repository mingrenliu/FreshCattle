using CenterClient.Endpoints;
using System;

namespace CenterClient
{
    public class RemoteConfigurationOptions
    {
        /// <summary>
        /// 默认远程配置中心(根据域名)
        /// </summary>
        public string RemoteHost { get; set; } = DefaultRemoteCenter.DefaultCenter;
        /// <summary>
        /// 远程配置中心请求前缀
        /// </summary>
        public string PathPrefix { get; set; } = ProtocolRoutePaths.PathPrefix;
        /// <summary>
        /// 配置发现
        /// </summary>
        public string DiscoveryPath { get; set; } = ProtocolRoutePaths.Discovery;
        /// <summary>
        /// 获取配置
        /// </summary>
        public string LoadPath { get; set; } = ProtocolRoutePaths.Load;
        /// <summary>
        /// 配置变更订阅
        /// </summary>
        public string SubscribePath { get; set; } = ProtocolRoutePaths.Subscribe;
        /// <summary>
        /// 订阅回调
        /// </summary>
        public string? CallBackPath { get; set; }
        /// <summary>
        /// 是否订阅配置变更消息
        /// </summary>
        public bool SubscribeChanges=true;
        public Uri GetLoadPath()
        {
            return new Uri(new Uri(RemoteHost), Path.Combine(PathPrefix, LoadPath));
        }
    }
}