using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static EFEntityAnalyzer.SyntaxParseFactory;

namespace EFEntityAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "LY0021";
        public const string ClassPrefix = "class_";
        public const string PropPrefix = "prop_";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(CommentResource.AnalyzerTitle), CommentResource.ResourceManager, typeof(CommentResource));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(CommentResource.AnalyzerMessageFormat), CommentResource.ResourceManager, typeof(CommentResource));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(CommentResource.AnalyzerDescription), CommentResource.ResourceManager, typeof(CommentResource));
        private const string Category = "EntityComment";

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeComment, SyntaxKind.ClassDeclaration);
        }

        public static void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax node && node.Modifiers.All(x => !x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                if (ExistAttribute(node.AttributeLists, "Table"))
                {
                    var comments = new Dictionary<string, string>();
                    var locations = new List<Location>();
                    if (ParseComment(node.AttributeLists, out var classComment) && CommentNotExist(node))
                    {
                        comments[ClassPrefix + node.Identifier.ValueText] = classComment;
                        locations.Add(node.Identifier.GetLocation());
                    }
                    foreach (var prop in node.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                    {
                        if (ParseComment(prop.AttributeLists, out string propComment) && CommentNotExist(prop))
                        {
                            comments[PropPrefix + prop.Identifier.ValueText] = propComment;
                            locations.Add(prop.Identifier.GetLocation());
                        }
                    }
                    if (locations.Any())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), locations, comments.ToImmutableDictionary(), node.Identifier.ValueText));
                    }
                }
            }
        }

        private static bool CommentNotExist(SyntaxNode node)
        {
            if (TryGetCommentSyntax(node, out var trivia))
            {
                if (TryGetTriviaComment(trivia, out var _))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ParseComment(SyntaxList<AttributeListSyntax> attributes, out string comment)
        {
            comment = string.Empty;
            if (TryGetAttribute(attributes, "Comment", out var attribute) && TryGetComment(attribute, out comment))
            {
                return true;
            }
            return false;
        }
    }
}