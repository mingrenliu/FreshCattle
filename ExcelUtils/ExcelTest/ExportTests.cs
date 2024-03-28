using ExcelTest.Mocks;
using ExcelUtile.ExcelCore;
using ExcelUtileTest.Mocks;
using System.ComponentModel;
using System.Drawing;

namespace ExcelTest
{
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
        public void New_Export_Person_WithMerged_Return_List_Test()
        {
            var persons = PersonMock.Persons();
            var direction = LocationHelper.GetExportResourcesPath();
            var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
            var timer = StartTimer();
            var bytes = ExcelHelper.Export(persons, null, new List<MergedRegion>() { new() { ColumnEndIndex = 3, ColumnStartIndex = 1, RowStartIndex = 3, RowEndIndex = 5, Value = DateTime.Now } });
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
                DynamicExport = new ListDynamicHandler<Person, decimal?>(new ColumnInfo[] {
                new("扩展行1",typeof(decimal?),10), new("扩展行2", typeof(decimal?), 10) }, p => new List<decimal?> { 1, null })
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
                DynamicExport = new ListDynamicHandler<Person, decimal?>(new ColumnInfo[] {
                new("扩展行1",typeof(decimal?),10), new("扩展行2", typeof(decimal?), 10) }, p => new List<decimal?> { 1, null }),
                HeaderLineIndex = 1,
                StartLineIndex = 3,
                MergedRegions = new List<MergedRegion>() {
                    new() { ColumnEndIndex = 8, ColumnStartIndex = 7, RowStartIndex = 0, RowEndIndex = 0, Value = "化验数据" },
                    new() { ColumnEndIndex = 7, ColumnStartIndex = 7, RowStartIndex = 2, RowEndIndex =2, Value = "标准1" },
                    new() { ColumnEndIndex = 8, ColumnStartIndex = 8, RowStartIndex = 2, RowEndIndex = 2, Value = "标准2" },
                    new() { ColumnEndIndex = 0, ColumnStartIndex = 0, RowStartIndex = 0, RowEndIndex = 2, Value = "姓名" },
                    new() { ColumnEndIndex = 1, ColumnStartIndex = 1, RowStartIndex = 0, RowEndIndex = 2, Value = "性别" },
                    new() { ColumnEndIndex = 2, ColumnStartIndex = 2, RowStartIndex = 0, RowEndIndex = 2, Value = "年龄" },
                    new() { ColumnEndIndex =3, ColumnStartIndex = 3, RowStartIndex = 0, RowEndIndex = 2, Value = "工资" },
                    new() { ColumnEndIndex = 4, ColumnStartIndex = 4, RowStartIndex = 0, RowEndIndex = 2, Value = "生日" },
                    new() { ColumnEndIndex = 5, ColumnStartIndex = 5, RowStartIndex = 0, RowEndIndex = 2, Value = "是否在职" },
                    new() { ColumnEndIndex = 6, ColumnStartIndex = 6, RowStartIndex = 0, RowEndIndex = 2, Value = "父亲名字" },
                }
            };
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
}