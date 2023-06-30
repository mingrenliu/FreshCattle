namespace CenterClient.Endpoints;

internal class ProtocolRoutePaths
{
    public const string PathPrefix = "center";
    public const string Discovery ="/discovery";
    public const string Load ="/load";
    public const string LoadAndSub ="/loadAndSub";
    public const string Subscribe = "/subscribe";
}

internal class EndpointNames
{
    public const string Discovery = "Discovery";
    public const string Load = "Load";
    public const string LoadAndSub = "LoadAndSub";
    public const string Subscribe = "Subscribe";
}

public class DefaultRemoteCenter
{
    /// <summary>
    /// 默认配置中心域名
    /// </summary>
    public const string DefaultCenter = "http://ConfigurationCenter";
}