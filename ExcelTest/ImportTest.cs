using ExcelTest.Entitys;
using ExcelUtile.OldVersion;
using NPOI.HSSF.Record;
using NPOI.XSSF.UserModel;

namespace ExcelTest;

[TestFixture]
[Order(1)]
internal class ImportTest
{

    [TestCaseSource(typeof(ImportSourceCases), nameof(ImportSourceCases.ImportTestCases))]
    public int Excel_Import_Return_Count_Test(string filename)
    {
        var workBook = GetWorkBook(filename);
        var result = ExcelHelper<Person>.Import(workBook).ToList();
        return result.Count;
    }

    [TestCase("Error")]
    public void Excel_Import_Throw_Exception_Test(string filename)
    {
        var workBook = GetWorkBook(filename);
        _ = Assert.Catch<Exception>(() => ExcelHelper<Person>.Import(workBook).ToList());
    }
    [TestCase("person")]
    public void Excel_Import_Person_Test(string filename)
    {
        var workBook = GetWorkBook(filename);
        var lst=ExcelSerializer<Person>.Deserialize(workBook);
        Assert.That(lst.Count(), Is.EqualTo(40));
    }

    static XSSFWorkbook GetWorkBook(string filename)
    {
        var location = LocationHelper.GetImportResourcesPath();
        var fileStream = new FileStream(Path.Combine(location, filename + ".xlsx"), FileMode.Open);
        return new XSSFWorkbook(fileStream);
    }
}
internal class ImportSourceCases
{
    public static IEnumerable ImportTestCases
    {
        get
        {
            yield return new TestCaseData("Normal").Returns(1);
            yield return new TestCaseData("Normal").Returns(0);
        }
    }
}