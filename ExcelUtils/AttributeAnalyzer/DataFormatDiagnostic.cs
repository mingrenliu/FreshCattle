using Microsoft.CodeAnalysis;

namespace AttributeAnalyzer
{
    public class DataFormatDiagnostic
    {
        public const string DiagnosticId = "LY0001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DataFormatResources.AnalyzerTitle), DataFormatResources.ResourceManager, typeof(DataFormatResources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DataFormatResources.AnalyzerMessageFormat), DataFormatResources.ResourceManager, typeof(DataFormatResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DataFormatResources.AnalyzerDescription), DataFormatResources.ResourceManager, typeof(DataFormatResources));
        private const string Category = "AttributeApply";
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
    }
    public class DataFormatTypeDiagnostic
    {
        public const string DiagnosticId = "LY0003";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DataFormatTypeResources.AnalyzerTitle), DataFormatTypeResources.ResourceManager, typeof(DataFormatTypeResources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DataFormatTypeResources.AnalyzerMessageFormat), DataFormatTypeResources.ResourceManager, typeof(DataFormatTypeResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DataFormatTypeResources.AnalyzerDescription), DataFormatTypeResources.ResourceManager, typeof(DataFormatTypeResources));
        private const string Category = "AttributeApply";
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
    }
}