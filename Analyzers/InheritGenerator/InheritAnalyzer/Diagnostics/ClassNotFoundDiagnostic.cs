using InheritAnalyzer.Diagnostics;
using Microsoft.CodeAnalysis;

namespace ServiceAnalyzer.Diagnostics
{
    public class ClassNotFoundDiagnostic
    {
        public const string DiagnosticId = "LY0010";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ClassNotFoundResource.AnalyzerTitle), ClassNotFoundResource.ResourceManager, typeof(ClassNotFoundResource));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ClassNotFoundResource.AnalyzerMessageFormat), ClassNotFoundResource.ResourceManager, typeof(ClassNotFoundResource));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ClassNotFoundResource.AnalyzerDescription), ClassNotFoundResource.ResourceManager, typeof(ClassNotFoundResource));
        private const string Category = "Inherit";
        internal static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
    }
}