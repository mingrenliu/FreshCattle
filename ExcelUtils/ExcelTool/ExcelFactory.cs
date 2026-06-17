namespace ExcelTool;

/// <summary>
/// Excel 工作簿工厂：创建 NPOI Workbook、Sheet 命名、字节转换等。
/// </summary>
public static class ExcelFactory
{
    private static readonly char[] InvalidChars = new char[] { '\\', '/', '*', '?', ':', '？', '：' };
    public static readonly Regex SheetNameRegex = new(string.Format("[\\[\\]{0}]", Regex.Escape(new string(InvalidChars))));

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
            name = SheetNameFormat(name);
        }
        return string.IsNullOrWhiteSpace(name) ? workbook.CreateSheet() : (workbook.GetSheet(name) ?? workbook.CreateSheet(name));
    }

    public static string SheetNameFormat(string name)
    {
        name = SheetNameRegex.Replace(name.Trim('\'', ' '), "");
        if (name.Length > 31)
        {
            name = name[..31];
        }
        return name;
    }

    public static ISheet CreateNewSheet(this ISheet sheet, string? name = null)
    {
        return sheet.Workbook.CreateNewSheet(name);
    }
}