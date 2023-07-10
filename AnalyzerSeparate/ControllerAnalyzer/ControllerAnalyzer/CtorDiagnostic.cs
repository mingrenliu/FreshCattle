using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControllerAnalyzer
{
    public class CtorDiagnostic
    {
        public const string DiagnosticId = "LY0013";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(CtorResources.AnalyzerTitle), CtorResources.ResourceManager, typeof(CtorResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(CtorResources.AnalyzerMessageFormat), CtorResources.ResourceManager, typeof(CtorResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(CtorResources.AnalyzerDescription), CtorResources.ResourceManager, typeof(CtorResources));
        private const string Category = "ControllerHint";

        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);
    }
}
