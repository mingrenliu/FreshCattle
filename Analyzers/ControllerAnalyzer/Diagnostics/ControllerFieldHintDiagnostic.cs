using Microsoft.CodeAnalysis;

namespace ControllerAnalyzer.Diagnostics
{
    public class ControllerFieldHintDiagnostic
    {
        public const string DiagnosticId = "LY0013";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ControllerFieldHintResource.AnalyzerTitle), ControllerFieldHintResource.ResourceManager, typeof(ControllerFieldHintResource));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ControllerFieldHintResource.AnalyzerMessageFormat), ControllerFieldHintResource.ResourceManager, typeof(ControllerFieldHintResource));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ControllerFieldHintResource.AnalyzerDescription), ControllerFieldHintResource.ResourceManager, typeof(ControllerFieldHintResource));
        private const string Category = "ControllerHint";
        public static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);
    }
}