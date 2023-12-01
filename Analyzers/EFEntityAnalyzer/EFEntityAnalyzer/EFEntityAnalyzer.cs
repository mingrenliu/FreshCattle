using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EFEntityAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EFEntityAnalyzer : DiagnosticAnalyzer
    {
        public static List<string> IdentifiedName = new List<string>() { "DateTime", "DateOnly", "DateTimeOffset" };
        public const string DiagnosticId = "LY0020";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "EntityAttribute";

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeEntity, SyntaxKind.ClassDeclaration);
        }

        private static bool ExistAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
        {
            return attributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == attributeName);
        }

        private static bool ExistModifier(SyntaxTokenList modifiers, SyntaxKind kind)
        {
            return modifiers.Any(x => x.IsKind(kind));
        }

        private static bool IsBaseType(TypeSyntax type, SyntaxNodeAnalysisContext context)
        {
            if (type is PredefinedTypeSyntax)
            {
                return true;
            }
            else if (type is NullableTypeSyntax nullType)
            {
                return IsBaseType(nullType.ElementType, context);
            }
            else if (type is QualifiedNameSyntax qualifiedNode)
            {
                IsBaseType(qualifiedNode.Right, context);
            }
            else if (type is IdentifierNameSyntax nameType)
            {
                if (IdentifiedName.Contains(nameType.Identifier.ValueText))
                {
                    return true;
                }
                var symbol = context.SemanticModel.GetTypeInfo(nameType);
                if (symbol.Type != null)
                {
                    return symbol.Type.TypeKind == TypeKind.Enum;
                }
            }
            return false;
        }

        private static void AnalyzeEntity(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax node && node.Modifiers.All(x => !x.IsKind(SyntaxKind.AbstractKeyword)))
            {
                if (ExistAttribute(node.AttributeLists, "Table"))
                {
                    var locations = new List<Location>();
                    if (!ExistAttribute(node.AttributeLists, "Comment"))
                    {
                        locations.Add(node.Identifier.GetLocation());
                    }
                    foreach (var prop in node.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                    {
                        if (ExistModifier(prop.Modifiers, SyntaxKind.PublicKeyword) && !ExistAttribute(prop.AttributeLists, "NotMapped"))
                        {
                            if (IsBaseType(prop.Type, context) && !ExistAttribute(prop.AttributeLists, "Comment"))
                            {
                                locations.Add(prop.Identifier.GetLocation());
                            }
                        }
                    }
                    if (locations.Any())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), locations, node.Identifier.ValueText));
                    }
                }
            }
        }
    }
}