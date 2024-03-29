namespace ExcelUtile.Formats;

public class OnlyTimeFormat : DateTimeFormat
{
    protected override string? Format => "hh:mm:ss";
}