using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
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
    public class ControllerAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ControllerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.Last();
            diagnostic.Properties.TryGetValue("FieldName", out var fieldName);
            diagnostic.Properties.TryGetValue("UnUsedMethods", out var methodStr);
            diagnostic.Properties.TryGetValue("SourceCode", out var SourceCodeStr);
            var unUsedMethods = methodStr.Split(',');
            var sourceCode = SourceText.From(SourceCodeStr);
            var methods=(await CSharpSyntaxTree.ParseText(sourceCode).GetRootAsync(context.CancellationToken)).DescendantNodesAndSelf()
                .OfType<MethodDeclarationSyntax>().Where(x=>unUsedMethods.Contains(x.Identifier.ValueText));
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = (ClassDeclarationSyntax)syntaxToken.Parent;
            context.RegisterCodeFix(CodeAction.Create(CodeFixResources.MethodCodeFixTitle,
                createChangedDocument: token => GenerateMethodsAsync(context.Document, target, fieldName, methods, token),
                equivalenceKey: CodeFixResources.MethodCodeFixTitle), diagnostic);
        }
        public static string GetServiceName(string controllerName)
        {
            return "I" + controllerName.Substring(0, controllerName.Length - 10) + "Service";
        }
        private static ConstructorDeclarationSyntax CreateCtor(ClassDeclarationSyntax target, string paraName, string serviceName, string fieldName)
        {
            return ConstructorDeclaration(
                    Identifier(target.Identifier.ValueText))
                    .WithModifiers(
                        TokenList(XmlCreator.CreateXml(paraName,false)))
                    .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList(
                                Parameter(
                                    Identifier(paraName))
                                .WithType(
                                    IdentifierName(serviceName)))).WithoutLeadingTrivia())
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName(fieldName),
                                        IdentifierName(paraName))))));
        }
        public static async Task<Document> GenerateMethodsAsync(Document doc, ClassDeclarationSyntax target, string fieldName, IEnumerable<MethodDeclarationSyntax> methods, CancellationToken token)
        {
            var members = new List<MemberDeclarationSyntax>();
            var serviceName = GetServiceName(target.Identifier.ValueText.ToString());
            if (string.IsNullOrEmpty(fieldName))
            {
                var paraName = serviceName[1].ToString().ToLower() + serviceName.Substring(2);
                fieldName = "_" + paraName;
                var field = FieldDeclaration(VariableDeclaration(IdentifierName(serviceName), SingletonSeparatedList(VariableDeclarator(Identifier(fieldName)))))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)));
                var ctor = CreateCtor(target, paraName, serviceName, fieldName);
                members.Add(field);members.Add(ctor);
            }
            foreach (var node in methods)
            {
                var (isAsync, haveReturn) = ParseMethodReturnType(node.ReturnType);
                var methodName = node.Identifier.ValueText.EndsWith("Async") ? node.Identifier.ValueText.Substring(0, node.Identifier.ValueText.Length - 5) : node.Identifier.ValueText;
                var method = MethodDeclaration(node.ReturnType, Identifier(methodName))
                    .WithHttpMethods(HttpMethod.HttpPost, XmlCreator.CreateXml(node.ParameterList.Parameters.Select(x=>x.Identifier.ValueText)))
                    .WithMethodModifiers(isAsync)
                    .WithParameterList(node.ParameterList)
                    .WithMethodBody(fieldName, isAsync, haveReturn, node);
                members.Add(method);
            }
            var newClass = target.AddMembers(members.ToArray());
            var root = await doc.GetSyntaxRootAsync(token);
            return doc.WithSyntaxRoot(root.ReplaceNode(target, newClass));
        }

        public static (bool, bool) ParseMethodReturnType(TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax predefinedNode && predefinedNode.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return (false, false);
            }
            else if (type is IdentifierNameSyntax identifierNode)
            {
                return identifierNode.Identifier.ValueText.StartsWith("Task") ? (true, false) : (false, true);
            }
            else if (type is GenericNameSyntax genericNode)
            {
                return genericNode.Identifier.ValueText.StartsWith("Task") ? (true, true) : (false, true);
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
