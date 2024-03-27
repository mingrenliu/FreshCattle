namespace ExcelUtile.Formats;

public abstract class ExcelConverter<T> : ExcelConverter where T : notnull
{
    public bool CanConvert() => CanConvert(typeof(T));

    protected ExcelConverter() : base(typeof(T))
    {
    }
}

public interface IConverter<T>
{
    public void Write(ICell cell, T? value);

    public T? Read(ICell cell);
}

public abstract class ExcelReferenceConverter<T> : ExcelConverter<T>, IConverter<T?> where T : class
{
    public override void WriteAsObject(ICell cell, object? obj)
    {
        if (obj == null) return;
        if (obj is T value)
        {
            Write(cell, value);
        }
        else
        {
            base.WriteAsObject(cell, obj);
        }
    }

    public abstract void Write(ICell cell, T? value);

    public abstract T? Read(ICell cell);

    public override object? ReadFromCell(ICell cell)
    {
        return Read(cell);
    }
}

public abstract class ExcelStructConverter<T> : ExcelConverter<T>, IConverter<T?> where T : struct
{
    public override void WriteAsObject(ICell cell, object? obj)
    {
        if (obj == null) return;
        if (obj is T value)
        {
            Write(cell, value);
        }
        else
        {
            base.WriteAsObject(cell, obj);
        }
    }

    public abstract void Write(ICell cell, T? value);

    public abstract T? Read(ICell cell);

    public override object? ReadFromCell(ICell cell)
    {
        return Read(cell);
    }
}