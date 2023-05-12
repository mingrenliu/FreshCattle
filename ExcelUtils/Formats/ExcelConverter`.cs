using ExcelUtils.ExcelCore;

namespace ExcelUtils.Formats
{
    public abstract class ExcelConverter<T> : ExcelConverter where T : struct
    {
        protected ExcelConverter() : base(typeof(T))
        {
        }
        
        private T? Read(ICell cell,Type type)
        {
            return default(T?);
        }

        //todo：修改参数和逻辑
        private void Write(ICell cell,T? value)
        {

        }
    }
}