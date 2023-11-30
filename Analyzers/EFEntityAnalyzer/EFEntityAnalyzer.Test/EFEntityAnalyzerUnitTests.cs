using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = EFEntityAnalyzer.Test.CSharpCodeFixVerifier<
    EFEntityAnalyzer.EFEntityAnalyzerAnalyzer,
    EFEntityAnalyzer.EFEntityAnalyzerCodeFixProvider>;

namespace EFEntityAnalyzer.Test
{
    [TestClass]
    public class EFEntityAnalyzerUnitTest
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
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("EFEntityAnalyzer").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
