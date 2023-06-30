namespace CenterClient;

public class RemoteConfigurationProvider : ConfigurationProvider
{
    private readonly RemoteConfigurationSource _source;

    public RemoteConfigurationProvider(RemoteConfigurationSource source)
    {
        _source = source;
    }

    public override void Load()
    {
        var url = _source.Options.GetLoadPath();
        using var client = new HttpClient();
        var result = client.GetAsync(url).GetAwaiter().GetResult();
        if (result.IsSuccessStatusCode)
        {
            using var stream = result.Content.ReadAsStream();
            using var doc = JsonDocument.Parse(stream);
            Data = new JsonDocumentParser().Parse(doc);
        }
    }
}