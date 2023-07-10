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

namespace ControllerAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ControllerAnalyzerCodeFixProvider)), Shared]
    public class ControllerCtorAnalyzerCodeFixProvider : CodeFixProvider
    {
        public override sealed ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CtorDiagnostic.DiagnosticId); }
        }

        public override sealed FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = (ClassDeclarationSyntax)syntaxToken.Parent;
            var serviceName = GetServiceName(target.Identifier.ValueText.ToString());
            context.RegisterCodeFix(CodeAction.Create(CodeFixResources.FieldCodeFixTitle,
                createChangedDocument: token => GenerateMethodsAsync(context.Document, target, serviceName, token),
                equivalenceKey: CodeFixResources.FieldCodeFixTitle), diagnostic);
        }

        public static string GetServiceName(string controllerName)
        {
            return "I" + controllerName.Substring(0, controllerName.Length - 10) + "Service";
        }

        public static async Task<Document> GenerateMethodsAsync(Document doc, ClassDeclarationSyntax target, string serviceName, CancellationToken token)
        {
            var paraName = serviceName[1].ToString().ToLower() + serviceName.Substring(2);
            var fieldName = "_" + paraName;
            var space = target.OpenBraceToken.LeadingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.WhitespaceTrivia));
            var leadingSpace = space == null ? TriviaList(Whitespace("    ")) : TriviaList(space, Whitespace("    "));
            var members = target.Members;
            var field = FieldDeclaration(VariableDeclaration(IdentifierName(serviceName), SingletonSeparatedList(VariableDeclarator(Identifier(fieldName)))))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword).WithLeadingTrivia(leadingSpace), Token(SyntaxKind.ReadOnlyKeyword))).WithSemicolonToken(Token(SyntaxKind.SemicolonToken).WithTrailingTrivia(SyntaxTrivia(SyntaxKind.EndOfLineTrivia,"newlines"))).NormalizeWhitespace();
            var ctor = CreateCtor(target, paraName, serviceName, fieldName,leadingSpace);
            var list = new MemberDeclarationSyntax[] { field, ctor }.Concat(members);
            var newInterface = target.WithMembers(List(list)).NormalizeWhitespace();
            var root = await doc.GetSyntaxRootAsync(token);
            return doc.WithSyntaxRoot(root.ReplaceNode(target, newInterface));
        }

        private static ConstructorDeclarationSyntax CreateCtor(ClassDeclarationSyntax target, string paraName, string serviceName, string fieldName,SyntaxTriviaList leadingSpace)
        {
            return ConstructorDeclaration(
                        target.Identifier)
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword).WithLeadingTrivia()))
                    .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList(
                                Parameter(
                                    Identifier(paraName))
                                .WithType(
                                    IdentifierName(serviceName)))))
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName(fieldName),
                                        IdentifierName(paraName)))))).NormalizeWhitespace();
        }
    }
}