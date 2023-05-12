using NPOI.XSSF.UserModel;
using static ExcelUtils.ExcelFactory;

namespace ExcelUtils.OldVersion;

public class ExcelHelper<T> where T : class, new()
{
    public static byte[] Export(IEnumerable<T> datas)
    {
        return Export(new Dictionary<string, IEnumerable<T>>() { ["sheet1"] = datas });
    }

    public static byte[] Export(Dictionary<string, IEnumerable<T>> datas)
    {
        var workbook = ExportData(datas);
        using var stream = new MemoryStream();
        workbook.Write(stream);
        return stream.ToArray();
    }

    private static XSSFWorkbook ExportData(Dictionary<string, IEnumerable<T>> datas)
    {
        var workbook = CreateWorkBooke();
        var exportHandler = new ExcelExportHandler<T>();
        foreach (var data in datas)
        {
            exportHandler.SetDatas(workbook.WithSheet(data.Key), data.Value.ToList());
        }
        return workbook;
    }

    public static Dictionary<string, IEnumerable<T>> ImportMulti(Stream file)
    {
        var results = new Dictionary<string, IEnumerable<T>>();
        var workbook = file.CreateWorkBooke();
        var handler = new ExcelImportHandler<T>();
        for (int i = 0; i < workbook.NumberOfSheets; i++)
        {
            var sheet = workbook.GetSheetAt(i);
            results[sheet.SheetName] = handler.GetDatas(sheet);
        }
        return results;
    }

    public static IEnumerable<T> Import(Stream file)
    {
        var workbook = file.CreateWorkBooke();
        return new ExcelImportHandler<T>().GetDatas(workbook.GetSheetAt(0));
    }

    internal static IEnumerable<T> Import(XSSFWorkbook workBook)
    {
        return new ExcelImportHandler<T>().GetDatas(workBook.GetSheetAt(0));
    }
}