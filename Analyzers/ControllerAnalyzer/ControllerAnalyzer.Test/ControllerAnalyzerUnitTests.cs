﻿using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            var sourceCode = @"#nullable enable
using System;
using System.Threading.Tasks;
namespace ControllerTest
{
    public class {|#0:TestController|}
    {
        private readonly ITestService _testService;

        /// <summary>
        ///
        /// </summary>
        /// <param name=""testService""></param>
        public TestController(ITestService testService)
        {
            _testService = testService;
        }
    }
    public interface ITestService
    {
        Task DeleteAsync(string? id);

        Task<string> Get(TimeSpan id);

        void Display(Uri name);

        string GetName(string name);
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RequiredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : Attribute
    {
    }
}
";
            var fixedSource = @"#nullable enable
using System;
using System.Threading.Tasks;

namespace ControllerTest
{
    public class TestController
    {
        private readonly ITestService _testService;
        /// <summary>
        ///
        /// </summary>
        /// <param name = ""testService""></param>
        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name = ""id""></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete(string? id)
        {
            await _testService.DeleteAsync(id);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name = ""id""></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Get([Required] TimeSpan id)
        {
            return await _testService.Get(id);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name = ""name""></param>
        /// <returns></returns>
        [HttpGet]
        public void Display([Required] Uri name)
        {
            _testService.Display(name);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name = ""name""></param>
        /// <returns></returns>
        [HttpGet]
        public string GetName([Required] string name)
        {
            return _testService.GetName(name);
        }
    }

    public interface ITestService
    {
        Task DeleteAsync(string? id);
        Task<string> Get(TimeSpan id);
        void Display(Uri name);
        string GetName(string name);
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RequiredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : Attribute
    {
    }
}";
            var excepted = VerifyCS.Diagnostic(ControllerAnalyzer.DiagnosticId).WithLocation(0).WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("TestController");
            await VerifyCS.VerifyCodeFixAsync(sourceCode, excepted, fixedSource);
        }
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class RequiredAttribute : Attribute
{
}

/// <summary>
///
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class HttpPostAttribute : Attribute
{
}