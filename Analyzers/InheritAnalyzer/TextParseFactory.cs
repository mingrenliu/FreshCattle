using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InheritAnalyzer
{
    public static class TextParseFactory
    {
        public static readonly string Inherit = "Inherit";
        public static readonly string InheritAttribute = "InheritAttribute";
        public static readonly string InheritIgnore = "InheritIgnore";
        public static readonly string AssemblyName = "InheritCore";
        public const string PropertyNameBase = "build_property.";
        public const string RootNameSpace = "RootNamespace";
        public const string ProjectDir = "ProjectDir";
        public const string AdditionalFilePath = "AdditionalFilePath";
        public static async Task<IEnumerable<ClassInfo>> ParseText(AdditionalText text)
        {
            if (text?.GetText() == null) return Enumerable.Empty<ClassInfo>();
            var root=await CSharpSyntaxTree.ParseText(text.GetText()).GetRootAsync();
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
    }
}