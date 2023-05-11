using ExcelTest.Entitys;
using ExcelUtils.OldVersion;
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

    static XSSFWorkbook GetWorkBook(string filename)
    {
        var location = LocationHelper.GetImportResourcesPath();
        var filestream = new FileStream(Path.Combine(location, filename + ".xlsx"), FileMode.Open);
        return new XSSFWorkbook(filestream);
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