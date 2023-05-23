using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AttributeAnalyzer
{
    public class DisplayDiagnostic
    {
        public const string DiagnosticId = "LY0002";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DisplayResources.AnalyzerTitle), DisplayResources.ResourceManager, typeof(DisplayResources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DisplayResources.AnalyzerMessageFormat), DisplayResources.ResourceManager, typeof(DisplayResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DisplayResources.AnalyzerDescription), DisplayResources.ResourceManager, typeof(DisplayResources));
        private const string Category = "AttributeApply";
        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
    }
}
