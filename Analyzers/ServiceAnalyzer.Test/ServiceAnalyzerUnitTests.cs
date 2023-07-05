using Microsoft;
using ServiceAnalyzer.Diagnostics;
using System.Threading.Tasks;
using VerifyCS = ServiceAnalyzer.Test.CSharpCodeFixVerifier<
    ServiceAnalyzer.ServiceDiagnostic,
    ServiceAnalyzer.ServiceCodeFixProvider>;

namespace ServiceAnalyzer.Test
{
    [TestClass]
    public class ServiceAnalyzerUnitTest
    {
        [TestMethod]
        public async Task LocalIntCouldBeConstant_Diagnostic()
        {
            var sourceCode = @"using System;

namespace ServiceAnalyzer.Test
{
    public class {|#1:ConfigService|} : IConfigService
    {
        public void Display(string name)
        {
            Console.WriteLine(name);
        }
    }

    public interface {|#0:IConfigService|}
    {
        void Display(string name);
    }
}
";
            var fixedCode= @"using System;

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
        public void Display1(string name)
        {
            Console.WriteLine(name);
        }
    }
}
";
            var expected = VerifyCS.Diagnostic(ServiceHintDiagnostic.DiagnosticId).WithLocation(0).WithLocation(1).WithArguments("ConfigService")
                .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
            
            await VerifyCS.VerifyCodeFixAsync(fixedCode,fixedCode);

        }
    }
}