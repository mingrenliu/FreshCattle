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
    protected override void WriteAsObject(ICell cell, object? obj)
    {
        if (obj == null) return;
        if (obj is T value)
        {
            WriteValue(cell, value);
        }
        else
        {
            base.WriteAsObject(cell, obj);
        }
    }

    protected abstract void WriteValue(ICell cell, T? value);

    public abstract T? Read(ICell cell);

    public override object? ReadFromCell(ICell cell)
    {
        return Read(cell);
    }

    public void Write(ICell cell, T? value)
    {
        FormatCell(cell);
        WriteValue(cell, value);
    }
}

public abstract class ExcelStructConverter<T> : ExcelConverter<T>, IConverter<T?> where T : struct
{
    protected override void WriteAsObject(ICell cell, object? obj)
    {
        if (obj == null) return;
        if (obj is T value)
        {
            WriteValue(cell, value);
        }
        else
        {
            base.WriteAsObject(cell, obj);
        }
    }

    protected abstract void WriteValue(ICell cell, T? value);

    public abstract T? Read(ICell cell);

    public override object? ReadFromCell(ICell cell)
    {
        return Read(cell);
    }

    public void Write(ICell cell, T? value)
    {
        FormatCell(cell);
        WriteValue(cell, value);
    }
}