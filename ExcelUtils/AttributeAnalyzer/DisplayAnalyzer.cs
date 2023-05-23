using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AttributeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisplayAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DisplayDiagnostic.Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(CheckPropertyDisplayName, SymbolKind.NamedType);
        }
        private static void CheckPropertyDisplayName(SymbolAnalysisContext context)
        {
            var classSymbol = (INamedTypeSymbol)context.Symbol;
            var displayNameDic = new Dictionary<string, (string, AttributeData)>();
            var dicReport = new Dictionary<string, Diagnostic>();//key property name
            foreach (var item in classSymbol.GetMembers())
            {
                if (item is IPropertySymbol prop)
                {
                    var attribute = prop.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == "DisplayAttribute" && x.AttributeClass.ContainingAssembly.Name == "ExcelUtile");
                    if (attribute != null)
                    {
                        string displayName = (string)attribute.ConstructorArguments.First().Value;
                        if (displayNameDic.TryGetValue(displayName, out var preValue))
                        {
                            //current
                            var attributeNode = attribute.ApplicationSyntaxReference.GetSyntax();
                            var diagnostic = Diagnostic.Create(DisplayDiagnostic.Rule, attributeNode.GetLocation(), prop.Name, displayName);
                            dicReport.Add(prop.Name, diagnostic);
                            // pre
                            if (!dicReport.ContainsKey(preValue.Item1))
                            {
                                var preAttributeNode = preValue.Item2.ApplicationSyntaxReference.GetSyntax();
                                var preDiagnostic = Diagnostic.Create(DisplayDiagnostic.Rule, preAttributeNode.GetLocation(), preValue.Item1, displayName);
                                dicReport.Add(preValue.Item1, preDiagnostic);
                            }
                        }
                        else
                        {
                            displayNameDic[displayName] = (prop.Name, attribute);
                        }
                    }
                }
            }
            foreach (var report in dicReport.Values)
            {
                context.ReportDiagnostic(report);
            }
        }
    }
}