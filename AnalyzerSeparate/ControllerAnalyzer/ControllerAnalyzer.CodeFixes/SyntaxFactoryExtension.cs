using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ControllerAnalyzer
{
    public static class SyntaxFactoryExtension
    {
        public static MethodDeclarationSyntax WithHttpMethods(this MethodDeclarationSyntax node, HttpMethod method,SyntaxToken token)
        {
            return node.WithAttributeLists(
                                SingletonList(
                                    AttributeList(
                                        SingletonSeparatedList(
                                            Attribute(
                                                IdentifierName(method.ToString())))).WithOpenBracketToken(token)));
        }

        public static MethodDeclarationSyntax WithMethodModifiers(this MethodDeclarationSyntax node, bool isAsync)
        {
            var tokens = isAsync ? new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword) } : new[] { Token(SyntaxKind.PublicKeyword) };
            return node.WithModifiers(TokenList(tokens));
        }
        public static MethodDeclarationSyntax WithMethodBody(this MethodDeclarationSyntax node, string fieldName, bool isAsync, bool haveReturn, MethodDeclarationSyntax method)
        {
            ExpressionSyntax invokeExpression = isAsync ? AwaitExpression(CreateInvokeExpression(fieldName, method)) : CreateInvokeExpression(fieldName, method);
            StatementSyntax statement = haveReturn ? ReturnStatement(invokeExpression) : ExpressionStatement(invokeExpression);
            return node.WithBody(
                Block(
                    SingletonList(
                        statement)));
        }

        private static InvocationExpressionSyntax CreateInvokeExpression(string fieldName, MethodDeclarationSyntax method)
        {
            var arguments = new List<ArgumentSyntax>();
            foreach (var parameter in method.ParameterList.Parameters)
            {
                arguments.Add(Argument(IdentifierName(parameter.Identifier.ValueText)));
            }
            var result= InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(fieldName), IdentifierName(method.Identifier)))
                        .WithArgumentList(ArgumentList(SeparatedList(arguments)));
            return result;
        }
    }

    public enum HttpMethod
    {
        HttpPost,
        HttpGet,
        HttpDelete,
    }
}