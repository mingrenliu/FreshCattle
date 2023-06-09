﻿using ExcelTest.Mocks;
using ExcelUtile.ExcelCore;
using ExcelUtileTest.Mocks;

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
            Console.WriteLine("计算毫秒数："+timer.GetMilliseconds() + ";条数:" + persons.Count());
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
            Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds()+";条数:"+records.Count());
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
            var bytes = ExcelHelper.Export(records, CreateOption(dic,true));
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
            var bytes = ExcelHelper.Export(records, CreateOption(dic, false));
            Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds() + ";条数:" + records.Count());
            File.WriteAllBytes(path, bytes);
            Assume.That(bytes.Length, Is.AtLeast(2000));
        }
        private static ExcelSerializeOptions CreateOption(Dictionary<string, string> map, bool strict = true)
        {
            var result = new ExcelSerializeOptions();
            result.SetProperty(type => new MapOverridePropertySelector(map, strict).GetTypeInfo(type));
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