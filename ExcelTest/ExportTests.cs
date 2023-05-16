using ExcelTest.Mocks;
using ExcelUtile.OldVersion;
using NPOI.Util.ArrayExtensions;

namespace ExcelTest
{
    [TestFixture]
    [Order(2)]
    internal class ExportTests : TestBase 
    {

        [TestCaseSource(nameof(Persons))]
        [TestCaseSource(nameof(Records))]
        public void Export_Return_List_Test<T>(List<T> values) where T : class, new()
        {
            var bytes = ExcelHelper<T>.Export(values);
            var direction = LocationHelper.GetExportResourcesPath();
            var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
            File.WriteAllBytes(path, bytes);
            Console.WriteLine("导出数据量为:" + values.Count);
            Assume.That(bytes.Length, Is.AtLeast(10));
        }
        [Test]
        public void New_Export_Return_List_Test()
        {
            var bytes = ExcelSerializer<Person>.Serialize(PersonMock.Persons());
            var direction = LocationHelper.GetExportResourcesPath();
            var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
            File.WriteAllBytes(path, bytes);
            Assume.That(bytes.Length, Is.AtLeast(10));
        }
        private static IEnumerable<IEnumerable<Person>> Persons()
        {
            var lst = new List<Person>();
            for (int i = 0; i < Random.Shared.NextInt64(10, 50); i++)
            {
                lst.Add(new Person()
                {
                    Birthday = DateTime.Now.AddDays(i),
                    Age = i * 2,
                    FatharName = i.ToString() + ":test",
                    IsEnable = i % 3 == 0,
                    Money = i * 15.45,
                    Name = "name:" + i,
                    Sex = i % 2 == 0 ? "male" : "femal",
                    Remark = "beizhu:" + i
                });
            }
            yield return lst;
        }

        private static IEnumerable<IEnumerable<Record>> Records()
        {
            var lst1 = new List<Record>();
            for (int i = 0; i < Random.Shared.NextInt64(10, 50); i++)
            {
                lst1.Add(new Record()
                {
                    CreatedTime = DateTime.Now.AddDays(i),
                    Tags = i.ToString() + ":test",
                    IsEnable = i % 3 == 0,
                    Mass = i * 15.45,
                    Name = "name:" + i,
                    GroupName = "grouptype:" + i * 18
                });
            }
            yield return lst1;
        }

        public override void Init()
        {
            var direction = LocationHelper.GetExportResourcesPath();
            var files=Directory.CreateDirectory(direction).GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }
            base.Init();
        }
    }
}