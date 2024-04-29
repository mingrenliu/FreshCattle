using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = EFEntityAnalyzer.Test.CSharpCodeFixVerifier<
    EFEntityAnalyzer.EFEntityAnalyzer,
    EFEntityAnalyzer.EFEntityCodeFixProvider>;

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
            var test = @"using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace EFEntityAnalyzer.Test
{
    [Table(""sample"")]
    public class {|#0:Sample|}
    {
        public string {|#1:Name|} { get; set; }
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
    [Table(""sample"")]
    [Comment("""")]
    public class Sample
    {
        [StringLength(50)]
        [Comment("""")]
        public string Name { get; set; }

        [Precision(0)]
        [Comment("""")]
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

            var expected = VerifyCS.Diagnostic(EFEntityAnalyzer.Rule).WithLocation(0).WithLocation(0).WithLocation(1).WithLocation(2).WithArguments("Sample");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task TestMethodWithEnum()
        {
            var test = @"using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace EFEntityAnalyzer.Test
{
    [Table(""sample"")]
    public class {|#0:Sample|}
    {
        /// <summary>
        /// fewf
        /// </summary>
        public string {|#1:Name|} { get; set; }
        public DateTime {|#2:Date|} { get; set; }
        public UploadStatus {|#3:Status|} { get; set; }
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
    public enum UploadStatus
    {
        /// <summary>
        /// 待上传
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 上传失败
        /// </summary>
        Fail = 1,

        /// <summary>
        /// 上传中
        /// </summary>
        Uploading = 2,

        /// <summary>
        /// 上传成功
        /// </summary>
        Success = 3
    }
}";

            var fixtest = @"using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EFEntityAnalyzer.Test
{
    [Table(""sample"")]
    [Comment("""")]
    public class Sample
    {
        /// <summary>
        /// fewf
        /// </summary>
        [StringLength(50)]
        [Comment("""")]
        public string Name { get; set; }

        [Precision(0)]
        [Comment("""")]
        public DateTime Date { get; set; }

        [Comment("""")]
        public UploadStatus Status { get; set; }
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

    public enum UploadStatus
    {
        /// <summary>
        /// 待上传
        /// </summary>
        Pending = 0,
        /// <summary>
        /// 上传失败
        /// </summary>
        Fail = 1,
        /// <summary>
        /// 上传中
        /// </summary>
        Uploading = 2,
        /// <summary>
        /// 上传成功
        /// </summary>
        Success = 3
    }
}";

            var expected = VerifyCS.Diagnostic(EFEntityAnalyzer.Rule).WithLocation(0).WithLocation(0).WithLocation(1).WithLocation(2).WithLocation(3).WithArguments("Sample");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}