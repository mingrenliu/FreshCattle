using ControllerAnalyzer.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ControllerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControllerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string _suffix = "Controller";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ControllerFieldHintDiagnostic.Rule, ControllerHintDiagnostic.Rule);

        public override void Initialize(AnalysisContext context)
        {
            /*            if (!Debugger.IsAttached)
                        {
                            Debugger.Launch();
                        }*/
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(ReportControllerHints, SyntaxKind.ClassDeclaration);
        }

        public static void ReportControllerHints(SyntaxNodeAnalysisContext context)
        {
            var options = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions;
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
                    context.ReportDiagnostic(Diagnostic.Create(ControllerFieldHintDiagnostic.Rule, node.GetLocation(), node.Identifier.ValueText));
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
                    context.ReportDiagnostic(Diagnostic.Create(ControllerHintDiagnostic.Rule, node.GetLocation(), additionalLocations, new object[] { node.Identifier.ValueText.ToString() }));
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