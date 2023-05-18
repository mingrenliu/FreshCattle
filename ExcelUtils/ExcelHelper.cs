using ExcelUtile.ExcelCore;
using NPOI.SS.UserModel;

namespace ExcelUtile
{
    public static class ExcelHelper
    {
        public static IEnumerable<T> Import<T>(Stream stream) where T : class => Import<T>(stream.CreateWorkBook());

        public static Dictionary<string, IEnumerable<T>> ImportAll<T>(Stream stream) where T : class => ImportAll<T>(stream.CreateWorkBook());

        public static IEnumerable<T> Import<T>(IWorkbook workbook) where T : class
        {
            return new ExcelReader<T>(workbook).ReadOneSheet();
        }
        public static Dictionary<string, IEnumerable<T>> ImportAll<T>(IWorkbook workbook) where T : class
        {
            return new ExcelReader<T>(workbook).ReadMultiSheet();
        }

        public static void Export<T>(Stream stream, IEnumerable<T> data) where T : class
        {
            new ExcelWriter<T>(data).Write().Write(stream, true);
        }

        public static void Export<T>(Stream stream, Dictionary<string, IEnumerable<T>> data) where T : class
        {
            new ExcelWriter<T>(data).Write().Write(stream,true);
        }

        public static byte[] Export<T>(IEnumerable<T> data) where T : class
        {
            return new ExcelWriter<T>(data).Write().ToArray();
        }

        public static byte[] Export<T>(Dictionary<string, IEnumerable<T>> data) where T : class
        {
            return new ExcelWriter<T>(data).Write().ToArray();
        }
        public static byte[] ExportTemplate<T>() where T : class
        {
            return new ExcelWriter<T>(Enumerable.Empty<T>()).Write().ToArray();
        }
        public static void Export<T>(Stream stream) where T : class
        {
            new ExcelWriter<T>(Enumerable.Empty<T>()).Write().Write(stream, true);
        }
    }
}