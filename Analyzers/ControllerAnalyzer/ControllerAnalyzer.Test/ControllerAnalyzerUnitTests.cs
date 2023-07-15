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
        //no work
        [TestMethod]
        public async Task TwoLocationTest_Diagnostic()
        {
            var sourceCode = @"using System.Threading.Tasks;
namespace TestWebApi.Controllers
{
    public class {|#0:TestController|}
    {
        private readonly string _service;
        public TestController(string service)
        {
            _service = service;
        }
        public void Display(string message)
        {
            _service.ToString();
        }
    }
    public interface ITestService
    {
        Task DeleteAsync(string id);

        Task<string> Get(string id);

        void Display(string name);

        string GetName(string name);
    }
}
";
            var fixedSource = @"using System.Threading.Tasks;
namespace TestWebApi.Controllers
{
    public class {|#0:TestController|}
    {
        private readonly ITestService _service;
        public TestController(ITestService service)
        {
            _service = service;
        }
        public void Display(string message)
        {
            _service.Display(message);
        }
    }
    public interface ITestService
    {
        Task DeleteAsync(string id);

        Task<string> Get(string id);

        void Display(string name);

        string GetName(string name);
    }
}
";
            var excepted = VerifyCS.Diagnostic(ControllerAnalyzer.DiagnosticId).WithLocation(0).WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("TestController");
            await VerifyCS.VerifyCodeFixAsync(sourceCode, excepted, fixedSource);
        }
    }
}