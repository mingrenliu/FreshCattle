using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace ControllerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControllerAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string _suffix = "Controller";

        public const string DiagnosticId = "LY0012";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "ControllerHint";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule,CtorDiagnostic.Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(ReportControllerHints, SyntaxKind.ClassDeclaration);
        }

        public static void ReportControllerHints(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax node)
            {
                if (node.Identifier.ValueText.EndsWith(_suffix))
                {
                    var serviceName = GetServiceName(node.Identifier.ValueText);
                    foreach (var field in node.DescendantNodes().OfType<FieldDeclarationSyntax>())
                    {
                        if (field.Declaration.Type.ToString() == serviceName)
                        {
                            ReportServiceMethods(field);
                            return;
                        }
                    }
                    context.ReportDiagnostic(Diagnostic.Create(CtorDiagnostic.Rule, node.GetLocation(), node.Identifier.ValueText));
                }
            }
            void ReportServiceMethods(FieldDeclarationSyntax field)
            {
                var symbol = context.SemanticModel.GetSymbolInfo(field.Declaration.Type).Symbol as ITypeSymbol;
                var methods = symbol.GetMembers().OfType<IMethodSymbol>();
                var invokes = node.DescendantNodes().OfType<InvocationExpressionSyntax>();
                var notUsedMethods = methods.Except(FindMethodsFromInvokeExpression(invokes, context.SemanticModel));
                if (notUsedMethods.Any())
                {
                    var additionalLocations = new List<Location>();
                    foreach (var item in notUsedMethods)
                    {
                        additionalLocations.Add(item.Locations.First());
                    }
                    context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation(), additionalLocations,
                        new Dictionary<string, string>() { ["FieldName"] = symbol.Name }, new object[] { node.Identifier.ValueText.ToString() }));
                }
            }
        }

        public static IEnumerable<IMethodSymbol> FindMethodsFromInvokeExpression(IEnumerable<InvocationExpressionSyntax> invokes, SemanticModel model)
        {
            var results = new List<IMethodSymbol>();
            foreach (var invoke in invokes)
            {
                if (model.GetSymbolInfo(invoke).Symbol is IMethodSymbol methodSymbol)
                {
                    results.Add(methodSymbol);
                }
            }
            return results;
        }

        public static string GetServiceName(string controllerName)
        {
            return "I" + controllerName.Substring(0, controllerName.Length - 10) + "Service";
        }
    }
}
