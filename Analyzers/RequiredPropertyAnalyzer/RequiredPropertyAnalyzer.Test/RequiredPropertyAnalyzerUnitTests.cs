using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = RequiredPropertyAnalyzer.Test.CSharpCodeFixVerifier<
    RequiredPropertyAnalyzer.RequiredPropertyAnalyzer,
    RequiredPropertyAnalyzer.RequiredPropertyCodeFixProvider>;

namespace RequiredPropertyAnalyzer.Test
{
    [TestClass]
    public class RequiredPropertyAnalyzerUnitTest
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
        public async Task TestMethod2()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RequiredPropertyAnalyzer.Test
{
#pragma warning disable CS8618
    public class {|#0:TestModel|}
    {
        public int Age { get; set; }
        public string {|#1:Name|} { get; set; }
        public TestEnum Status { get; set; }
        public DateTime? Date { get; set; }
        [Required]
        public string Name1 { get; set; }
    }
    public enum TestEnum
    {
        A,
        B,
        C
    }
}
#pragma warning restore CS8618";

            var fixtest = @"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RequiredPropertyAnalyzer.Test
{
#pragma warning disable CS8618
    public class TestModel
    {
        public int Age { get; set; }

        [Required]
        public string Name { get; set; }
        public TestEnum Status { get; set; }
        public DateTime? Date { get; set; }

        [Required]
        public string Name1 { get; set; }
    }

    public enum TestEnum
    {
        A,
        B,
        C
    }
}
#pragma warning restore CS8618
";
            
            var expected = VerifyCS.Diagnostic(RequiredPropertyAnalyzer.DiagnosticId).WithLocation(0).WithLocation(1).WithArguments("TestModel");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
