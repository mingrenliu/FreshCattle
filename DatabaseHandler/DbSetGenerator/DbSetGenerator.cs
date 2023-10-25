using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace DbSetGenerator;

[Generator(LanguageNames.CSharp)]
public class DbSetGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        /*        if (!Debugger.IsAttached)
                {
                    Debugger.Launch();
                }*/
        var entityNames = context.SyntaxProvider.ForAttributeWithMetadataName("System.ComponentModel.DataAnnotations.Schema.TableAttribute", NotAbstract, EntityName).Collect();
        var dbInfo = context.CompilationProvider.Select((ctx, token) => ctx.GetSymbolsWithName(x => x.EndsWith("DbContext", StringComparison.OrdinalIgnoreCase), SymbolFilter.Type, token)
        .OfType<INamedTypeSymbol>().Where(x => !x.IsAbstract && !x.IsGenericType).Where(x => IsDbContext(x)).Select(x => new ClassInfo() { Name = x.Name, NameSpace = x.ContainingNamespace.ToString() })).SelectMany((x, _) => x.ToImmutableArray());
        var combine = dbInfo.Combine(entityNames);
        context.RegisterSourceOutput(combine, GenerateFile);
    }

    public static void GenerateFile(SourceProductionContext context, (ClassInfo, ImmutableArray<EntityInfo>) source)
    {
        if (!string.IsNullOrEmpty(source.Item1?.Name))
        {
            var file = FileGenerateFactory.Generate(source.Item1, source.Item2);
            context.AddSource(source.Item1.Name + ".gen.cs", file);
        }
    }

    public static bool IsDbContext(INamedTypeSymbol symbol)
    {
        var underType = symbol.BaseType;
        while (underType != null)
        {
            if (underType.Name == "DbContext" && underType.ContainingNamespace.Name == "EntityFrameworkCore")
            {
                return true;
            }
            underType = underType.BaseType;
        }
        return false;
    }

    public static bool NotAbstract(SyntaxNode syntaxNode, CancellationToken token)
    {
        if (syntaxNode is not ClassDeclarationSyntax node)
        {
            return false;
        }
        foreach (var item in node.Modifiers)
        {
            if (item.IsKind(SyntaxKind.AbstractKeyword))
            {
                return false;
            }
        }
        return true;
    }

    public static EntityInfo EntityName(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        var result = new EntityInfo();
        if (context.TargetNode is ClassDeclarationSyntax node)
        {
            result.Name = node.Identifier.ValueText;
            foreach (var item in node.Ancestors())
            {
                if (item is FileScopedNamespaceDeclarationSyntax scopeSpace)
                {
                    result.NameSpace = scopeSpace.Name.ToString();
                    break;
                }
                if (item is NamespaceDeclarationSyntax space)
                {
                    result.NameSpace = space.Name.ToString();
                    break;
                }
            }
            //获取所属的DbContext
            foreach (var item in node.AttributeLists)
            {
                foreach (var attr in item.Attributes)
                {
                    if (attr.Name.ToString() == "DbContext" && attr.ArgumentList.Arguments.Count > 0)
                    {
                        var type = attr.ArgumentList.Arguments.First().ChildNodes().OfType<TypeOfExpressionSyntax>().FirstOrDefault();
                        if (type != null)
                        {
                            result.DbContexts.Add(type.Type.ToString().Split('.').Last());
                        }
                    }
                }
            }
        }
        return result;
    }
}