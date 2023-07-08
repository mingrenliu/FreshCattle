using System;
using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo;
public class ClassInfo : Dictionary<string, PropertyInfo>,IEqualityComparer<ClassInfo>
{
    /// <summary>
    /// 类名
    /// </summary>
    public string ClassName { get; }
    /// <summary>
    /// 泛型类型个数
    /// </summary>
    public int TypeParameterCount { get; }

    public ClassInfo(string className, int typeParameterCount=0)
    {
        ClassName = className;
        TypeParameterCount = typeParameterCount;
    }
    public bool Equals(ClassInfo x, ClassInfo y)
    {
        if (x.ClassName != y.ClassName) return false;
        if (x.TypeParameterCount != y.TypeParameterCount) return false;
        if (x.Keys.Count != y.Keys.Count) return false;
        foreach (var key in x.Keys)
        {
            if (!y.ContainsKey(key)) return false;
            if (x[key] != y[key]) return false;
        }
        return true;
    }

    public int GetHashCode(ClassInfo obj)
    {
        return obj.ClassName.GetHashCode();
    }
}