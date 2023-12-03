using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RequiredPropertyAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RequiredPropertyCodeFixProvider)), Shared]
    public class RequiredPropertyCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RequiredPropertyAnalyzer.DiagnosticId, RequiredPropertyAnalyzer.RemoveDiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (!context.Document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.RequiredAttributeName",out var attributeName)|| string.IsNullOrWhiteSpace(attributeName))
            {
                attributeName = "Required";
            }
            var diagnostic = context.Diagnostics.Last();
            var syntaxToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            var target = syntaxToken.Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (target == null || !diagnostic.AdditionalLocations.Any()) return;
            if (diagnostic.Descriptor.Id==RequiredPropertyAnalyzer.Rule.Id)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: AddReuiredCodeFixResources.CodeFixTitle,
                        createChangedDocument: c => AddAttributesAsync(root, context.Document,target,attributeName, diagnostic.AdditionalLocations, c),
                        equivalenceKey: AddReuiredCodeFixResources.CodeFixTitle),
                    diagnostic);
            }
            else if(diagnostic.Descriptor.Id == RequiredPropertyAnalyzer.RemoveRule.Id)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: RemoveReuiredCodeFixResource.CodeFixTitle,
                        createChangedDocument: c => RemoveAttributesAsync(root,context.Document, target,attributeName, diagnostic.AdditionalLocations, c),
                        equivalenceKey: RemoveReuiredCodeFixResource.CodeFixTitle),
                    diagnostic);
            }

        }
        public static async Task<Document> AddAttributesAsync(SyntaxNode root, Document doc, ClassDeclarationSyntax target,string requiredName, IReadOnlyList<Location> locations, CancellationToken token)
        {
            var old = target;
            var nodes = new List<PropertyDeclarationSyntax>();
            foreach (var location in locations)
            {
                var node = root.FindToken(location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
                if (node == null) continue;
                nodes.Add(node);
            }
            if (nodes.Any())
            {
                target = target.ReplaceNodes(nodes, (oldNode, newNode) => oldNode.AddAttributeLists(CreateRequiredAttribute(requiredName)));
            }
            return doc.WithSyntaxRoot(root.ReplaceNode(old, target).NormalizeWhitespace());
        }
        public static async Task<Document> RemoveAttributesAsync(SyntaxNode root,Document doc, ClassDeclarationSyntax target, string requiredName, IReadOnlyList<Location> locations, CancellationToken token)
        {
            var old = target;
            var nodes = new List<PropertyDeclarationSyntax>();
            foreach (var location in locations)
            {
                var node = root.FindToken(location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
                if (node == null) continue;
                nodes.Add(node);
            }
            if (nodes.Any())
            {
                var required = requiredName== "Required"?new string[] { requiredName} : new string[] { requiredName, "Required" };
                target = target.ReplaceNodes(nodes, (oldNode, newNode) => oldNode.WithAttributeLists(RemoveAttribute(oldNode.AttributeLists, required)));
            }
            return doc.WithSyntaxRoot(root.ReplaceNode(old, target).NormalizeWhitespace());
        }
        public static SyntaxList<AttributeListSyntax> RemoveAttribute(SyntaxList<AttributeListSyntax> attributeLists, IEnumerable<string> attributeName)
        {
            return List(attributeLists.Where(x => x.Attributes.All(y => !attributeName.Contains(y.Name.ToString()))));
        }
        public static AttributeListSyntax CreateRequiredAttribute(string name)
        {   
            if (string.Equals( name, "Required",StringComparison.OrdinalIgnoreCase))
            {
                return CreateAttributeWithNoPara(name);
            }
            else
            {
                return CreateAttribute(name, WithArguments(""));
            }
        }
        public static AttributeListSyntax CreateAttributeWithNoPara(string name)
        {
            return   AttributeList(
                               SingletonSeparatedList(
                                                      Attribute(
                                                                IdentifierName(name),null)));
        }
        private static AttributeArgumentListSyntax WithArguments(string value)
        {
            return AttributeArgumentList(
                                         SingletonSeparatedList(
                                                                AttributeArgument(
                                                                                  LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)))));
        }
        private static AttributeListSyntax CreateAttribute(string name, AttributeArgumentListSyntax args)
        {
            return AttributeList(
                               SingletonSeparatedList(
                                                      Attribute(
                                                                IdentifierName(name))
                                                      .WithArgumentList(args)));
        }
    }
}
