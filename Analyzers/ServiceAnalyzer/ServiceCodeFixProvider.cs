using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServiceAnalyzer.Diagnostics;
using ServiceAnalyzer.Diagnotics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ServiceAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ServiceCodeFixProvider)), Shared]
    public class ServiceCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ServiceHintDiagnostic.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = (InterfaceDeclarationSyntax)syntaxToken.Parent;
            context.RegisterCodeFix(CodeAction.Create(ServiceCodeFixResource.CodeFixTitle,
                createChangedDocument: token => GenerateMethodsAsync(context.Document, target, diagnostic.AdditionalLocations, token),
                equivalenceKey: ServiceCodeFixResource.CodeFixTitle),
                context.Diagnostics.First());
        }

        public static async Task<Document> GenerateMethodsAsync(Document doc, InterfaceDeclarationSyntax target, IEnumerable<Location> locations, CancellationToken token)
        {
            var space=target.OpenBraceToken.LeadingTrivia.FirstOrDefault(x=>x.IsKind(SyntaxKind.WhitespaceTrivia));
            var leadingSpace= space==null ? TriviaList(Whitespace("    ")) :TriviaList(space, Whitespace("    "));
            var methods = target.Members;
            foreach (var location in locations)
            {
                var node = (MethodDeclarationSyntax)(await location.SourceTree.GetRootAsync()).FindToken(location.SourceSpan.Start).Parent;
                methods=methods.Add(MethodDeclaration(node.ReturnType, node.Identifier).WithParameterList(node.ParameterList).WithLeadingTrivia(leadingSpace).WithoutTrailingTrivia().WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
            }
            var newInterface = target.WithMembers(methods);
            var root = await doc.GetSyntaxRootAsync();
            var temp1 = root.GetText();
            var temp = doc.WithSyntaxRoot(root.ReplaceNode(target, newInterface));
            var str=temp.GetSyntaxRootAsync().GetAwaiter().GetResult().GetText();
            return temp;
        }
    }
}