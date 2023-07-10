﻿using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServiceAnalyzer.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace InheritAnalyzer
{
    [Generator(LanguageNames.CSharp)]
    public class InheritGenerator : IIncrementalGenerator
    {
        public static readonly string Inherit = "Inherit";
        public static readonly string InheritAttribute = "InheritAttribute";
        public static readonly string InheritIgnore = "InheritIgnore";
        public static readonly string AssemblyName = "InheritCore";
        public const string PropertyNameBase = "build_property.";
        public const string RootNameSpace = "rootnamespace";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
/*            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }*/
            var rootNameSpace = context.AnalyzerConfigOptionsProvider.Select((source, token) =>
            {
                if (!source.GlobalOptions.TryGetValue(PropertyNameBase + RootNameSpace, out var spaceName) || string.IsNullOrWhiteSpace(spaceName))
                {
                    spaceName = "DeepInherit.AutoGeneratedCode";
                }
                return spaceName;
            });
            context.RegisterSourceOutput(rootNameSpace, (ctx, token) =>
            {
            });
            var classSource = context.AdditionalTextsProvider.Select((source, token) => source.GetText(token)).SelectMany((source, token) =>
            {
                var root = CSharpSyntaxTree.ParseText(source, cancellationToken: token).GetRootAsync(token).GetAwaiter().GetResult();
                var results = new List<ClassInfo>();
                foreach (var item in root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>())
                {
                    var classInfo = new ClassInfo(item.Identifier.ValueText, item);
                    results.Add(classInfo);
                    foreach (var prop in item.DescendantNodes().OfType<PropertyDeclarationSyntax>().Where(PropertyFilter))
                    {
                        if (PropertyFilterByIgnoreAttribute(prop))
                        {
                            classInfo.Add(prop.Identifier.ValueText, new PropertyInfo(classInfo.ClassName, prop.Identifier.ValueText, prop.Type.ToFullString(), prop));
                        }
                    }
                }
                return results;
            });
            var classSources = classSource.Collect();
            var inheritClassSource = context.SyntaxProvider.CreateSyntaxProvider((node, _) => ClassFilterByAttribute(node), (context, token) =>
            {
                return GetInfo(context.Node, context.SemanticModel);
            });
            var deepSource = inheritClassSource.Where(x => x.IsDeepInherit).Collect().Combine(classSources);
            var generatedNames = deepSource.Select((source, token) =>
            {
                return GetDeepInheritClassNames(source.Left, source.Right);
            });
            var deepGeneratedClass = classSource.Combine(generatedNames).Where(x => x.Right.Any(y => y.ClassName == x.Left.ClassName && y.TypeParameterCount == x.Left.TypeParameterCount))
                .Select((source, token) => source.Left).Combine(rootNameSpace);

            context.RegisterSourceOutput(deepGeneratedClass, (ctx, source) =>
            {
                ctx.AddSource(source.Left.ClassName + ".gen.cs", CodeGeneratorFactory.GenerateCode(source.Left, source.Right));
            });
            var combineSource = inheritClassSource.Combine(classSources);
            context.RegisterSourceOutput(combineSource, (ctx, source) =>
            {
                var currentClass = source.Left;
                var classSources = source.Right;
                var inheritedClass = classSources.FirstOrDefault(x => x.ClassName == currentClass.InheritedClassName);
                if (inheritedClass != null)
                {
                    ctx.AddSource(currentClass.CurrentClassName + ".gen.cs", CodeGeneratorFactory.GenerateCode(currentClass, inheritedClass));
                }
                else
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(ClassNotFoundDiagnostic.Rule, currentClass.AttributeLocation, new object[] { currentClass.InheritedClassName }));
                }
            });
        }

        public static void DeepSearchClass(Dictionary<(string, int), (bool, ClassInfo)> data, ClassInfo info)
        {
            foreach (var prop in info.Values)
            {
                DeepSearchProperty(prop.PropertyNode.Type, data);
            }
        }

        public static void DeepSearchProperty(TypeSyntax type, Dictionary<(string, int), (bool, ClassInfo)> data)
        {
            if (type.IsKind(SyntaxKind.PredefinedType))
            {
                return;
            }
            if (type is IdentifierNameSyntax identifierNode)
            {
                TryAdd(data, identifierNode.Identifier.ValueText.ToString(), 0);
            }
            else if (type is GenericNameSyntax genericNode)
            {
                TryAdd(data, genericNode.Identifier.ValueText.ToString(), genericNode.TypeArgumentList.Arguments.Count);
                foreach (var item in genericNode.TypeArgumentList.Arguments)
                {
                    DeepSearchProperty(item, data);
                }
            }
            else if (type is NullableTypeSyntax nullableType)
            {
                DeepSearchProperty(nullableType.ElementType, data);
            }
            else if (type is QualifiedNameSyntax qualifiedNode)
            {
                DeepSearchProperty(qualifiedNode.Right, data);
            }
            else if (type is ArrayTypeSyntax arrayNode)
            {
                DeepSearchProperty(arrayNode.ElementType, data);
            }
            else if (type is AliasQualifiedNameSyntax aliasNode)
            {
                DeepSearchProperty(aliasNode.Name, data);
            }
            else if (type is TupleTypeSyntax tupleNode)
            {
                foreach (var subItem in tupleNode.Elements)
                {
                    DeepSearchProperty(subItem.Type, data);
                }
            }
        }

        public static IEnumerable<DeepClassInfo> GetDeepInheritClassNames(ImmutableArray<InheritInfo> info, ImmutableArray<ClassInfo> classSources)
        {
            var generatedClass = classSources.Distinct(new ClassComparer()).ToDictionary(x => (x.ClassName, x.TypeParameterCount), x => (false, x));
            foreach (var item in info)
            {
                var obj = classSources.FirstOrDefault(x => x.ClassName == item.InheritedClassName);
                if (obj != null)
                {
                    foreach (var prop in obj)
                    {
                        if (!item.Contains(prop.Key))
                        {
                            var node = prop.Value.PropertyNode;
                            DeepSearchProperty(prop.Value.PropertyNode.Type, generatedClass);
                        }
                    }
                }
            }
            foreach (var item in generatedClass)
            {
                if (item.Value.Item1)
                {
                    yield return new DeepClassInfo(item.Key.ClassName, item.Key.TypeParameterCount);
                }
            }
        }

        public static void TryAdd(Dictionary<(string, int), (bool, ClassInfo)> data, string name, int count = 0)
        {
            if (data.TryGetValue((name, count), out var value) && !value.Item1)
            {
                data[(name, count)] = (true, value.Item2);
                DeepSearchClass(data, value.Item2);
            }
        }

        public static bool ClassFilterByAttribute(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classNode)
            {
                foreach (var attributeList in classNode.AttributeLists)
                {
                    foreach (var item in attributeList.Attributes)
                    {
                        var name = item.Name.ToString();
                        if (name == Inherit)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool PropertyFilterByIgnoreAttribute(PropertyDeclarationSyntax node)
        {
            foreach (var attributeList in node.AttributeLists)
            {
                foreach (var item in attributeList.Attributes)
                {
                    if (item.Name.ToString() == InheritIgnore)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static InheritInfo GetInfo(SyntaxNode node, SemanticModel semantic)
        {
            if (node is ClassDeclarationSyntax classNode)
            {
                var symbol = semantic.GetDeclaredSymbol(classNode);
                AttributeData attribute = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == InheritAttribute && x.AttributeClass.ContainingAssembly.Name == AssemblyName);
                if (attribute != null)
                {
                    var arguments = attribute.ConstructorArguments;
                    var name = (string)arguments.First().Value;
                    bool isDeep = arguments.Length > 1 && (bool)arguments[1].Value;
                    var attributeLocation = classNode.AttributeLists.SelectMany(x => x.Attributes).First(x => x.Name.ToString() == Inherit).GetLocation();
                    var result = new InheritInfo(classNode.Identifier.ValueText, name, classNode, attributeLocation, isDeep);
                    foreach (var prop in classNode.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                    {
                        result.Add(prop.Identifier.ValueText);
                    }
                    return result;
                }
            }
            return null;
        }

        public static bool PropertyFilter(PropertyDeclarationSyntax prop)
        {
            if (prop.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                return false;
            }
            if (prop.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword) || x.IsKind(SyntaxKind.InternalKeyword)))
            {
                return true;
            }
            return false;
        }
    }
}