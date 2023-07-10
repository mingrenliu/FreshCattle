using ControllerAnalyzer.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ControllerAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ControllerCtorCodeFixProvider)), Shared]
    public class ControllerCtorCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ControllerFieldHintDiagnostic.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = (ClassDeclarationSyntax)syntaxToken.Parent;
            var serviceName = ControllerAnalyzer.GetServiceName(target.Identifier.ValueText.ToString());
            context.RegisterCodeFix(CodeAction.Create(ControllerCodeFixResource.FieldCodeFixTitle,
                createChangedDocument: token => GenerateMethodsAsync(context.Document, target, serviceName, token),
                equivalenceKey: ControllerCodeFixResource.FieldCodeFixTitle), diagnostic);
        }

        public static async Task<Document> GenerateMethodsAsync(Document doc, ClassDeclarationSyntax target, string serviceName, CancellationToken token)
        {
            var paraName = serviceName[1].ToString().ToLower() + serviceName.Substring(2);
            var fieldName = "_" + paraName;
            var space = target.OpenBraceToken.LeadingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.WhitespaceTrivia));
            var leadingSpace = space == null ? TriviaList(Whitespace("    ")) : TriviaList(space, Whitespace("    "));
            var members = target.Members;
            var field = FieldDeclaration(VariableDeclaration(IdentifierName(serviceName), SingletonSeparatedList(VariableDeclarator(Identifier(fieldName)))))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))).WithLeadingTrivia(leadingSpace).NormalizeWhitespace();
            var ctor = ConstructorDeclaration(
                        target.Identifier)
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)))
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
                                        IdentifierName(paraName)))))).WithLeadingTrivia(leadingSpace).NormalizeWhitespace();

            var newInterface = target.WithMembers(List(members.Insert(0, ctor).Insert(0, field)));
            var root = await doc.GetSyntaxRootAsync(token);
            return doc.WithSyntaxRoot(root.ReplaceNode(target, newInterface));
        }
    }
}