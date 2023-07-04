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
    }
}
";
            var fixedCode= @"using System;

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
";
            var expected = VerifyCS.Diagnostic(ServiceHintDiagnostic.DiagnosticId).WithLocation(0).WithLocation(1).WithArguments("ConfigService")
                .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
            
            await VerifyCS.VerifyCodeFixAsync(sourceCode,expected, sourceCode);

        }
    }
}