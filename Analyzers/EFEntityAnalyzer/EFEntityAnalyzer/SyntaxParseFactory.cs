using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace EFEntityAnalyzer
{
    public class SyntaxParseFactory
    {
        public static bool TryGetComment(AttributeSyntax node, out string comment)
        {
            comment = string.Empty;
            var first = node.ArgumentList.Arguments.FirstOrDefault();
            if (first == null)
            {
                return false;
            }
            if (first.Expression is LiteralExpressionSyntax literal)
            {
                comment = literal.Token.ValueText;
                return !string.IsNullOrWhiteSpace(comment);
            }
            return default;
        }

        public static bool TryGetCommentSyntax(SyntaxNode node, out DocumentationCommentTriviaSyntax trivia)
        {
            var subNodes = node.GetLeadingTrivia().Select(i => i.GetStructure()).OfType<DocumentationCommentTriviaSyntax>();
            trivia = subNodes.FirstOrDefault();
            return subNodes.Any();
        }

        public static bool TryGetTriviaComment(DocumentationCommentTriviaSyntax trivia, out string comment)
        {
            comment = string.Empty;
            foreach (var node in trivia.DescendantNodes().OfType<XmlElementSyntax>())
            {
                if (node.StartTag.Name.ToString() == "summary")
                {
                    foreach (var text in node.Content.OfType<XmlTextSyntax>())
                    {
                        comment = string.Join(" ", text.TextTokens.Where(x => x.IsKind(SyntaxKind.XmlTextLiteralToken)).Select(x => x.ValueText));
                        return !string.IsNullOrWhiteSpace(comment);
                    }
                }
            }
            return false;
        }

        public static bool ExistAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
        {
            return attributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == attributeName);
        }

        public static bool TryGetAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName, out AttributeSyntax attributeSyntax)
        {
            attributeSyntax = attributeLists.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name.ToString() == attributeName);
            return attributeSyntax != null;
        }
    }
}