using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AttributeAnalyzer
{
    internal readonly struct ColumnInfo
    {
        public readonly string ColumnName;
        public readonly string PropName;
        public readonly Location Location;
        public ColumnInfo(string columnName, string propName, Location location)
        { ColumnName = columnName; PropName = propName; Location = location; }
    }

    internal sealed class ClassColumnInfos
    {
        public readonly string ClassName;
        public readonly ImmutableArray<ColumnInfo> Columns;
        public ClassColumnInfos(string className, ImmutableArray<ColumnInfo> columns)
        { ClassName = className; Columns = columns; }
    }

    /// <summary>
    /// 检测同一 class 内 Excel 列名（包含 [ExcelColumn] 指定名称与属性名）是否重复。
    /// 参考 InheritGenerator 的管道模式：先收集 class 节点并去重，再逐个 class 分析属性。
    /// </summary>
    [Generator]
    public class ExcelColumnGenerator : IIncrementalGenerator
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: "LY0012",
            title: "Duplicate Excel column name",
            messageFormat: "Property '{0}' has duplicate Excel column name '{1}' within class '{2}'",
            category: "ExcelColumn",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Each [ExcelColumn] Name must be unique within a class. Unattributed properties use their own name.");

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 步骤1：收集所有包含 [ExcelColumn] 属性的 class，并提取每个 class 的列信息
            var classInfos = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (node, _) => node is ClassDeclarationSyntax c
                        && c.Members.OfType<PropertyDeclarationSyntax>().Any(p =>
                            p.AttributeLists.Count > 0
                            && p.AttributeLists.Any(al => al.Attributes.Any(a => IsExcelColumn(a)))),
                    transform: (ctx, _) => GetClassColumnInfos(ctx.Node, ctx.SemanticModel))
                .Where(x => x != null)
                .Collect();

            // 步骤2：分析每个 class 的列名是否有重复，有则 report diagnostic
            context.RegisterSourceOutput(classInfos, (spc, infos) =>
            {
                foreach (var classInfo in infos)
                {
                    if (classInfo == null) continue;
                    var seen = new Dictionary<string, ColumnInfo>();
                    foreach (var col in classInfo.Columns)
                    {
                        if (seen.TryGetValue(col.ColumnName, out var first))
                            spc.ReportDiagnostic(Diagnostic.Create(Rule,
                                col.Location ?? first.Location,
                                col.PropName, col.ColumnName, classInfo.ClassName));
                        else
                            seen[col.ColumnName] = col;
                    }
                }
            });
        }

        /// <summary>
        /// 从 class 节点获取该 class 内所有属性的列名信息。
        /// 有 [ExcelColumn] 使用其 Name，没有则使用属性名。
        /// </summary>
        private static ClassColumnInfos GetClassColumnInfos(SyntaxNode node, SemanticModel semantic)
        {
            if (!(node is ClassDeclarationSyntax classNode)) return null;
            var symbol = semantic.GetDeclaredSymbol(classNode);
            if (symbol == null) return null;

            var className = symbol.Name;
            var columns = ImmutableArray.CreateBuilder<ColumnInfo>();

            foreach (var member in symbol.GetMembers().OfType<IPropertySymbol>())
            {
                // 跳过静态属性、索引器、重写属性
                if (member.IsStatic || member.IsIndexer || member.IsOverride) continue;

                var attr = member.GetAttributes().FirstOrDefault(a =>
                    a.AttributeClass != null && a.AttributeClass.Name == "ExcelColumnAttribute");
                string columnName;

                if (attr != null)
                {
                    var nameArg = attr.NamedArguments.FirstOrDefault(na => na.Key == "Name");
                    columnName = nameArg.Value.Value as string;
                    if (string.IsNullOrWhiteSpace(columnName))
                        columnName = member.Name;
                }
                else
                {
                    // 没有 [ExcelColumn]，直接使用属性名
                    columnName = member.Name;
                }

                var location = member.Locations.FirstOrDefault();
                columns.Add(new ColumnInfo(columnName, member.Name, location));
            }

            if (columns.Count == 0) return null;
            return new ClassColumnInfos(className, columns.ToImmutable());
        }

        private static bool IsExcelColumn(AttributeSyntax attr)
        {
            var name = attr.Name.ToString();
            return name == "ExcelColumn" || name == "ExcelColumnAttribute";
        }
    }
}
