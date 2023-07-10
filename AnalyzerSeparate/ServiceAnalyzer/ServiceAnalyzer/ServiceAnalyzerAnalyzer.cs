﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace ServiceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string _suffix = "Service";
        public const string DiagnosticId = "ServiceAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "ServiceHint";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(ReportServiceHints, SymbolKind.NamedType);
        }

        public static void ReportServiceHints(SymbolAnalysisContext context)
        {
            if (context.Symbol is INamedTypeSymbol symbol)
            {
                if (symbol.Name.EndsWith(_suffix))
                {
                    var implement = symbol.AllInterfaces.FirstOrDefault(x => x.Name.Equals("I" + symbol.Name));
                    if (implement != null)
                    {
                        var methods = FindMethodsInClass(symbol);
                        var implementedMethods = FindImplementedMethods(symbol, FindMethodsInInterface(implement));
                        var locations = GetLocations(methods.Except(implementedMethods));
                        if (locations.Any())
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, implement.Locations.First(), locations, new object[] { symbol.Name }));
                        }
                    }
                }
            }
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
