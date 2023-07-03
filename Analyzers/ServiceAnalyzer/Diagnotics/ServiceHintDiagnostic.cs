using Microsoft.CodeAnalysis;
using ServiceAnalyzer.Diagnotics;

namespace ServiceAnalyzer.Diagnostics
{
    public class ServiceHintDiagnostic
    {
        public const string DiagnosticId = "LY0011";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ServiceHintResource.AnalyzerTitle), ServiceHintResource.ResourceManager, typeof(ServiceHintResource));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ServiceHintResource.AnalyzerMessageFormat), ServiceHintResource.ResourceManager, typeof(ServiceHintResource));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ServiceHintResource.AnalyzerDescription), ServiceHintResource.ResourceManager, typeof(ServiceHintResource));
        private const string Category = "ServiceHint";
        internal static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);
    }
}