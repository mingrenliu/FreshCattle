using ExcelTest.Mocks;
namespace ExcelTest
{
    [TestFixture]
    [Order(2)]
    internal class ExportTests : TestBase
    {

        [Test]
        public void New_Export_Return_List_Test()
        {
            var persons = PersonMock.Persons();
            var timer = StartTimer();
            var bytes = ExcelHelper.Export<Person>(persons);
            var direction = LocationHelper.GetExportResourcesPath();
            var path = Path.Combine(direction, Guid.NewGuid().ToString() + ".xlsx");
            File.WriteAllBytes(path, bytes);
            Console.WriteLine("计算毫秒数："+timer.GetMilliseconds());
            Assume.That(bytes.Length, Is.AtLeast(10));
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