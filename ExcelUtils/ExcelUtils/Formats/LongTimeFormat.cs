namespace ExcelUtile.Formats;

public class LongTimeFormat : DateTimeFormat
{
    protected override string? Format => "yyyy-mm-dd hh:mm:ss";
    public override void Write(ICell cell, DateTime? value)
    {
        if (value != null)
        {
            cell.SetCellValue(value.Value);
        }
    }
}