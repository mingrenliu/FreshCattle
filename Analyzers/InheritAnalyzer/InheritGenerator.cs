﻿using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServiceAnalyzer.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using static InheritAnalyzer.TextParseFactory;

namespace InheritAnalyzer
{
    [Generator(LanguageNames.CSharp)]
    public class InheritGenerator : IIncrementalGenerator
    {
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
            var classSources = context.AnalyzerConfigOptionsProvider.Select((source, token) =>
            {
                source.GlobalOptions.TryGetValue(PropertyNameBase + ProjectDir, out var projectDir);
                source.GlobalOptions.TryGetValue(PropertyNameBase + AdditionalFilePath, out var additionalFilePath);
                if (string.IsNullOrEmpty(additionalFilePath))
                {
                    return null;
                }
                return ClassInfoProvider.Create(Path.Combine(additionalFilePath, projectDir));
            });
            //深度继承
            var inheritClassSource = context.SyntaxProvider.ForAttributeWithMetadataName(AssemblyName + InheritAttribute, (node,_) => node is ClassDeclarationSyntax, (context, token) =>
            {
                return GetInfo(context.TargetNode, context.SemanticModel);
            });
            var deepSource = inheritClassSource.Where(x => x.IsDeepInherit).Collect().Combine(classSources);
            var generatedNames = deepSource.Select((source, token) =>
            {
                return GetDeepInheritClassNames(source.Left, source.Right);
            });
            var deepGeneratedClass = classSources.Combine(generatedNames).SelectMany((x, token) => GetClassInfo(x.Right, x.Left)).Combine(rootNameSpace);
            context.RegisterSourceOutput(deepGeneratedClass, (ctx, source) =>
            {
                ctx.AddSource(source.Left.ClassName + ".gen.cs", CodeGeneratorFactory.GenerateCode(source.Left, source.Right));
            });
            //继承
            var combineSource = inheritClassSource.Combine(classSources);
            context.RegisterSourceOutput(combineSource, (ctx, source) =>
            {
                var currentClass = source.Left;
                var classSources = source.Right;
                if (classSources != null && classSources.TryGetClassInfo(new ClassSimpleInfo(currentClass.InheritedClassName, 0), out var info))
                {
                    ctx.AddSource(currentClass.CurrentClassName + ".gen.cs", CodeGeneratorFactory.GenerateCode(currentClass, info));
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

        public static IEnumerable<ClassSimpleInfo> GetDeepInheritClassNames(ImmutableArray<InheritInfo> info, ClassInfoProvider classSources)
        {
            var generatedClass = classSources.Values.ToDictionary(x => (x.ClassName, x.TypeParameterCount), x => (false, x));
            foreach (var item in info)
            {
                if (classSources.TryGetClassInfo(new ClassSimpleInfo(item.InheritedClassName, 0), out var obj))
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
                    yield return new ClassSimpleInfo(item.Key.ClassName, item.Key.TypeParameterCount);
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

        public static IEnumerable<ClassInfo> GetClassInfo(IEnumerable<ClassSimpleInfo> info, ClassInfoProvider provider)
        {
            foreach (var generatedName in info)
            {
                if (provider.TryGetClassInfo(generatedName, out var value))
                {
                    yield return value;
                }
            }
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
    }
}