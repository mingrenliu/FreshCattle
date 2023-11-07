using ExcelUtile.ExcelCore;
using NPOI.XSSF.UserModel;

namespace ExcelUtile
{
    public static class ExcelHelper
    {
        public static IEnumerable<T> Import<T>(Stream stream, ExcelSerializeOptions? option = null) where T : class => Import<T>(stream.CreateWorkBook(), option);

        public static Dictionary<string, IEnumerable<T>> ImportAll<T>(Stream stream, ExcelSerializeOptions? option = null) where T : class => ImportAll<T>(stream.CreateWorkBook(), option);

        public static IEnumerable<T> Import<T>(IWorkbook workbook, ExcelSerializeOptions? option = null) where T : class
        {
            return new ExcelReader<T>(workbook, option).ReadOneSheet();
        }

        public static Dictionary<string, IEnumerable<T>> ImportAll<T>(IWorkbook workbook, ExcelSerializeOptions? option = null) where T : class
        {
            return new ExcelReader<T>(workbook, option).ReadMultiSheet();
        }

        public static void Export<T>(Stream stream, IEnumerable<T> data, ExcelSerializeOptions? option = null) where T : class
        {
            new ExcelWriter<T>(data, option).Write().Write(stream, true);
        }

        public static void Export<T>(Stream stream, Dictionary<string, IEnumerable<T>> data, ExcelSerializeOptions? option = null) where T : class
        {
            new ExcelWriter<T>(data, option).Write().Write(stream, true);
        }

        public static byte[] Export<T>(IEnumerable<T> data, ExcelSerializeOptions? option = null) where T : class
        {
            return new ExcelWriter<T>(data, option).Write().ToBytes();
        }

        public static byte[] Export<T>(Dictionary<string, IEnumerable<T>> data, ExcelSerializeOptions? option = null) where T : class
        {
            return new ExcelWriter<T>(data, option).Write().ToBytes();
        }

        public static byte[] ExportTemplate<T>(ExcelSerializeOptions? option = null) where T : class
        {
            return new ExcelWriter<T>(Enumerable.Empty<T>(), option).Write().ToBytes();
        }

        public static void Export<T>(Stream stream, ExcelSerializeOptions? option = null) where T : class
        {
            new ExcelWriter<T>(Enumerable.Empty<T>(), option).Write().Write(stream, true);
        }
    }
}