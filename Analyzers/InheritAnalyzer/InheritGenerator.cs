using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServiceAnalyzer.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace InheritAnalyzer
{
    [Generator(LanguageNames.CSharp)]
    public class InheritGenerator : IIncrementalGenerator
    {
        public static readonly string FullName = "TestGenerator.InheritAttribute";
        public static readonly string AssemblyName = "TestGenerator";
        public static readonly string Name = "InheritAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            var additionClass = context.AdditionalTextsProvider.SelectMany((source, token) =>
            {
                var text = source.GetText(token);
                SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
                return "string";
            });

            var allClassName = context.SyntaxProvider.CreateSyntaxProvider((node, _) => node is ClassDeclarationSyntax, (context, _) =>
            {
                var refAssembly = context.SemanticModel.Compilation.ReferencedAssemblyNames.FirstOrDefault(x => x.Name == "TestLib");
                if (context.Node is ClassDeclarationSyntax classNode)
                {
                    var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
                    var result = SearchInfo(symbol);
                    result.InheritorNode = classNode;
                    return result;
                }
                return null;
            }).Collect();
            context.RegisterSourceOutput(allClassName, (context, source) =>
            {

            });
            var inheritorInfo = context.SyntaxProvider.ForAttributeWithMetadataName(FullName, (node, _) => node is ClassDeclarationSyntax, Transform).WithTrackingName("PropName");
            var pairInfo = inheritorInfo.Combine(allClassName);
            context.RegisterSourceOutput(pairInfo, (context, source) =>
            {
                if (source.Left == null) return;
                var inherited = source.Right.FirstOrDefault(x => x.ClassName == source.Left.InheritedClassName);
                if (inherited == null)
                {
                    var location = source.Left.InheritorNode.DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault(x => x.Name.ToString() == "InheritAttribute" || x.Name.ToString() == "Inherit")?.GetLocation();
                    context.ReportDiagnostic(Diagnostic.Create(ClassNotFoundDiagnostic.Rule, location ?? source.Left.InheritorNode.GetLocation()));
                    return;
                }
                var nodes = inherited.InheritorNode.DescendantNodes().OfType<PropertyDeclarationSyntax>();
                var addProperties = new List<PropertyDeclarationSyntax>();
                var recurClass = new List<ITypeSymbol>();
                foreach (var node in nodes)
                {
                    var name = node.Identifier.Text;
                    if (inherited.IgnoreProperties.Contains(name) || source.Left.PropertyNames.Contains(name))
                    {
                        continue;
                    }
                    if (inherited.ComplexProperties.ContainsKey(name))
                    {
                        if (source.Left.Recursion)
                        {
                            addProperties.Add(node);
                            recurClass.Add(inherited.ComplexProperties[name]);
                            continue;
                        }
                    }
                    else
                    {
                        addProperties.Add(node);
                    }
                }
            });
        }

        private static bool PropertyFilter(PropertyDeclarationSyntax prop)
        {
            if (prop.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                return false;
            }
            if (prop.Modifiers.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
            {
                return true;
            }
            return false;
        }

        public static ClassInfo SearchInfo(ISymbol symbol)
        {
            var ignoreProperties = new List<string>();
            var complexProperties = new Dictionary<string, ITypeSymbol>();
            var result = new ClassInfo() { IgnoreProperties = ignoreProperties, ClassName = symbol.Name, ComplexProperties = complexProperties };
            if (symbol is INamedTypeSymbol namedSymbol)
            {
                var props = namedSymbol.GetMembers().OfType<IPropertySymbol>();
                foreach (var prop in props)
                {
                    var attributes = prop.GetAttributes();
                    if (attributes.Any(x => x.AttributeClass.Name == "IgnoreAttribute" && x.AttributeClass.ContainingAssembly.Name == AssemblyName))
                    {
                        ignoreProperties.Add(prop.Name);
                        continue;
                    }
                    var type1 = prop.Type;
                    /*                    if(!prop.Type.ContainingAssembly.Name.StartsWith("System."))
                                        {
                                            var type= prop.Type;
                                            complexProperties[prop.Name]=prop.Type;
                                        }*/
                }
            }
            return result;
        }

        public static InheritorInfo Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            if (context.TargetSymbol is INamedTypeSymbol symbol)
            {
                var attribute = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == Name && x.AttributeClass.ContainingAssembly.Name == AssemblyName);
                if (attribute != null)
                {
                    var len = attribute.ConstructorArguments.Length;
                    var name = attribute.ConstructorArguments[0].Value.ToString();
                    var recur = len <= 1 || (bool)attribute.ConstructorArguments[1].Value;
                    if (context.TargetNode != null && context.TargetNode is ClassDeclarationSyntax node)
                    {
                        var names = new List<string>();
                        var props = node.DescendantNodes().OfType<PropertyDeclarationSyntax>();
                        foreach (var prop in props)
                        {
                            if (!PropertyFilter(prop))
                            {
                                continue;
                            }
                            names.Add(prop.Identifier.Text);
                        }
                        return new InheritorInfo()
                        {
                            InheritedClassName = name,
                            InheritorNode = node,
                            Recursion = recur,
                            PropertyNames = names
                        };
                    }
                }
            }
            return null;
        }
    }
}