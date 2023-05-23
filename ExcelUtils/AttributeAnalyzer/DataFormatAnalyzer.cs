using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AttributeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DataFormatAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DataFormatDiagnostic.Rule, DataFormatTypeDiagnostic.Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(CheckApplyType, SymbolKind.Property);
        }

        private static void CheckApplyType(SymbolAnalysisContext context)
        {
            if (context.Symbol is IPropertySymbol prop)
            {
                AttributeData attribute = prop.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == "DataFormatAttribute" && x.AttributeClass.ContainingAssembly.Name == "ExcelUtile");
                if (attribute != null)
                {
                    var value = attribute.ConstructorArguments.First().Value;
                    var typeName = (value is TypedConstant secondType) ? secondType.Value.ToString() : value.ToString();
                    var info = context.Compilation.GetTypeByMetadataName(typeName);
                    while (info != null && !info.IsGenericType)
                    {
                        info = info.BaseType;
                    }
                    var attributeNode = attribute.ApplicationSyntaxReference.GetSyntax();
                    if (info != null)
                    {
                        var declareType = (INamedTypeSymbol)prop.Type;
                        var baseTypeName = declareType.IsGenericType && declareType.Name == "Nullable" ? declareType.TypeArguments.First().Name : prop.Type.Name;
                        var argTypeName = info.TypeArguments.First().Name;
                        if (baseTypeName != argTypeName)
                        {
                            var diagnostic = Diagnostic.Create(DataFormatDiagnostic.Rule, attributeNode.GetLocation(), argTypeName, baseTypeName);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                    else
                    {
                        var diagnostic = Diagnostic.Create(DataFormatTypeDiagnostic.Rule, attributeNode.GetLocation(), typeName);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}