using NPOI.XSSF.UserModel;

namespace ExcelUtile;

public static class ExcelFactory
{
    public static XSSFWorkbook CreateWorkBook()
    {
        return new XSSFWorkbook();
    }

    public static XSSFWorkbook CreateWorkBook(this Stream stream)
    {
        return new XSSFWorkbook(stream);
    }

    public static byte[] ToBytes(this XSSFWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.Write(stream, true);
        return stream.ToArray();
    }
}