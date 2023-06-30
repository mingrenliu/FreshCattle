using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
namespace CenterClient.Extensions;

public static class EndpointExtension
{
    public static IConfigurationBuilder ConfigCenter(this IConfigurationBuilder builder, Action<RemoteConfigurationSource> action)
    {
        var source=new RemoteConfigurationSource();
        action.Invoke(source);
        builder.Add(action);
        return builder;
    }
}