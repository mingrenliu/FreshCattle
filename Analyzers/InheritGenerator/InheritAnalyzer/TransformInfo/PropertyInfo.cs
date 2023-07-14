using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo;

/// <summary>
/// 属性信息
/// </summary>
public class PropertyInfo : IEqualityComparer<PropertyInfo>
{
    /// <summary>
    /// 被继承类名
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 属性名称
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// 属性类型
    /// </summary>
    public string PredefinedType { get; set; }

    /// <summary>
    /// 属性节点
    /// </summary>
    public PropertyDeclarationSyntax PropertyNode { get; set; }

    public PropertyInfo(string className, string propertyName, string predefinedType, PropertyDeclarationSyntax node)
    {
        ClassName = className;
        PropertyName = propertyName;
        PredefinedType = predefinedType;
        PropertyNode = node;
    }

    public bool Equals(PropertyInfo x, PropertyInfo y)
    {
        if (x.ClassName != y.ClassName) return false;
        if (x.PropertyName != y.PropertyName) return false;
        if (x.PredefinedType != y.PredefinedType) return false;
        return true;
    }

    public int GetHashCode(PropertyInfo obj)
    {
        return (obj.ClassName + obj.PropertyName + obj.PredefinedType).GetHashCode();
    }
}