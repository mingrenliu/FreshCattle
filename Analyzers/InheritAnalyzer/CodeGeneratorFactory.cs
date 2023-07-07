using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace InheritAnalyzer
{
    public class CodeGeneratorFactory
    {
        public static SourceText GenerateCode(InheritInfo to, ClassInfo from)
        {
            var node = to.ClassNode;
            var result = CompilationUnit().WithUsings(List(node.Ancestors().OfType<UsingDirectiveSyntax>()));
            var scopeNamespace= node.Ancestors().OfType<UsingDirectiveSyntax>().FirstOrDefault();
            if (scopeNamespace != null)
            {
            }
            else
            {

            }
            return default;
            //to.ClassNode.SyntaxTree.GetRoot().us
        }
    }
}
