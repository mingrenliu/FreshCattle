using Microsoft.CodeAnalysis;

namespace ControllerAnalyzer.Diagnostics
{
    public class ControllerHintDiagnostic
    {
        public const string DiagnosticId = "LY0012";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ControllerHintResource.AnalyzerTitle), ControllerHintResource.ResourceManager, typeof(ControllerHintResource));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ControllerHintResource.AnalyzerMessageFormat), ControllerHintResource.ResourceManager, typeof(ControllerHintResource));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ControllerHintResource.AnalyzerDescription), ControllerHintResource.ResourceManager, typeof(ControllerHintResource));
        private const string Category = "ControllerHint";
        public static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);
    }
}