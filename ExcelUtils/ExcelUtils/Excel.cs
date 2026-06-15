using ExcelUtile.ExcelCore;

namespace ExcelUtile;

/// <summary>
/// Excel 序列化器，提供导入导出功能的主入口。
/// 对应 System.Text.Json 的 JsonSerializer。
/// 
/// <para>使用示例：</para>
/// <code>
/// // 导出
/// var bytes = Excel.Serialize(persons);
/// Excel.Serialize(stream, persons);
/// 
/// // 导入
/// var list = Excel.Deserialize&lt;Person&gt;(stream);
/// 
/// // opt-out 模式（自动包含所有属性）
/// var opt = ExcelOptions.AutoIncludeAll;
/// var bytes = Excel.Serialize(persons, opt);
/// </code>
/// </summary>
public static class Excel
{
    #region Serialize

    public static byte[] Serialize<T>(IEnumerable<T> data, ExcelOptions? options = null, string? sheetName = null) where T : class
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        var sheet = workbook.CreateNewSheet(sheetName);
        SerializeSheet(sheet, data, options);
        return workbook.ToBytes();
    }

    public static void Serialize<T>(Stream stream, IEnumerable<T> data, ExcelOptions? options = null, string? sheetName = null) where T : class
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        var sheet = workbook.CreateNewSheet(sheetName);
        SerializeSheet(sheet, data, options);
        workbook.Write(stream, true);
    }

    public static byte[] Serialize<T>(Dictionary<string, IEnumerable<T>> sheets, ExcelOptions? options = null) where T : class
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        foreach (var (name, data) in sheets)
        {
            var sheet = workbook.CreateNewSheet(name);
            SerializeSheet(sheet, data, options);
        }
        return workbook.ToBytes();
    }

    public static void Serialize<T>(Stream stream, Dictionary<string, IEnumerable<T>> sheets, ExcelOptions? options = null) where T : class
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        foreach (var (name, data) in sheets)
        {
            var sheet = workbook.CreateNewSheet(name);
            SerializeSheet(sheet, data, options);
        }
        workbook.Write(stream, true);
    }

    public static byte[] CreateTemplate<T>(ExcelOptions? options = null, string? sheetName = null) where T : class
    {
        return Serialize(Enumerable.Empty<T>(), options, sheetName);
    }

    public static void CreateTemplate<T>(Stream stream, ExcelOptions? options = null, string? sheetName = null) where T : class
    {
        Serialize(stream, Enumerable.Empty<T>(), options, sheetName);
    }

    #endregion

    #region Deserialize

    public static IEnumerable<T> Deserialize<T>(Stream stream, ExcelOptions? options = null) where T : class, new()
    {
        using var workbook = ExcelFactory.CreateWorkBook(stream);
        var sheet = workbook.GetSheetAt(0);
        return DeserializeSheet<T>(sheet, options);
    }

    public static IEnumerable<T> Deserialize<T>(IWorkbook workbook, ExcelOptions? options = null) where T : class, new()
    {
        var sheet = workbook.GetSheetAt(0);
        return DeserializeSheet<T>(sheet, options);
    }

    public static Dictionary<string, IEnumerable<T>> DeserializeAll<T>(Stream stream, ExcelOptions? options = null) where T : class, new()
    {
        using var workbook = ExcelFactory.CreateWorkBook(stream);
        return DeserializeAllSheets<T>(workbook, options);
    }

    public static Dictionary<string, IEnumerable<T>> DeserializeAll<T>(IWorkbook workbook, ExcelOptions? options = null) where T : class, new()
    {
        return DeserializeAllSheets<T>(workbook, options);
    }

    #endregion

    #region Internal Helpers

    private static void SerializeSheet<T>(ISheet sheet, IEnumerable<T> data, ExcelOptions? options) where T : class
    {
        options ??= ExcelOptions.Default;
        var writeTypeInfo = ExcelTypeInfoResolver.ResolveWrite(typeof(T), options);
        var sheetWriter = new ExcelSheetWriter(sheet);
        var objectWriter = new ExcelDataWriter<T>(sheetWriter, writeTypeInfo, options);
        objectWriter.Write(data);
    }

    private static IEnumerable<T> DeserializeSheet<T>(ISheet sheet, ExcelOptions? options) where T : class, new()
    {
        options ??= ExcelOptions.Default;
        var readTypeInfo = ExcelTypeInfoResolver.ResolveRead(typeof(T), options);
        var sheetReader = new ExcelSheetReader(sheet);
        var objectReader = new ExcelDataReader<T>(sheetReader, readTypeInfo, options);
        return objectReader.Read();
    }

    private static Dictionary<string, IEnumerable<T>> DeserializeAllSheets<T>(IWorkbook workbook, ExcelOptions? options) where T : class, new()
    {
        options ??= ExcelOptions.Default;
        var result = new Dictionary<string, IEnumerable<T>>();
        for (int i = 0; i < workbook.NumberOfSheets; i++)
        {
            var sheet = workbook.GetSheetAt(i);
            result[sheet.SheetName] = DeserializeSheet<T>(sheet, options).ToList();
        }
        return result;
    }

    #endregion
}
