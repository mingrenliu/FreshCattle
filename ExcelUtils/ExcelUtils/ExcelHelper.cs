using ExcelUtile.ExcelCore;

namespace ExcelUtile
{
    public static class ExcelHelper
    {
        public static IEnumerable<T> Import<T>(Stream stream, ExcelImportOption<T>? option = null) where T : class, new() => Import<T>(stream.CreateWorkBook(), option);

        public static Dictionary<string, IEnumerable<T>> ImportAll<T>(Stream stream, ExcelImportOption<T>? option = null) where T : class, new() => ImportAll<T>(stream.CreateWorkBook(), option);

        public static IEnumerable<T> Import<T>(IWorkbook workbook, ExcelImportOption<T>? option = null) where T : class, new()
        {
            return new ExcelReader<T>(workbook, option).ReadOneSheet();
        }

        public static Dictionary<string, IEnumerable<T>> ImportAll<T>(IWorkbook workbook, ExcelImportOption<T>? option = null) where T : class, new()
        {
            return new ExcelReader<T>(workbook, option).ReadMultiSheet();
        }

        public static void Export<T>(Stream stream, IEnumerable<T> data, ExcelExportOption<T>? option = null, IEnumerable<MergedRegion>? regions = null) where T : class
        {
            new ExcelWriter<T>(data, option, regions).Write().Write(stream, true);
        }

        public static void Export<T>(Stream stream, Dictionary<string, IEnumerable<T>> data, ExcelExportOption<T>? option = null) where T : class
        {
            new ExcelWriter<T>(data, option).Write().Write(stream, true);
        }

        public static void Export<T>(Stream stream, Dictionary<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>> data, ExcelExportOption<T>? option = null) where T : class
        {
            new ExcelWriter<T>(data, option).Write().Write(stream, true);
        }

        public static byte[] Export<T>(IEnumerable<T> data, ExcelExportOption<T>? option = null, IEnumerable<MergedRegion>? regions = null) where T : class
        {
            return new ExcelWriter<T>(data, option, regions).Write().ToBytes();
        }

        public static byte[] Export<T>(ExcelExportSheetOption<T> sheetOption, IEnumerable<T> data, ExcelExportOption<T>? option = null) where T : class
        {
            return new ExcelWriter<T>(sheetOption, data, option).Write().ToBytes();
        }

        public static byte[] Export<T>(Dictionary<string, IEnumerable<T>> data, ExcelExportOption<T>? option = null) where T : class
        {
            return new ExcelWriter<T>(data, option).Write().ToBytes();
        }

        public static byte[] Export<T>(Dictionary<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>> data, ExcelExportOption<T>? option = null) where T : class
        {
            return new ExcelWriter<T>(data, option).Write().ToBytes();
        }

        public static byte[] Export<T>(Dictionary<string, Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>> data, ExcelExportOption<T>? option = null) where T : class
        {
            return new ExcelWriter<T>(data, option).Write().ToBytes();
        }

        public static byte[] ExportTemplate<T>(ExcelExportOption<T>? option = null) where T : class
        {
            return new ExcelWriter<T>(Enumerable.Empty<T>(), option).Write().ToBytes();
        }

        public static void ExportTemplate<T>(Stream stream, ExcelExportOption<T>? option = null) where T : class
        {
            new ExcelWriter<T>(Enumerable.Empty<T>(), option).Write().Write(stream, true);
        }

        public static void Export<T>(Stream stream, ExcelExportOption<T>? option = null) where T : class
        {
            new ExcelWriter<T>(Enumerable.Empty<T>(), option).Write().Write(stream, true);
        }
    }
}