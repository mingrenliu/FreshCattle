using System;

namespace ServiceAnalyzer.Test
{
    public class ConfigService : IConfigService
    {
        public void Display(string name)
        {
            Console.WriteLine(name);
        }
    }

    public interface IConfigService
    {
        void Display(string name);
    }
}