using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo
{
    public class DeepClassComparer : IEqualityComparer<ClassSimpleInfo>
    {
        public bool Equals(ClassSimpleInfo x, ClassSimpleInfo y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(ClassSimpleInfo obj)
        {
            return obj.GetHashCode();
        }
    }
    public readonly struct ClassSimpleInfo : IEqualityComparer<ClassSimpleInfo>
    {
        public string ClassName { get; }

        /// <summary>
        /// 泛型类型个数
        /// </summary>
        public int TypeParameterCount { get; }

        public ClassSimpleInfo(string className, int typeParameterCount)
        {
            ClassName = className;
            TypeParameterCount = typeParameterCount;
        }
        public readonly bool Equals(ClassSimpleInfo x, ClassSimpleInfo y)
        {
            if (x.ClassName != y.ClassName) return false;
            if (x.TypeParameterCount != y.TypeParameterCount) return false;
            return true;
        }

        public readonly int GetHashCode(ClassSimpleInfo obj)
        {
            return (obj.ClassName + obj.TypeParameterCount).GetHashCode();
        }
    }
}