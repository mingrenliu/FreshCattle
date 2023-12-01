using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EFEntityAnalyzer;

public class AttributeFactory
{
    public static AttributeListSyntax CreateAttributeWithStringPara(string name, string value = "")
    {
        return CreateAttribute(name, WithArguments(value));
    }

    public static AttributeListSyntax CreateAttributeWithIntPara(string name, int value)
    {
        return CreateAttribute(name, WithArguments(value));
    }

    public static AttributeListSyntax CreateAttributeWithIntPara(string name, int[] value)
    {
        if (value.Length == 0)
        {
            value = new[] { 0 };
        }
        return CreateAttribute(name, WithArguments(value));
    }

    public static AttributeListSyntax CreateAttributeWithNoPara(string name)
    {
        return AttributeList(
                           SingletonSeparatedList(
                                                  Attribute(
                                                            IdentifierName(name))));
    }

    private static AttributeListSyntax CreateAttribute(string name, AttributeArgumentListSyntax args)
    {
        return AttributeList(
                           SingletonSeparatedList(
                                                  Attribute(
                                                            IdentifierName(name))
                                                  .WithArgumentList(args)));
    }

    private static AttributeArgumentListSyntax WithArguments(string value)
    {
        return AttributeArgumentList(
                                     SingletonSeparatedList(
                                                            AttributeArgument(
                                                                              LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)))));
    }

    private static AttributeArgumentListSyntax WithArguments(int value)
    {
        return AttributeArgumentList(
                                     SingletonSeparatedList(
                                                            AttributeArgument(
                                                                              LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)))));
    }

    private static AttributeArgumentListSyntax WithArguments(int[] value)
    {
        if (value.Length == 1)
        {
            return WithArguments(value.First());
        }
        else
        {
            var token = new List<SyntaxNodeOrToken>();
            for (int i = 0; i < value.Length; i++)
            {
                token.Add(AttributeArgument(
                                            LiteralExpression(
                                                SyntaxKind.NumericLiteralExpression,
                                                Literal(value[i]))));
                if (i != value.Length - 1)
                {
                    token.Add(Token(SyntaxKind.CommaToken));
                }
            }
            return AttributeArgumentList(
                                         SeparatedList<AttributeArgumentSyntax>(token));
        }
    }
}