using System;

namespace ServiceAnalyzer.Test;

public class ConfigController
{
    private readonly IConfigService _configService;
    public void Dis(string name)
    {
        _configService.Display(name);
    }
}

public partial class ConfigService : IConfigService
{
    public void Display(string name)
    {
        Console.WriteLine(name);
    }

    public void Display1(string name)
    {
        Console.WriteLine(name);
    }
}

public interface IConfigService

{
    void Display(string name);

    void Display1(string name);
}