using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace InheritAnalyzer.TransformInfo
{
    public class ClassInfo
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 类的语法节点
        /// </summary>
        public ClassDeclarationSyntax InheritorNode { get; set; }
        /// <summary>
        /// 忽略的属性
        /// </summary>
        public List<string> IgnoreProperties { get; set; }
        /// <summary>
        /// 自定义类型属性
        /// </summary>
        public Dictionary<string, ITypeSymbol> ComplexProperties { get; set; }
    }
}
