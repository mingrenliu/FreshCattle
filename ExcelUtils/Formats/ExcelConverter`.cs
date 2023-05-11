namespace ExcelUtils.Formats
{
    public abstract class ExcelConverter<T> : ExcelConverter where T : struct
    {
        protected ExcelConverter() : base(typeof(T))
        {
        }
        //todo：修改参数和逻辑
        private T? Read()
        {
            return default(T?);
        }

        //todo：修改参数和逻辑
        private void Write(T? value)
        {
        }
    }
}