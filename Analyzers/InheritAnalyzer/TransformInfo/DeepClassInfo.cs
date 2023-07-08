using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo
{
    public class DeepClassComparer : IEqualityComparer<DeepClassInfo>
    {
        public bool Equals(DeepClassInfo x, DeepClassInfo y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(DeepClassInfo obj)
        {
            return obj.GetHashCode();
        }
    }
    public struct DeepClassInfo : IEqualityComparer<DeepClassInfo>
    {
        public string ClassName { get; }

        /// <summary>
        /// 泛型类型个数
        /// </summary>
        public int TypeParameterCount { get; }

        public DeepClassInfo(string className, int typeParameterCount)
        {
            ClassName = className;
            TypeParameterCount = typeParameterCount;
        }
        public bool Equals(DeepClassInfo x, DeepClassInfo y)
        {
            if (x.ClassName != y.ClassName) return false;
            if (x.TypeParameterCount != y.TypeParameterCount) return false;
            return true;
        }

        public int GetHashCode(DeepClassInfo obj)
        {
            return (obj.ClassName + obj.TypeParameterCount).GetHashCode();
        }
    }
}