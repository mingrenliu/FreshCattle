using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace ExcelAnalyzer
{
    /// <summary>
    /// 检测 [ExcelColumn] 和 [ExcelIgnore] 同时标注在同一属性上的冲突。
    /// [ExcelIgnore] 用于 opt-out 模式排除属性，若同时标注 [ExcelColumn] 则语义矛盾。
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExcelColumnIgnoreConflictAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: "LY0014",
            title: "[ExcelColumn] and [ExcelIgnore] conflict",
            messageFormat: "Property '{0}' has both [ExcelColumn] and [ExcelIgnore] — these are mutually exclusive",
            category: "ExcelColumn",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "[ExcelIgnore] excludes a property from serialization, so [ExcelColumn] on the same property has no effect.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(CheckConflict, SymbolKind.Property);
        }

        private static void CheckConflict(SymbolAnalysisContext context)
        {
            if (!(context.Symbol is IPropertySymbol prop)) return;

            var hasColumn = prop.GetAttributes().Any(a =>
                IsExcelAttribute(a, "ExcelColumnAttribute"));
            var hasIgnore = prop.GetAttributes().Any(a =>
                IsExcelAttribute(a, "ExcelIgnoreAttribute"));

            if (hasColumn && hasIgnore)
            {
                var location = prop.Locations.FirstOrDefault();
                if (location != null)
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location, prop.Name));
            }
        }

        private static bool IsExcelAttribute(AttributeData attr, string name)
        {
            return attr.AttributeClass != null
                && attr.AttributeClass.Name == name;
        }
    }
}
