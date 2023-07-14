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
    public static class CodeGeneratorFactory
    {
        public static SourceText GenerateCode(ClassInfo baseClass,string rootNameSpace)
        {
            var usingList = baseClass.ClassNode.Ancestors().OfType<CompilationUnitSyntax>().First().Usings;
            var props = new List<PropertyDeclarationSyntax>();
            foreach (var node in baseClass.ClassNode.DescendantNodes().OfType<PropertyDeclarationSyntax>())
            {
                props.Add(CreateProperty(node));
            }
            var newClass = ClassDeclaration(baseClass.ClassNode.Identifier).WithModifiers(baseClass.ClassNode.Modifiers).WithMembers(List<MemberDeclarationSyntax>(props))
                .WithTypeParameterList(baseClass.ClassNode.TypeParameterList).WithConstraintClauses(baseClass.ClassNode.ConstraintClauses).WithXml(baseClass.ClassNode).NormalizeWhitespace();
            var result = CompilationUnit().WithUsings(List(usingList)).WithMembers(
                SingletonList<MemberDeclarationSyntax>(FileScopedNamespaceDeclaration(CreateNameSpace(rootNameSpace))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(newClass)))).NormalizeWhitespace();
            return result.GetText(new UTF8Encoding());
        }
        public static NameSyntax CreateNameSpace(string rootNameSpace)
        {
            var lst= rootNameSpace.Split(new char[] { '.' },StringSplitOptions.RemoveEmptyEntries);
            if (lst.Length>1)
            {
                var current = QualifiedName(IdentifierName(lst[0]), IdentifierName(lst[1]));
                for (int i = 2; i < lst.Length; i++)
                {
                    current=QualifiedName(current, IdentifierName(lst[i]));
                }
                return current.NormalizeWhitespace();
            }
            else
            {
                return IdentifierName(lst[0]);
            }
        }
        public static SourceText GenerateCode(InheritInfo to, ClassInfo from)
        {
            var node = to.ClassNode;
            var usingList = to.ClassNode.Ancestors().OfType<CompilationUnitSyntax>().First().Usings;
            var result = CompilationUnit().WithUsings(List(usingList));
            var scopeNamespace = node.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
            if (scopeNamespace != null)
            {
                scopeNamespace = scopeNamespace.WithMembers(SingletonList<MemberDeclarationSyntax>(CreateClass(to, from)));
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

        private static PropertyDeclarationSyntax CreateProperty(PropertyDeclarationSyntax node)
        {
            return node.Initializer != null ? PropertyDeclaration(node.Type, node.Identifier).WithAccessorList(node.AccessorList)
                    .WithModifiers(TokenList(node.Modifiers)).WithInitializer(node.Initializer).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)).WithXml(node).NormalizeWhitespace()
                    : PropertyDeclaration(node.Type, node.Identifier).WithAccessorList(node.AccessorList)
                    .WithModifiers(TokenList(node.Modifiers)).WithXml(node).NormalizeWhitespace();
        }
        private static PropertyDeclarationSyntax WithXml(this PropertyDeclarationSyntax currentNode, PropertyDeclarationSyntax node)
        {
            var trivia = node.DescendantTrivia().FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
            return currentNode.WithLeadingTrivia(TriviaList(trivia));
        }
        private static ClassDeclarationSyntax WithXml(this ClassDeclarationSyntax currentNode, ClassDeclarationSyntax node)
        {
            var trivia = node.DescendantTrivia().FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
            return currentNode.WithLeadingTrivia(TriviaList(trivia));
        }
        private static ClassDeclarationSyntax CreateClass(InheritInfo to, ClassInfo from)
        {
            var props = new List<PropertyDeclarationSyntax>();
            foreach (var item in from)
            {
                if (!to.Contains(item.Key))
                {
                    props.Add(CreateProperty(item.Value.PropertyNode));
                }
            }
            var modifiers = to.ClassNode.Modifiers;
            if (modifiers.All(x => !x.IsKind(SyntaxKind.PartialKeyword)))
            {
                modifiers = modifiers.Add(Token(SyntaxKind.PartialKeyword));
            }
            return ClassDeclaration(to.ClassNode.Identifier).WithModifiers(modifiers).WithMembers(List<MemberDeclarationSyntax>(props))
                .WithTypeParameterList(to.ClassNode.TypeParameterList).WithConstraintClauses(to.ClassNode.ConstraintClauses).WithXml(from.ClassNode);
        }
    }
}