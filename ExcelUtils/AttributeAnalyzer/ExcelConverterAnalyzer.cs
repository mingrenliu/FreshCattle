using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AttributeAnalyzer
{
    /// <summary>
    /// 检测 [ExcelConverter(typeof(T))] 中 T 是否继承 ExcelConverter，以及泛型参数是否与属性类型匹配。
    /// 参考 DataFormatAnalyzer 的模式：检查 ContainingAssembly、使用 GetTypeByMetadataName 解析类型。
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExcelConverterAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                ExcelConverterTypeMismatchDiagnostic.Rule,
                ExcelConverterNotConverterDiagnostic.Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(CheckApplyType, SymbolKind.Property);
        }

        private static void CheckApplyType(SymbolAnalysisContext context)
        {
            if (!(context.Symbol is IPropertySymbol prop)) return;

            var attribute = prop.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass != null
                && x.AttributeClass.Name == "ExcelConverterAttribute");

            if (attribute == null) return;

            var ctorArg = attribute.ConstructorArguments.FirstOrDefault();
            var value = ctorArg.Value;

            // 解析转换器类型：优先通过 metadata name，兼容直接获得的 INamedTypeSymbol
            INamedTypeSymbol converterType = null;
            string typeName;

            if (value is INamedTypeSymbol namedType)
            {
                converterType = namedType;
                typeName = namedType.Name;
            }
            else
            {
                typeName = value?.ToString();
                if (!string.IsNullOrEmpty(typeName))
                    converterType = context.Compilation.GetTypeByMetadataName(typeName);
            }

            var attributeNode = attribute.ApplicationSyntaxReference?.GetSyntax();
            var location = attributeNode?.GetLocation();
            if (location == null) return;

            if (converterType == null)
            {
                // 无法解析类型 → 报告无效转换器
                context.ReportDiagnostic(Diagnostic.Create(
                    ExcelConverterNotConverterDiagnostic.Rule, location, typeName ?? "?"));
                return;
            }

            // 查找 ExcelConverter<T> 基类
            var excelConverterBase = FindExcelConverterBase(converterType);
            if (excelConverterBase == null || excelConverterBase.TypeArguments.Length != 1)
            {
                // 未继承 ExcelConverter<T> → 报告无效转换器
                context.ReportDiagnostic(Diagnostic.Create(
                    ExcelConverterNotConverterDiagnostic.Rule, location, converterType.Name));
                return;
            }

            var converterValueType = excelConverterBase.TypeArguments[0];

            // 处理属性类型（含 Nullable<T>）
            if (!(prop.Type is INamedTypeSymbol propNamedType)) return;

            var propType = propNamedType;
            if (propType.IsGenericType && propType.Name == "Nullable")
                propType = propType.TypeArguments[0] as INamedTypeSymbol ?? propType;

            // 类型不匹配 → 报告 LY0011
            if (!SymbolEqualityComparer.Default.Equals(propType, converterValueType))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    ExcelConverterTypeMismatchDiagnostic.Rule, location,
                    converterValueType.Name, propType.Name));
            }
        }

        /// <summary>
        /// 沿继承链查找 ExcelConverter&lt;&gt; 泛型基类。
        /// </summary>
        private static INamedTypeSymbol FindExcelConverterBase(INamedTypeSymbol type)
        {
            var current = type.BaseType;
            while (current != null)
            {
                if (current.Name == "ExcelConverter" && current.IsGenericType)
                    return current;
                current = current.BaseType;
            }
            return null;
        }
    }
}
