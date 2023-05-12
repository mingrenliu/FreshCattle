namespace ExcelUtils
{
    public static class ExcelSerializer
    {
        public static IEnumerable<T> Deserialize<T>(Stream stream) => Deserialize<T>(stream.CreateWorkBooke());

        public static Dictionary<string, IEnumerable<T>> DeserializeAll<T>(Stream stream) => stream.CreateWorkBooke().DeserializeAll<T>();

        public static IEnumerable<T> Deserialize<T>(this IWorkbook workbook)
        {
            return default;
        }
        public static Dictionary<string, IEnumerable<T>> DeserializeAll<T>(this IWorkbook workbook)
        {
            return default;
        }

        public static void Serialize<T>(Stream stream, IEnumerable<T> datas)
        {

        }

        public static void Serialize<T>(Stream stream, Dictionary<string, IEnumerable<T>> datas)
        {
        }

        public static byte[] Serialize<T>(IEnumerable<T> datas)
        {
            return default;
        }

        public static byte[] Serialize<T>(Dictionary<string, IEnumerable<T>> datas)
        {
            return default;
        }
    }
}