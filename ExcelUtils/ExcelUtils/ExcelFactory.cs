using ExcelUtile.ExcelCore;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;

namespace ExcelUtile;

public static class ExcelFactory
{
    private static readonly char[] InvalidateChars = new char[] { '\\', '/',  '*', '？','?', ':', '：' };
    public readonly static Regex SheetNameRegen = new(string.Format("[\\[\\]{0}]", Regex.Escape(new string(InvalidateChars))));

    public static IWorkbook CreateWorkBook()
    {
        return new XSSFWorkbook();
    }

    internal static IWorkbook CreateWorkBook(this Stream stream)
    {
        return new XSSFWorkbook(stream);
    }

    public static byte[] ToBytes(this IWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.Write(stream, true);
        return stream.ToArray();
    }

    public static Dictionary<string, ISheet> GetAllSheets(this IWorkbook workbook)
    {
        var result = new Dictionary<string, ISheet>();
        for (int i = 0; i < workbook.NumberOfSheets; i++)
        {
            var sheet = workbook.GetSheetAt(i);
            result[sheet.SheetName] = sheet;
        }
        return result;
    }

    public static ISheet CreateNewSheet(this IWorkbook workbook, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(name) is false)
        {
            name= SheetNameRegen.Replace(name, "");
        }
        return string.IsNullOrWhiteSpace(name) ? workbook.CreateSheet() : workbook.CreateSheet(name);
    }

    public static ISheet CreateNewSheet(this ISheet sheet, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(name) is false)
        {
            name = SheetNameRegen.Replace(name, "");
        }
        var workbook = sheet.Workbook;
        return string.IsNullOrWhiteSpace(name) ? workbook.CreateSheet() : workbook.CreateSheet(name);
    }

    public static ISheet WriteData<T>(this ISheet sheet, IEnumerable<T> data, ExcelExportOption<T>? option = null) where T : class
    {
        new ExcelWriter<T>(sheet, data, option).Write();
        return sheet;
    }

    public static IEnumerable<T> ReadData<T>(this ISheet sheet, ExcelImportOption<T>? option = null) where T : class, new()
    {
        return new ExcelReader<T>(sheet, option).Read();
    }
}