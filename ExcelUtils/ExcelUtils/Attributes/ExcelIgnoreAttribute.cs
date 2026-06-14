namespace ExcelUtile;

/// <summary>
/// 标识属性在 Excel 导入导出中被忽略。
/// 仅在 AutoInclude=true 时生效（opt-out 模式）。
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ExcelIgnoreAttribute : Attribute
{
}
