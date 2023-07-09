using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ControllerAnalyzer;

public class MethodUsageAnalyzer
{
    public const string Source = @"namespace InheritTestLib
{
    public class TestController
    {
        private readonly ITestService _testService;
        public void Test1(string name)
        {
            _testService.Display(name);
        }
    }
    public interface ITestService
    {
        void Display1(string message);
        void Display(string message);
    }
}

";
    public static void AnalyzeClass(string classCode)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        var root = (CompilationUnitSyntax)syntaxTree.GetRoot();

        var semanticModel = GetSemanticModel(root);
        var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

        var fieldDeclarations = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>();
        foreach (var fieldDeclaration in fieldDeclarations)
        {
            var type = fieldDeclaration.Declaration.Type;
            var typeName = type.ToString();
            var symbol = semanticModel.GetSymbolInfo(type).Symbol as ITypeSymbol;

            if (symbol != null && symbol.TypeKind == TypeKind.Interface)
            {
                var interfaceMethods = symbol.GetMembers().OfType<IMethodSymbol>();
                var usedMethods = new List<IMethodSymbol>();

                var methodInvocations = classDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in methodInvocations)
                {
                    var methodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                    if (methodSymbol != null && interfaceMethods.Any(X=>X.Equals(methodSymbol)))
                    {
                        usedMethods.Add(methodSymbol);
                    }
                }

                Console.WriteLine($"Interface '{typeName}' methods used:");
                foreach (var method in usedMethods)
                {
                    Console.WriteLine($"- {method.Name}");
                }
            }
        }
    }

    private static SemanticModel GetSemanticModel(SyntaxNode root)
    {
        var compilation = CSharpCompilation.Create("TempCompilation")
                                           .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                                           .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        compilation = compilation.AddSyntaxTrees(root.SyntaxTree);
        return compilation.GetSemanticModel(root.SyntaxTree);
    }
}
