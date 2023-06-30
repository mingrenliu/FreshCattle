namespace CenterClient;

public class RemoteConfigurationSource : IConfigurationSource
{
    public RemoteConfigurationOptions Options { get; set; }=new RemoteConfigurationOptions();

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new RemoteConfigurationProvider(this);
}