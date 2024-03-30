namespace ExcelUtileTest.Tests;

[TestFixture]
[Order(0)]
internal class CommonTest : TestBase
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CreateWorkBooke_Upload_Test()
    {
        var location = LocationHelper.GetImportResourcesPath();
        var filestream = new FileStream(Path.Combine(location, "Normal.xlsx"), FileMode.Open);
        var workBook = new XSSFWorkbook(filestream);
        Assert.That(workBook, Is.Not.Null);
    }
}