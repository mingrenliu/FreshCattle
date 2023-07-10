using ControllerAnalyzer.Diagnostics;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ControllerMethodCodeFixProvider)), Shared]
    public class ControllerMethodCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ControllerHintDiagnostic.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            diagnostic.Properties.TryGetKey("FieldName", out var fieldName);
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = (ClassDeclarationSyntax)syntaxToken.Parent;
            context.RegisterCodeFix(CodeAction.Create(ControllerCodeFixResource.MethodCodeFixTitle,
                createChangedDocument: token => GenerateMethodsAsync(context.Document, target, fieldName, diagnostic.AdditionalLocations, token),
                equivalenceKey: ControllerCodeFixResource.MethodCodeFixTitle), diagnostic);
        }

        public static async Task<Document> GenerateMethodsAsync(Document doc, ClassDeclarationSyntax target, string fieldName,IEnumerable<Location> methodLocations, CancellationToken token)
        {
            var members = target.Members;
            foreach (var location in methodLocations)
            {
                var node = (MethodDeclarationSyntax)(await location.SourceTree.GetRootAsync()).FindToken(location.SourceSpan.Start).Parent;
                var (isAsync, haveReturn) = ParseMethodReturnType(node.ReturnType);
                var methodName = node.Identifier.ValueText.EndsWith("Async") ? node.Identifier.ValueText.Substring(0, node.Identifier.ValueText.Length - 5) : node.Identifier.ValueText;
                var method = MethodDeclaration(node.ReturnType, Identifier(methodName))
                    .WithMethodModifiers(isAsync)
                    .WithHttpMethods(HttpMethod.HttpPost)
                    .WithParameterList(node.ParameterList)
                    .WithMethodBody(fieldName,isAsync,haveReturn, node);
                members=members.Add(method);
            }
            var newClass = target.WithMembers(members);
            var root = await doc.GetSyntaxRootAsync(token);
            return doc.WithSyntaxRoot(root.ReplaceNode(target, newClass));
        }

        public static (bool, bool) ParseMethodReturnType(TypeSyntax type)
        {
            if (type.IsKind(SyntaxKind.VoidKeyword))
            {
                return (false, false);
            }
            else if (type is IdentifierNameSyntax identifierNode)
            {
                return identifierNode.Identifier.ValueText.StartsWith("Task")? (true, false):(false, true);
            }
            else if (type is GenericNameSyntax genericNode)
            {
                return genericNode.Identifier.ValueText.StartsWith("Task")?(true, true):(false, true);
            }
            else if (type is NullableTypeSyntax nullableType)
            {
                return ParseMethodReturnType(nullableType.ElementType);
            }
            else if (type is QualifiedNameSyntax qualifiedNode)
            {
                return ParseMethodReturnType(qualifiedNode.Right);
            }
            else
            {
                return (false, true);
            }
        }
    }
}