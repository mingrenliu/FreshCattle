namespace ExcelTest.Tests;

/// <summary>
/// Excel API 综合测试：OptIn/OptOut 模式、RoundTrip、多 Sheet、列名映射、模板。
/// </summary>
[TestFixture]
internal class SerializationApiTests : TestBase
{
    #region Opt-In 模式 (AutoInclude=false, 默认)

    [Test]
    public void OptIn_Serialize_OnlyExcelColumn()
    {
        List<Student> students = [
            new() { Name = "张三", Age = 20, Score = 88.5, Graduated = true, EnrollDate = new DateTime(2023, 9, 1), InternalCode = "SECRET" },
            new() { Name = "李四", Age = 22, Score = 92.0, Graduated = false, EnrollDate = new DateTime(2022, 9, 1), InternalCode = "X999" },
        ];

        var bytes = Excel.Serialize(students);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
        Assume.That(bytes.Length, Is.AtLeast(50));
    }

    [Test]
    public void OptIn_Deserialize_RoundTrip()
    {
        List<Student> students = [
            new() { Name = "王五", Age = 25, Score = 76.0, Graduated = true, EnrollDate = new DateTime(2020, 9, 1) },
            new() { Name = "赵六", Age = 23, Score = 85.5, Graduated = false, EnrollDate = new DateTime(2021, 9, 1) },
        ];

        var exported = Excel.Serialize(students);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), exported);
        using var ms = new MemoryStream(exported);
        var imported = Excel.Deserialize<Student>(ms).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(imported, Has.Count.EqualTo(2));
            Assert.That(imported[0].Name, Is.EqualTo("王五"));
            Assert.That(imported[0].Age, Is.EqualTo(25));
            Assert.That(imported[1].Name, Is.EqualTo("赵六"));
        });
    }

    [Test]
    public void OptIn_InternalCode_NotExported()
    {
        var student = new Student { Name = "Test", InternalCode = "SECRET" };
        var bytes = Excel.Serialize<Student>([student]);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 1), bytes);
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<Student>(ms).First();
        Assert.Multiple(() =>
        {
            Assert.That(imported.Name, Is.EqualTo("Test"));
            Assert.That(imported.InternalCode, Is.Null); // 未导出，保持默认
        });
    }

    [Test]
    public void OptIn_CreateTemplate()
    {
        var bytes = Excel.CreateTemplate<Student>();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));
    }

    #endregion

    #region Opt-Out 模式 (AutoInclude=true)

    [Test]
    public void OptOut_Serialize_AllExceptIgnored()
    {
        List<Employee> employees = [
            new() { Name = "Alice", Code = "E001", Department = "研发部", Salary = 15000.50m, HireDate = new DateTime(2023, 3, 15), IsActive = true, InternalNote = "优秀员工", PasswordHash = "abc123" },
            new() { Name = "Bob", Code = "E002", Department = "市场部", Salary = 12000.00m, HireDate = new DateTime(2022, 7, 1), IsActive = true, InternalNote = "待考核", PasswordHash = "xyz789" },
        ];

        var options = ExcelOptions.AutoIncludeAll;
        var bytes = Excel.Serialize(employees, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, employees.Count), bytes);
        Assume.That(bytes.Length, Is.AtLeast(50));
    }

    [Test]
    public void OptOut_Deserialize_RoundTrip()
    {
        List<Employee> original = [
            new() { Name = "Charlie", Code = "E003", Department = "财务部", Salary = 18000m, HireDate = new DateTime(2021, 1, 10), IsActive = true },
            new() { Name = "Diana", Code = "E004", Department = "人事部", Salary = 14000m, HireDate = new DateTime(2023, 6, 20), IsActive = false },
        ];

        var options = ExcelOptions.AutoIncludeAll;
        var exported = Excel.Serialize(original, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, original.Count), exported);

        using var ms = new MemoryStream(exported);
        var imported = Excel.Deserialize<Employee>(ms, options).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(imported, Has.Count.EqualTo(2));
            Assert.That(imported[0].Name, Is.EqualTo("Charlie"));
            Assert.That(imported[0].Department, Is.EqualTo("财务部"));
            Assert.That(imported[0].InternalNote, Is.Null);  // [ExcelIgnore] 不导出
        });
    }

    [Test]
    public void OptOut_CreateTemplate()
    {
        var options = ExcelOptions.AutoIncludeAll;
        var bytes = Excel.CreateTemplate<Employee>(options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, 0), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));
    }

    #endregion

    #region RoundTrip - 全类型

    [Test]
    public void AllTypes_RoundTrip()
    {
        List<AllTypesEntity> original = [
            new() { StringValue = "A", IntValue = 1, LongValue = 2, ShortValue = 3, ByteValue = 4,
                FloatValue = 1.1f, DoubleValue = 2.2, DecimalValue = 3.3m, BoolValue = true,
                DateTimeValue = new DateTime(2024,1,1), DateOnlyValue = new DateOnly(2024,1,1),
                TimeOnlyValue = new TimeOnly(8,0), TimeSpanValue = TimeSpan.FromHours(2),
                DateTimeOffsetValue = DateTimeOffset.Now, GuidValue = Guid.NewGuid(),
                NullableInt = 99, NullableDouble = 1.23, NullableDateTime = new DateTime(2024,12,25), NullableBool = true },
            new() { StringValue = "B", IntValue = 100, LongValue = 200, ShortValue = 30, ByteValue = 40,
                FloatValue = 10.1f, DoubleValue = 20.2, DecimalValue = 30.3m, BoolValue = false,
                DateTimeValue = new DateTime(2023,6,15), NullableInt = null, NullableDouble = null, NullableDateTime = null, NullableBool = null },
        ];

        var bytes = Excel.Serialize(original);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(19, original.Count), bytes);
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<AllTypesEntity>(ms).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(imported, Has.Count.EqualTo(2));
            for (int i = 0; i < 2; i++)
            {
                var o = original[i]; var r = imported[i];
                Assert.That(r.StringValue, Is.EqualTo(o.StringValue));
                Assert.That(r.IntValue, Is.EqualTo(o.IntValue));
                Assert.That(r.LongValue, Is.EqualTo(o.LongValue));
                Assert.That(r.DoubleValue, Is.EqualTo(o.DoubleValue).Within(0.01));
                Assert.That(r.DecimalValue, Is.EqualTo(o.DecimalValue).Within(0.1m));
                Assert.That(r.BoolValue, Is.EqualTo(o.BoolValue));
                Assert.That(r.GuidValue, Is.EqualTo(o.GuidValue));
                Assert.That(r.DateTimeValue.Year, Is.EqualTo(o.DateTimeValue.Year));
                Assert.That(r.DateOnlyValue, Is.EqualTo(o.DateOnlyValue));
                Assert.That(r.NullableInt, Is.EqualTo(o.NullableInt));
                Assert.That(r.NullableBool, Is.EqualTo(o.NullableBool));
            }
        });
    }

    [Test]
    public void NullValues_RoundTrip()
    {
        var e = new AllTypesEntity { StringValue = "", IntValue = 0, NullableInt = null, NullableDouble = null, NullableDateTime = null, NullableBool = null, GuidValue = Guid.Empty };
        var bytes = Excel.Serialize<AllTypesEntity>([e]);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(19, 1), bytes);
        using var ms = new MemoryStream(bytes);
        var r = Excel.Deserialize<AllTypesEntity>(ms).First();
        Assert.Multiple(() =>
        {
            Assert.That(r.NullableInt, Is.Null);
            Assert.That(r.NullableDouble, Is.Null);
            Assert.That(r.NullableDateTime, Is.Null);
            Assert.That(r.NullableBool, Is.Null);
        });
    }

    [Test]
    public void MaxMinValues_RoundTrip()
    {
        var e = new AllTypesEntity { StringValue = "边界", IntValue = int.MaxValue, LongValue = long.MaxValue, ShortValue = short.MaxValue, ByteValue = byte.MaxValue, DoubleValue = double.MaxValue, DecimalValue = 999999999.99m, BoolValue = true };
        var bytes = Excel.Serialize<AllTypesEntity>([e]);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(19, 1), bytes);
        using var ms = new MemoryStream(bytes);
        var r = Excel.Deserialize<AllTypesEntity>(ms).First();
        Assert.Multiple(() =>
        {
            Assert.That(r.IntValue, Is.EqualTo(int.MaxValue));
            Assert.That(r.LongValue, Is.EqualTo(long.MaxValue));
            Assert.That(r.ShortValue, Is.EqualTo(short.MaxValue));
            Assert.That(r.ByteValue, Is.EqualTo(byte.MaxValue));
        });
    }

    [Test]
    public void EmptyAndSingleData()
    {
        // 空列表
        var empty = Excel.Serialize(Enumerable.Empty<Student>());
        using var ms1 = new MemoryStream(empty);
        var emptyCount = Excel.Deserialize<Student>(ms1).Count();

        // 单条数据
        var single = new Student { Name = "唯一", Age = 30 };
        var bytes = Excel.Serialize<Student>([single]);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 1), bytes);
        using var ms2 = new MemoryStream(bytes);
        var r = Excel.Deserialize<Student>(ms2).First();

        Assert.Multiple(() =>
        {
            Assert.That(emptyCount, Is.EqualTo(0));
            Assert.That(r.Name, Is.EqualTo("唯一"));
            Assert.That(r.Age, Is.EqualTo(30));
        });
    }

    #endregion

    #region FieldScope 字段范围

    [Test]
    public void FieldScope_ExportOnlySpecified()
    {
        List<Student> students = [new() { Name = "A", Age = 18, Score = 90, Graduated = true, EnrollDate = DateTime.Today }];
        var options = new ExcelOptions { FieldScope = ["Name", "Age"] };
        var bytes = Excel.Serialize(students, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);

        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<Student>(ms, options).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(imported[0].Name, Is.EqualTo("A"));
            Assert.That(imported[0].Age, Is.EqualTo(18));
            Assert.That(imported[0].Score, Is.EqualTo(0));
        });
    }

    [Test]
    public void FieldScope_AutoInclude_FiltersCorrectly()
    {
        List<Employee> employees = [new() { Name = "Tom", Code = "T01", Department = "IT", IsActive = true }];
        var options = new ExcelOptions { AutoInclude = true, FieldScope = ["Name", "Department"] };
        var bytes = Excel.Serialize(employees, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, employees.Count), bytes);
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<Employee>(ms, options).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(imported[0].Name, Is.EqualTo("Tom"));
            Assert.That(imported[0].Department, Is.EqualTo("IT"));
            Assert.That(imported[0].Code, Is.Empty);
        });
    }

    #endregion

    #region ColumnNameMap 列名映射

    [Test]
    public void ColumnNameMap_OverrideHeaders()
    {
        List<Student> students = [new() { Name = "测试1", Age = 18, Score = 99, Graduated = true, EnrollDate = DateTime.Today }];
        var options = new ExcelOptions
        {
            ColumnNameMap = new Dictionary<string, string>
            {
                ["Name"] = "StudentName", ["Age"] = "StudentAge", ["Score"] = "StudentScore",
                ["Graduated"] = "IsGraduated", ["EnrollDate"] = "EnrollmentDate",
            }
        };
        var bytes = Excel.Serialize(students, options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));
    }

    [Test]
    public void ColumnNameMap_PartialOverride()
    {
        var student = new Student { Name = "部分覆盖", Age = 20 };
        var options = new ExcelOptions { ColumnNameMap = new Dictionary<string, string> { ["Name"] = "FullName" } };
        var bytes = Excel.Serialize<Student>([student], options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 1), bytes);
        // 验证导出文件包含重命名后的表头
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));
        var headers = reader.ReadHeaders(0);
        Assert.That(headers.Values, Contains.Item("FullName"));
    }

    #endregion

    #region 自定义读写器

    #region DelegateColumnWriter

    [Test]
    public void DelegateWriter_Dict_RoundTrip()
    {
        var student = new Student { Name = "张三", Age = 20 };
        var dict = new Dictionary<string, object> { ["语文"] = 90, ["数学"] = 85 };
        var writers = DelegateColumnWriter<Student>.FromDict<object>(
            ["语文", "数学"], _ => dict.ToDictionary(kv => kv.Key, kv => kv.Value));
        var options = new ExcelOptions { Writers = writers };

        var bytes = Excel.Serialize<Student>([student], options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(7, 1), bytes);

        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));
        var headers = reader.ReadHeaders(0);
        Assert.Multiple(() =>
        {
            Assert.That(headers.Values, Contains.Item("语文"));
            Assert.That(headers.Values, Contains.Item("数学"));
            Assert.That(reader.GetString(1, headers.First(x => x.Value == "语文").Key), Is.EqualTo("90"));
        });
    }

    [Test]
    public void DelegateWriter_List_RoundTrip()
    {
        List<int> items = [10, 20, 30];
        var writers = DelegateColumnWriter<Student>.FromList(
            ["项目1", "项目2", "项目3"], _ => items);
        var options = new ExcelOptions { Writers = writers };
        var bytes = Excel.Serialize<Student>([new Student { Name = "测试" }], options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(8, 1), bytes);

        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));
        var headers = reader.ReadHeaders(0);
        var col3 = headers.First(x => x.Value == "项目3").Key;
        Assert.That(reader.GetString(1, col3), Is.EqualTo("30"));
    }

    [Test]
    public void DelegateWriter_Array_RoundTrip()
    {
        string[] arr = ["A", "B", "C", "D"];
        var writers = DelegateColumnWriter<Student>.FromArray(
            ["C1", "C2", "C3", "C4"], _ => arr);
        var options = new ExcelOptions { Writers = writers };
        var bytes = Excel.Serialize<Student>([new Student { Name = "ArrTest" }], options);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(9, 1), bytes);

        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));
        var headers = reader.ReadHeaders(0);
        var col4 = headers.First(x => x.Value == "C4").Key;
        Assert.That(reader.GetString(1, col4), Is.EqualTo("D"));
    }

    #endregion

    #region DelegateColumnReader

    [Test]
    public void DelegateReader_FullRoundTrip()
    {
        // 导出带 Student 属性 + 额外自定义列
        var student = new Student { Name = "ReaderTest", Age = 25 };
        var writers = DelegateColumnWriter<Student>.FromDict<string>(
            ["备注"], _ => new Dictionary<string, string> { ["备注"] = "自定义内容" });
        var exportOptions = new ExcelOptions { Writers = writers };
        var bytes = Excel.Serialize<Student>([student], exportOptions);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(6, 1), bytes);

        // 用 DelegateReader 导入自定义列
        string? capturedNote = null;
        var reader = new DelegateColumnReader<Student>(
            setter: (row, val) => capturedNote = val?.ToString(),
            column: "备注", converter: null);
        var importOptions = new ExcelOptions { Readers = [reader] };

        using var ms = new MemoryStream(bytes);
        Excel.Deserialize<Student>(ms, importOptions);
        Assert.That(capturedNote, Is.EqualTo("自定义内容"));
    }

    #endregion

    #region CollectionColumnReader

    [Test]
    public void CollectionReader_Dict_RoundTrip()
    {
        // 导出: 用 Dict Writer 写入额外字段
        var dict = new Dictionary<string, string> { ["科目"] = "物理", ["等级"] = "A" };
        var writers = DelegateColumnWriter<Student>.FromDict<string>(
            ["科目", "等级"], _ => dict);
        var exportOptions = new ExcelOptions { Writers = writers };
        var bytes = Excel.Serialize<Student>([new Student { Name = "CD" }], exportOptions);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(7, 1), bytes);

        // 导入: 用 CollectionReader.FromDict 读取
        var capturedDict = new Dictionary<string, string>();
        var reader = CollectionColumnReader<Student>.FromDict<string>(
            ["科目", "等级"], _ => capturedDict);
        var importOptions = new ExcelOptions { Readers = [reader] };

        using var ms = new MemoryStream(bytes);
        Excel.Deserialize<Student>(ms, importOptions);
        Assert.Multiple(() =>
        {
            Assert.That(capturedDict["科目"], Is.EqualTo("物理"));
            Assert.That(capturedDict["等级"], Is.EqualTo("A"));
        });
    }

    [Test]
    public void CollectionReader_List_RoundTrip()
    {
        // 导出: List 类型自定义列
        List<double> list = [1.5, 2.5, 3.5];
        var writers = DelegateColumnWriter<Student>.FromList(
            ["V1", "V2", "V3"], _ => list.Select(x => (object)x).ToList()!);
        var exportOptions = new ExcelOptions { Writers = writers };
        var bytes = Excel.Serialize<Student>([new Student { Name = "CL" }], exportOptions);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(8, 1), bytes);

        // 导入: CollectionReader.FromList
        List<object> capturedList = [];
        var reader = CollectionColumnReader<Student>.FromList<object>(
            ["V1", "V2", "V3"], _ => capturedList);
        var importOptions = new ExcelOptions { Readers = [reader] };

        using var ms = new MemoryStream(bytes);
        Excel.Deserialize<Student>(ms, importOptions);
        Assert.That(capturedList, Has.Count.GreaterThanOrEqualTo(3));
    }

    [Test]
    public void CollectionReader_Array_RoundTrip()
    {
        // 导出: Array 类型自定义列
        int[] arr = [100, 200, 300];
        var writers = DelegateColumnWriter<Student>.FromArray(
            ["Arr1", "Arr2", "Arr3"], _ => arr.Cast<object>().ToArray());
        var exportOptions = new ExcelOptions { Writers = writers };
        var bytes = Excel.Serialize<Student>([new Student { Name = "CA" }], exportOptions);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(8, 1), bytes);

        // 导入: CollectionReader.FromArray
        var capturedArr = new object[3];
        var reader = CollectionColumnReader<Student>.FromArray<object>(
            ["Arr1", "Arr2", "Arr3"], _ => capturedArr);
        var importOptions = new ExcelOptions { Readers = [reader] };

        using var ms = new MemoryStream(bytes);
        Excel.Deserialize<Student>(ms, importOptions);
        Assert.That(capturedArr[0], Is.Not.Null);
    }

    #endregion

    #endregion

    #region ConverterRegistry

    [Test]
    public void ConverterRegistry_AddAndGet()
    {
        var registry = new ConverterRegistry();
        registry.AddConverter<string>(new StringConverter());
        ExcelConverter? c = registry.GetConverter(typeof(string));
        Assert.Multiple(() =>
        {
            Assert.That(c, Is.Not.Null);
            Assert.That(c.Type, Is.EqualTo(typeof(string)));
        });
    }

    [Test]
    public void ConverterRegistry_FallbackForUnknown()
    {
        var registry = new ConverterRegistry();
        ExcelConverter? c = registry.GetConverter(typeof(SerializationApiTests));
        Assert.That(c, Is.Not.Null);
    }

    #endregion

    #region 多 Sheet

    [Test]
    public void MultiSheet_SerializeAndDeserialize()
    {
        var sheets = new Dictionary<string, IEnumerable<Student>>
        {
            ["一班"] = [new Student { Name = "A1", Age = 15, Score = 80 }],
            ["二班"] = [new Student { Name = "B1", Age = 16, Score = 90.4514243 }],
        };
        var bytes = Excel.Serialize(sheets);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 2), bytes);
        using var ms = new MemoryStream(bytes);
        var r = Excel.DeserializeAll<Student>(ms);
        Assert.Multiple(() =>
        {
            Assert.That(r, Has.Count.EqualTo(2));
            Assert.That(r["一班"].First().Name, Is.EqualTo("A1"));
            Assert.That(r["二班"].First().Name, Is.EqualTo("B1"));
        });
    }

    [Test]
    public void MultiSheet_ToStream()
    {
        var sheets = new Dictionary<string, IEnumerable<Student>>
        {
            ["S1"] = [new Student { Name = "X", Age = 10 }],
            ["S2"] = [new Student { Name = "Y", Age = 20 }],
        };
        using var ms = new MemoryStream();
        Excel.Serialize(ms, sheets);
        Assert.That(ms.Length, Is.GreaterThan(100));
    }

    #endregion

    #region 模板

    [Test]
    public void CreateTemplate_ToBytes()
    {
        var bytes = Excel.CreateTemplate<Student>();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 0), bytes);
        Assert.That(bytes, Has.Length.AtLeast(20));
    }

    [Test]
    public void CreateTemplate_ToStream()
    {
        using var ms = new MemoryStream();
        Excel.CreateTemplate<Student>(ms);
        Assert.That(ms.Length, Is.AtLeast(20));
    }

    [Test]
    public void CreateTemplate_VerifyEmpty()
    {
        var bytes = Excel.CreateTemplate<Student>();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 0), bytes);
        using var ms = new MemoryStream(bytes);
        var r = Excel.Deserialize<Student>(ms).ToList();
        Assert.That(r, Is.Empty);
    }

    #endregion

    #region Setup / Teardown

    public override void Init()
    {
        var direction = LocationHelper.GetExportResourcesPath();
        if (!Directory.Exists(direction))
            Directory.CreateDirectory(direction);

        foreach (var file in Directory.GetFiles(direction, "*.xlsx"))
        {
            try { File.Delete(file); } catch { }
        }
    }

    #endregion
}
