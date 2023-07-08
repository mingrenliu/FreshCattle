using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo
{
    public class InheritInfo : HashSet<string>, IEqualityComparer<InheritInfo>
    {
        /// <summary>
        /// 当前类名
        /// </summary>
        public string CurrentClassName { get; }

        /// <summary>
        /// 是否为深度继承
        /// </summary>
        public bool IsDeepInherit { get; }

        /// <summary>
        /// 被继承的类名
        /// </summary>
        public string InheritedClassName { get; }

        /// <summary>
        /// 类节点
        /// </summary>
        public ClassDeclarationSyntax ClassNode { get; }

        public AttributeSyntax AttributeNode { get; }

        public InheritInfo(string currentClassName, string inheritedClassName, ClassDeclarationSyntax classNode, AttributeSyntax attributeNode, bool isDeepInherit=false)
        {
            CurrentClassName = currentClassName;
            InheritedClassName = inheritedClassName;
            ClassNode = classNode;
            AttributeNode = attributeNode;
            IsDeepInherit = isDeepInherit;
        }

        public bool Equals(InheritInfo x, InheritInfo y)
        {
            if (x.CurrentClassName != y.CurrentClassName) return false;
            if (x.InheritedClassName != y.InheritedClassName) return false;
            if (x.Count != y.Count) return false;
            foreach (var item in x)
            {
                if (!y.Contains(item)) return false;
            }
            return true;
        }

        public int GetHashCode(InheritInfo obj)
        {
            return (obj.CurrentClassName + obj.InheritedClassName).GetHashCode();
        }
    }
}