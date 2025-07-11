using ExcelUtile.ExcelCore;

namespace ExcelUtile;

public static class ExcelHelper
{
    public static IEnumerable<T> Import<T>(Stream stream, ExcelImportOption<T>? option = null) where T : class, new() => Import<T>(stream.CreateWorkBook(), option);

    public static Dictionary<string, IEnumerable<T>> ImportAll<T>(Stream stream, ExcelImportOption<T>? option = null) where T : class, new() => ImportAll<T>(stream.CreateWorkBook(), option);

    public static IEnumerable<T> Import<T>(IWorkbook workbook, ExcelImportOption<T>? option = null) where T : class, new()
    {
        return workbook.GetSheetAt(0).ReadData<T>(option);
    }

    public static Dictionary<string, IEnumerable<T>> ImportAll<T>(IWorkbook workbook, ExcelImportOption<T>? option = null) where T : class, new()
    {
        var result = new Dictionary<string, IEnumerable<T>>();
        foreach (var item in workbook.GetAllSheets())
        {
            result[item.Key] = item.Value.ReadData<T>(option);
        }
        return result;
    }

    public static void Export<T>(Stream stream, IEnumerable<T> data, ExcelExportOption<T>? option = null, string? sheetName = null) where T : class
    {
        var workbook = ExcelFactory.CreateWorkBook();
        workbook.CreateNewSheet(sheetName).WriteData(data, option);
        workbook.Write(stream, true);
    }

    public static void Export<T>(Stream stream, Dictionary<string, IEnumerable<T>> data, ExcelExportOption<T>? option = null) where T : class
    {
        var workbook = ExcelFactory.CreateWorkBook();
        foreach (var item in data)
        {
            workbook.CreateNewSheet(item.Key).WriteData(item.Value, option);
        }
        workbook.Write(stream, true);
    }

    public static byte[] Export<T>(IEnumerable<T> data, ExcelExportOption<T>? option = null,string? sheetName=null) where T : class
    {
        var workbook = ExcelFactory.CreateWorkBook();
        workbook.CreateNewSheet(sheetName).WriteData(data, option);
        return workbook.ToBytes();
    }

    public static byte[] Export<T>(Dictionary<string, IEnumerable<T>> data, ExcelExportOption<T>? option = null) where T : class
    {
        var workbook = ExcelFactory.CreateWorkBook();
        foreach (var item in data)
        {
            workbook.CreateNewSheet(item.Key).WriteData(item.Value, option);
        }
        return workbook.ToBytes();
    }

    public static byte[] ExportTemplate<T>(ExcelExportOption<T>? option = null, string? sheetName = null) where T : class
    {
        return Export(Enumerable.Empty<T>(), option,sheetName);
    }

    public static void ExportTemplate<T>(Stream stream, ExcelExportOption<T>? option = null, string? sheetName = null) where T : class
    {
        Export(stream, Enumerable.Empty<T>(), option,sheetName);
    }
    public static IWorkbook CreateWorkBook()
    {
       return ExcelFactory.CreateWorkBook();
    }
    public static IWorkbook CreateWorkBook(Stream stream)
    {
        return stream.CreateWorkBook();
    }
}