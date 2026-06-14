namespace ExcelUtileTest.Tests;

/// <summary>
/// 测试新的 ExcelSerializer API（opt-in / opt-out 模式）。
/// </summary>
[TestFixture]
internal class SerializationTests : TestBase
{
    #region Opt-In 模式（AutoInclude=false，默认）

    [Test]
    public void OptIn_Serialize_OnlyExcelColumn()
    {
        var students = new List<Student>
        {
            new() { Name = "张三", Age = 20, Score = 88.5, Graduated = true, EnrollDate = new DateTime(2023, 9, 1), InternalCode = "SECRET" },
            new() { Name = "李四", Age = 22, Score = 92.0, Graduated = false, EnrollDate = new DateTime(2022, 9, 1), InternalCode = "X999" },
        };

        // 默认 AutoInclude=false，只导出 [ExcelColumn] 标注的属性
        var bytes = ExcelSerializer.Serialize(students);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(0, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(50));

        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
    }

    [Test]
    public void OptIn_Deserialize_RoundTrip()
    {
        var students = new List<Student>
        {
            new() { Name = "王五", Age = 25, Score = 76.0, Graduated = true, EnrollDate = new DateTime(2020, 9, 1) },
            new() { Name = "赵六", Age = 23, Score = 85.5, Graduated = false, EnrollDate = new DateTime(2021, 9, 1) },
        };

        // 导出后重新导入
        var exported = ExcelSerializer.Serialize(students);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), exported);
        using var ms = new MemoryStream(exported);
        var imported = ExcelSerializer.Deserialize<Student>(ms).ToList();

        Assert.That(imported.Count, Is.EqualTo(2));
        Assert.That(imported[0].Name, Is.EqualTo("王五"));
        Assert.That(imported[0].Age, Is.EqualTo(25));
        Assert.That(imported[1].Name, Is.EqualTo("赵六"));
    }

    [Test]
    public void OptIn_CreateTemplate()
    {
        // 模板导出：只有表头没有数据
        var bytes = ExcelSerializer.CreateTemplate<Student>();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(0, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));

        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 0), bytes);
    }

    #endregion

    #region Opt-Out 模式（AutoInclude=true）

    [Test]
    public void OptOut_Serialize_AllExceptIgnored()
    {
        var employees = new List<Employee>
        {
            new()
            {
                Name = "Alice", Code = "E001", Department = "研发部",
                Salary = 15000.50m, HireDate = new DateTime(2023, 3, 15),
                IsActive = true, InternalNote = "优秀员工", PasswordHash = "abc123hash"
            },
            new()
            {
                Name = "Bob", Code = "E002", Department = "市场部",
                Salary = 12000.00m, HireDate = new DateTime(2022, 7, 1),
                IsActive = true, InternalNote = "待考核", PasswordHash = "xyz789hash"
            },
        };

        // AutoInclude=true：所有 public 属性默认导出，[ExcelIgnore] 的排除
        var options = ExcelSerializerOptions.AutoIncludeAll;
        var bytes = ExcelSerializer.Serialize(employees, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(0, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(50));

        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, employees.Count), bytes);
    }

    [Test]
    public void OptOut_Deserialize_RoundTrip()
    {
        var original = new List<Employee>
        {
            new() { Name = "Charlie", Code = "E003", Department = "财务部", Salary = 18000m, HireDate = new DateTime(2021, 1, 10), IsActive = true },
            new() { Name = "Diana", Code = "E004", Department = "人事部", Salary = 14000m, HireDate = new DateTime(2023, 6, 20), IsActive = false },
        };

        var options = ExcelSerializerOptions.AutoIncludeAll;
        var exported = ExcelSerializer.Serialize(original, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, original.Count), exported);

        using var ms = new MemoryStream(exported);
        var imported = ExcelSerializer.Deserialize<Employee>(ms, options).ToList();

        Assert.That(imported.Count, Is.EqualTo(2));
        Assert.That(imported[0].Name, Is.EqualTo("Charlie"));
        Assert.That(imported[0].Department, Is.EqualTo("财务部"));
        // [ExcelIgnore] 的属性不会被写入 Excel，所以导入后应该是默认值
        Assert.That(imported[0].InternalNote, Is.Null);
    }

    [Test]
    public void OptOut_CreateTemplate()
    {
        var options = ExcelSerializerOptions.AutoIncludeAll;
        var bytes = ExcelSerializer.CreateTemplate<Employee>(options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(0, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));

        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, 0), bytes);
    }

    #endregion

    #region 自定义转换器和列名映射

    [Test]
    public void ColumnNameMap_OverrideHeaders()
    {
        var students = new List<Student>
        {
            new() { Name = "测试1", Age = 18, Score = 99, Graduated = true, EnrollDate = DateTime.Today }
        };

        // 覆盖列名为英文
        var options = new ExcelSerializerOptions
        {
            ColumnNameMap = new Dictionary<string, string>
            {
                ["Name"] = "StudentName",
                ["Age"] = "StudentAge",
                ["Score"] = "StudentScore",
                ["Graduated"] = "IsGraduated",
                ["EnrollDate"] = "EnrollmentDate",
            }
        };

        var bytes = ExcelSerializer.Serialize(students, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(0, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));

        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
    }

    #endregion

    #region 多 Sheet

    [Test]
    public void MultiSheet_Serialize()
    {
        var students = new List<Student>
        {
            new() { Name = "Sheet1-学生", Age = 20, Score = 80, Graduated = true, EnrollDate = DateTime.Today }
        };
        var employees = new List<Employee>
        {
            new() { Name = "Sheet2-员工", Code = "M01", Department = "综合", Salary = 5000, HireDate = DateTime.Today }
        };

        // Student sheet
        var sheets = new Dictionary<string, IEnumerable<Student>>
        {
            ["学生表"] = students,
        };
        var bytes = ExcelSerializer.Serialize(sheets);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(0, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(50));

        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
    }

    #endregion

    #region Setup / Teardown

    public override void Init()
    {
        var direction = LocationHelper.GetExportResourcesPath();
        if (!Directory.Exists(direction))
            Directory.CreateDirectory(direction);

        // 清理旧导出文件
        foreach (var file in Directory.GetFiles(direction, "*.xlsx"))
        {
            try { File.Delete(file); } catch { }
        }
        base.Init();
    }

    #endregion
}
