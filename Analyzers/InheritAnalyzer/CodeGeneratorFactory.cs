using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace InheritAnalyzer
{
    public class CodeGeneratorFactory
    {
        public static SourceText GenerateCode(InheritInfo to, ClassInfo from)
        {
            var node = to.ClassNode;
            var lst=to.ClassNode.Ancestors().OfType<CompilationUnitSyntax>().First().Usings;
            var result = CompilationUnit().WithUsings(List(lst));
            var scopeNamespace= node.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
            if (scopeNamespace != null)
            {
                scopeNamespace=scopeNamespace.WithMembers(SingletonList<MemberDeclarationSyntax>(CreateClass(to, from)));
                result = result.WithMembers(SingletonList<MemberDeclarationSyntax>(scopeNamespace)).NormalizeWhitespace();
            }
            else
            {
                var currentNamespace = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                if (currentNamespace != null)
                {
                    currentNamespace = currentNamespace.WithMembers(SingletonList<MemberDeclarationSyntax>(CreateClass(to, from)));
                    result = result.WithMembers(SingletonList<MemberDeclarationSyntax>(currentNamespace)).NormalizeWhitespace();
                }
                else
                {
                    result = result.WithMembers(SingletonList<MemberDeclarationSyntax>(CreateClass(to, from))).NormalizeWhitespace();
                }
            }
            return result.GetText(new UTF8Encoding());
        }
        private static ClassDeclarationSyntax CreateClass(InheritInfo to, ClassInfo from)
        {
            var props = new List<PropertyDeclarationSyntax>();
            foreach (var item in from)
            {
                if (!to.Contains(item.Key))
                {
                    var node = item.Value.PropertyNode;
                    props.Add(PropertyDeclaration(node.Type, node.Identifier).WithAccessorList(node.AccessorList).WithModifiers(node.Modifiers));
                }
            }
            var modifiers = to.ClassNode.Modifiers;
            if (modifiers.All(x => !x.IsKind(SyntaxKind.PartialKeyword)))
            {
                modifiers = modifiers.Add(Token(SyntaxKind.PartialKeyword));
            }
            return ClassDeclaration(to.ClassNode.Identifier).WithModifiers(modifiers).WithMembers(List<MemberDeclarationSyntax>(props))
                .WithTypeParameterList(to.ClassNode.TypeParameterList).WithConstraintClauses(to.ClassNode.ConstraintClauses);
        }
    }
}
