using Microsoft.AspNetCore.Http;
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

    public static XSSFWorkbook CreateWorkBooke(this IFormFile file)
    {
        if (!SupportMimeType.ValidMimeType(file.ContentType))
        {
            throw new Exception("仅支持XLSX格式的Excel文件，请上传正确的文件");
        }
        using var stream = file.OpenReadStream();
        return new XSSFWorkbook(stream);
    }

    public static XSSFWorkbook CreateWorkBooke(this Stream stream)
    {
        return new XSSFWorkbook(stream);
    }
}