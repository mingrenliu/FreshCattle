using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EFEntityAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CommentCodeFixProvider)), Shared]
    public class CommentCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CommentAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.Last();
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = syntaxToken.Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (target == null || !diagnostic.AdditionalLocations.Any()) return;
            var properties = diagnostic.Properties;
            var haveClass = diagnostic.Location == diagnostic.AdditionalLocations.First();
            context.RegisterCodeFix(CodeAction.Create(CommentCodeFixResources.CodeFixTitle,
                createChangedDocument: token => GenerateCommentAsync(context.Document, properties, target, haveClass, diagnostic.AdditionalLocations, token),
                equivalenceKey: CommentCodeFixResources.CodeFixTitle),
                context.Diagnostics.Last());
        }

        public static async Task<Document> GenerateCommentAsync(Document doc, ImmutableDictionary<string, string> properties, ClassDeclarationSyntax target, bool containClass, IReadOnlyList<Location> locations, CancellationToken token)
        {
            var old = target;
            var props = locations.Skip(containClass ? 1 : 0);
            var root = await doc.GetSyntaxRootAsync(token);
            if (props.Any())
            {
                var list = new List<PropertyDeclarationSyntax>();
                foreach (var location in props)
                {
                    var node = root.FindToken(location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
                    if (node == null) continue;
                    if (properties.TryGetValue(CommentAnalyzer.PropPrefix + node.Identifier.ValueText, out var propComment))
                    {
                        list.Add(node);
                    }
                }
                if (list.Any())
                {
                    target = target.ReplaceNodes(list, (oldNode, newNode) => Replace(oldNode, newNode, properties));
                }
            }
            if (containClass && properties.TryGetValue(CommentAnalyzer.ClassPrefix + target.Identifier.ValueText, out var classComment))
            {
                target = ReplaceComment(target, classComment);
            }
            return doc.WithSyntaxRoot(root.ReplaceNode(old, target).NormalizeWhitespace());
        }

        private static PropertyDeclarationSyntax Replace(PropertyDeclarationSyntax oldNode, PropertyDeclarationSyntax newNode, ImmutableDictionary<string, string> properties)
        {
            var propComment = properties[CommentAnalyzer.PropPrefix + oldNode.Identifier.ValueText];
            return ReplaceComment(newNode, propComment);
        }

        private static T ReplaceComment<T>(T node, string value) where T : SyntaxNode
        {
            if (SyntaxParseFactory.TryGetCommentSyntax(node, out var xmlDocument))
            {
                return node.ReplaceNode(xmlDocument, CreateXmlDocument(value));
            }
            else
            {
                var trivia = node.GetLeadingTrivia();
                return node.WithLeadingTrivia(trivia.Add(Trivia(CreateXmlDocument(value))));
            }
        }

        private static DocumentationCommentTriviaSyntax CreateXmlDocument(string value)
        {
            return DocumentationCommentTrivia(
                                        SyntaxKind.SingleLineDocumentationCommentTrivia,
                                        List(
                                            new XmlNodeSyntax[]{
                                                XmlText()
                                                .WithTextTokens(
                                                    TokenList(
                                                        XmlTextLiteral(
                                                            TriviaList(
                                                                DocumentationCommentExterior("///")),
                                                            " ",
                                                            " ",
                                                            TriviaList()))),
                                                XmlExampleElement(
                                                    SingletonList<XmlNodeSyntax>(
                                                        XmlText()
                                                        .WithTextTokens(
                                                            TokenList(
                                                                new []{
                                                                    XmlTextNewLine(
                                                                        TriviaList(),
                                                                        Environment.NewLine,
                                                                        Environment.NewLine,
                                                                        TriviaList()),
                                                                    XmlTextLiteral(
                                                                        TriviaList(
                                                                            DocumentationCommentExterior("    ///")),
                                                                        " "+value,
                                                                        " "+value,
                                                                        TriviaList()),
                                                                    XmlTextNewLine(
                                                                        TriviaList(),
                                                                        Environment.NewLine,
                                                                        Environment.NewLine,
                                                                        TriviaList()),
                                                                    XmlTextLiteral(
                                                                        TriviaList(
                                                                            DocumentationCommentExterior("    ///")),
                                                                        " ",
                                                                        " ",
                                                                        TriviaList())}))))
                                                .WithStartTag(
                                                    XmlElementStartTag(
                                                        XmlName(
                                                            Identifier("summary"))))
                                                .WithEndTag(
                                                    XmlElementEndTag(
                                                        XmlName(
                                                            Identifier("summary")))),
                                                XmlText()
                                                .WithTextTokens(
                                                    TokenList(
                                                        XmlTextNewLine(
                                                            TriviaList(),
                                                            Environment.NewLine,
                                                            Environment.NewLine,
                                                            TriviaList())))}));
        }
    }
}