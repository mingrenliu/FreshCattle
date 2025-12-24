namespace ExcelUtileTest.Tests;

[TestFixture]
[Order(2)]
internal class ExportTests : TestBase
{
    [Test]
    public void New_Export_Person_Return_List_Test()
    {
        var persons = PersonMock.Persons();
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var timer = StartTimer();
        var bytes = ExcelHelper.Export(persons);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + persons.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(10));
    }
    [Test]
    public void Decimal_Export_Record_Return_List_Test()
    {
        var persons = new List<Record>() {
            new(){ Volume=125.0M},
            new(){ Volume=125},
            new(){ Volume=125.124M},
            new(){ Volume=125.10M},
            new(){ Volume=null},
        };
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var timer = StartTimer();
        var bytes = ExcelHelper.Export(persons);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + persons.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(3));
    }
    [Test]
    public void New_Export_Person_WithMerged_Return_List_Test()
    {
        var persons = PersonMock.Persons();
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var timer = StartTimer();
        var bytes = ExcelHelper.Export(persons, new ExcelExportOption<Person>(){MergedRegions= new List<MergedRegion>() { new(3,5,1,3) {  Value = DateTime.Now,FormatCellStyle=(cell)=>{
            var style=cell.Sheet.Workbook.CreateDefaultCellStyle();
            style.DataFormat=cell.Sheet.Workbook.GetDataFormat("yyyy-mm-dd hh:mm:ss");
            style.Alignment = HorizontalAlignment.Left;
            return style;
        } }
        } },"中国*？?人");
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + persons.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(10));
    }

    [Test]
    public void New_DynamicExport_Person_Return_List_Test()
    {
        var persons = PersonMock.Persons(1);
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var option = new ExcelExportOption<Person>()
        {
            DynamicExports = new List<DictionaryReaderWriter<Person, decimal?>>(){ new (new ColumnInfo[] {
                new("扩展行1", typeof(decimal?), 10)
                {
                    FormatCellStyle = (cell) =>
                    {
                       var style= cell.Sheet.Workbook.CreateDefaultCellStyle();
                        style.Alignment=HorizontalAlignment.Right;
                        return style;
                    }
                }, new("扩展行2", typeof(decimal?), 10){
                    FormatCellStyle = (cell) =>
                    {
                       var style= cell.Sheet.Workbook.CreateDefaultCellStyle();
                        style.Alignment=HorizontalAlignment.Left;
                        return style;
                    }
                } }, p => new Dictionary<string, decimal?> { ["扩展行1"] =1,["扩展行2"] = 58 })
        }
        };
        var timer = StartTimer();
        var bytes = ExcelHelper.Export(persons, option);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + persons.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(10));
    }

    [Test]
    public void New_DynamicHeader_Person_Return_List_Test()
    {
        var persons = PersonMock.Persons(100);
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var option = new ExcelExportOption<Person>()
        {
            DynamicExports = new List<DictionaryReaderWriter<Person, decimal?>>(){ new(new ColumnInfo[] {
            new("扩展行1",typeof(decimal?),10), new("扩展行2", typeof(decimal?), 10) }, p => new Dictionary<string, decimal?> { ["扩展行1"] =1,["扩展行2"] = null  }) },
            HeaderLineIndex = 1,
            StartLineIndex = 3,
            MergedRegions = new List<MergedRegion>() {
                new(0,0,8,9) {  Value = "化验数据" },
                new(2,2,8,8) {Value = "标准1" },
                new(2,2,9,9) { Value = "标准2" },
                new(0,2,0,0) { Value = "姓名" },
                new(0,2,1,1) { Value = "性别" },
                new(0,2,2,2) { Value = "年龄" },
                new(0,2,3,3) {Value = "工资" },
                new(0,2,4,4) { Value = "生日" },
                new(0,2,5,5) { Value = "是否在职" },
                new(0,2,6,6) { Value = "是否不在职" },
                new(0,2,7,7) { Value = "父亲名字" },
            }
        }
        ;
        var timer = StartTimer();
        var bytes = ExcelHelper.Export(persons, option);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + persons.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(10));
    }

    [Test]
    public void New_Export_Record_Return_List_Test()
    {
        var records = RecordMock.Records();
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var timer = StartTimer();
        var bytes = ExcelHelper.Export(records);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + records.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(2000));
    }

    [Test]
    public void New_Export_Record_WithCustomName_Strict_Return_List_Test()
    {
        var records = RecordMock.Records();
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var timer = StartTimer();
        var dic = new Dictionary<string, string>() { ["Name"] = "名称", ["Mass"] = "质量" };
        var bytes = ExcelHelper.Export(records, CreateOption<Record>(dic, true));
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + records.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(2000));
    }

    [Test]
    public void New_Export_Record_WithCustomName_NoStrict_Return_List_Test()
    {
        var records = RecordMock.Records();
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        var timer = StartTimer();
        var dic = new Dictionary<string, string>() { ["Name"] = "名称", ["Mass"] = "质量" };
        var bytes = ExcelHelper.Export(records, CreateOption<Record>(dic, false));
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + records.Count());
        File.WriteAllBytes(path, bytes);
        Assume.That(bytes.Length, Is.AtLeast(2000));
    }

    private static ExcelExportOption<T> CreateOption<T>(Dictionary<string, string> map, bool strict = true) where T : class, new()
    {
        var result = new ExcelExportOption<T>() { Selector = () => new MapOverridePropertySelector(map, strict).GetTypeInfo(typeof(T)) };
        return result;
    }

    [Test]
    public void New_Export_Empty_Test()
    {
        var timer = StartTimer();
        var bytes = ExcelHelper.ExportTemplate<Person>();
        var direction = LocationHelper.GetExportResourcesPath();
        var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
        File.WriteAllBytes(path, bytes);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds());
        Assume.That(bytes.Length, Is.AtLeast(10));
    }

    public override void Init()
    {
        var direction = LocationHelper.GetExportResourcesPath();
        var files = Directory.CreateDirectory(direction).GetFiles();
        foreach (var file in files)
        {
            file.Delete();
        }
        base.Init();
    }
}