using ExcelUtile;
using ExcelUtile.Converters;
using ExcelUtile.ExcelCore;
using ExcelUtileTest.Entities;
using ExcelUtileTest.Utilities;

namespace ExcelUtileTest.Tests;

/// <summary>
/// 测试新功能：FieldScope、DelegateColumnReader/Writer、ConverterRegistry、ExcelFactory。
/// </summary>
[TestFixture]
internal class CustomColumnTests : TestBase
{
    #region FieldScope

    [Test]
    public void FieldScope_ExportOnlySpecified()
    {
        var students = new List<Student>
        {
            new() { Name = "A", Age = 18, Score = 90, Graduated = true, EnrollDate = DateTime.Today }
        };
        var options = new ExcelSerializerOptions { FieldScope = new[] { "Name", "Age" } };
        var bytes = ExcelSerializer.Serialize(students, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));

        // 导入回来只有 Name、Age
        using var ms = new MemoryStream(bytes);
        var imported = ExcelSerializer.Deserialize<Student>(ms, options).ToList();
        Assert.That(imported[0].Name, Is.EqualTo("A"));
        Assert.That(imported[0].Age, Is.EqualTo(18));
        Assert.That(imported[0].Score, Is.EqualTo(0));  // 未导出
    }

    [Test]
    public void FieldScope_AutoInclude_FiltersCorrectly()
    {
        var employees = new List<Employee>
        {
            new() { Name = "Tom", Code = "T01", Department = "IT", IsActive = true }
        };
        var options = new ExcelSerializerOptions
        {
            AutoInclude = true,
            FieldScope = new[] { "Name", "Department" }
        };
        var bytes = ExcelSerializer.Serialize(employees, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, employees.Count), bytes);
        using var ms = new MemoryStream(bytes);
        var imported = ExcelSerializer.Deserialize<Employee>(ms, options).ToList();
        Assert.That(imported[0].Name, Is.EqualTo("Tom"));
        Assert.That(imported[0].Department, Is.EqualTo("IT"));
        Assert.That(imported[0].Code, Is.Empty);   // 被 FieldScope 过滤，默认值
    }

    #endregion

    #region CollectionColumnReader

    [Test]
    public void CollectionReader_FromDict()
    {
        var reader = CollectionColumnReader<Student>.FromDict<object>(
            null, row => new Dictionary<string, object> { ["Extra"] = "hello" }
        );
        Assert.That(reader.Match("Extra"), Is.True);
        Assert.That(reader.Match("Anything"), Is.True);

        var student = new Student();
        reader.SetValue(student, "Extra", "world");
    }

    #endregion

    #region DelegateColumnReader

    [Test]
    public void DelegateReader_Basic()
    {
        var reader = new DelegateColumnReader<Student>(
            setter: (row, val) => row.Name = val?.ToString() ?? "",
            column: "Custom",
            converter: null
        );
        Assert.That(reader.Match("Custom"), Is.True);
        Assert.That(reader.Match("Other"), Is.False);

        var student = new Student();
        reader.SetValue(student, "Custom", "DelegateSet");
        Assert.That(student.Name, Is.EqualTo("DelegateSet"));
    }

    #endregion

    #region DelegateColumnWriter

    [Test]
    public void DelegateWriter_Dict()
    {
        var writers = DelegateColumnWriter<Student>.FromDict<object>(
            new[] { "Extra" },
            row => new Dictionary<string, object> { ["Extra"] = "extra_value" }
        );
        Assert.That(writers.Length, Is.EqualTo(1));
        Assert.That(writers[0].ColumnName, Is.EqualTo("Extra"));
        Assert.That(writers[0].GetValue(new Student()), Is.EqualTo("extra_value"));
    }

    [Test]
    public void DelegateWriter_ExportWithCustomWriter()
    {
        var student = new Student { Name = "Test", Age = 20 };
        var writers = DelegateColumnWriter<Student>.FromDict<object>(
            new[] { "CustomCol" },
            row => new Dictionary<string, object> { ["CustomCol"] = "custom_val" }
        );
        var options = new ExcelSerializerOptions { Writers = writers };
        var bytes = ExcelSerializer.Serialize(new[] { student }, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 1), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));
    }

    #endregion

    #region ConverterRegistry

    [Test]
    public void ConverterRegistry_AddAndGet()
    {
        var registry = new ConverterRegistry();
        registry.AddConverter<string>(new StringConverter());
        var c = registry.GetConverter(typeof(string));
        Assert.That(c, Is.Not.Null);
        Assert.That(c.Type, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void ConverterRegistry_FallbackForUnknown()
    {
        var registry = new ConverterRegistry();
        var c = registry.GetConverter(typeof(CustomColumnTests));
        Assert.That(c, Is.Not.Null); // 返回 BuiltinConverters.Fallback
    }

    #endregion

    #region ExcelFactory

    [Test]
    public void ExcelFactory_CreateWorkBookAndToBytes()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet("Test");
        sheet.CreateRow(0).CreateCell(0).SetCellValue("Hello");
        var bytes = wb.ToBytes();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(1, 1), bytes);
        Assert.That(bytes.Length, Is.AtLeast(50));
    }

    [Test]
    public void ExcelFactory_SheetNameFormat()
    {
        var name = ExcelFactory.SheetNameFormat("abc/def*ghi?jkl");
        Assert.That(name, Does.Not.Contain("/"));
        Assert.That(name, Does.Not.Contain("*"));
        Assert.That(name, Does.Not.Contain("?"));
    }

    [Test]
    public void ExcelFactory_GetAllSheets()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        wb.CreateNewSheet("S1");
        wb.CreateNewSheet("S2");
        var sheets = wb.GetAllSheets();
        Assert.That(sheets.Count, Is.EqualTo(2));
    }

    #endregion

    #region SupportMimeType

    [Test]
    public void SupportMimeType_ValidXlsx()
    {
        Assert.That(SupportMimeType.ValidMimeType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"), Is.True);
    }

    [Test]
    public void SupportMimeType_Invalid()
    {
        Assert.That(SupportMimeType.ValidMimeType("text/plain"), Is.False);
    }

    #endregion

    #region CreateTemplate

    [Test]
    public void CreateTemplate_ToBytes()
    {
        var bytes = ExcelSerializer.CreateTemplate<Student>();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 0), bytes);
        Assert.That(bytes.Length, Is.AtLeast(20));
    }

    [Test]
    public void CreateTemplate_ToStream()
    {
        using var ms = new MemoryStream();
        ExcelSerializer.CreateTemplate<Student>(ms);
        Assert.That(ms.Length, Is.AtLeast(20));
    }

    #endregion
}
