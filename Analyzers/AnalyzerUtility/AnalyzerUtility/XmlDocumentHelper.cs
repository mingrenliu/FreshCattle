using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AnalyzerUtility;

public static class XmlDocumentHelper
{
    public static bool TryGetXmlDocument(this SyntaxNode node, out DocumentationCommentTriviaSyntax? doc)
    {
        doc = node.GetLeadingTrivia().Select(i => i.GetStructure()).OfType<DocumentationCommentTriviaSyntax>().FirstOrDefault();
        return doc != null;
    }

    public static bool TryGetComment(this DocumentationCommentTriviaSyntax doc, out string comment)
    {
        comment = string.Empty;
        foreach (var node in doc.DescendantNodes().OfType<XmlElementSyntax>())
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
}