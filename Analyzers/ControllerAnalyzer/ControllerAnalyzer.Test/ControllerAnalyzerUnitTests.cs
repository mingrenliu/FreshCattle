using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = ControllerAnalyzer.Test.CSharpCodeFixVerifier<
    ControllerAnalyzer.ControllerAnalyzer,
    ControllerAnalyzer.ControllerAnalyzerCodeFixProvider>;

namespace ControllerAnalyzer.Test
{
    [TestClass]
    public class ControllerAnalyzerUnitTest
    {
        [TestMethod]
        public async Task TwoLocationTest_Diagnostic()
        {
            var sourceCode = @"namespace TestWebApi.Controllers
{
    public class {|#0:TestController|}
    {
    }
}
";
            var fixedSource = @"namespace TestWebApi.Controllers
{
    public class {|#0:TestController|}
    {
    }
}
";
            var excepted = VerifyCS.Diagnostic(ControllerAnalyzer.DiagnosticId).WithLocation(0).WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("TestController");
            await VerifyCS.VerifyCodeFixAsync(sourceCode, excepted, fixedSource);
        }
    }
}