namespace ExcelUtile.Formats;

/// <summary>
/// 泛型Excel读写转换器
/// </summary>
/// <typeparam name="T"> </typeparam>
public abstract class ExcelConverter<T> : ExcelConverter where T : notnull
{
    public bool CanConvert() => CanConvert(typeof(T));

    protected ExcelConverter() : base(typeof(T))
    {
    }
}

/// <summary>
/// 泛型单元格读写接口
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IConverter<T>
{
    public void Write(ICell cell, T? value, ICellStyle? style);

    public T? Read(ICell cell);
}

/// <summary>
/// 泛型引用类型Excel读写转换器
/// </summary>
/// <typeparam name="T"> </typeparam>
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

    public void Write(ICell cell, T? value, ICellStyle? style = null)
    {
        FormatCell(cell, style);
        WriteValue(cell, value);
    }
}

/// <summary>
/// 泛型值类型Excel读写转换器
/// </summary>
/// <typeparam name="T"> </typeparam>
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

    public void Write(ICell cell, T? value, ICellStyle? style = null)
    {
        FormatCell(cell, style);
        WriteValue(cell, value);
    }
}