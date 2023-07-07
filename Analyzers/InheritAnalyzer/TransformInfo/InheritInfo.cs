using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo
{
    public class InheritInfo : HashSet<string>, IEqualityComparer<InheritInfo>
    {
        /// <summary>
        /// 当前类名
        /// </summary>
        public string CurrentClassName { get; set; }

        /// <summary>
        /// 被继承的类名
        /// </summary>
        public string InheritedClassName { get; set; }

        /// <summary>
        /// 类节点
        /// </summary>
        public ClassDeclarationSyntax ClassNode { get; set; }
        public AttributeSyntax AttributeNode { get; set; }

        public InheritInfo(string currentClassName, string inheritedClassName, ClassDeclarationSyntax classNode, AttributeSyntax attributeNode)
        {
            CurrentClassName = currentClassName;
            InheritedClassName = inheritedClassName;
            ClassNode = classNode;
            AttributeNode = attributeNode;
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