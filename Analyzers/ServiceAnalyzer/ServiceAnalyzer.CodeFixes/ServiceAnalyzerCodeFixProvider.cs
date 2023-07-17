using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ServiceAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ServiceAnalyzerCodeFixProvider)), Shared]
    public class ServiceAnalyzerCodeFixProvider : CodeFixProvider
    {
        public override sealed ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ServiceAnalyzerAnalyzer.DiagnosticId); }
        }

        public override sealed FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.Last ();
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target =syntaxToken.Parent.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().FirstOrDefault();
            if (target == null ||!diagnostic.AdditionalLocations.Any()) return;
            context.RegisterCodeFix(CodeAction.Create(CodeFixResources.CodeFixTitle,
                createChangedDocument: token => GenerateMethodsAsync(context.Document, target, diagnostic.AdditionalLocations, token),
                equivalenceKey: CodeFixResources.CodeFixTitle),
                context.Diagnostics.Last());
        }

        public static async Task<Document> GenerateMethodsAsync(Document doc, InterfaceDeclarationSyntax target, IEnumerable<Location> locations, CancellationToken token)
        {
            var methods =new List<MemberDeclarationSyntax>();
            foreach (var location in locations)
            {
                var node =(await location.SourceTree.GetRootAsync()).FindToken(location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                if (node == null) continue;
                var temp = MethodDeclaration(node.ReturnType, node.Identifier).WithParameterList(node.ParameterList).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                methods.Add(temp);
            }
            var newInterface = target.AddMembers(methods.ToArray());
            var root = await doc.GetSyntaxRootAsync(token);
            return doc.WithSyntaxRoot(root.ReplaceNode(target, newInterface).NormalizeWhitespace());
        }
    }
}