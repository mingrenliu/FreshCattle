namespace ExcelUtile.Formats;

public class OnlyTimeFormat : DateTimeFormat
{
    private const string DefaultFormat = "hh:mm:ss";

    public OnlyTimeFormat() : this(DefaultFormat)
    {
    }

    public OnlyTimeFormat(string? format) : base(format)
    {
    }
}