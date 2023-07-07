using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace InheritAnalyzer
{
    [Generator(LanguageNames.CSharp)]
    public class InheritGenerator : IIncrementalGenerator
    {
        public static readonly string ShallowInherit = "ShallowInherit";
        public static readonly string Inherit = "Inherit";
        public static readonly string DeepInherit = "DeepInherit";
        public static readonly string InheritIgnore = "InheritIgnore";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            var classSource = context.AdditionalTextsProvider.Select((source, token) => source.GetText(token)).SelectMany((source, token) =>
            {
                var root = CSharpSyntaxTree.ParseText(source, cancellationToken: token).GetRootAsync(token).GetAwaiter().GetResult();
                var results = new List<ClassInfo>();
                foreach (var item in root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>())
                {
                    var classInfo = new ClassInfo(item.Identifier.ValueText);
                    results.Add(classInfo);
                    foreach (var prop in item.DescendantNodes().OfType<PropertyDeclarationSyntax>().Where(PropertyFilter))
                    {
                        if (IsBasicType(prop.Type))
                        {
                            classInfo.Add(prop.Identifier.ValueText, new PropertyInfo(classInfo.ClassName, prop.Identifier.ValueText, prop.Type.ToFullString(), prop));
                        }
                    }
                }
                return results;
            });
            var classSources = classSource.Collect();
            var inheritClassSource = context.SyntaxProvider.CreateSyntaxProvider((node, _) => ClassWithAttributeFilter(node), (context, token) =>
            {
                return GetInfo(context.Node);
            });
            var combineSource=inheritClassSource.Combine(classSources);
            context.RegisterSourceOutput(combineSource, (ctx, source) =>
            {
                var currentClass = source.Left;
                var classSources=source.Right;
                if (classSources.Any(x=>x.ClassName== currentClass.InheritedClassName))
                {
                    ctx.AddSource(currentClass.CurrentClassName + ".gen.cs",);
                }
                ctx.ReportDiagnostic();
            });
        }

        public static bool ClassWithAttributeFilter(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classNode)
            {
                foreach (var attributeList in classNode.AttributeLists)
                {
                    foreach (var item in attributeList.Attributes)
                    {
                        if (item.Name.ToFullString() == ShallowInherit)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static InheritInfo GetInfo(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classNode)
            {
                foreach (var attributeList in classNode.AttributeLists)
                {
                    foreach (var item in attributeList.Attributes)
                    {
                        if (item.Name.ToFullString() == ShallowInherit)
                        {
                            var argument = item.ArgumentList.Arguments.FirstOrDefault();
                            if (argument!=null)
                            {
                                var result= new InheritInfo(classNode.Identifier.ValueText, argument.ToFullString(),classNode);
                                foreach (var prop in classNode.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                                {
                                    result.Add(prop.Identifier.ValueText);
                                }
                                return result;
                            }
                        }
                    }
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

        private static bool IsBasicType(TypeSyntax type)
        {
            if (type == null) return false;
            if (type.IsKind(SyntaxKind.PredefinedType))
            {
                return true;
            }
            if (type.IsKind(SyntaxKind.GenericName))
            {
                return false;
            }
            if (type is NullableTypeSyntax nullableType)
            {
                return IsBasicType(nullableType.ElementType);
            }
            return false;
        }
    }
}