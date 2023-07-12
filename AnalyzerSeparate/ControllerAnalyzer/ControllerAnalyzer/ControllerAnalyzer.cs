using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ControllerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ControllerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string _suffix = "Controller";
        private static readonly string _service = "Service";
        public const string PropertyNameBase = "build_property.";
        public static List<Regex> Patterns = new() { new Regex(@"^I(\w+)Service$"), new Regex(@"^(\w+)Service(.\w+)+$") };
        public const string DiagnosticId = "LY0012";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "ControllerHint";

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public bool TryGetInterfaceByFileName(List<Regex> patterns, string fileName, out string name)
        {
            name = string.Empty;
            foreach (var pattern in patterns)
            {
                var match = pattern.Match(fileName);
                if (match.Success)
                {
                    name = match.Groups[1].Value;
                    return true;
                }
            }
            return false;
        }

        public override void Initialize(AnalysisContext context)
        {
/*            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }*/
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(ctx =>
            {
                var haveOption = ctx.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(PropertyNameBase + "ServiceNameModel", out var serviceNameModel) && !string.IsNullOrEmpty(serviceNameModel);
                var patterns = haveOption ? new List<Regex>() { new Regex("^" + serviceNameModel + "$") } : Patterns;
                var files = new Dictionary<string, AdditionalText>();
                var additionalFiles=new List<AdditionalText>() { new LocalTest() };
                //var additionalFiles = ctx.Options.AdditionalFiles;
                foreach (var item in additionalFiles)
                {
                    var fileName = item.Path.Split(Path.DirectorySeparatorChar).Last();
                    if (fileName.EndsWith(".cs") && TryGetInterfaceByFileName(patterns, fileName.Substring(0, fileName.Length - 3), out var name))
                    {
                        files[name] = item;
                    }
                }
                ctx.RegisterSyntaxNodeAction(ctx => ReportControllerHints(ctx, files), SyntaxKind.ClassDeclaration);
            });
        }

        public static List<Location> GetLocations(IEnumerable<MethodDeclarationSyntax> methods)
        {
            var locations = new List<Location>();
            foreach (var item in methods)
            {
                locations.Add(item.GetLocation());
            }
            return locations;
        }

        public static async void ReportControllerHints(SyntaxNodeAnalysisContext context, Dictionary<string, AdditionalText> files)
        {
            if (context.Node is ClassDeclarationSyntax node)
            {
                if (node.Identifier.ValueText.EndsWith(_suffix))
                {
                    var serviceName = GetServiceName(node.Identifier.ValueText);
                    var baseName = GetBaseName(node.Identifier.ValueText);
                    if (!files.TryGetValue(baseName, out var file))
                    {
                        return;
                    }
                    var interfaceNode = (await CSharpSyntaxTree.ParseText(file.GetText()).GetRootAsync()).DescendantNodesAndSelf().OfType<InterfaceDeclarationSyntax>().FirstOrDefault(x => x.Identifier.ValueText == serviceName);
                    if (interfaceNode == null) return;
                    var field = node.DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault(x => x.Declaration.Type.ToString() == serviceName);
                    var methods = interfaceNode.DescendantNodes().OfType<MethodDeclarationSyntax>();
                    if (field != null)
                    {
                        var invokedMethodNames = GetMethods(node.DescendantNodes().OfType<MemberAccessExpressionSyntax>(),field.Declaration.Variables.First().Identifier.ValueText);
                        methods = methods.Where(x=>!invokedMethodNames.Contains(x.Identifier.ValueText));
                    }
                    if (methods.Any())
                    {
                        var dic = new Dictionary<string, string>() { ["FieldName"] = field?.Declaration.Variables.First().Identifier.ValueText }.ToImmutableDictionary();
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), GetLocations(methods), dic, node.Identifier.ValueText.ToString()));
                    }
                }
            }
        }

        public static List<string> GetMethods(IEnumerable<MemberAccessExpressionSyntax> invokes, string name)
        {
            var data=new HashSet<string>();
            foreach (var invocation in invokes)
            {
                var token= invocation.Expression.DescendantTokens().FirstOrDefault();
                if (token!=null && token.ValueText==name)
                {
                    data.Add(invocation.Name.Identifier.ValueText);
                }
            }
            return data.ToList();
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

        public static string GetBaseName(string controllerName)
        {
            return controllerName.Substring(0, controllerName.Length - 10);
        }

        public static string GetServiceName(string controllerName)
        {
            return "I" + controllerName.Substring(0, controllerName.Length - 10) + _service;
        }
    }
}