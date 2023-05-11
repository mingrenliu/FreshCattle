namespace ExcelTest;

[TestFixture]
[Order(0)]
internal class CommonTest : TestBase
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [Order(0)]
    public void Parse_Propertys_Count_Test()
    {
        var attributes = PropertyUtil.Propertys<Person>();
        Assert.That(attributes, Has.Count.EqualTo(7));
    }

    [Test]
    [Order(1)]
    public void CreateWorkBooke_Blank_Test()
    {
        var book = ExcelFactory.CreateWorkBooke();
        Assert.That(book, Is.Not.Null);
    }

    [Test]
    public void CreateSheet_Test()
    {
        var book = ExcelFactory.CreateWorkBooke();
        var sheet = book.WithSheet("customsheet");
        Assert.That(sheet.SheetName, Is.EqualTo("customsheet"));
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