using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServiceAnalyzer.Diagnostics;
using ServiceAnalyzer.Diagnotics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            //var root=await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var syntaxToken =(await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var implement=syntaxToken.Parent.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().First();
            //
            var lst = new List<MethodDeclarationSyntax>();
            foreach (var item in diagnostic.AdditionalLocations)
            {
                lst.AddRange(await FindMethodsInClass(item));
            }
            var sourceTree = diagnostic.AdditionalLocations.First().SourceTree;
            context.RegisterCodeFix(CodeAction.Create(ServiceCodeFixResource.CodeFixTitle,
                createChangedDocument:token=>GenerateMethodsAsync(context.Document,implement,lst,token),
                equivalenceKey:ServiceCodeFixResource.CodeFixTitle), 
                context.Diagnostics.First());
        }
        public static async Task<Document> GenerateMethodsAsync(Document doc,InterfaceDeclarationSyntax node,List<MethodDeclarationSyntax> methods,CancellationToken token)
        {
            return await Task.FromResult(doc);
        }
        public static async Task<IEnumerable<InterfaceDeclarationSyntax>> FindMethodsInInterface(Location location)
        {
            var rootTree = await location.SourceTree.GetRootAsync();
            return rootTree.FindToken(location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().First()
                .DescendantNodes().OfType<InterfaceDeclarationSyntax>();
        }
        public static async Task<IEnumerable<MethodDeclarationSyntax>> FindMethodsInClass(Location location)
        {
            var rootTree = await location.SourceTree.GetRootAsync();
            return rootTree.FindToken(location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First()
                .DescendantNodes().OfType<MethodDeclarationSyntax>();
        }
    }   

}