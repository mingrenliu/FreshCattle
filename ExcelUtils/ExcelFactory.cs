using NPOI.XSSF.UserModel;

namespace ExcelUtile;

internal static class ExcelFactory
{
    public static ISheet WithSheet(this XSSFWorkbook workbook, string sheetName)
    {
        return workbook.CreateSheet(sheetName);
    }

    public static XSSFWorkbook CreateWorkBook()
    {
        return new XSSFWorkbook();
    }

    public static XSSFWorkbook CreateWorkBook(this Stream stream)
    {
        return new XSSFWorkbook(stream);
    }
    public static byte[] ToArray(this IWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.Write(stream, true);
        return stream.ToArray();
    }
}