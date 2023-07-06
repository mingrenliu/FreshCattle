using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ServiceAnalyzer.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ServiceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceDiagnostic : DiagnosticAnalyzer
    {
        private static readonly string _suffix = "Service";
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ServiceHintDiagnostic.Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(ReportHints, SymbolKind.NamedType);
        }

        public static void ReportHints(SymbolAnalysisContext context)
        {
            var option = context.Options;
            if (context.Symbol is INamedTypeSymbol symbol)
            {
/*                if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("", out var suffix) || string.IsNullOrWhiteSpace(suffix))
                {
                    suffix = _suffix;
                }*/
                if (symbol.Name.EndsWith(_suffix))
                {
                    var implement = symbol.AllInterfaces.FirstOrDefault(x => x.Name.Equals("I" + symbol.Name));
                    if (implement != null)
                    {
                        var methods = FindMethodsInClass(symbol);
                        var implementedMethods =FindImplementedMethods(symbol,FindMethodsInInterface(implement));
                        var locations = GetLocations(methods.Except(implementedMethods));
                        if (locations.Any())
                        {
                            context.ReportDiagnostic(Diagnostic.Create(ServiceHintDiagnostic.Rule, implement.Locations.First(), locations, new object[] { symbol.Name }));
                        }
                    }
                }
            }
        }

        public static IEnumerable<Location> GetMethodNotInInterface(INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceSymbol)
        {
            foreach (var method in classSymbol.GetMembers().OfType<IMethodSymbol>().Except(classSymbol.Constructors))
            {
                if (method.IsAbstract) continue;
                if (method.DeclaredAccessibility == Accessibility.Public || method.DeclaredAccessibility == Accessibility.Internal)
                {
                    yield return method.Locations.First();
                }
            }
        }

        public static IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol symbol)
        {
            var methods = symbol.GetMembers().OfType<IMethodSymbol>();
            return methods.Where(x => !x.IsAbstract && (x.DeclaredAccessibility == Accessibility.Public || x.DeclaredAccessibility == Accessibility.Internal)).Except(symbol.Constructors);
        }

        public static IEnumerable<IMethodSymbol> FindMethodsInInterface(INamedTypeSymbol symbol)
        {
            return symbol.GetMembers().OfType<IMethodSymbol>();
        }

        public static IEnumerable<IMethodSymbol> FindImplementedMethods(INamedTypeSymbol symbol, IEnumerable<IMethodSymbol> methods)
        {
            foreach (var item in methods)
            {
                var method = symbol.FindImplementationForInterfaceMember(item);
                if (method is IMethodSymbol result)
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<Location> GetLocations(IEnumerable<IMethodSymbol> methods)
        {
            foreach (var item in methods)
            {
                yield return item.Locations.First();
            }
        }

        public static IEnumerable<IMethodSymbol> FindMethodsInClass(INamedTypeSymbol symbol)
        {
            var methods = symbol.GetMembers().OfType<IMethodSymbol>().Except(symbol.Constructors);
            foreach (var method in methods)
            {
                if (method.IsStatic || method.IsGenericMethod)
                {
                    continue;
                }
                if (method.DeclaredAccessibility == Accessibility.Internal || method.DeclaredAccessibility == Accessibility.Public)
                {
                    yield return method;
                }
            }
        }
    }
}