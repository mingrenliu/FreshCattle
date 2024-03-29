﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ServiceAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string _suffix = "Service";
        public const string DiagnosticId = "LY0011";

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

            context.RegisterSyntaxNodeAction(ServiceHints, SyntaxKind.InterfaceDeclaration);
        }

        public static void ServiceHints(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InterfaceDeclarationSyntax node && node.Identifier.ValueText.EndsWith(_suffix))
            {
                var interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(node);
                var symbol = context.Compilation.GetSymbolsWithName(x => x == node.Identifier.ValueText.Substring(1), SymbolFilter.Type).FirstOrDefault();
                if (symbol != null && symbol is INamedTypeSymbol classSymbol && classSymbol.AllInterfaces.Contains(interfaceSymbol))
                {
                    var methods = FindMethodsInClass(classSymbol);
                    var implementedMethods = FindImplementedMethods(classSymbol, FindMethodsInInterface(interfaceSymbol));
                    var locations = GetLocations(methods.Except(implementedMethods));
                    if (locations.Any())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), locations, new object[] { classSymbol.Name }));
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
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.IsStatic || method.IsGenericMethod)
                {
                    continue;
                }
                if ((method.DeclaredAccessibility == Accessibility.Internal || method.DeclaredAccessibility == Accessibility.Public) && method.MethodKind == MethodKind.Ordinary)
                {
                    yield return method;
                }
            }
        }
    }
}