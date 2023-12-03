using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AnalyzerUtility;

public static class AttributeHelper
{
    public static bool HaveAttribute(this SyntaxList<AttributeListSyntax> attributes, string name)
    {
        return attributes.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == name);
    }

    public static bool TryGetAttribute(this SyntaxList<AttributeListSyntax> attributes, string name, out AttributeSyntax? attribute)
    {
        attribute = attributes.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name.ToString() == name);
        return attribute != null;
    }

    public static bool HaveAttribute(this SyntaxList<AttributeListSyntax> attributes, IEnumerable<string> names)
    {
        return attributes.SelectMany(x => x.Attributes).FirstOrDefault(x => names.Contains(x.Name.ToString())) != null;
    }

    public static bool TryGetAttributeFirstArgString(this AttributeSyntax node, out string content)
    {
        content = string.Empty;
        var first = node.ArgumentList?.Arguments.FirstOrDefault();
        if (first != null && first.Expression is LiteralExpressionSyntax literal)
        {
            content = literal.Token.ValueText;
            return !string.IsNullOrWhiteSpace(content);
        }
        return false;
    }
}