using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo;

public class ClassInfo : Dictionary<string, PropertyInfo>, IEqualityComparer<ClassInfo>
{
    public string ClassName { get; }

    public ClassInfo(string className)
    {
        ClassName = className;
    }

    public bool Equals(ClassInfo x, ClassInfo y)
    {
        if (x.ClassName != ClassName) return false;
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