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
            var sourceCode = @"using System.Threading.Tasks;
namespace ControllerTest
{
    public class {|#0:TestController|}
    {
        private readonly ITestService _testService;

        /// <summary>
        /// 
        /// </summary>
        /// <paramtestService=""testService""></param>
        public TestController(ITestService testService)
        {
            _testService = testService;
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
namespace ControllerTest
{
    public class {|#0:TestController|}
    {
        private readonly ITestService _testService;

        /// <summary>
        /// 
        /// </summary>
        /// <paramtestService=""testService""></param>
        public TestController(ITestService testService)
        {
            _testService = testService;
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