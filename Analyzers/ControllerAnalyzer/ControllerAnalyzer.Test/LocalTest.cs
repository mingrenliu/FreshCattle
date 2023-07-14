using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace ControllerAnalyzer
{
    internal class LocalTest : AdditionalText
    {
        public override string Path => "C:\\Users\\cybstar\\source\\repos\\CustomeService\\ITestService.cs";

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            var str = @"namespace CustomeService
{
    public interface ITestService
    {
        Task DeleteAsync(string id);
        Task<string> Get(string id);
        void Display(string name);
        string GetName(string name);
    }
}";
            return SourceText.From(str);
        }
    }
}