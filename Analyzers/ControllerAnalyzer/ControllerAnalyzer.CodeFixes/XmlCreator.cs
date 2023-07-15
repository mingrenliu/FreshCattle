using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ControllerAnalyzer
{
    public static class XmlCreator
    {
        public static SyntaxTriviaList CreateXml(IEnumerable<string> paras, bool withReturn = true)
        {
            return CreateXml(paras.ToArray(), withReturn);
        }
        public static SyntaxTriviaList CreateXml(string para, bool withReturn = true)
        {
            return CreateXml(new string[] { para} , withReturn);
        }
        public static SyntaxTriviaList CreateXml(string[] paras, bool withReturn = true)
        {
            var kind = withReturn ? SyntaxKind.OpenBracketToken : SyntaxKind.PublicKeyword;
            var results = new List<XmlNodeSyntax>();
            results.AddRange(CreateSummary());
            foreach (var item in paras)
            {
                results.AddRange(CreatePara(item));
            }
            if (withReturn)
            {
                results.AddRange(CreateReturn());
            }
            results.Add(CreateNewLine());
            return TriviaList(
                        Trivia(
                            DocumentationCommentTrivia(
                                SyntaxKind.SingleLineDocumentationCommentTrivia,
                                List(results),
                                Token(SyntaxKind.EndOfDocumentationCommentToken)
                                )
                            )
                        );
        }

        private static XmlNodeSyntax[] CreateSummary()
        {
            return new XmlNodeSyntax[]{
                    XmlText()
                    .WithTextTokens(
                        TokenList(
                            XmlTextLiteral(
                                TriviaList(
                                    DocumentationCommentExterior("///")),
                                " ",
                                " ",
                                TriviaList()))),
                    XmlExampleElement(
                        SingletonList<XmlNodeSyntax>(
                            XmlText()
                            .WithTextTokens(
                                TokenList(
                                    new []{
                                        XmlTextNewLine(
                                            TriviaList(),
                                            Environment.NewLine,
                                            Environment.NewLine,
                                            TriviaList()),
                                        XmlTextLiteral(
                                            TriviaList(
                                                DocumentationCommentExterior("        ///")),
                                            " ",
                                            " ",
                                            TriviaList()),
                                        XmlTextNewLine(
                                            TriviaList(),
                                            Environment.NewLine,
                                            Environment.NewLine,
                                            TriviaList()),
                                        XmlTextLiteral(
                                            TriviaList(
                                                DocumentationCommentExterior("        ///")),
                                            " ",
                                            " ",
                                            TriviaList())}))))
                    .WithStartTag(
                        XmlElementStartTag(
                            XmlName(
                                Identifier("summary"))))
                    .WithEndTag(
                        XmlElementEndTag(
                            XmlName(
                                Identifier("summary")))) };
        }

        private static XmlNodeSyntax[] CreatePara(string name)
        {
            return new XmlNodeSyntax[]
            {
                XmlText()
                    .WithTextTokens(
                        TokenList(
                            new []{
                                XmlTextNewLine(
                                    TriviaList(),
                                    Environment.NewLine,
                                    Environment.NewLine,
                                    TriviaList()),
                                XmlTextLiteral(
                                    TriviaList(
                                        DocumentationCommentExterior("        ///")),
                                    " ",
                                    " ",
                                    TriviaList())})),
                    XmlExampleElement()
                    .WithStartTag(
                        XmlElementStartTag(
                            XmlName(
                                Identifier(
                                    TriviaList(),
                                    SyntaxKind.ParamKeyword,
                                    "param",
                                    "param",
                                    TriviaList())))
                        .WithAttributes(
                            SingletonList<XmlAttributeSyntax>(
                                XmlNameAttribute(
                                    XmlName(
                                        Identifier(name)),
                                    Token(SyntaxKind.DoubleQuoteToken),
                                    IdentifierName(name),
                                    Token(SyntaxKind.DoubleQuoteToken)))))
                    .WithEndTag(
                        XmlElementEndTag(
                            XmlName(
                                Identifier(
                                    TriviaList(),
                                    SyntaxKind.ParamKeyword,
                                    "param",
                                    "param",
                                    TriviaList()))))
            };
        }

        private static XmlNodeSyntax[] CreateReturn()
        {
            return new XmlNodeSyntax[]
            {
                XmlText()
                    .WithTextTokens(
                        TokenList(
                            new []{
                                XmlTextNewLine(
                                    TriviaList(),
                                    Environment.NewLine,
                                    Environment.NewLine,
                                    TriviaList()),
                                XmlTextLiteral(
                                    TriviaList(
                                        DocumentationCommentExterior("        ///")),
                                    " ",
                                    " ",
                                    TriviaList())})),
                    XmlExampleElement()
                    .WithStartTag(
                        XmlElementStartTag(
                            XmlName(
                                Identifier("returns"))))
                    .WithEndTag(
                        XmlElementEndTag(
                            XmlName(
                                Identifier("returns"))))
            };
        }

        private static XmlNodeSyntax CreateNewLine()
        {
            return XmlText()
                        .WithTextTokens(
                            TokenList(
                                XmlTextNewLine(
                                    TriviaList(),
                                    Environment.NewLine,
                                    Environment.NewLine,
                                    TriviaList())));
        }
    }
}