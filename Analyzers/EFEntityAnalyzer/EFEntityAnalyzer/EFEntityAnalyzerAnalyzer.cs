using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

namespace EFEntityAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EFEntityAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public static List<string> IdentifiedName = new List<string>() { "DateTime", "DateOnly", "DateTimeOffset" };
        public const string DiagnosticId = "LY0020";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "EntityComment";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeEntity, SyntaxKind.ClassDeclaration);
        }

        private static bool ExistAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
        {
            return attributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == attributeName);
        }

        private static bool TryGetAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName, out AttributeSyntax attributeSyntax)
        {
            attributeSyntax = attributeLists.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name.ToString() == attributeName);
            return attributeSyntax != null;
        }

        private static bool ExistModifier(SyntaxTokenList modifiers, SyntaxKind kind)
        {
            return modifiers.Any(x => x.IsKind(kind));
        }

        private static bool IsBaseType(TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax)
            {
                return true;
            }
            else if (type is NullableTypeSyntax nullType)
            {
                return IsBaseType(nullType.ElementType);
            }
            else if (type is IdentifierNameSyntax nameType)
            {
                return IdentifiedName.Contains(nameType.Identifier.ValueText);
            }
            return false;
        }

        public static void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax node && node.Modifiers.All(x => !x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                if (ExistAttribute(node.AttributeLists, "Table"))
                {
                    if (TryGetAttribute(node.AttributeLists, "Comment", out var attribute) && TryGetComment(attribute, out string comment))
                    {
                        if (TryGetCommentSyntax(node.AttributeLists.First(), out var trivia))
                        {
                            return;
                        }
                        else
                        {
                        }
                    }
                    foreach (var prop in node.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                    {
                        if (ExistModifier(prop.Modifiers, SyntaxKind.PublicKeyword) && !ExistAttribute(prop.AttributeLists, "NotMapped"))
                        {
                            if (IsBaseType(prop.Type) && !ExistAttribute(prop.AttributeLists, "Comment"))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Identifier.GetLocation(), prop.Identifier.ValueText));
                            }
                        }
                    }
                }
            }
        }

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
                comment = literal.Token.ToString();
                return string.IsNullOrWhiteSpace(comment);
            }
            return default;
        }

        private static bool TryGetCommentSyntax(AttributeListSyntax node, out DocumentationCommentTriviaSyntax trivia)
        {
            foreach (var subNode in node.DescendantNodes())
            {
                if (subNode is DocumentationCommentTriviaSyntax Comment)
                {
                    trivia = Comment;
                    return true;
                }
            }
            trivia = default;
            return false;
        }

        private static bool CommentIsMatch(AttributeSyntax attribute, string name)
        {
        }

        private static void AnalyzeEntity(SyntaxNodeAnalysisContext context)
        {
            context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree).TryGetValue("build_property.RootNamespace", out var rootNamespace);
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            if (context.Node is ClassDeclarationSyntax node && node.Modifiers.All(x => !x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                if (ExistAttribute(node.AttributeLists, "Table"))
                {
                    if (ExistAttribute(node.AttributeLists, "Comment"))
                    {
                        foreach (var prop in node.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                        {
                            if (ExistModifier(prop.Modifiers, SyntaxKind.PublicKeyword) && !ExistAttribute(prop.AttributeLists, "NotMapped"))
                            {
                                if (IsBaseType(prop.Type) && !ExistAttribute(prop.AttributeLists, "Comment"))
                                {
                                    context.ReportDiagnostic(Diagnostic.Create(Rule, prop.Identifier.GetLocation(), prop.Identifier.ValueText));
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), node.Identifier.ValueText));
                        return;
                    }
                }
            }
        }
    }
}