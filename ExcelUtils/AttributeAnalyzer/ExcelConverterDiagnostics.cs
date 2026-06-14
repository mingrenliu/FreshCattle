using Microsoft.CodeAnalysis;

namespace AttributeAnalyzer
{
    /// <summary>ExcelConverter 类型不匹配诊断。</summary>
    internal static class ExcelConverterTypeMismatchDiagnostic
    {
        public const string Id = "LY0011";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: Id,
            title: "ExcelConverter type mismatch",
            messageFormat: "ExcelConverter<{0}> cannot be applied to property of type '{1}'",
            category: "ExcelConverter",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The generic type argument of ExcelConverter should match the property type.");
    }

    /// <summary>ExcelConverter 类型无效诊断（未继承 ExcelConverter）。</summary>
    internal static class ExcelConverterNotConverterDiagnostic
    {
        public const string Id = "LY0013";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: Id,
            title: "Invalid ExcelConverter type",
            messageFormat: "Type '{0}' does not inherit from ExcelConverter",
            category: "ExcelConverter",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The type passed to [ExcelConverter] must inherit from ExcelConverter.");
    }
}
