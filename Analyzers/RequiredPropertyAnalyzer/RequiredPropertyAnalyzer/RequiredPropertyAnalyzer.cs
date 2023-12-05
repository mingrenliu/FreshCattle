using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace RequiredPropertyAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RequiredPropertyAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string[] IgnoreAttribute = new string[] { "JsonIgnore", "ValidateNever" };
        public static IEnumerable<string> RequiredAttribute = new List<string> { "Required", "FormatRequired" };

        #region add required

        public const string DiagnosticId = "LY0031";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(AddRequiredResources.AnalyzerTitle), AddRequiredResources.ResourceManager, typeof(AddRequiredResources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(AddRequiredResources.AnalyzerMessageFormat), AddRequiredResources.ResourceManager, typeof(AddRequiredResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(AddRequiredResources.AnalyzerDescription), AddRequiredResources.ResourceManager, typeof(AddRequiredResources));
        private const string Category = "PropertyAttribute";

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description, customTags: new string[] { "NotConfigurable" });

        #endregion add required

        #region remove required

        public const string RemoveDiagnosticId = "LY0032";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString RemoveTitle = new LocalizableResourceString(nameof(RemoveRequiredResources.AnalyzerTitle), RemoveRequiredResources.ResourceManager, typeof(RemoveRequiredResources));

        private static readonly LocalizableString RemoveMessageFormat = new LocalizableResourceString(nameof(RemoveRequiredResources.AnalyzerMessageFormat), RemoveRequiredResources.ResourceManager, typeof(RemoveRequiredResources));
        private static readonly LocalizableString RemoveDescription = new LocalizableResourceString(nameof(RemoveRequiredResources.AnalyzerDescription), RemoveRequiredResources.ResourceManager, typeof(RemoveRequiredResources));

        public static readonly DiagnosticDescriptor RemoveRule = new DiagnosticDescriptor(RemoveDiagnosticId, RemoveTitle, RemoveMessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: RemoveDescription, customTags: new string[] { "NotConfigurable" });

        #endregion remove required

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        { get { return ImmutableArray.Create(Rule, RemoveRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzerProperty, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzerProperty(SyntaxNodeAnalysisContext context)
        {
            var attributes = RequiredAttribute;
            if (context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.RequiredAttributeName", out var attributeName))
            {
                if (!attributes.Contains(attributeName))
                {
                    attributes = attributes.Concat(new List<string>() { attributeName });
                }
            }
            if (!context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.ModelSuffix", out string modelSuffix))
            {
                modelSuffix = "Model";
            }
            if (context.Node is ClassDeclarationSyntax node && node.Identifier.ValueText.EndsWith(modelSuffix))
            {
                var addRequired = new List<PropertyDeclarationSyntax>();
                var removeRequired = new List<PropertyDeclarationSyntax>();
                foreach (var prop in node.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                {
                    if (!ExistModifier(prop.Modifiers, SyntaxKind.PublicKeyword))
                    {
                        continue;
                    }
                    if (ExistAttribute(prop.AttributeLists, IgnoreAttribute))
                    {
                        continue;
                    }
                    var isNullable = prop.Type is NullableTypeSyntax;
                    if (ExistAttribute(prop.AttributeLists, attributes))
                    {
                        if (isNullable)
                        {
                            removeRequired.Add(prop);
                        }
                    }
                    else
                    {
                        if ((isNullable is false) && !IsValueType(prop.Type, context))
                        {
                            addRequired.Add(prop);
                        }
                    }
                }

                if (addRequired.Any())
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), GetLocations(addRequired), node.Identifier.ValueText));
                }
                else if (removeRequired.Any())
                {
                    context.ReportDiagnostic(Diagnostic.Create(RemoveRule, node.Identifier.GetLocation(), GetLocations(removeRequired), node.Identifier.ValueText));
                }
            }
        }

        private static bool IsValueType(TypeSyntax type, SyntaxNodeAnalysisContext context)
        {
            if (type is PredefinedTypeSyntax preDefinedType)
            {
                return !preDefinedType.Keyword.IsKind(SyntaxKind.StringKeyword);
            }
            else if (type is NullableTypeSyntax)
            {
                return false;
            }
            else if (type is AliasQualifiedNameSyntax aliasType)
            {
                IsValueType(aliasType.Alias, context);
            }
            else if (type is QualifiedNameSyntax qualifiedNode)
            {
                IsValueType(qualifiedNode.Right, context);
            }
            else if (type is TupleTypeSyntax)
            {
                return true;
            }
            else if (type is IdentifierNameSyntax nameType)
            {
                if (IdentifiedName.Contains(nameType.Identifier.ValueText))
                {
                    return true;
                }
                return context.SemanticModel.GetTypeInfo(nameType).Type.IsValueType;
            }
            else if (type is NameSyntax commonNameType)
            {
                return context.SemanticModel.GetTypeInfo(commonNameType).Type.IsValueType;
            }
            return false;
        }

        private static readonly string[] IdentifiedName = new string[] { "DateTime", "DateTimeOffset", "DateOnly", "TimeOnly", "Guid" };

        private static IEnumerable<Location> GetLocations(IEnumerable<PropertyDeclarationSyntax> props)
        {
            return props.Select(x => x.Identifier.GetLocation());
        }

        private static bool ExistModifier(SyntaxTokenList modifiers, SyntaxKind kind)
        {
            return modifiers.Any(x => x.IsKind(kind));
        }

        public static bool ExistAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
        {
            return attributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == attributeName);
        }

        public static bool ExistAttribute(SyntaxList<AttributeListSyntax> attributeLists, IEnumerable<string> attributeNames)
        {
            return attributeLists.SelectMany(x => x.Attributes).Any(x => attributeNames.Contains(x.Name.ToString()));
        }
    }
}