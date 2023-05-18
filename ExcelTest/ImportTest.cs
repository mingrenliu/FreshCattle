namespace ExcelTest;

[TestFixture]
[Order(1)]
internal class ImportTest
{
    [TestCase("person")]
    public void Excel_Import_Person_Test(string filename)
    {
        var str = "234234";
        var wfef=str.Substring(0,1521);
        var timer = StartTimer();
        var workBook = GetWorkBook(filename);
        var lst = ExcelHelper.Import<Person>(workBook);
        Console.WriteLine("计算毫秒数：" + timer.GetMilliseconds());
        Assert.That(lst.Count(), Is.AtLeast(40));
    }

    private static XSSFWorkbook GetWorkBook(string filename)
    {
        var location = LocationHelper.GetImportResourcesPath();
        var fileStream = new FileStream(Path.Combine(location, filename + ".xlsx"), FileMode.Open);
        return new XSSFWorkbook(fileStream);
    }
}