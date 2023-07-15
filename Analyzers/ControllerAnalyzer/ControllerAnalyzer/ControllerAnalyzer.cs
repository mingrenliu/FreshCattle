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
        public const string FieldName = "FieldName";

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
                    var serviceName = GetServiceName(node.Identifier.ValueText.ToString());
                    var (field, fieldName) = GetField(node, serviceName);
                    if (field == null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), node.Identifier.ValueText.ToString()));
                        return;
                    }
                    else
                    {
                        if (context.SemanticModel.GetSymbolInfo(field).Symbol is INamedTypeSymbol symbol)
                        {
                            var methods = symbol.GetMembers().OfType<IMethodSymbol>();
                            if (!methods.Any())
                            {
                                return;
                            }
                            var implementMethods = GetMethods(GetMethods(node.DescendantNodes().OfType<MemberAccessExpressionSyntax>(), fieldName), context.SemanticModel);
                            var unUsedMethods = methods.Except(implementMethods);
                            if (unUsedMethods.Any())
                            {
                                var props = new Dictionary<string, string>() { [FieldName] = fieldName }.ToImmutableDictionary();
                                context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), props, node.Identifier.ValueText.ToString()));
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<IMethodSymbol> GetMethods(IEnumerable<MemberAccessExpressionSyntax> invokes, SemanticModel model)
        {
            foreach (var item in invokes)
            {
                yield return model.GetSymbolInfo(item).Symbol as IMethodSymbol;
            }
        }

        public static IEnumerable<MemberAccessExpressionSyntax> GetMethods(IEnumerable<MemberAccessExpressionSyntax> invokes, string name)
        {
            foreach (var invocation in invokes)
            {
                var token = invocation.Expression.DescendantTokens().FirstOrDefault();
                if (token != null && token.ValueText == name)
                {
                    yield return invocation;
                }
            }
        }

        public static string GetServiceName(string controllerName)
        {
            var baseName = controllerName.Substring(0, controllerName.Length - 10);
            return "I" + baseName + "Service";
        }

        public static (TypeSyntax,string) GetField(ClassDeclarationSyntax node, string name)
        {
            foreach (var item in node.DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                if (item.Declaration.Type.ToString() == name) return (item.Declaration.Type, item.Declaration.Variables.First().Identifier.ValueText);
            }
            return (null,null);
        }
    }
}