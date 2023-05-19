using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace AttributeApplyAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DataFormatAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            new DiagnosticDescriptor("LY0001",
                                     "apply type error",
                                     "this data format must use in {0} type property",
                                     "LY",
                                     DiagnosticSeverity.Error,
                                     true,
                                     "应用类型错误",
                                     "https://github.com/mingrenliu/FreshCattle/tree/master/ExcelUtils"));

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationAction(CheckApplyType);
        }
        private void CheckApplyType(CompilationAnalysisContext context)
        {

        }
    }
}