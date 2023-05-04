namespace YangCode.ServiceStorage.Abstractions;

public class ServiceInfo
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public required string ServiceName { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 登记时间
    /// </summary>
    public DateTime? RegisterTime { get; set; } = DateTime.Now;

    /// <summary>
    /// ip地址
    /// </summary>
    public required string IPAddress { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 请求协议
    /// </summary>
    public string Schemal { get; set; } = "https";
}