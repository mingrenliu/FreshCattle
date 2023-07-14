global using Mapster;
using InheritCore;

namespace InheritAnalyzerTest
{
    [Inherit("UserConfig",true)]
    public partial class Class1
    {
        void Display()
        {
            var a=new Class1();
            Console.WriteLine(a.Config);
        }
    }

    public interface ITest
    {
        string GetString(string name, IEnumerable<string> list);
    }
}