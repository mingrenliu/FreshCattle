using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace InheritAnalyzer
{
    public class InheritorInfo
    {
        /// <summary>
        /// 被继承类名
        /// </summary>
        public string InheritedClassName { get; set; }

        /// <summary>
        /// 递归继承
        /// </summary>
        public bool Recursion { get; set; }

        /// <summary>
        /// 继承类语法节点
        /// </summary>
        public ClassDeclarationSyntax InheritorNode { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public List<string> PropertyNames { get; set; }
    }
}