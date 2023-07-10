using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = ControllerAnalyzer.Test.CSharpCodeFixVerifier<
    ControllerAnalyzer.ControllerAnalyzerAnalyzer,
    ControllerAnalyzer.ControllerAnalyzerCodeFixProvider>;

namespace ControllerAnalyzer.Test
{
    [TestClass]
    public class ControllerAnalyzerUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TwoLocationTest_Diagnostic()
        {
            var sourceCode = @"
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
";
            await VerifyCS.VerifyAnalyzerAsync(sourceCode);
        }
    }
}
