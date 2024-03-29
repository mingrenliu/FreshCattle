namespace ExcelUtile.Formats;

public class LongTimeFormat : DateTimeFormat
{
    protected override string? Format => "yyyy-mm-dd hh:mm:ss";
}