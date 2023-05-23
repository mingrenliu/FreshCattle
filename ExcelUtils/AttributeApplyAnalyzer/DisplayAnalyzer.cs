using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;

namespace AttributeApplyAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisplayAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "LY0002";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DisplayResources.AnalyzerTitle), DisplayResources.ResourceManager, typeof(DisplayResources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DisplayResources.AnalyzerMessageFormat), DisplayResources.ResourceManager, typeof(DisplayResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DisplayResources.AnalyzerDescription), DisplayResources.ResourceManager, typeof(DisplayResources));
        private const string Category = "AttributeApply";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<挂起>")]
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        { get { return ImmutableArray.Create(Rule); } }

        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> displayInfo = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeDisplayAttributeSymbol, SymbolKind.Property);
        }

        private static bool CheckDisplayName(string className, string displayName, string propName)
        {
            var lst = displayInfo.GetOrAdd(className, new ConcurrentDictionary<string, string>());
            var prop = lst.GetOrAdd(displayName, propName);
            return prop != propName;
        }

        private static void AnalyzeDisplayAttributeSymbol(SymbolAnalysisContext context)
        {
            if (context.Symbol is IPropertySymbol prop)
            {
                AttributeData attribute = prop.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == "DisplayAttribute" && x.AttributeClass.ContainingAssembly.Name == "ExcelUtile");
                if (attribute != null)
                {
                    var className = prop.ContainingType.ContainingNamespace.Name + "." + prop.ContainingType.Name;
                    string displayName = (string)attribute.ConstructorArguments.First().Value;
                    if (CheckDisplayName(className, displayName, prop.Name))
                    {
                        var attributeNode = attribute.ApplicationSyntaxReference.GetSyntax();
                        var diagnostic = Diagnostic.Create(Rule, attributeNode.GetLocation(), prop.Name, displayName);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}