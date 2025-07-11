namespace ExcelUtile.Formats;

public class ShortTimeFormat : DateTimeFormat
{
    private const string DefaultFormat = "yyyy-mm-dd";

    public ShortTimeFormat() : this(DefaultFormat)
    {
    }

    public ShortTimeFormat(string? format) : base(format)
    {
    }
}