namespace ExcelUtile.Formats;

public class LongTimeFormat : DateTimeFormat
{
    private const string DefaultFormat = "yyyy-MM-dd HH:mm:ss";

    public LongTimeFormat() : this(DefaultFormat)
    {
    }

    public LongTimeFormat(string? format):base(format)
    {
    }
}