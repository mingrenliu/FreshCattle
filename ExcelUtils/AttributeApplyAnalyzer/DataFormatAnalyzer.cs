using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AttributeApplyAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DataFormatAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "LY0001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DataFormatResources.AnalyzerTitle), DataFormatResources.ResourceManager, typeof(DataFormatResources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DataFormatResources.AnalyzerMessageFormat), DataFormatResources.ResourceManager, typeof(DataFormatResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DataFormatResources.AnalyzerDescription), DataFormatResources.ResourceManager, typeof(DataFormatResources));
        private const string Category = "AttributeApply";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<挂起>")]
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(CheckApplyType, SymbolKind.Property);
        }

        private void CheckApplyType(SymbolAnalysisContext context)
        {
            if (context.Symbol is IPropertySymbol prop)
            {
                ImmutableArray<AttributeData> attributes = prop.GetAttributes();
                AttributeData attribute = prop.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == "DataFormatAttribute" && x.AttributeClass.ContainingAssembly.Name == "ExcelUtile");
                if (attribute != null)
                {
                    var info = context.Compilation.GetTypeByMetadataName(attribute.ConstructorArguments.First().Value.ToString());
                    while (info != null && !info.IsGenericType)
                    {
                        info = info.BaseType;
                    }
                    if (info != null)
                    {
                        var originalTypeName = prop.Type.Name;
                        var declareType = (INamedTypeSymbol)prop.Type;
                        var baseTypeName = declareType.IsGenericType && declareType.Name == "Nullable" ? declareType.TypeArguments.First().Name : null;
                        var argTypeName = info.TypeArguments.First().Name;
                        if (argTypeName != originalTypeName && baseTypeName != argTypeName)
                        {
                            var attributeNode = attribute.ApplicationSyntaxReference.GetSyntax();
                            var diagnostic = Diagnostic.Create(Rule, attributeNode.GetLocation(), argTypeName, baseTypeName ?? originalTypeName);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }
}