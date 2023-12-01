using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EFEntityAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EFEntityCodeFixProvider)), Shared]
    public class EFEntityCodeFixProvider : CodeFixProvider
    {
        static readonly List<string> NumberType = new() { "decimal", "float", "double" };
        const string StringLength = "StringLength";
        const string Precision = "Precision";
        const string Comment = "Comment";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(EFEntityAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var option = AttributeOption.ParseOption(context.Document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GlobalOptions);
            var diagnostic = context.Diagnostics.Last();
            var syntaxToken = (await diagnostic.Location.SourceTree.GetRootAsync()).FindToken(diagnostic.Location.SourceSpan.Start);
            var target = syntaxToken.Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            if (target == null || !diagnostic.AdditionalLocations.Any()) return;
            var haveClass = diagnostic.Location == diagnostic.AdditionalLocations.First();
            context.RegisterCodeFix(CodeAction.Create(CodeFixResources.CodeFixTitle,
                createChangedDocument: token => GenerateAttributesAsync(context.Document, option, target, haveClass, diagnostic.AdditionalLocations, token),
                equivalenceKey: CodeFixResources.CodeFixTitle),
                context.Diagnostics.Last());
        }

        public static async Task<Document> GenerateAttributesAsync(Document doc, AttributeOption option, ClassDeclarationSyntax target, bool containClass, IReadOnlyList<Location> locations, CancellationToken token)
        {
            var old = target;
            var props = locations.Skip(containClass ? 1 : 0);
            var root = await doc.GetSyntaxRootAsync(token);
            if (props.Any())
            {
                var nodes = new List<PropertyDeclarationSyntax>();
                foreach (var location in props)
                {
                    var node = root.FindToken(location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
                    if (node == null) continue;
                    nodes.Add(node);
                }
                if (nodes.Any())
                {
                    target = target.ReplaceNodes(nodes, (oldNode, newNode) => newNode.AddAttributeLists(CreateAttribute(oldNode.Type, oldNode, option)));
                }
            }
            target = containClass ? target.AddAttributeLists(AttributeFactory.CreateAttributeWithStringPara(Comment)) : target;
            return doc.WithSyntaxRoot(root.ReplaceNode(old, target).NormalizeWhitespace());
        }

        private static AttributeListSyntax[] CreateAttribute(TypeSyntax type, PropertyDeclarationSyntax prop, AttributeOption option)
        {
            if (type is NullableTypeSyntax nullType)
            {
                return CreateAttribute(nullType.ElementType, prop, option);
            }
            else if (type is QualifiedNameSyntax qualifiedNode)
            {
                return CreateAttribute(qualifiedNode.Right, prop, option);
            }
            var list = new List<AttributeListSyntax>();
            if (type is PredefinedTypeSyntax)
            {
                var str = type.ToString();
                if (str == "string")
                {
                    if (!SyntaxParseFactory.ExistAttribute(prop.AttributeLists, StringLength))
                    {
                        list.Add(AttributeFactory.CreateAttributeWithIntPara(StringLength, option.MaxLengthOfString));
                    }
                }
                else if (NumberType.Contains(str))
                {
                    if (!SyntaxParseFactory.ExistAttribute(prop.AttributeLists, Precision))
                    {
                        list.Add(AttributeFactory.CreateAttributeWithIntPara(Precision, option.PrecisionOfDecimal));
                    }
                }
            }
            else if (type is IdentifierNameSyntax nameType)
            {
                if (option.EnableDataTimePrecision)
                {
                    if (EFEntityAnalyzer.IdentifiedName.Contains(nameType.Identifier.ValueText))
                    {
                        list.Add(AttributeFactory.CreateAttributeWithIntPara(Precision, option.PrecisionOfDataTime));
                    }
                }
            }
            list.Add(AttributeFactory.CreateAttributeWithStringPara(Comment));
            return list.ToArray();
        }
    }
}