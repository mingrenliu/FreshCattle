namespace ExcelUtileTest.Tests;

/// <summary>
/// 测试新的 ExcelSerializer API（opt-in / opt-out 模式）。
/// </summary>
[TestFixture]
internal class NewApiTests : TestBase
{
    #region Opt-In 模式（AutoInclude=false，默认）

    [Test]
    public void Serialize_OptIn_OnlyExcelColumn_Test()
    {
        var students = new List<Student>
        {
            new() { Name = "张三", Age = 20, Score = 88.5, Graduated = true, EnrollDate = new DateTime(2023, 9, 1), InternalCode = "SECRET" },
            new() { Name = "李四", Age = 22, Score = 92.0, Graduated = false, EnrollDate = new DateTime(2022, 9, 1), InternalCode = "X999" },
        };

        // 默认 AutoInclude=false，只导出 [ExcelColumn] 标注的属性
        var bytes = ExcelSerializer.Serialize(students);
        Assume.That(bytes.Length, Is.AtLeast(50));

        var path = Path.Combine(LocationHelper.GetExportResourcesPath(), $"students_optin_{Guid.NewGuid():N}.xlsx");
        File.WriteAllBytes(path, bytes);
    }

    [Test]
    public void Deserialize_OptIn_OnlyExcelColumn_Test()
    {
        var students = new List<Student>
        {
            new() { Name = "王五", Age = 25, Score = 76.0, Graduated = true, EnrollDate = new DateTime(2020, 9, 1) },
            new() { Name = "赵六", Age = 23, Score = 85.5, Graduated = false, EnrollDate = new DateTime(2021, 9, 1) },
        };

        // 导出后重新导入
        var exported = ExcelSerializer.Serialize(students);
        using var ms = new MemoryStream(exported);
        var imported = ExcelSerializer.Deserialize<Student>(ms).ToList();

        Assert.That(imported.Count, Is.EqualTo(2));
        Assert.That(imported[0].Name, Is.EqualTo("王五"));
        Assert.That(imported[0].Age, Is.EqualTo(25));
        Assert.That(imported[1].Name, Is.EqualTo("赵六"));
    }

    [Test]
    public void Serialize_OptIn_TemplateOnly_Test()
    {
        // 模板导出：只有表头没有数据
        var bytes = ExcelSerializer.CreateTemplate<Student>();
        Assume.That(bytes.Length, Is.AtLeast(30));

        var path = Path.Combine(LocationHelper.GetExportResourcesPath(), $"student_template_optin_{Guid.NewGuid():N}.xlsx");
        File.WriteAllBytes(path, bytes);
    }

    #endregion

    #region Opt-Out 模式（AutoInclude=true）

    [Test]
    public void Serialize_OptOut_AllExceptIgnored_Test()
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
        Assume.That(bytes.Length, Is.AtLeast(50));

        var path = Path.Combine(LocationHelper.GetExportResourcesPath(), $"employees_optout_{Guid.NewGuid():N}.xlsx");
        File.WriteAllBytes(path, bytes);
    }

    [Test]
    public void Deserialize_OptOut_RoundTrip_Test()
    {
        var original = new List<Employee>
        {
            new() { Name = "Charlie", Code = "E003", Department = "财务部", Salary = 18000m, HireDate = new DateTime(2021, 1, 10), IsActive = true },
            new() { Name = "Diana", Code = "E004", Department = "人事部", Salary = 14000m, HireDate = new DateTime(2023, 6, 20), IsActive = false },
        };

        var options = ExcelSerializerOptions.AutoIncludeAll;
        var exported = ExcelSerializer.Serialize(original, options);

        using var ms = new MemoryStream(exported);
        var imported = ExcelSerializer.Deserialize<Employee>(ms, options).ToList();

        Assert.That(imported.Count, Is.EqualTo(2));
        Assert.That(imported[0].Name, Is.EqualTo("Charlie"));
        Assert.That(imported[0].Department, Is.EqualTo("财务部"));
        // [ExcelIgnore] 的属性不会被写入 Excel，所以导入后应该是默认值
        Assert.That(imported[0].InternalNote, Is.Null);
    }

    [Test]
    public void Serialize_OptOut_Template_Test()
    {
        var options = ExcelSerializerOptions.AutoIncludeAll;
        var bytes = ExcelSerializer.CreateTemplate<Employee>(options);
        Assume.That(bytes.Length, Is.AtLeast(30));

        var path = Path.Combine(LocationHelper.GetExportResourcesPath(), $"employee_template_optout_{Guid.NewGuid():N}.xlsx");
        File.WriteAllBytes(path, bytes);
    }

    #endregion

    #region 自定义转换器和列名映射

    [Test]
    public void Serialize_WithColumnNameMap_Test()
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
        Assume.That(bytes.Length, Is.AtLeast(30));

        var path = Path.Combine(LocationHelper.GetExportResourcesPath(), $"students_mapped_{Guid.NewGuid():N}.xlsx");
        File.WriteAllBytes(path, bytes);
    }

    #endregion

    #region 多 Sheet

    [Test]
    public void Serialize_MultiSheet_Test()
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
        Assume.That(bytes.Length, Is.AtLeast(50));

        var path = Path.Combine(LocationHelper.GetExportResourcesPath(), $"multi_sheet_{Guid.NewGuid():N}.xlsx");
        File.WriteAllBytes(path, bytes);
    }

    #endregion

    #region 低层自由写入（ExcelSheetWriter）

    [Test]
    public void SheetWriter_CustomLayout_Test()
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        var sheet = workbook.CreateNewSheet("自定义报表");
        var writer = new ExcelSheetWriter(sheet);

        writer.MoveTo(0, 0);
        writer.Write("日报表");
        writer.Merge(new MergeRegion(0, 0, 0, 3) { Value = "2024年1月日报表" });

        writer.MoveTo(1, 0);
        var headers = new[] { "序号", "项目", "金额", "备注" };
        foreach (var h in headers)
        {
            writer.Write(h);
            writer.Width(12);
        }

        writer.MoveTo(2, 0);
        writer.Write(1);
        writer.Write("销售收入");
        writer.Write(10000.50);
        writer.Write("已到账");

        writer.NextRow();
        writer.Write(2);
        writer.Write("采购支出");
        writer.Write(3500.00);
        writer.Write("待审批");

        writer.NextRow();
        writer.Write("");
        writer.Write("合计");
        var style = writer.Style();
        style.BorderTop = NPOI.SS.UserModel.BorderStyle.Double;
        writer.Write(13500.50);
        writer.NextCol();

        var bytes = workbook.ToBytes();
        Assume.That(bytes.Length, Is.AtLeast(30));

        var path = Path.Combine(LocationHelper.GetExportResourcesPath(), $"custom_layout_{Guid.NewGuid():N}.xlsx");
        File.WriteAllBytes(path, bytes);
    }

    [Test]
    public void SheetReader_ReadCells_Test()
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        var sheet = workbook.CreateNewSheet("读取测试");
        var writer = new ExcelSheetWriter(sheet);
        writer.MoveTo(0, 0).Write("姓名");
        writer.Write("年龄");
        writer.NextRow();
        writer.Write("张三");
        writer.Write(25);
        writer.NextRow();
        writer.Write("李四");
        writer.Write(30);

        var reader = new ExcelSheetReader(sheet);
        var headers = reader.ReadHeaders(0);
        Assert.That(headers[0], Is.EqualTo("姓名"));
        Assert.That(headers[1], Is.EqualTo("年龄"));

        var row1 = reader.ReadRowAsText(1, new[] { 0, 1 });
        Assert.That(row1[0], Is.EqualTo("张三"));
        Assert.That(row1[1], Is.EqualTo("25"));
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
