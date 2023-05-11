using System.Reflection;

namespace ExcelUtils.ExcelCore;

//待定：后面根据功能再重构
internal class ExcelSerializerOptions
{
    private Func<PropertyInfo, bool>? FieldFilter;
}