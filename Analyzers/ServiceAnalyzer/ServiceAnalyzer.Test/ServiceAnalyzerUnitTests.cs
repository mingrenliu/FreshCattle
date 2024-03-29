﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = ServiceAnalyzer.Test.CSharpCodeFixVerifier<
    ServiceAnalyzer.ServiceAnalyzerAnalyzer,
    ServiceAnalyzer.ServiceAnalyzerCodeFixProvider>;

namespace ServiceAnalyzer.Test
{
    [TestClass]
    public class ServiceAnalyzerUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TwoLocationTest_Diagnostic()
        {
            var sourceCode = @"using System;

namespace ServiceAnalyzer.Test
{
    public class ConfigService : IConfigService
    {
        public void Display(string name)
        {
            Console.WriteLine(name);
        }

        public void {|#1:Display1|}(string name)
        {
            Console.WriteLine(name);
        }
    }

    public interface {|#0:IConfigService|}
    {
        void Display(string name);
    }
}";
            var fixedCode = @"using System;

namespace ServiceAnalyzer.Test
{
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

    public interface IConfigService
    {
        void Display(string name);
        void Display1(string name);
    }
}";
            var expected = VerifyCS.Diagnostic(ServiceAnalyzerAnalyzer.DiagnosticId).WithLocation(0).WithLocation(1).WithArguments("ConfigService")
                .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Info);

            await VerifyCS.VerifyCodeFixAsync(sourceCode, expected, fixedCode);

        }
        [TestMethod]
        public async Task ThreeLocationTest_Diagnostic()
        {
            var sourceCode = @"using System;

namespace ServiceAnalyzer.Test
{
    public class ConfigService : IConfigService
    {
        public void {|#2:Display|}(string name)
        {
            Console.WriteLine(name);
        }

        public void {|#1:Display1|}(string name)
        {
            Console.WriteLine(name);
        }
    }

    public interface {|#0:IConfigService|}
    {
    }
}";
            var fixedCode = @"using System;

namespace ServiceAnalyzer.Test
{
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

    public interface IConfigService
    {
        void Display(string name);
        void Display1(string name);
    }
}";
            var expected = VerifyCS.Diagnostic(ServiceAnalyzerAnalyzer.DiagnosticId).WithLocation(0).WithLocation(2).WithLocation(1).WithArguments("ConfigService")
                .WithSeverity(Microsoft.CodeAnalysis.DiagnosticSeverity.Info);

            await VerifyCS.VerifyCodeFixAsync(sourceCode, expected, fixedCode);

        }
    }
}
