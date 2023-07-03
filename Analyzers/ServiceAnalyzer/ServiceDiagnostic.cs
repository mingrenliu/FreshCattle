using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using ServiceAnalyzer.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace ServiceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceDiagnostic : DiagnosticAnalyzer
    {
        private readonly static string _suffix = "Service";
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ServiceHintDiagnostic.Rule);
        public override void Initialize(AnalysisContext context)
        {
/*            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }*/
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(ReportHints, SymbolKind.NamedType);
        }
        private static void ReportHints(SymbolAnalysisContext context)
        {
            var option = context.Options;
            if (context.Symbol is INamedTypeSymbol symbol)
            {
                if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("", out var suffix)|| string.IsNullOrWhiteSpace(suffix))
                {
                    suffix =_suffix;
                }
                if (symbol.Name.EndsWith(suffix))
                {
                    var implement = symbol.AllInterfaces.FirstOrDefault(x=>x.Name.Equals("I"+symbol.Name));
                    if (implement!=null)
                    {
                        var methods = GetMethods(symbol);
                        var ImpMethods=GetMethods(implement);
                        foreach (var item in methods)
                        {
                            if (item.ExplicitInterfaceImplementations.Any())
                            {
                                var haveContained=item.ExplicitInterfaceImplementations.Any(x => x.ContainingType.Equals(implement, SymbolEqualityComparer.Default));
                                if (haveContained)
                                {
                                    continue;
                                }    
                            }
                            context.ReportDiagnostic(Diagnostic.Create(ServiceHintDiagnostic.Rule, implement.Locations.First(),symbol.Locations,new object[] { symbol.Name }));
                            return;
                        }
                    }
                }
            }
        }
        private static IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol symbol)
        {
            return symbol.GetMembers().OfType<IMethodSymbol>().Where(x => !x.IsAbstract && x.DeclaredAccessibility == Accessibility.Public || x.DeclaredAccessibility == Accessibility.Internal);
        }
    }
}