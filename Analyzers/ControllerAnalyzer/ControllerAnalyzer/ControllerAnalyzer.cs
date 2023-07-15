using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace ControllerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControllerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string _suffix = "Controller";

        public const string DiagnosticId = "LY0012";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "ControllerHint";

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
/*            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }*/
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(ctx => ReportControllerHints(ctx), SyntaxKind.ClassDeclaration);
        }

        public static void ReportControllerHints(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax node)
            {
                if (node.Identifier.ValueText.EndsWith(_suffix))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), node.Identifier.ValueText.ToString()));
                }
            }
        }
    }
}