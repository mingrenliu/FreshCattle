using ExcelUtile.ExcelCore;
using NPOI.SS.UserModel;

namespace ExcelUtile
{
    public static class ExcelSerializer<T> where T : class
    {
        public static IEnumerable<T> Deserialize(Stream stream) => Deserialize(stream.CreateWorkBook());

        public static Dictionary<string, IEnumerable<T>> DeserializeAll(Stream stream) => DeserializeAll(stream.CreateWorkBook());

        public static IEnumerable<T> Deserialize(IWorkbook workbook)
        {
            return new ExcelReader<T>(workbook).ReadOneSheet();
        }
        public static Dictionary<string, IEnumerable<T>> DeserializeAll(IWorkbook workbook)
        {
            return new ExcelReader<T>(workbook).ReadMultiSheet();
        }

        public static void Serialize(Stream stream, IEnumerable<T> data)
        {
            new ExcelWriter<T>(data).Write().Write(stream, true);
        }

        public static void Serialize(Stream stream, Dictionary<string, IEnumerable<T>> data)
        {
            new ExcelWriter<T>(data).Write().Write(stream,true);
        }

        public static byte[] Serialize(IEnumerable<T> data)
        {
            return new ExcelWriter<T>(data).Write().ToArray();
        }

        public static byte[] Serialize(Dictionary<string, IEnumerable<T>> data)
        {
            return new ExcelWriter<T>(data).Write().ToArray();
        }

    }
}