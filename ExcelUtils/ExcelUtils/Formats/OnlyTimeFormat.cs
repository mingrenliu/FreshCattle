namespace ExcelUtile.Formats;

public class OnlyTimeFormat : DateTimeFormat
{
    protected override string? Format => "hh:mm:ss";

    protected override void WriteValue(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value.ToString("HH:mm:ss"));
        }
    }
}