namespace ExcelUtile.Formats;

public class ShortTimeFormat : DateTimeFormat
{
    private const string DefaultFormat = "yyyy-MM-dd";

    public ShortTimeFormat() : this(DefaultFormat)
    {
    }

    public ShortTimeFormat(string? format) : base(format)
    {
    }
}