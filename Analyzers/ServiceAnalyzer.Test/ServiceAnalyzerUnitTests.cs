using System.Threading.Tasks;
using VerifyCS = ServiceAnalyzer.Test.CSharpCodeFixVerifier<
    ServiceAnalyzer.ServiceDiagnostic,
    ServiceAnalyzer.ServiceCodeFixProvider>;

namespace MakeConst.Test
{
    [TestClass]
    public class MakeConstUnitTest
    {
        [TestMethod]
        public async Task LocalIntCouldBeConstant_Diagnostic()
        {
            await VerifyCS.VerifyCodeFixAsync(@"using System;

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
    }
}
", @"using System;

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
}");
        }
    }
}