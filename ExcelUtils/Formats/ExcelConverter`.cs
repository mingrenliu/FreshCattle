namespace ExcelUtile.Formats
{
    public abstract class ExcelConverter<T> : ExcelConverter where T : struct
    {

        protected ExcelConverter() : base(typeof(T))
        {
        }

        public abstract T? Read(ICell cell, Type type);

        public override object? ReadFromCell(ICell cell)
        {
            return Read(cell,_type);
        }

        //todo：修改参数和逻辑
        public abstract void Write(ICell cell, T? value);

        public override void WriteAsObject(ICell cell, object? obj)
        {
            if (obj is T value)
            {
                Write(cell, value);
            }
            else
            {
                base.WriteAsObject(cell, obj);
            }
        }
    }
}