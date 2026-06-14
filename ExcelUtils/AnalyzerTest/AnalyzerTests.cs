using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace AnalyzerTest;

/// <summary>
/// 分析器 / 增量生成器测试。
/// 注意：属性类型内联定义，不依赖 ExcelUtile.dll，避免 net6.0/net10.0 程序集版本问题。
/// </summary>
[TestFixture]
public class AnalyzerTests
{
    /// <summary>内联的属性定义，在每个测试源码前拼接。</summary>
    private const string Attributes = @"
using System;

namespace ExcelUtile
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnAttribute : Attribute
    {
        public string? Name { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public bool Required { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelConverterAttribute : Attribute
    {
        public Type ConverterType { get; }
        public ExcelConverterAttribute(Type converterType) { ConverterType = converterType; }
    }
}

namespace ExcelUtile.Converters
{
    public abstract class ExcelConverter
    {
        public abstract Type Type { get; }
    }

    public abstract class ExcelConverter<T> : ExcelConverter
    {
        public override Type Type => typeof(T);
    }

    public class BooleanConverter : ExcelConverter<bool> { }
    public class Int32Converter : ExcelConverter<int> { }
    public class StringConverter : ExcelConverter<string> { }
    public class DoubleConverter : ExcelConverter<double> { }
    public class DateTimeConverter : ExcelConverter<DateTime> { }
    public class ObjectConverter : ExcelConverter<object> { }
    public class CustomBoolConverter : ExcelConverter<bool> { }
}
";

    // ----------------------------------------------------------------
    //  LY0012 — ExcelColumnGenerator：重复列名
    // ----------------------------------------------------------------

    [Test]
    public void DuplicateColumn_ReportsError()
    {
        var code = Attributes + @"
public class T
{
    [ExcelColumn(Name = ""A"")] public string X { get; set; }
    [ExcelColumn(Name = ""A"")] public string Y { get; set; }
}";
        var diags = RunGenerator(code);
        Assert.That(diags.Any(d => d.Id == "LY0012"), Is.True);
    }

    [Test]
    public void UniqueColumn_NoError()
    {
        var code = Attributes + @"
public class T
{
    [ExcelColumn(Name = ""A"")] public string X { get; set; }
    [ExcelColumn(Name = ""B"")] public int Y { get; set; }
}";
        var diags = RunGenerator(code);
        Assert.That(diags.Any(d => d.Id == "LY0012"), Is.False);
    }

    [Test]
    public void NoColumn_NoError()
    {
        var code = Attributes + @"
public class T
{
    public string X { get; set; }
    public int Y { get; set; }
}";
        var diags = RunGenerator(code);
        Assert.That(diags.Any(d => d.Id == "LY0012"), Is.False);
    }

    [Test]
    public void Success_ImplicitName_NoImport()
    {
        var code = Attributes + @"
public class T
{
    [ExcelColumn] public string X { get; set; }
    [ExcelColumn(Name = ""X"")] public string Y { get; set; }
}";
        var diags = RunGenerator(code);
        Assert.That(diags.Any(d => d.Id == "LY0012"), Is.False);
    }

    [Test]
    public void Duplicate_ThreeExplicit_TwoErrors()
    {
        var diags = RunGenerator(Attributes + @"
public class T {
    [ExcelColumn(Name = ""A"")] public string X { get; set; }
    [ExcelColumn(Name = ""A"")] public int Y { get; set; }
    [ExcelColumn(Name = ""A"")] public double Z { get; set; }
}");
        Assert.That(diags.Count(d => d.Id == "LY0012"), Is.EqualTo(2));
    }

    [Test]
    public void Duplicate_DifferentClasses_NoError()
    {
        var diags = RunGenerator(Attributes + @"
public class T1 { [ExcelColumn(Name = ""A"")] public string X { get; set; } }
public class T2 { [ExcelColumn(Name = ""A"")] public string Y { get; set; } }
");
        Assert.That(diags.Any(d => d.Id == "LY0012"), Is.False);
    }

    [Test]
    public void Duplicate_CaseSensitive_Different()
    {
        var diags = RunGenerator(Attributes + @"
public class T {
    [ExcelColumn(Name = ""A"")] public string X { get; set; }
    [ExcelColumn(Name = ""a"")] public string Y { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0012"), Is.False);
    }

    [Test]
    public void Duplicate_EmptyNameFallsBackToPropName()
    {
        var diags = RunGenerator(Attributes + @"
public class T {
    [ExcelColumn(Name = """")] public string X { get; set; }
    [ExcelColumn(Name = """")] public string Y { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0012"), Is.False);
    }

    [Test]
    public void Duplicate_MultipleDuplicatesInClass()
    {
        var diags = RunGenerator(Attributes + @"
public class T {
    [ExcelColumn(Name = ""A"")] public string P1 { get; set; }
    [ExcelColumn(Name = ""A"")] public string P2 { get; set; }
    [ExcelColumn(Name = ""B"")] public string P3 { get; set; }
    [ExcelColumn(Name = ""B"")] public string P4 { get; set; }
}");
        Assert.That(diags.Count(d => d.Id == "LY0012"), Is.EqualTo(2));
    }

    // ----------------------------------------------------------------
    //  LY0011 / LY0013 — ExcelConverterAnalyzer：转换器类型
    // ----------------------------------------------------------------

    [Test]
    public async Task ConverterTypeMismatch_Error()
    {
        var code = Attributes + @"
public class T
{
    [ExcelColumn]
    [ExcelConverter(typeof(BooleanConverter))]
    public int X { get; set; }
}";
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(code);
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.True);
    }

    [Test]
    public async Task ConverterTypeMatch_NoError()
    {
        var code = Attributes + @"
public class T
{
    [ExcelColumn]
    [ExcelConverter(typeof(Int32Converter))]
    public int X { get; set; }
}";
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(code);
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.False);
    }

    [Test]
    public async Task InvalidConverter_Error()
    {
        var code = Attributes + @"
public class T
{
    [ExcelColumn]
    [ExcelConverter(typeof(string))]
    public string X { get; set; }
}";
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(code);
        Assert.That(diags.Any(d => d.Id == "LY0013"), Is.True);
    }

    // -- 更多 LY0011 类型匹配测试 --

    [Test]
    public async Task Converter_BoolOnBool_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(BooleanConverter))] public bool X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.False);
    }

    [Test]
    public async Task Converter_IntOnString_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(Int32Converter))] public string X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.True);
    }

    [Test]
    public async Task Converter_StringOnInt_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(StringConverter))] public int X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.True);
    }

    [Test]
    public async Task Converter_DoubleOnDouble_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(DoubleConverter))] public double X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.False);
    }

    [Test]
    public async Task Converter_DoubleOnBool_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(DoubleConverter))] public bool X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.True);
    }

    [Test]
    public async Task Converter_DateTimeOnDateTime_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(DateTimeConverter))] public DateTime X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.False);
    }

    // -- 更多 LY0013 非法转换器测试 --

    [Test]
    public async Task Converter_IntAsConverter_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(int))] public int X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0013"), Is.True);
    }

    [Test]
    public async Task Converter_ObjectAsConverter_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(object))] public object X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0013"), Is.True);
    }

    [Test]
    public async Task Converter_AbstractBaseAsConverter_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(ExcelConverter))] public string X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0013"), Is.True);
    }

    [Test]
    public async Task Converter_NoAttribute_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelColumn] public int X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011" || d.Id == "LY0013"), Is.False);
    }

    // -- 边界场景：Nullable --

    [Test]
    public async Task Converter_NullableIntWithInt32_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(Int32Converter))] public int? X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.False);
    }

    [Test]
    public async Task Converter_NullableBoolWithInt_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(Int32Converter))] public bool? X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.True);
    }

    // -- 自定义转换器 --

    [Test]
    public async Task Converter_CustomOnBool_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(CustomBoolConverter))] public bool X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.False);
    }

    [Test]
    public async Task Converter_CustomOnInt_Error()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(CustomBoolConverter))] public int X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0011"), Is.True);
    }

    // -- 多属性混合 --

    [Test]
    public async Task Converter_MultipleMismatchesInClass()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelConverterAnalyzer>(Attributes + @"
public class T {
    [ExcelConverter(typeof(BooleanConverter))] public int P1 { get; set; }
    [ExcelConverter(typeof(Int32Converter))] public string P2 { get; set; }
    [ExcelConverter(typeof(string))] public string P3 { get; set; }
}");
        Assert.That(diags.Count(d => d.Id == "LY0011"), Is.EqualTo(2));
        Assert.That(diags.Count(d => d.Id == "LY0013"), Is.EqualTo(1));
    }

    // ----------------------------------------------------------------
    //  LY0014 — ExcelColumnIgnoreConflictAnalyzer：[ExcelColumn] + [ExcelIgnore] 冲突
    // ----------------------------------------------------------------

    [Test]
    public async Task ColumnWithIgnore_Error()
    {
        var code = Attributes + @"
public class T
{
    [ExcelColumn]
    [ExcelIgnore]
    public string X { get; set; }
}";
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelColumnIgnoreConflictAnalyzer>(code);
        Assert.That(diags.Any(d => d.Id == "LY0014"), Is.True);
    }

    [Test]
    public async Task IgnoreWithoutColumn_NoError()
    {
        var code = Attributes + @"
public class T
{
    [ExcelIgnore]
    public string X { get; set; }
}";
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelColumnIgnoreConflictAnalyzer>(code);
        Assert.That(diags.Any(d => d.Id == "LY0014"), Is.False);
    }

    [Test]
    public async Task Conflict_ColumnOnly_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelColumnIgnoreConflictAnalyzer>(Attributes + @"
public class T {
    [ExcelColumn] public string X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0014"), Is.False);
    }

    [Test]
    public async Task Conflict_NoAttribute_NoError()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelColumnIgnoreConflictAnalyzer>(Attributes + @"
public class T {
    public string X { get; set; }
}");
        Assert.That(diags.Any(d => d.Id == "LY0014"), Is.False);
    }

    [Test]
    public async Task Conflict_MixedProperties_OnlyConflicting()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelColumnIgnoreConflictAnalyzer>(Attributes + @"
public class T {
    [ExcelColumn] public string A { get; set; }
    [ExcelIgnore] public string B { get; set; }
    [ExcelColumn] [ExcelIgnore] public string C { get; set; }
    public string D { get; set; }
}");
        var errs = diags.Where(d => d.Id == "LY0014").ToList();
        Assert.That(errs.Count, Is.EqualTo(1));
        Assert.That(errs[0].GetMessage(), Does.Contain("C"));
    }

    [Test]
    public async Task Conflict_MultipleConflictsInClass()
    {
        var diags = await RunAnalyzerAsync<AttributeAnalyzer.ExcelColumnIgnoreConflictAnalyzer>(Attributes + @"
public class T {
    [ExcelColumn] [ExcelIgnore] public string A { get; set; }
    [ExcelColumn] [ExcelIgnore] public string B { get; set; }
}");
        Assert.That(diags.Count(d => d.Id == "LY0014"), Is.EqualTo(2));
    }

    // ----------------------------------------------------------------
    //  诊断验证
    // ----------------------------------------------------------------

    /// <summary>验证编译本身能正确解析属性（基线测试）。</summary>
    [Test]
    public void Compilation_SemanticModel_CanResolveAttribute()
    {
        var code = Attributes + @"
public class T {
    [ExcelColumn(Name = ""A"")] public string X { get; set; }
}";
        var compilation = CreateCompilation(code);
        var tree = compilation.SyntaxTrees.Single();
        var model = compilation.GetSemanticModel(tree);
        var classDecl = tree.GetRoot().DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(c => c.Identifier.Text == "T");
        var classSymbol = model.GetDeclaredSymbol(classDecl);
        Assert.That(classSymbol, Is.Not.Null);
        var propSymbol = classSymbol!.GetMembers("X").OfType<IPropertySymbol>().First();
        var attrs = propSymbol.GetAttributes();
        Assert.That(attrs.Any(a => a.AttributeClass!.Name == "ExcelColumnAttribute"), Is.True,
            "编译应能解析内联定义的 [ExcelColumn] 属性");
    }

    // ----------------------------------------------------------------
    //  Helpers
    // ----------------------------------------------------------------

    /// <summary>运行 IIncrementalGenerator 并返回其产生的诊断。</summary>
    private static ImmutableArray<Diagnostic> RunGenerator(string code)
    {
        var compilation = CreateCompilation(code);
        var generator = new AttributeAnalyzer.ExcelColumnGenerator().AsSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        return outputCompilation.GetDiagnostics();
    }

    /// <summary>运行 DiagnosticAnalyzer 并返回诊断。</summary>
    private static async Task<ImmutableArray<Diagnostic>> RunAnalyzerAsync<TAnalyzer>(string code)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var compilation = CreateCompilation(code);
        return await compilation
            .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new TAnalyzer()))
            .GetAllDiagnosticsAsync();
    }

    /// <summary>用给定的 C# 源码创建编译对象。</summary>
    private static CSharpCompilation CreateCompilation(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.CSharp10));
        var refs = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        };

        // 补充必要的系统程序集
        var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        foreach (var name in new[] { "System.Runtime", "System.Collections" })
        {
            var path = Path.Combine(coreDir, name + ".dll");
            if (File.Exists(path))
                refs.Add(MetadataReference.CreateFromFile(path));
        }

        return CSharpCompilation.Create(
            "TestAssembly",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
