using System.Text.Json.Serialization;

namespace YangCode.ServiceStorage.Abstractions;
public class ServiceInstanceInfo
{

}
public class ServiceInfo
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 登记时间
    /// </summary>
    [JsonIgnore]
    public DateTime RegisterTime { get; set; } = DateTime.Now;

    /// <summary>
    /// ip地址
    /// </summary>
    public string IPAddress { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 请求协议
    /// </summary>
    public string Schema { get; set; } = "https";

    public ServiceInfo(string serviceName, string ip)
    {
        ServiceName = serviceName;
        IPAddress = ip;
    }
}