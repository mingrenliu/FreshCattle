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
using static ControllerAnalyzer.ControllerAnalyzer;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ControllerAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ControllerAnalyzerCodeFixProvider)), Shared]
    public class ControllerAnalyzerCodeFixProvider : CodeFixProvider
    {
        public override sealed ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ControllerAnalyzer.DiagnosticId); }
        }

        public override sealed FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.Last();
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = (ClassDeclarationSyntax)syntaxToken.Parent;
            var serviceName = GetServiceName(target.Identifier.ValueText.ToString());
            IEnumerable<IMethodSymbol> unUsedMethods = new List<IMethodSymbol>();
            var (field, fieldName) = GetField(target, serviceName);
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                var project = context.Document.Project.Solution.Projects.FirstOrDefault(x => x.Name.EndsWith("Service"));
                if (project != null)
                {
                    var compile = await project.GetCompilationAsync(context.CancellationToken);
                    if (compile.GetSymbolsWithName(name => name == serviceName, SymbolFilter.Type).FirstOrDefault() is INamedTypeSymbol symbol)
                    {
                        unUsedMethods = symbol.GetMembers().OfType<IMethodSymbol>();
                    }
                }
            }
            else
            {
                var model = await context.Document.GetSemanticModelAsync(context.CancellationToken);
                var symbol = model.GetSymbolInfo(field).Symbol as INamedTypeSymbol;
                var methods = symbol?.GetMembers().OfType<IMethodSymbol>();
                if (methods == null || !methods.Any()) return;
                var implementMethods = GetMethods(GetMethods(target.DescendantNodes().OfType<MemberAccessExpressionSyntax>(), fieldName), model);
                unUsedMethods = methods.Except(implementMethods);
                if (!unUsedMethods.Any()) return;
            }
            context.RegisterCodeFix(CodeAction.Create(CodeFixResources.MethodCodeFixTitle,
                createChangedDocument: token => GenerateMethodsAsync(context.Document, target, fieldName, unUsedMethods, token),
                equivalenceKey: CodeFixResources.MethodCodeFixTitle), diagnostic);
        }

        private static ConstructorDeclarationSyntax CreateCtor(ClassDeclarationSyntax target, string paraName, string serviceName, string fieldName)
        {
            return ConstructorDeclaration(
                    Identifier(target.Identifier.ValueText))
                    .WithModifiers(
                        TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithLeadingTrivia(XmlCreator.CreateXml(paraName, false))
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

        public static async Task<Document> GenerateMethodsAsync(Document doc, ClassDeclarationSyntax target, string fieldName, IEnumerable<IMethodSymbol> methods, CancellationToken token)
        {
            var members = new List<MemberDeclarationSyntax>();
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                var serviceName = GetServiceName(target.Identifier.ValueText.ToString());
                var paraName = serviceName[1].ToString().ToLower() + serviceName.Substring(2);
                fieldName = "_" + paraName;
                var field = FieldDeclaration(VariableDeclaration(IdentifierName(serviceName), SingletonSeparatedList(VariableDeclarator(Identifier(fieldName)))))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)));
                var ctor = CreateCtor(target, paraName, serviceName, fieldName);
                members.Add(field); members.Add(ctor);
            }
            foreach (var methodSymbol in methods)
            {
                var node = methodSymbol.DeclaringSyntaxReferences.First().GetSyntaxAsync().GetAwaiter().GetResult() as MethodDeclarationSyntax;
                var (isAsync, haveReturn) = ParseMethodReturnType(node.ReturnType);
                var methodName = node.Identifier.ValueText.EndsWith("Async") ? node.Identifier.ValueText.Substring(0, node.Identifier.ValueText.Length - 5) : node.Identifier.ValueText;
                var method = MethodDeclaration(node.ReturnType, Identifier(methodName))
                    .WithHttpMethods(GetMethod(methodSymbol))
                    .WithLeadingTrivia(TriviaList(XmlCreator.CreateXml(node.ParameterList.Parameters.Select(x => x.Identifier.ValueText))))
                    .WithMethodModifiers(isAsync)
                    .WithParameterList(GetParameterList(node.ParameterList))
                    .WithMethodBody(fieldName, isAsync, haveReturn, node);
                members.Add(method);
            }
            var newClass = target.AddMembers(members.ToArray());
            var root = await doc.GetSyntaxRootAsync(token);
            return doc.WithSyntaxRoot(root.ReplaceNode(target, newClass).NormalizeWhitespace());
        }

        public static ParameterListSyntax GetParameterList(ParameterListSyntax paras)
        {
            var required = new List<ParameterSyntax>();
            foreach (var para in paras.Parameters)
            {
                if (para.Type is NullableTypeSyntax)
                {
                    continue;
                }
                if (!ExistAttribute(para.AttributeLists, "Required"))
                {
                    if (para.DescendantNodes().Any(x => x.IsKind(SyntaxKind.EqualsValueClause)))
                    {
                        continue;
                    }
                    required.Add(para);
                }
            }
            if (required.Any())
            {
                return paras.ReplaceNodes(required, (oldNode, newNode) =>
                {
                    return newNode.AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Required")))));
                });
            }
            else
            {
                return paras;
            }
        }

        static HttpMethod GetMethod(IMethodSymbol symbol)
        {
            var paras = symbol.Parameters;
            var containDelete = symbol.Name.Contains("Delete") || symbol.Name.Contains("delete");
            if (paras.Length == 0)
            {
                return containDelete ? HttpMethod.HttpDelete : HttpMethod.HttpGet;
            }
            else
            {
                foreach (var item in symbol.Parameters)
                {
                    if (item.Type.TypeKind == TypeKind.Array)
                    {
                        return HttpMethod.HttpPost;
                    }
                    else if (item.Type.TypeKind == TypeKind.Interface)
                    {
                        return HttpMethod.HttpPost;
                    }
                    else if (item.Type.TypeKind == TypeKind.Class)
                    {
                        var name = item.Type.ToDisplayString(NullableFlowState.NotNull);
                        if (name == "System.Uri" || name == "string")
                        {
                            continue;
                        }
                        return HttpMethod.HttpPost;
                    }
                    else if (item.Type.TypeKind == TypeKind.Struct || item.Type.TypeKind == TypeKind.Structure)
                    {
                        if (item.Type.ToDisplayString(NullableFlowState.NotNull) == "System.TimeSpan")
                        {
                            return HttpMethod.HttpPost;
                        }
                    }
                }
                return containDelete ? HttpMethod.HttpDelete : HttpMethod.HttpGet;
            }
        }

        public static bool ExistAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
        {
            return attributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == attributeName);
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