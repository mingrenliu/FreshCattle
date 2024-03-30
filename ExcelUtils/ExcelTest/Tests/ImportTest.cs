namespace ExcelUtileTest.Tests;

[TestFixture]
[Order(1)]
internal class ImportTest
{
    [TestCase("person")]
    public void Excel_Import_Person_Test(string filename)
    {
        var timer = StartTimer();
        var workBook = GetWorkBook(filename);
        var lst = ExcelHelper.Import(workBook, new ExcelImportOption<Person>() { HeaderLineIndex = 1 });
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds());
        Assert.That(lst.Count(), Is.AtLeast(40));
    }

    [TestCase("DynamicImport")]
    public void Excel_DynamicImport_Person_Test(string filename)
    {
        var timer = StartTimer();
        var workBook = GetWorkBook(filename);
        var option = new ExcelImportOption<Person>()
        {
            DynamicImport = new ListDynamicHandler<Person, decimal?>(new ColumnInfo[] {
                new("扩展行1",typeof(decimal?),10), new("扩展行2", typeof(decimal?), 10) }, p => p.Data),
            HeaderLineIndex = 1,
            StartLineIndex = 3,
        };
        var lst = ExcelHelper.Import(workBook, option);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds());
        Assert.That(lst.Count(), Is.AtLeast(40));
    }

    [TestCase("record")]
    public void Excel_Import_Record_Test(string filename)
    {
        var timer = StartTimer();
        var workBook = GetWorkBook(filename);
        var lst = ExcelHelper.Import<Record>(workBook);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds());
        Assert.That(lst.Count(), Is.AtLeast(40));
    }

    [TestCase("product")]
    public void Excel_Import_Product_Test(string filename)
    {
        var timer = StartTimer();
        var workBook = GetWorkBook(filename);
        var lst = ExcelHelper.Import<Product>(workBook);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds());
        Assert.That(lst.Count(), Is.AtLeast(1));
    }

    private static IWorkbook GetWorkBook(string filename)
    {
        var location = LocationHelper.GetImportResourcesPath();
        var fileStream = new FileStream(Path.Combine(location, filename + ".xlsx"), FileMode.Open);
        return new XSSFWorkbook(fileStream);
    }
}