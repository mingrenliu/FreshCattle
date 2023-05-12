using NPOI.XSSF.UserModel;

namespace ExcelUtils;

internal static class ExcelFactory
{
    public static ISheet WithSheet(this XSSFWorkbook workbook, string sheetName)
    {
        return workbook.CreateSheet(sheetName);
    }

    public static XSSFWorkbook CreateWorkBooke()
    {
        return new XSSFWorkbook();
    }

    public static XSSFWorkbook CreateWorkBooke(this Stream stream)
    {
        return new XSSFWorkbook(stream);
    }
}