namespace ExcelUtile.Formats;

public class OnlyTimeFormat : DateTimeFormat
{
    protected override string? Format => "hh:mm:ss";
    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value.ToString("HH:mm:ss"));
        }
    }
}