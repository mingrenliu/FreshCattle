using NPOI.XSSF.UserModel;

namespace ExcelUtile;

public static class ExcelFactory
{
    public static IWorkbook CreateWorkBook()
    {
        return new XSSFWorkbook();
    }

    public static IWorkbook CreateWorkBook(this Stream stream)
    {
        return new XSSFWorkbook(stream);
    }

    public static byte[] ToBytes(this IWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.Write(stream, true);
        return stream.ToArray();
    }
}