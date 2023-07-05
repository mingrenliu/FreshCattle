using System;

namespace ServiceAnalyzer.Test
{
    public interface IConfigService
    {
        void Display(string name);
    }
    public class ConfigService : IConfigService
    {
        public void Display(string name)
        {
            Console.WriteLine(name);
        }
    }
}