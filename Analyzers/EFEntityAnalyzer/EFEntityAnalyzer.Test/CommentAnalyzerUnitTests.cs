using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using EFEntityAnalyzer;
using VerifyCS = EFEntityAnalyzer.Test.CSharpCodeFixVerifier<
    EFEntityAnalyzer.CommentAnalyzer,
    EFEntityAnalyzer.CommentCodeFixProvider>;

namespace CommentAnalyzer.Test
{
    [TestClass]
    public class CommentAnalyzerUnitTest
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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EFEntityAnalyzer.Test
{
    [Table(""sample"")]
    [Comment(""表名"")]
    public class {|#0:Sample|}
    {
        [StringLength(50)]
        [Comment(""名称"")]
        public string {|#1:Name|} { get; set; }

        [Precision(0)]
        [Comment(""日期"")]
        public DateTime {|#2:Date|} { get; set; }
    }

    public class CommentAttribute : Attribute
    {
        public CommentAttribute(string comment)
        {
        }
    }

    public class PrecisionAttribute : Attribute
    {
        public PrecisionAttribute(int value)
        {
        }

        public PrecisionAttribute(int value, int value2)
        {
        }
    }
}";
            var fixtest = @"using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EFEntityAnalyzer.Test
{
    /// <summary>
    /// 表名
    /// </summary>
    [Table(""sample"")]
    [Comment(""表名"")]
    public class Sample
    {
        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        [Comment(""名称"")]
        public string Name { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Precision(0)]
        [Comment(""日期"")]
        public DateTime Date { get; set; }
    }

    public class CommentAttribute : Attribute
    {
        public CommentAttribute(string comment)
        {
        }
    }

    public class PrecisionAttribute : Attribute
    {
        public PrecisionAttribute(int value)
        {
        }

        public PrecisionAttribute(int value, int value2)
        {
        }
    }
}";

            var expected = VerifyCS.Diagnostic(EFEntityAnalyzer.CommentAnalyzer.Rule).WithLocation(0).WithLocation(0).WithLocation(1).WithLocation(2).WithArguments("Sample");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}